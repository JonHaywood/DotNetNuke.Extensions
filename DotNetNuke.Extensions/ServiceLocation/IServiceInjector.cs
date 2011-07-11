using System;

namespace DotNetNuke.Extensions.ServiceLocation
{
    /// <summary>
    /// Represents a service which can perform dependency injection.
    /// </summary>
    public interface IServiceInjector
    {
        /// <summary>
        /// Construct a new instance of type <typeparamref name="T"/> including 
        /// it's dependencies.
        /// </summary>
        /// <typeparam name="T">Type to construct.</typeparam>
        /// <returns>Instance of <typeparamref name="T"/>.</returns>
        T Construct<T>();

        /// <summary>
        /// Inject a dependency into an existing instance.
        /// </summary>
        /// <param name="instance">Instance to inject into.</param>
        void Inject(object instance);
    }
}
