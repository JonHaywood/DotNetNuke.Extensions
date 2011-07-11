using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;

namespace DotNetNuke.Extensions.Modules
{
    /// <summary>
    /// Extenions for the PortalModuleBase class.
    /// </summary>
    public static class PortalModuleBaseExtensions
    {
        #region ProcessException Methods
        /// <summary>
        /// Processes the exception and displays the error message to the user.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <param name="msg">Message to display before the exception.</param>        
        public static void ProcessException(this PortalModuleBase module, Exception ex, string msg)
        {
            ProcessException(module, ex, msg, true);
        }

        /// <summary>
        /// Processes the exception.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <param name="msg">Message to display before the exception.</param>
        /// <param name="displayException">True to display the exception to the user.</param>
        public static void ProcessException(this PortalModuleBase module, Exception ex, string msg, bool displayException)
        {            
            DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);

            if (displayException)
            {
                string errorMessage = msg + ": " + ex.Message;
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(
                        module, errorMessage, DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
            }
        }
        #endregion

        #region IsUserAdmin Methods
        /// <summary>
        /// Determines whether the user is an administrator or not in the current portal. All super users will be administrators.
        /// </summary>
        /// <param name="module">The module.</param>        
        /// <param name="userId">User ID.</param>
        /// <returns>
        ///   <c>true</c> current user is an administrator; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsUserAnAdmin(this PortalModuleBase module, int userId)
        {
            try
            {
                var userInfo = UserController.GetUserById(module.PortalId, userId);
                return userInfo.IsSuperUser || userInfo.IsInRole("Administrator");
            }
            catch (System.Exception err)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(err);

                return false;
            }
        }

        /// <summary>
        /// Determines whether the user is an administrator or not. All super users will be administrators.
        /// </summary>
        /// <param name="module">The module.</param>
        /// <param name="portalId">ID of portal to check against.</param>
        /// <param name="userId">User ID.</param>
        /// <returns>
        ///   <c>true</c> current user is an administrator; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsUserAnAdmin(this PortalModuleBase module, int portalId, int userId)
        {
            try
            {
                var userInfo = UserController.GetUserById(portalId, userId);
                return userInfo.IsSuperUser || userInfo.IsInRole("Administrator");
            }
            catch (System.Exception err)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(err);

                return false;
            }
        }

        /// <summary>
        /// Determines whether the current user is an administrator or not. All super users will be administrators.
        /// </summary>
        /// <param name="module">The module.</param>
        /// <returns>
        ///   <c>true</c> current user is an administrator; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsCurrentUserAnAdmin(this PortalModuleBase module)
        {
            try
            {
                return module.UserInfo.IsSuperUser ||
                    UserController.GetUserById(module.PortalId, module.UserId).IsInRole("Administrator");
            }
            catch (System.Exception err)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(err);

                return false;
            }
        } 
        #endregion
    }
}
