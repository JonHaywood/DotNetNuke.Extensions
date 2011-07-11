using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetNuke.ComponentModel;
using Microsoft.Practices.ServiceLocation;

namespace DotNetNuke.Extensions.ServiceLocation
{
    /// <summary>
    /// Class that provides an implementation of the <see cref="IServiceLocator"/> interface
    /// specific to DotNetNuke's ComponentModel contracts.
    /// </summary>
    public class DotNetNukeServiceLocator : ServiceLocatorImplBase
    {
        /// <summary>
        /// Reference to the DNN IoC container.
        /// </summary>
        private IContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="DotNetNukeServiceLocator"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public DotNetNukeServiceLocator(IContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// When implemented by inheriting classes, this method will do the actual work of
        /// resolving all the requested service instances.
        /// </summary>
        /// <param name="serviceType">Type of service requested.</param>
        /// <returns>
        /// Sequence of service instance objects.
        /// </returns>
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return _container.GetComponentList(serviceType);
        }

        /// <summary>
        /// When implemented by inheriting classes, this method will do the actual work of resolving
        /// the requested service instance.
        /// </summary>
        /// <param name="serviceType">Type of instance requested.</param>
        /// <param name="key">Name of registered service you want. May be null.</param>
        /// <returns>
        /// The requested service instance.
        /// </returns>
        protected override object DoGetInstance(Type serviceType, string key)
        {
            return _container.GetComponent(key, serviceType);
        }
    }
}
