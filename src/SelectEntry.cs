using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cypretex.Data.Selectors
{
    public class SelectEntry
    {
        public string Property { get; set; }
        public IList<SelectEntry> Childs { get; set; } = new List<SelectEntry>();

        public SelectEntry AddChildProperty(SelectEntry entry)
        {
            Childs.Add(entry);
            return this;
        }

        public bool HasProperty(string name)
        {
            string[] properties = name.Split(".");
            SelectEntry entry = this;
            if (properties.Count() == 1)
            {
                return Property.Equals(properties[0], System.StringComparison.InvariantCultureIgnoreCase);
            }
            else
            {
                entry = Childs.Where(o => o.Property == properties[0]).FirstOrDefault();
                return entry == null ? false : entry.HasProperty(string.Join(".", properties.Skip(1)));
            }
        }

        public string ToSelector()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Property);
            if (this.Childs.Count > 0)
            {
                sb.Append("[");
                for (int i = 0; i < this.Childs.Count; i++)
                {
                    SelectEntry entry = this.Childs[i];
                    if (entry != null)
                    {
                        sb.Append(entry.ToSelector());
                        if (i < this.Childs.Count - 1)
                        {
                            sb.Append(",");
                        }
                    }
                }
                sb.Append("]");
            }
            return sb.ToString();
        }
    }

}