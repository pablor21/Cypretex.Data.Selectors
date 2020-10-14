using System.Collections.Generic;

namespace Cypretex.Data.Selectors
{
    
    public class Selector
    {
        public virtual IList<SelectEntry> Fields { get; set; } = new List<SelectEntry>();

        public virtual ISelectorParser Parser { get; set; } = new DefaultSelectorParser();


        public virtual Selector AddEntry(SelectEntry entry)
        {
            this.Fields.Add(entry);
            return this;
        }

        public virtual bool HasProperty(string name)
        {
            foreach (SelectEntry entry in this.Fields)
            {
                if (entry.HasProperty(name))
                {
                    return true;
                }
            }
            return false;
        }
        
        public virtual Selector ParseProperties(string properties)
        {
            this.Fields = Utils.ParsePropertyNames(properties);
            return this;
        }

        public static Selector Properties(string properties)
        {
            return new Selector().ParseProperties(properties);
        }

        public virtual TResult Parse<T, TResult>(object source) where TResult : class, new()
        {
            return this.Parser.Parse<T, TResult>(this, source);
        }

        public virtual dynamic Parse<T>(object source)
        {
            return this.Parser.Parse<T>(this, source);
        }

        public virtual IEnumerable<TResult> Parse<T, TResult>(IEnumerable<T> source) where TResult : class, new()
        {
            return this.Parser.Parse<T, TResult>(this, source);
        }

        public virtual IEnumerable<dynamic> Parse<T>(IEnumerable<T> source)
        {
            return this.Parser.Parse<T>(this, source);
        }
    }
}