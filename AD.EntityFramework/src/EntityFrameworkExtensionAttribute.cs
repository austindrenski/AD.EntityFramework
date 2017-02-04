using System;
using JetBrains.Annotations;

namespace AD.EntityFramework
{
    /// <summary>
    /// This attribute flags an extension method as a target for calls to <see cref="ReduceExpressionExtensions"/>.
    /// </summary>
    [PublicAPI]
    public class EntityFrameworkExtensionAttribute : Attribute { }
}
