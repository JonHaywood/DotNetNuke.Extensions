using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetNuke.ComponentModel;
using System.Reflection;

namespace DotNetNuke.Extensions.ServiceLocation
{
    /// <summary>
    /// Service injector which uses DotNetNuke's IoC container to perform dependency injection.
    /// </summary>
    /// <see cref="http://zacharysnow.net/2010/06/21/implementing-basic-dependency-injection-using-services-container/"/>
    public class ServiceInjector : IServiceInjector
    {
        /// <summary>
        /// Reference to the DNN container.
        /// </summary>
        private readonly IContainer Container;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceInjector"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public ServiceInjector(IContainer container)
        {
            Container = container;
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Service.</returns>
        protected object GetService(Type type)
        {
            Check.Require(type != null, "Type cannot be null.", new ArgumentNullException("type"));

            // get the service
            object service = Container.GetComponent(type);

            return service;
        }

        /// <summary>
        /// Gets the injectable service if it exists.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Service.</returns>
        protected object GetInjectableService(Type type)
        {
            if (type == typeof(IServiceInjector))
            {
                return this;
            }
            else
            {
                object service = GetService(type);

                // make sure we have a service
                Check.Ensure(service != null, "Failed to find " + type + " dependency.");

                return service;
            }
        }

        /// <summary>
        /// Construct a new instance of type <typeparamref name="T"/> including
        /// it's dependencies.
        /// </summary>
        /// <typeparam name="T">Type to construct.</typeparam>
        /// <returns>
        /// Instance of <typeparamref name="T"/>.
        /// </returns>
        public T Construct<T>()
        {
            // get the first constructor which has the injectableattribute on it
            ConstructorInfo injectableConstructor = typeof(T).GetConstructors()
                .Where(c => c.GetCustomAttributes(true).Cast<Attribute>().Any(a => a is InjectableAttribute)).FirstOrDefault();

            // make sure we have a constructor
            Check.Require(injectableConstructor != null, "No injectable constructor found.");

            // get the parameters of the constructor
            var parameters = injectableConstructor.GetParameters();
            var services = new object[parameters.Length];

            // construct each parameter
            int i = 0;
            foreach (ParameterInfo parameter in parameters)
                services[i++] = GetInjectableService(parameter.ParameterType);

            // construct this type
            return (T)injectableConstructor.Invoke(services);  
        }

        /// <summary>
        /// Inject a dependency into an existing instance.
        /// </summary>
        /// <param name="instance">Instance to inject into.</param>
        public void Inject(object instance)
        {
            // get all properties which has the injectableattribute on it
            var propertyInfos = instance.GetType().GetProperties()
                .Where(p => p.GetCustomAttributes(true).Cast<Attribute>().Any(a => a is InjectableAttribute));

            foreach (var property in propertyInfos)
            {
                if (!property.CanWrite)
                    throw new InvalidOperationException(property.Name + " is marked as Injectable but not writable.");

                property.SetValue(instance, GetInjectableService(property.PropertyType), null);  
            }
        }
    }
}
