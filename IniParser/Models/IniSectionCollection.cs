using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IniParser.Models
{
    public class IniSectionCollection
    {
        private Dictionary<string, IniSection> sectionCollection;

        public IniKeyValueCollection this[string sectionName]
        {
            get 
            { 
                if (sectionCollection.ContainsKey(sectionName))
                {
                    return sectionCollection[sectionName].KeyValueCollection;
                }
                else
                {
                    return null;
                }
            }
        }

        public int Count 
        { 
            get
            {
                return sectionCollection.Count;
            }
        }

        public IniSectionCollection() : this(EqualityComparer<string>.Default) { }

        public IniSectionCollection(IEqualityComparer<string> equalityComparer)
        {
            sectionCollection = new Dictionary<string, IniSection>(equalityComparer);
        }

        public bool ContainsSection(string sectionName)
        {
            return sectionCollection.ContainsKey(sectionName);
        }

        public void AddSection(string sectionName)
        {
            sectionCollection.Add(sectionName, new IniSection(sectionName));
        }
    }
}
