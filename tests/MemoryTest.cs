using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using System.Linq.Expressions;
using System.Dynamic;

namespace Cypretex.Data.Selectors.Tests
{
    public class DynamicDictionary : DynamicObject
    {
        // The inner dictionary.
        Dictionary<string, object> dictionary
            = new Dictionary<string, object>();

        // This property returns the number of elements
        // in the inner dictionary.
        public int Count
        {
            get
            {
                return dictionary.Count;
            }
        }

        // If you try to get a value of a property
        // not defined in the class, this method is called.
        public override bool TryGetMember(
            GetMemberBinder binder, out object result)
        {
            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            string name = binder.Name.ToLower();

            // If the property name is found in a dictionary,
            // set the result parameter to the property value and return true.
            // Otherwise, return false.
            return dictionary.TryGetValue(name, out result);
        }

        // If you try to set a value of a property that is
        // not defined in the class, this method is called.
        public override bool TrySetMember(
            SetMemberBinder binder, object value)
        {
            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            dictionary[binder.Name.ToLower()] = value;

            // You can always add a value to a dictionary,
            // so this method always returns true.
            return true;
        }
    }
    public class MemoryTests
    {

        public MemoryTests()
        {
            var c = new List<User>();
            int documentIndex = 1;
            for (int i = 1; i < 101; i++)
            {
                User user = new User()
                {
                    Id = i,
                    FirstName = $"FirstName {i}",
                    LastName = $"LastName {i}",
                    AnualSalary = i,
                    Phone = i.ToString(),
                    Documents = new List<Document>(),
                    Parent = i > 1 ? c[i - 2] : null
                };

                for (var j = 1; j < 10; j++)
                {
                    user.Documents.Add(new Document()
                    {
                        Owner = user,
                        Name = $"Document {documentIndex}",
                        Id = documentIndex,
                    });
                    documentIndex++;
                }
                //Console.WriteLine(user.Documents.Count());
                //user.PrincipalDocument = user.Documents.FirstOrDefault();
                //user.Documents = null;
                c.Add(user);
            }
            users = c.AsQueryable();
        }

        protected IQueryable<User> users;



