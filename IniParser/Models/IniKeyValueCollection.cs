using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IniParser.Models
{
    public class IniKeyValueCollection
    {
        /// <summary>
        /// Get the number of entries in the <see cref="IniKeyValueCollection"/>
        /// </summary>
        public int Count 
        { 
            get { return kvpCollection.Count; } 
        }

        /// <summary>
        /// Get or set the value of an existing key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>If the key exists in the collection, the value will be returned.
        /// Otherwise, the indexer will return null.</returns>
        public string this[string key]
        {
            get 
            {
                if (ContainsKey(key))
                {
                    return kvpCollection[key].Value;
                }

                return null;
            }
            set 
            {
                if (ContainsKey(key) && string.IsNullOrEmpty(kvpCollection[key].Value))
                {
                    kvpCollection[key].Value = value;
                }
                else if (ContainsKey(key))
                {
                    var existingKeyValuePair = kvpCollection[key];
                    existingKeyValuePair.Values.Add(value);
                }
                else
                {
                    AddKey(key);
                    if (value != null)
                    {
                        kvpCollection[key].Value = value;
                    }
                }
            }
        }

        private Dictionary<string, IniKeyValuePair> kvpCollection;

        public IniKeyValueCollection() : this(EqualityComparer<string>.Default) { }

        /// <summary>
        /// Provide a custom comparer for the <see cref="IniKeyValueCollection"/> to use
        /// when checking for key existence in the backing field
        /// </summary>
        /// <param name="equalityComparer"></param>
        public IniKeyValueCollection(IEqualityComparer<string> equalityComparer)
        {
            kvpCollection = new Dictionary<string, IniKeyValuePair>(equalityComparer);
        }

        /// <summary>
        /// Add a new key and its specified value to the collection
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="keyValue"></param>
        /// <returns><see cref="true"/> if the addition was successful, <see cref="false"/> otherwise</returns>
        public bool AddKey(string keyName, string keyValue)
        {
            // TODO: will always return true due to the nature of INI files with duplicate keys
            if (!kvpCollection.ContainsKey(keyName))
            {
                kvpCollection.Add(keyName, new IniKeyValuePair(keyName, keyValue));
                return true;
            }
            else
            {
                kvpCollection[keyName].Values.Add(keyValue);
                return true;
            }
        }

        /// <summary>
        /// Add a new key without a value to the collection
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns><see cref="true"/> if the addition was successful, <see cref="false"/> otherwise</returns>
        public bool AddKey(string keyName)
        {
            if (!kvpCollection.ContainsKey(keyName))
            {
                kvpCollection.Add(keyName, new IniKeyValuePair(keyName));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Add a <see cref="IniKeyValuePair"/> to the collection
        /// </summary>
        /// <param name="kvp"></param>
        /// <returns><see cref="true"/> if the addition was successful, <see cref="false"/> otherwise</returns>
        public bool AddKey(IniKeyValuePair kvp)
        {
            if (!kvpCollection.ContainsKey(kvp.Key))
            {
                kvpCollection.Add(kvp.Key, kvp);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determine if the supplied key exists in the collection
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns><see cref="true"/> if the key exists, <see cref="false"/> otherwise</returns>
        public bool ContainsKey(string keyName)
        {
            return kvpCollection.ContainsKey(keyName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public IniKeyValuePair TryGetValue(string keyName)
        {
            var kvpExists = kvpCollection.TryGetValue(keyName, out IniKeyValuePair iniKvp);

            if (kvpExists)
            {
                return iniKvp;
            }

            return default;
        }

        public IEnumerator<IniKeyValuePair> GetEnumerator()
        {
            foreach (var key in kvpCollection.Keys)
            {
                yield return kvpCollection[key];
            }
        }
    }
}
