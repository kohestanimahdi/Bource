using System;

namespace Bource.WebConfiguration.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ExcludeApiResultFilterAttribute : Attribute
    {
    }
}
