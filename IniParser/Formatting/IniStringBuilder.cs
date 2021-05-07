using IniParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IniParser.Formatting
{
    public class IniStringBuilder
    {
        public string ToString(IniFile fileData)
        {
            StringBuilder sb = new();

            foreach (var section in fileData.Sections)
            {
                WriteSectionAndKeys(section, sb);
            }

            return sb.ToString();
        }

        private void WriteSectionAndKeys(IniSection section, StringBuilder sb)
        {
            // Make sure to add a new line if this isn't the first entry
            // such that the sections are properly separated
            if (sb.Length > 0)
            {
                sb.Append(Environment.NewLine);
            }

            sb.Append($"[{section.Name}]{Environment.NewLine}");

            foreach (var kvp in section.KeyValueCollection)
            {
                sb.Append($"{kvp.Key}={kvp.Value}{Environment.NewLine}");

                if (kvp.Values.Any())
                {
                    foreach (var extraValue in kvp)
                    {
                        sb.Append($"{kvp.Key}={extraValue}{Environment.NewLine}");
                    }
                }
            }
        }
    }
}
