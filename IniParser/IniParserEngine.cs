using IniParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IniParser
{
    public class IniParserEngine
    {
        private string currentSection;

        public IniParserEngine()
        {

        }

        /// <summary>
        /// Parse the contents of the INI file to a plain old C# object
        /// </summary>
        /// <param name="iniString">The contents of the INI file</param>
        /// <returns>A <see cref="IniFile"/> object that represents the 
        /// contents of the file</returns>
        public IniFile Parse(string iniString)
        {
            IniFile ini = new IniFile();

            if (string.IsNullOrEmpty(iniString))
            {
                throw new ArgumentException("There was no INI file data to process.");
            }

            var lines = iniString.Split(Environment.NewLine, StringSplitOptions.None);

            for (int i = 0; i < lines.Length; i++)
            {
                var currentLine = lines[i].Trim();

                if (currentLine == string.Empty)
                {
                    continue;
                }

                ParseLine(currentLine, ini);
            }

            return ini;
        }

        /// <summary>
        /// Pseudo-switch that determines which parsing method to use for
        /// the given line based on validation method calls within this method
        /// </summary>
        /// <param name="currentLine">The current line of the INI file to parse</param>
        /// <param name="ini">The <see cref="IniFile"/> object storing this data</param>
        private void ParseLine(string currentLine, IniFile ini)
        {
            if (CurrentLineIsA(IniLineType.Section, currentLine))
            {
                ParseAndAddSection(currentLine, ini);
            }
            else if (CurrentLineIsA(IniLineType.KeyValue, currentLine))
            {
                ParseAndAddKeyValue(currentLine, ini);
            }
            else if (CurrentLineIsA(IniLineType.Comment, currentLine))
            {
                ParseComment(currentLine, ini);
            }
            else
            {
                throw new Exception("Replace this exception with a custom one saying that the line couldn't be parsed");
            }
        }

        /// <summary>
        /// Parses the current line for its subject name and adds it to the
        /// provided <see cref="IniFile"/> object
        /// </summary>
        /// <param name="currentLine">The line to parse for its section name</param>
        /// <param name="ini">The <see cref="IniFile"/> storing this section</param>
        private void ParseAndAddSection(string currentLine, IniFile ini)
        {
            /*
             * Regex Translation:
             *  (?<=\[)
             *      This is a lookbehind to determine if the previous character was an
             *      opening bracket. When the regex reaches the first character we want
             *      (the current character) and looks back at the previous one, it'll 
             *      find an opening bracket and know that it's time to start capturing
             *      from the current character.
             *  ([\s\S]*)
             *      Just capture damn-well everything in between.
             *  (?=\])
             *      This is a lookforward to determine if the character after the current
             *      one is a closing bracket. When the regex sees this closing bracket is
             *      coming up, it'll stop capturing whatever's in the middle and bring the
             *      regex to an end.
             */ 
            var sectionRegex = Regex.Match(currentLine, @"(?<=\[)([\s\S]*)(?=\])");

            if (sectionRegex.Success)
            {
                var sectionName = sectionRegex.Value;
                currentSection = sectionName;

                //TODO: Don't want to spend time with handling whether or not user wants
                //duplicates since I'M the user and I'm fine with duplicate sections for now
                //if (!ini.Sections.ContainsSection(sectionName))
                //{
                //    ini.Sections.AddSection(sectionName);
                //}

                ini.Sections.AddSection(sectionName);
            }
        }

        /// <summary>
        /// Parses the current line for the key-value pair it contains and adds the
        /// pair to the provided <see cref="IniFile"/>
        /// </summary>
        /// <param name="currentLine">The line to parse for its key-value pair</param>
        /// <param name="ini">The <see cref="IniFile"/> storing the key-value pair</param>
        private void ParseAndAddKeyValue(string currentLine, IniFile ini)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parses the current line as an INI file comment and adds it to the provided
        /// <see cref="IniFile"/>. Not yet implemented.
        /// </summary>
        /// <param name="currentLine">The line to parse as a comment</param>
        /// <param name="ini">The <see cref="IniFile"/> storing the comment</param>
        private void ParseComment(string currentLine, IniFile ini)
        {
            throw new NotImplementedException();
        }

        private bool CurrentLineIsA(IniLineType lineType, string l)
        {
            return lineType switch
            {
                IniLineType.Section => l.StartsWith('[') && l.EndsWith(']'),
                IniLineType.KeyValue => l.Contains('='),
                IniLineType.Comment => l.StartsWith(';'),
                _ => throw new InvalidOperationException("An unrecognized line type was provided and cannot be analyzed"),
            };
        }
    }
}
