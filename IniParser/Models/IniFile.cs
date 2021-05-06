using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IniParser.Models
{
    public class IniFile
    {
        private IniSectionCollection sectionCollection;
        public IniKeyValueCollection Globals { get; set; }

        public IniSectionCollection Sections
        {
            get { return sectionCollection; }
        }

        public IniKeyValueCollection this[string sectionName] 
        {
            get 
            { 
                if (sectionCollection.ContainsSection(sectionName))
                {
                    return sectionCollection[sectionName];
                }
                else
                {
                    return null;
                }
            }
            //TODO:
            //set
            //{
            
            //}
        }

        public IniFile()
        {
            sectionCollection = new IniSectionCollection();
            Globals = new IniKeyValueCollection();
        }
    }
}
