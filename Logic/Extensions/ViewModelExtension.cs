using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using InfoViaLinq;
using Models.Interfaces;

namespace Logic.Extensions
{
    /// <summary>
    /// View model extensions
    /// </summary>
    public static class ViewModelExtension
    {
        /// <summary>
        /// Property name or display name value all via linq
        /// </summary>
        /// <param name="_"></param>
        /// <param name="expression"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string PropName<T>(this T _, Expression<Func<T, object>> expression) where T : IViewModel
        {
            var info = InfoViaLinq<T>.New()
                .PropLambda(expression);

            return info.GetAttribute<DisplayAttribute>()?.Name ?? info.GetPropertyName();
        }
    }
}