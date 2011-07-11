using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace DotNetNuke.Extensions.Web
{
    /// <summary>
    /// Exnteion methods for control.
    /// </summary>
    public static class ControlExtensions
    {
        #region Find Methods
        /// <summary>
        /// Finds the control by id.
        /// </summary>
        /// <param name="parentControl">The parent control.</param>
        /// <param name="id">The id.</param>
        /// <returns>Control. Null of not found.</returns>
        public static Control FindControlById(this Control parentControl, string id)
        {
            return FindControls(parentControl, c => !string.IsNullOrEmpty(c.ID) && c.ID == id).FirstOrDefault();
        }

        /// <summary>
        /// Finds the controls by the ID prefix.
        /// </summary>
        /// <param name="parentControl">The parent control.</param>
        /// <param name="idPrefix">The id prefix.</param>
        /// <returns>Controls that have the ID prefix.</returns>
        public static List<Control> FindControlsByPrefix(this Control parentControl, string idPrefix)
        {
            return FindControls(parentControl, c => !string.IsNullOrEmpty(c.ID) && c.ID.StartsWith(idPrefix));
        }

        /// <summary>
        /// Finds the first control in the control hierarchy that matches the filter function.
        /// </summary>
        /// <param name="parentControl">The parent control.</param>        
        /// <param name="filterFunc">Function that tests each control to see if it matches our criteria.</param>
        /// <returns>Control that match the filter function.</returns>
        public static Control FindControl(this Control parentControl, Func<Control, bool> filterFunc)
        {
            return FindControls(parentControl, true, filterFunc).FirstOrDefault();
        }

        /// <summary>
        /// Finds all controls that are in the control hierarchy below this control.
        /// </summary>
        /// <param name="parentControl">The parent control.</param>        
        /// <param name="filterFunc">Function that tests each control to see if it matches our criteria.</param>
        /// <returns>Controls that match the filter function.</returns>
        public static List<Control> FindControls(this Control parentControl, Func<Control, bool> filterFunc)
        {
            return FindControls(parentControl, false, filterFunc);
        }

        /// <summary>
        /// Finds all controls that are in the control hierarchy below this control.
        /// </summary>
        /// <param name="parentControl">The parent control.</param>
        /// <param name="findOne">True if only finding one, otherwise false.</param>
        /// <param name="filterFunc">Function that tests each control to see if it matches our criteria.</param>
        /// <returns>Controls that match the filter function.</returns>
        private static List<Control> FindControls(Control parentControl, bool findOne, Func<Control, bool> filterFunc)
        {
            // anonymous function to find ALL controls in a control hierarchy, recursively
            Func<ControlCollection, List<Control>> findControl = null;
            findControl = delegate(ControlCollection controls)
            {
                List<Control> foundList = new List<Control>();
                if (controls != null && controls.Count > 0)
                {
                    foreach (Control item in controls)
                    {
                        if (filterFunc(item))
                        {
                            foundList.Add(item);
                            if (findOne) break;
                        }
                        foundList.AddRange(findControl(item.Controls));
                    }
                }
                return foundList;
            };

            // call anonymous function
            List<Control> allControls = findControl(parentControl.Controls);

            return allControls;
        } 
        #endregion
    }
}
