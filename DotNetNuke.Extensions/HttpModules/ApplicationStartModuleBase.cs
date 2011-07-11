using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace DotNetNuke.Extensions.HttpModules
{
    /// <summary>
    /// This class enables code to be executed during application start without going into Global.ascx.
    /// Over the OnStart method to do so. 
    /// </summary>
    /// <see cref="http://erraticdev.blogspot.com/2011/01/how-to-correctly-use-ihttpmodule-to.html"/>
    public abstract class ApplicationStartModuleBase : IHttpModule
    {
        #region Static privates
        private static bool applicationStarted = false;
        private static object applicationStartLock = new object();
        #endregion

        #region IHttpModule implementation
        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
        /// </summary>
        public void Dispose()
        {
            // dispose any resources if needed
        }

        /// <summary>
        /// Initializes the specified module.
        /// </summary>
        /// <param name="context">The application context that instantiated and will be running this module.</param>
        public void Init(HttpApplication context)
        {
            if (!applicationStarted)
            {
                lock (applicationStartLock)
                {
                    if (!applicationStarted)
                    {
                        // this will run only once per application start
                        this.OnStart(context);
                        applicationStarted = true;
                    }
                }
            }
            // this will run on every HttpApplication initialization in the application pool
            this.OnInit(context);
        }        
        #endregion

        #region Extension Methods
        /// <summary>Initializes any data/resources on application start.</summary>
        /// <param name="context">The application context that instantiated and will be running this module.</param>
        public virtual void OnStart(HttpApplication context)
        {
            // put your application start code here
        }

        /// <summary>Initializes any data/resources on HTTP module start.</summary>
        /// <param name="context">The application context that instantiated and will be running this module.</param>
        public virtual void OnInit(HttpApplication context)
        {
            // put your module initialization code here
        }
        #endregion
    }
}
