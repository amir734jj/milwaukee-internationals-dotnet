using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using InfoViaLinq;
using Models.Interfaces;

namespace Logic.Extensions
{
    public static class ObjectExtension
    {
        public static string Name<T>(this Expression<Func<T, object>> obj) where T : IViewModel
        {
            var info = InfoViaLinq<T>.New()
                .PropLambda(obj);

            return info.GetAttribute<DisplayAttribute>().Name ?? info.GetPropertyName();
        }
    }
}