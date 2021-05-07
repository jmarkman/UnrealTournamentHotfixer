using IniParser;
using IniParser.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace UnrealTournamentHotfixer.Services
{
    public class ConfigEditor
    {
        /// <summary>
        /// The path to the "User" config file
        /// </summary>
        private readonly string PathToUserIni;

        /// <summary>
        /// The path to the "UT2004" config file
        /// </summary>
        private readonly string PathTo2k4Ini;

        /// <summary>
        /// The console command for changing packet send speed in UT2k4
        /// </summary>
        private readonly string NetspeedCommand = "netspeed";

        /// <summary>
        /// The "magic number" for adjusting netspeed
        /// </summary>
        private readonly string NetspeedValue = "10001";

        /// <summary>
        /// The most stable framerate that would provide a smooth experience
        /// while not causing any weird physics issues or dips in framerate
        /// </summary>
        private readonly string StableMaxClientFrameRate = "112.0";

        /// <summary>
        /// The .ini file parser
        /// </summary>
        private IniFileParser configParser;

        /// <summary>
        /// The contents of the "User.ini" file
        /// </summary>
        private IniFile userConfigData;

        /// <summary>
        /// The contents of the "UT2004.ini" file
        /// </summary>
        private IniFile ut2004ConfigData;

        public ConfigEditor(string filePath)
        {
            PathToUserIni = Path.Combine(filePath, "User.ini");
            PathTo2k4Ini = Path.Combine(filePath, "UT2004.ini");

            configParser = new();
            GetCleanUT2k4IniFileContents();

            userConfigData = configParser.ReadFile(PathToUserIni);
            ut2004ConfigData = configParser.ReadFile(PathTo2k4Ini);
        }

        /// <summary>
        /// Change the ConfiguredInternetSpeed setting in the User.ini file to unlock the
        /// framerate. In practice this doesn't work for everyone and the 'netspeed toggle'
        /// bind will be necessary, but we'll do it anyway.
        /// </summary>
        public void AdjustInternetSpeed()
        {
            var playerNetworkSettings = "Engine.Player";
            var internetSpeedSetting = "ConfiguredInternetSpeed";

            var internetSpeed = userConfigData[playerNetworkSettings][internetSpeedSetting];

            if (internetSpeed != NetspeedValue)
            {
                userConfigData[playerNetworkSettings][internetSpeedSetting] = NetspeedValue;
            }
        }

        /// <summary>
        /// This will bind the "P" key (generally unused key) to the command "netspeed 10001",
        /// which will adjust the number of packets being sent from the game client to the server
        /// such that the frame rate won't be locked at 84 fps
        /// </summary>
        public void CreateNetspeedToggleBind()
        {
            var inputSettings = "Engine.Input";

            userConfigData[inputSettings]["P"] = $"{NetspeedCommand} {NetspeedValue}";
        }

        /// <summary>
        /// This will adjust the MaxClientFrameRate setting to the most stable maximum framerate I
        /// found to work without odd frame dips
        /// </summary>
        public void AdjustMaxClientFrameRate()
        {
            var clientFramerateSection = "Engine.LevelInfo";
            var clientFrameRateSetting = "MaxClientFrameRate";

            ut2004ConfigData[clientFramerateSection][clientFrameRateSetting] = StableMaxClientFrameRate;
        }

        /// <summary>
        /// For those who want to use the server browser, this applies a config fix 
        /// that tells UT2k4 to ignore the Windows Firewall even though it can still
        /// connect to servers using the "open" command from the firewall. Whatever.
        /// </summary>
        public void AddFirewallSectionIfNotPresent()
        {
            var firewallSection = "FireWall";
            var ignoreSP2Setting = "IgnoreSP2";

            if (ut2004ConfigData[firewallSection] == null)
            {
                ut2004ConfigData.Sections.AddSection(firewallSection);
                ut2004ConfigData[firewallSection].AddKey(ignoreSP2Setting, "1");
            }
            else
            {
                if (ut2004ConfigData[firewallSection][ignoreSP2Setting] != "1")
                {
                    ut2004ConfigData[firewallSection][ignoreSP2Setting] = "1";
                }
            }
        }

        /// <summary>
        /// Write the config changes to disk. UT24k .INI files don't play well
        /// with the game if they're saved in UTF-8 so make sure that they're
        /// written to disk in ASCII
        /// </summary>
        public void SaveChanges()
        {
            configParser.WriteFile(PathToUserIni, userConfigData, Encoding.ASCII);
            configParser.WriteFile(PathTo2k4Ini, ut2004ConfigData, Encoding.ASCII);
        }

        /// <summary>
        /// For some godforsaken reason, in the UT2004.ini file, the names for
        /// the four MapListRecord entries have the character \u001b instead of
        /// a normal empty space character. It's JUST for this file and JUST in
        /// those four spots. This method will simultaneously read the lines from
        /// the file for usage in this class and prune the ESC (\u001b) characters. 
        /// </summary>
        /// <returns>The contents of the file as a list of strings</returns>
        private void GetCleanUT2k4IniFileContents()
        {
            var ini = File.ReadAllLines(PathTo2k4Ini);
            List<string> updated = new();

            for (int i = 0; i < ini.Length; i++)
            {
                var currentLine = ini[i];
                if (currentLine.Contains('\u001b'))
                {
                    var cleanedLine = Regex.Replace(currentLine, @"[\u001b]", " ");
                    updated.Add(cleanedLine);
                }
                else
                {
                    updated.Add(currentLine);
                }
            }

            File.WriteAllText(PathTo2k4Ini, string.Empty);
            File.WriteAllLines(PathTo2k4Ini, updated);
        }
    }
}
