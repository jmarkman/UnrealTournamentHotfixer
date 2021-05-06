using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IniParser.Models
{
    public class IniSection
    {
        public string Name { get; set; }
        public IniKeyValueCollection KeyValueCollection { get; set; }

        public IniSection(string sectionName) : this(sectionName, EqualityComparer<string>.Default)
        {

        }

        public IniSection(string sectionName, IEqualityComparer<string> equalityComparer)
        {
            if (string.IsNullOrEmpty(sectionName))
            {
                throw new ArgumentException("The section name cannot be empty");
            }

            Name = sectionName;
            KeyValueCollection = new IniKeyValueCollection(equalityComparer);
        }
    }
}
