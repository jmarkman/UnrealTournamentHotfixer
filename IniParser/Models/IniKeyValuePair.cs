using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IniParser.Models
{
    public class IniKeyValuePair
    {
        public string Key { get; private set; }
        public string Value { get; set; }
        public List<string> Values { get; set; }

        public IniKeyValuePair(string keyName, string keyValue = "")
        {
            if (string.IsNullOrEmpty(keyName))
            {
                throw new ArgumentException("The key name cannot be null or empty");
            }

            Key = keyName;
            Value = keyValue;
        }

        public IniKeyValuePair(IniKeyValuePair kvp)
        {
            Key = kvp.Key;
            Value = kvp.Value;
        }
    }
}
