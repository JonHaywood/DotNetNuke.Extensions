using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;

namespace DotNetNuke.Extensions.ServiceLocation
{
    /// <summary>
    /// Base class for service boot strappers. IServiceLocator is IoC agnostic however, each concrete IoC
    /// container will need to be bootstrapped differently. This class creates a consistent interface for
    /// each bootstrapper to setup the IoC container. 
    /// 
    /// The class has two important properties:
    /// Locator - interface for retrieving the service locator. It is created and all types registered here.
    /// Injector - interface that performs the actual dependency injection.
    /// 
    /// There are two interfaces instead of one because the practical reasons (the DNN IoC container doens't
    /// do any injection) and because of the SRP (http://en.wikipedia.org/wiki/Single_responsibility_principle).
    /// It is conceivable that one class could implement both interfaces if needed (for instance, building a
    /// Unity Application Block adapter).
    /// </summary>
    public abstract class CommonServiceBootStapper
    {
        /// <summary>
        /// Reference to the service locator.
        /// </summary>
        public readonly IServiceLocator Locator;

        /// <summary>
        /// Reference to the service injector.
        /// </summary>
        public readonly IServiceInjector Injector;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonServiceBootStapper"/> class.
        /// </summary>
        protected CommonServiceBootStapper()
        {
            Locator = CreateServiceLocator();
            Injector = CreateServiceInjector();
        }

        /// <summary>
        /// Creates the service locator.
        /// </summary>
        /// <returns><see cref="IServiceLocator"/></returns>
        protected abstract IServiceLocator CreateServiceLocator();

        /// <summary>
        /// Creates the service injector.
        /// </summary>
        /// <returns><see cref="IServiceInjector"/></returns>
        protected abstract IServiceInjector CreateServiceInjector();
    }
}
