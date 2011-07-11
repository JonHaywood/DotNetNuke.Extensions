using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using System.Collections;

namespace DotNetNuke.Extensions.Modules
{
    /// <summary>
    /// The type of module to add.
    /// </summary>
    public enum ModuleAddType
    {
        New,
        Copy,
        Reference
    }

    /// <summary>
    /// Add easy-to-use functionality to ModuleController to add and remove modules from tabs.
    /// </summary>
    public static class ModuleControllerExtensions
    {
        #region Public Methods
        /// <summary>
        /// Gets the modules on tab.
        /// </summary>
        /// <param name="moduleController">The module controller.</param>
        /// <param name="tab">The tab.</param>
        /// <returns></returns>
        public static IEnumerable<ModuleInfo> GetModulesOnTab(this ModuleController moduleController, TabInfo tab)
        {
            return moduleController.GetTabModules(tab.TabID).Values.ToList();
        }

        /// <summary>
        /// Adds the modules to tabs.
        /// </summary>
        /// <param name="moduleController">The module controller.</param>
        /// <param name="moduleIds">The module ids.</param>
        /// <param name="moduleType">Type of the module.</param>
        /// <param name="tabs">The tabs.</param>
        public static void AddModulesToTabs(this ModuleController moduleController, IEnumerable<int> moduleIds, ModuleAddType moduleType, List<int> tabs)
        {
            var modules = moduleIds.Select(m => moduleController.GetModule(m));

            foreach (int tabID in tabs)
                AddModulesToTab(moduleController, modules, moduleType, tabID);
        }

        /// <summary>
        /// Adds the modules to tab.
        /// </summary>
        /// <param name="moduleController">The module controller.</param>
        /// <param name="modules">The modules.</param>
        /// <param name="moduleType">Type of the module.</param>
        /// <param name="tabID">The tab ID.</param>
        public static void AddModulesToTab(this ModuleController moduleController, IEnumerable<ModuleInfo> modules, ModuleAddType moduleType, int tabID)
        {
            foreach (ModuleInfo module in modules)
                AddModuleToTab(moduleController, module, moduleType, tabID);
        }

        /// <summary>
        /// Adds the module to tab.
        /// </summary>
        /// <param name="moduleController">The module controller.</param>
        /// <param name="module">The module.</param>
        /// <param name="moduleType">Type of the module.</param>
        /// <param name="destinationTabID">The destination tab ID.</param>
        public static void AddModuleToTab(this ModuleController moduleController, ModuleInfo module, ModuleAddType moduleType, int destinationTabID)
        {
            switch (moduleType)
            {
                case ModuleAddType.Copy:
                    CopyModuleToPage(module, destinationTabID);
                    break;
                case ModuleAddType.New:
                    AddNewModuleToPage(module, destinationTabID);
                    break;
                case ModuleAddType.Reference:
                    AddReferencedModuleToPage(module, destinationTabID);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The module add type '{0}' is not supported.", moduleType));
            }
        }

        /// <summary>
        /// Deletes the modules from tabs.
        /// </summary>
        /// <param name="moduleController">The module controller.</param>
        /// <param name="moduleIds">The module ids.</param>
        /// <param name="tabs">The tabs.</param>
        public static void DeleteModulesFromTabs(this ModuleController moduleController, IEnumerable<int> moduleIds, List<int> tabs)
        {
            var modules = moduleIds.Select(m => moduleController.GetModule(m));

            foreach (ModuleInfo module in modules)
            {
                // get the tabs this module is on
                var moduleTabs = moduleController.GetModuleTabs(module.ModuleID).Cast<ModuleInfo>()
                    .Select(m => m.TabID);

                // get the if any of those intersect with the list of 
                // provided tabs, then delete it from those pages
                var commonTabs = moduleTabs.Intersect(tabs);
                foreach (int tabID in commonTabs)
                    DeleteModuleFromTab(moduleController, module, tabID);
            }
        }

        /// <summary>
        /// Deletes the module from tab.
        /// </summary>
        /// <param name="moduleController">The module controller.</param>
        /// <param name="module">The module.</param>
        /// <param name="tabID">The tab ID.</param>
        public static void DeleteModuleFromTab(this ModuleController moduleController, ModuleInfo module, int tabID)
        {
            moduleController.DeleteTabModule(tabID, module.ModuleID, true);
            moduleController.DeleteTabModuleSettings(module.TabModuleID);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Adds the referenced module to page.
        /// </summary>
        /// <param name="moduleToCopy">The module to copy.</param>
        /// <param name="destinationTabID">The destination tab ID.</param>
        private static void AddReferencedModuleToPage(ModuleInfo moduleToCopy, int destinationTabID)
        {
            // duplicate the module and assign it the new page
            ModuleInfo newModule = moduleToCopy.Clone();
            newModule.TabID = destinationTabID;

            // add it to the system - this takes care of adding the 
            // module and the tab module
            newModule.ModuleID = new ModuleController().AddModule(newModule);
        }

        /// <summary>
        /// Adds the new module to page.
        /// </summary>
        /// <param name="moduleToCopy">The module to copy.</param>
        /// <param name="destinationTabID">The destination tab ID.</param>
        private static void AddNewModuleToPage(ModuleInfo moduleToCopy, int destinationTabID)
        {
            // duplicate the module and assign it the new page
            ModuleInfo newModule = moduleToCopy.Clone();
            newModule.ModuleID = -1;
            newModule.TabID = destinationTabID;

            // add it to the system - this takes care of adding the 
            // module and the tab module
            newModule.ModuleID = new ModuleController().AddModule(newModule);

            // duplicate the settings for the tabmodule
            new ModuleController().CopyTabModuleSettings(moduleToCopy, newModule);
        }

        /// <summary>
        /// Copies the module to page.
        /// </summary>
        /// <param name="moduleToCopy">The module to copy.</param>
        /// <param name="destinationTabID">The destination tab ID.</param>
        private static void CopyModuleToPage(ModuleInfo moduleToCopy, int destinationTabID)
        {
            var  moduleController = new ModuleController();

            // duplicate the module and assign it the new page
            ModuleInfo newModule = moduleToCopy.Clone();
            newModule.ModuleID = -1;
            newModule.TabID = destinationTabID;

            // add it to the system - this takes care of adding the 
            // module and the tab module
            newModule.ModuleID = moduleController.AddModule(newModule);

            // duplicate the settings for the module
            DuplicateModuleSettings(moduleToCopy, newModule.ModuleID);

            // duplicate the settings for the tabmodule
            moduleController.CopyTabModuleSettings(moduleToCopy, newModule);
        }

        /// <summary>
        /// Duplicates the module settings.
        /// </summary>
        /// <param name="originalModule">The original module.</param>
        /// <param name="destinationModuleID">The destination module ID.</param>
        private static void DuplicateModuleSettings(ModuleInfo originalModule, int destinationModuleID)
        {
            IDictionaryEnumerator iterator = originalModule.ModuleSettings.GetEnumerator();
            while (iterator.MoveNext())
            {
                new ModuleController().UpdateModuleSetting(destinationModuleID, iterator.Key.ToString(), iterator.Value.ToString());
            }
        }
        #endregion
    }
}
