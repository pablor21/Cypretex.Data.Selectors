using System.Collections.Generic;

namespace Cypretex.Data.Selectors
{
    public interface ISelectorParser
    {
        TResult Parse<T, TResult>(Selector selector, object source) where TResult : class, new();
        dynamic Parse<T>(Selector selector, object source);
        IEnumerable<TResult> Parse<T, TResult>(Selector selector, IEnumerable<T> source) where TResult : class, new();
        IEnumerable<dynamic> Parse<T>(Selector selector, IEnumerable<T> source);
    }
}