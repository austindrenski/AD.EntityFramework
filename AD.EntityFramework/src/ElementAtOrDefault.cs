using JetBrains.Annotations;

namespace AD.EntityFramework
{
    /// <summary>
    /// 
    /// </summary>
    [PublicAPI]
    public class ElementAtOrDefaultExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [EntityFrameworkExtension]
        public static T ElementAtOrDefault<T>()
        {

            return default(T);
        }
    }
}
