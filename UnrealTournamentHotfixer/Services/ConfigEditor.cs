using IniParser;
using IniParser.Model.Configuration;
using IniParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnrealTournamentHotfixer.Services
{
    public class ConfigEditor
    {
        private FileIniDataParser configParser;
        private readonly string UserConfigFile = "User.ini";
        private readonly string UT2004ConfigFile = "UT2004.ini";

        public ConfigEditor()
        {
            var parserConfig = new IniParserConfiguration
            {
                AllowDuplicateKeys = true,
                AssigmentSpacer = string.Empty
            };

            configParser = new FileIniDataParser(new IniDataParser(parserConfig));
        }
    }
}
