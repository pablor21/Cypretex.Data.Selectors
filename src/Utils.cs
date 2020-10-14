using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Cypretex.Data.Selectors
{
    public static class Utils
    {
        public static readonly Type EnumerableType = typeof(Enumerable);
        public static readonly MethodInfo AsQueryableMethod = EnumerableType.GetRuntimeMethods().FirstOrDefault(
        method => method.Name == "AsEnumerable" && method.IsStatic);

        public static bool IsEnumerable(Expression prop)
        {
            return prop.Type.GetTypeInfo().ImplementedInterfaces.FirstOrDefault(x => x.Name == "IEnumerable") != null;
        }



        public static Type GetEnumerableTypeArg(Type type)
        {
            return type.GetGenericArguments()[0];
        }

        public static bool IsEnumerable(Type type)
        {
            return type.GetTypeInfo().ImplementedInterfaces.FirstOrDefault(x => x.Name == "IEnumerable") != null;
        }

        public static Type GetIEnumerableImpl(Type type)
        {
            // Get IEnumerable implementation. Either type is IEnumerable<T> for some T, 
            // or it implements IEnumerable<T> for some T. We need to find the interface.
            if (IsEnumerable(type))
                return type;
            Type[] t = type.FindInterfaces((m, o) => IsEnumerable(m), null);
            return t[0];
        }
        public static PropertyInfo GetPropertyInfo(Type t, string name)
        {
            var p = t.GetRuntimeProperties().SingleOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (p == null)
            {
                throw new InvalidOperationException(string.Format("Property '{0}' not found on type '{1}'", name, t));
            }

            if (t != p.DeclaringType)
            {
                p = p.DeclaringType.GetRuntimeProperties().SingleOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
            return p;
        }

        public static PropertyInfo GetPropertyInfo(Expression e, string name)
        {
            var t = e.Type;
            var p = t.GetRuntimeProperties().SingleOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (p == null)
            {
                throw new InvalidOperationException(string.Format("Property '{0}' not found on type '{1}'", name, t));
            }

            if (t != p.DeclaringType)
            {
                p = p.DeclaringType.GetRuntimeProperties().SingleOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
            return p;
        }
        public static List<SelectEntry> ParsePropertyNames(string properties, string prefix = "")
        {
            string pattern = @"((?<complex>[A-Za-z0-9\*]+)\[(?<props>[[A-Za-z0-9,\*]+)\]?)+|(?<simple>\w+)";
            List<SelectEntry> ret = new List<SelectEntry>();
            MatchCollection matches = Regex.Matches(properties.Replace(" ", ""), pattern);
            if (matches.Any())
            {

                matches.ToList().ForEach(o =>
                {
                    if (!string.IsNullOrEmpty(o.Groups["simple"].Value))
                    {
                        ret.Add(new SelectEntry()
                        {
                            Property = o.Value
                        });
                    }
                    else
                    {
                        SelectEntry entry = new SelectEntry()
                        {
                            Property = o.Groups["complex"].Value,
                            Childs = ParsePropertyNames(o.Groups["props"].Value)
                        };
                        ret.Add(entry);
                    }
                });
            }

            return ret;
        }
    }


}