using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using System.Reflection;
using System.Globalization;

namespace DotNetNuke.Extensions.ServiceLocation
{
    /// <summary>
    /// Provide a common access point to the service locator and service injector. Note that in the 
    /// entry point to your application (say in an HttpModule or Global.asax) this class must be 
    /// initialized with the correct boot strapper. Derived classes will be singletons.
    /// 
    /// By extending this class in your application, the service locator and service injector
    /// are unique to your application and cannot be used by another.
    /// </summary>
    /// <example><![CDATA[
    ///     // derived class.
    ///     public class MyLocatorManager : ServiceLocatorManagerBase<MyLocatorManager>
    ///     { }
    ///     
    ///     // using the locator manager
    ///     MyLocatorManager locatorMgr = MyLocatorManager.Instance;
    /// ]]></example>
    public abstract class ServiceLocatorManagerBase<T> where T : ServiceLocatorManagerBase<T>
    {
        private static volatile T _instance = null;
        private static volatile object _syncObject = new object();

        /// <summary>
        /// Gets the instance of the service locator singleton.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncObject)
                    {
                        if (_instance == null)
                        {
                            Type type = typeof(T);

                            try
                            {
                                _instance = (T)type.InvokeMember(type.Name,
                                    BindingFlags.CreateInstance | BindingFlags.Instance |
                                    BindingFlags.NonPublic, null, null, null,
                                    CultureInfo.InvariantCulture);
                            }
                            catch (MissingMethodException)
                            {
                                string message = type.FullName + 
                                    " must use either a private or protected " +
                                    "constructor to be a Singleton.";
                                throw new TypeLoadException(message); 
                            }
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Service boot strapper.
        /// </summary>
        private static CommonServiceBootStapper _bootStrapper;

        /// <summary>
        /// Reference to the <see cref="IServiceLocator"/> being used for this application.
        /// </summary>
        public static IServiceLocator Locator
        {
            get { return _bootStrapper.Locator;  }
        }

        /// <summary>
        /// Reference to the <see cref="IServiceInjector"/> being used for this application.
        /// </summary>
        public static IServiceInjector Injector
        {
            get { return _bootStrapper.Injector;  }
        }

        /// <summary>
        /// Initializes the specified boot strapper.
        /// </summary>
        /// <param name="bootStrapper">The boot strapper.</param>
        public void Initialize(CommonServiceBootStapper bootStrapper)
        {
            Check.Require(bootStrapper != null, "BootStrapper cannot be null");
            _bootStrapper = bootStrapper;
        }
    }
}
