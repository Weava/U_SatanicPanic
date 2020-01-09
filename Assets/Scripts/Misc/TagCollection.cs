using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Misc
{
    public class TagCollection
    {
        public Dictionary<string, string> Lookup;

        public void Add(string key, string value)
        {
            Lookup.Add(key, value);
        }

        public void Add(TagTypes type, string value)
        {
            Lookup.Add(type.ToString(), value);
        }

        public void Add(params string[] tags)
        {
            foreach(var tag in tags)
            {
                Lookup.Add(tag, tag);
            }
        }

        public void Add(Dictionary<string, string> tags)
        {
            tags.ToList().ForEach(x => Lookup.Add(x.Key, x.Value));
        }

        public List<string> ToList()
        {
            return Lookup.Select(s => s.Value).ToList();
        }

        public string Get(TagTypes type)
        {
            return Lookup[type.ToString()];
        }

        public string Get(string key)
        {
            return Lookup[key];
        }
    }

    public enum TagTypes
    {
        Region
    }
}