        [Fact]
        public void TestMemory()
        {

            var source = users;
            var selector = Selector.Properties("Id,FullName");
            var result = selector.Parse<User>(source);
            Console.WriteLine(source.SelectProperties<User>("Id,FullName").DisplayProperties(true));
            // var source = users;
            // var selector = Selector.From("Id");
            // var result = (object)selector.Parse<User, User>(source);
            // Console.WriteLine(result.DisplayProperties(true));
            // Type sourceType = source.GetType();
            // var properties = new[] { "Id", "FirstName", "LastName" }
            //    .Select(f => sourceType.GetProperty(f))
            //    .ToList();

            // var sourceParameter = Expression.Parameter(sourceType, sourceType.Name);
            // //var bindings = properties.Select(p => Expression.Bind(typeof(ExpandoObject).GetProperty(p.Name), Expression.Property(sourceParameter, p.Name)));
            // object result = null;
            // var destParameter = Expression.Parameter(typeof(object));
            // dynamic o = new ExpandoObject();
            // foreach (PropertyInfo info in properties)
            // {
            //     // var binder = Microsoft.CSharp.RuntimeBinder.Binder.GetMember(
            //     //     CSharpBinderFlags.None,
            //     //     info.Name,
            //     //     typeof(User), // or this.GetType() 
            //     //     new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
            //     // Func<dynamic, dynamic> f = Expression.Lambda<Func<dynamic, dynamic>>(
            //     //      Expression.Dynamic(binder, typeof(object), destParameter),
            //     // destParameter)
            //     // .Compile();
            //     var newExpression = Expression.New(typeof(ExpandoObject));
            //     var initExpression = Expression.MemberInit(newExpression, bindings);
            //     var param = Expression.Parameter(sourceType);
            //     Expression.Lambda<Func<dynamic, dynamic>>(initExpression, sourceParameter);
            //     ((IDictionary<string, object>)o)[info.Name] = info.GetValue(source);
            // }
            // Console.WriteLine(o.Id);

            // var newExpression = Expression.New(typeof(DynamicDictionary));
            // var initExpression=Expression.MemberInit(newExpression, bindings);
            // var result = Expression.Lambda<Func<User, dynamic>>(newExpression, sourceParameter);
            //Console.WriteLine(result);

            //  var sourceType = typeof(object);
            //     var underlyingType = source.First().GetType();
            //     var propertyType = underlyingType.GetProperty(sortProperty).PropertyType;
            //     var param = Expression.Parameter(sourceType);
            //     var body = Expression.Property(
            //         Expression.Convert(param, underlyingType), sortProperty
            //     );
            //     var lambda = Expression.Lambda(body, param);

            //     var sortMethod = sortOrder == SortOrder.Descending ? "OrderByDescending" : "OrderBy";
            //     var expr = Expression.Call(typeof(Queryable), sortMethod, new Type[] { sourceType, propertyType },
            //         source.Expression, lambda
            //     );
            //     return source.Provider.CreateQuery<object>(expr);


            // var binder = Microsoft.CSharp.RuntimeBinder.Binder.GetMember(
            //     CSharpBinderFlags.None,
            //     "Id",
            //     typeof(User), // or this.GetType() 
            //     new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });


            // var par = Expression.Parameter(typeof(object));

            // Func<dynamic, dynamic> f = Expression.Lambda<Func<dynamic, dynamic>>(
            //      Expression.Dynamic(binder, typeof(User), par),
            // par)
            // .Compile();

            // dynamic obj = users.FirstOrDefault();
            // object value = f(obj); // Hello
            // Console.WriteLine(value);
            // var source = Expression.Parameter(typeof(TSource), "o");
            // var properties = fields
            //     .Select(f => typeof(TSource).GetProperty(f))
            //     .Select(p => new DynamicProperty(p.Name, p.PropertyType))
            //     .ToList();
            // var resultType = DynamicClassFactory.CreateType(properties, false);
            // var bindings = properties.Select(p => Expression.Bind(resultType.GetProperty(p.Name), Expression.Property(source, p.Name)));
            // var result = Expression.MemberInit(Expression.New(resultType), bindings);
            // return Expression.Lambda<Func<TSource, dynamic>>(result, source);
            // var source = Expression.Parameter(typeof(User), "o");

            // var properties = new[] { "Id", "FirstName", "LastName" }
            //     .Select(f => typeof(User).GetProperty(f))
            //     .ToList();

            // // var resultType = DynamicClassFactory.CreateType(properties, false);
            // var bindings = properties.Select(p => Expression.Bind(typeof(DynamicDictionary).GetProperty(p.Name), Expression.Property(source, p.Name)));
            // var result = Expression.MemberInit(Expression.New(typeof(DynamicDictionary)), bindings);
            // Console.WriteLine(result);
            // return Expression.Lambda<Func<TSource, dynamic>>(result, source);
        }

    }

    public static class DynamicExtensions
    {
        public static T FromDynamic<T>(this IDictionary<string, object> dictionary)
        {
            var bindings = new List<MemberBinding>();
            foreach (var sourceProperty in typeof(T).GetProperties().Where(x => x.CanWrite))
            {
                var key = dictionary.Keys.SingleOrDefault(x => x.Equals(sourceProperty.Name, StringComparison.OrdinalIgnoreCase));
                if (string.IsNullOrEmpty(key)) continue;
                var propertyValue = dictionary[key];
                bindings.Add(Expression.Bind(sourceProperty, Expression.Constant(propertyValue)));
            }
            Expression memberInit = Expression.MemberInit(Expression.New(typeof(T)), bindings);
            return Expression.Lambda<Func<T>>(memberInit).Compile().Invoke();
        }

        public static dynamic ToDynamic<T>(this T obj)
        {
            IDictionary<string, object> expando = new ExpandoObject();

            foreach (var propertyInfo in typeof(T).GetProperties().Where(p => p.Name == "Id"))
            {
                var propertyExpression = Expression.Convert(Expression.Property(Expression.Constant(obj), propertyInfo), typeof(object));
                var currentValue = Expression.Lambda<Func<object>>(propertyExpression).Compile().Invoke();
                expando.Add(propertyInfo.Name.ToLower(), currentValue);
            }
            return expando as ExpandoObject;
        }
    }
}
