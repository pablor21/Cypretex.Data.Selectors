using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Cypretex.Data.Selectors
{
    public class DefaultSelectorParser : ISelectorParser
    {
        public TResult Parse<T, TResult>(Selector selector, object source) where TResult : class, new()
        {
            if (source == null)
            {
                return default(TResult);
            }

            if (typeof(T) != typeof(TResult))
            {
                source = ToObject<TResult>(source);
            }


            //return ToObject<TResult>(this.Parse<T>(selector, source) as IDictionary<string, Object>);
            return source != null ? ToObject<TResult>(this.CreateDynamicObject(selector.Fields, source, typeof(TResult))) : null;
        }

        public IEnumerable<TResult> Parse<T, TResult>(Selector selector, IEnumerable<T> source) where TResult : class, new()
        {
            if (source == null)
            {
                return null;
            }
            IList<TResult> result = new List<TResult>();
            IEnumerator<T> enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
            {
                T entry = enumerator.Current;
                if (entry != null)
                    result.Add(Parse<T, TResult>(selector, entry));
            }
            return result;
        }

        public dynamic Parse<T>(Selector selector, object source)
        {
            return source != null ? this.CreateDynamicObject(selector.Fields, source, source.GetType()) : null;
        }

        public IEnumerable<dynamic> Parse<T>(Selector selector, IEnumerable<T> source)
        {
            if (source == null)
            {
                return null;
            }
            IList<dynamic> result = new List<dynamic>();
            IEnumerator<T> enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
            {
                T entry = enumerator.Current;
                if (entry != null)
                    result.Add(Parse<T>(selector, entry));
            }
            return result;
        }

        protected virtual TResult ToObject<TResult>(object source, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public)
          where TResult : class, new()
        {
            TResult targetObject = new TResult();
            Type targetObjectType = typeof(TResult);
            Type sourceType = source.GetType();
            PropertyInfo[] sourceProperties = sourceType.GetProperties();
            var expando = new ExpandoObject();
            var dictionary = (IDictionary<string, object>)expando;


            // Go through all bound target object type properties...
            foreach (PropertyInfo property in targetObjectType.GetProperties(bindingFlags))
            {
                PropertyInfo sourceProperty = sourceProperties.Where(pi => pi.Name == property.Name).FirstOrDefault();
                if (sourceProperty != null && sourceProperty.PropertyType == property.PropertyType)
                {
                    property.SetValue(targetObject, sourceProperty.GetValue(source));
                }
            }

            return targetObject;
        }


        protected virtual TResult ToObject<TResult>(IDictionary<string, object> source, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public)
           where TResult : class, new()
        {
            TResult targetObject = new TResult();
            Type targetObjectType = typeof(TResult);

            // Go through all bound target object type properties...
            foreach (PropertyInfo property in targetObjectType.GetProperties(bindingFlags))
            {
                // ...and check that both the target type property name and its type matches
                // its counterpart in the ExpandoObject
                if (source.ContainsKey(property.Name) && property.SetMethod != null
                    && property.PropertyType == source[property.Name].GetType())
                {
                    property.SetValue(targetObject, source[property.Name]);
                }
            }

            return targetObject;
        }

        protected virtual dynamic CreateDynamicObject(IList<SelectEntry> entries, dynamic source, Type destType)
        {

            if (source == null)
            {
                return null;
            }

            Type sourceType = source.GetType();

            if (entries.Count == 0)
            {
                foreach (PropertyInfo pi in destType.GetProperties())
                {
                    entries.Add(new SelectEntry()
                    {
                        Property = pi.Name
                    });
                }
            }
            bool isCollection = Utils.IsEnumerable(sourceType);
            if (isCollection)
            {
                IList<dynamic> colRet = new List<dynamic>();
                destType = Utils.GetEnumerableTypeArg(sourceType);
                IEnumerable<dynamic> enumerable = (IEnumerable<dynamic>)source;
                IEnumerator enumerator = enumerable.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current != null)
                    {
                        colRet.Add(CreateDynamicObject(entries, enumerator.Current, destType));
                    }
                }
                return colRet;
            }
            else
            {
                IDictionary<string, object> ret = new ExpandoObject();
                foreach (SelectEntry entry in entries)
                {
                    PropertyInfo propertyInfo = Utils.GetPropertyInfo(sourceType, entry.Property);
                    if (propertyInfo != null)
                    {
                        var propertyExpression = Expression.Convert(Expression.Property(Expression.Constant(source), propertyInfo), typeof(object));
                        if (entry.Childs == null || entry.Childs.Count == 0)
                        {
                            var currentValue = Expression.Lambda<Func<object>>(propertyExpression).Compile().Invoke();
                            ret.Add(propertyInfo.Name, currentValue);
                        }
                        else
                        {
                            var value = propertyInfo.GetValue(source) ?? null;
                            ret.Add(propertyInfo.Name, this.CreateDynamicObject(entry.Childs, value, value.GetType()));
                        }
                    }
                }

                return ret as ExpandoObject;
                //return ret;
            }

        }

    }
}