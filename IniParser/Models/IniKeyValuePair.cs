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
        public bool IsComment { get; private set; }


        public IniKeyValuePair(string keyName, string keyValue = "", bool isComment = false)
        {
            if (string.IsNullOrEmpty(keyName))
            {
                throw new ArgumentException("The key name cannot be null or empty");
            }

            Key = keyName;
            Value = keyValue;
            Values = new List<string>();
            IsComment = isComment;
        }

        public IniKeyValuePair(IniKeyValuePair kvp)
        {
            Key = kvp.Key;
            Value = kvp.Value;
        }

        public IEnumerator<string> GetEnumerator()
        {
            foreach (var value in Values)
            {
                yield return value;
            }
        }
    }
}
