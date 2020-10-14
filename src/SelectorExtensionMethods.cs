using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace Cypretex.Data.Selectors
{
    public static class SelectorExtensionMethods
    {

        public static dynamic SelectProperties<T>(this T obj, string props = "")
        {
            return Selector.Properties(props).Parse<T>(obj);
        }

        public static TResult SelectProperties<T, TResult>(this T obj, string props = "") where TResult : class, new()
        {
            return Selector.Properties(props).Parse<T, TResult>(obj);
        }

        public static IEnumerable<dynamic> SelectProperties<T>(this IEnumerable<T> obj, string props = "") where T : class, new()
        {
            return Selector.Properties(props).Parse<T>(obj);
        }

        public static IEnumerable<TResult> SelectProperties<T, TResult>(this IEnumerable<T> obj, string props = "") where TResult : class, new()
        {
            return Selector.Properties(props).Parse<T, TResult>(obj);
        }

    }
}