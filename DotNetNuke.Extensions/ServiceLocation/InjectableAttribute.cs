using System;

namespace DotNetNuke.Extensions.ServiceLocation
{
    /// <summary>
    /// Classes or properties with this attribute are candidates for dependency injection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Property, 
        AllowMultiple = false, Inherited = true)]
    public class InjectableAttribute : Attribute
    {
    }
}
