using IniParser;
using IniParser.Model;
using IniParser.Model.Configuration;
using IniParser.Parser;
using System.IO;

namespace UnrealTournamentHotfixer.Services
{
    public class ConfigEditor
    {
        /// <summary>
        /// The filename and extension of the "User" config file
        /// </summary>
        private readonly string UserConfigFile = "User.ini";

        /// <summary>
        /// The filename and extension of the "UT2004" config file
        /// </summary>
        private readonly string UT2004ConfigFile = "UT2004.ini";

        /// <summary>
        /// The "System" directory in the game installation location
        /// </summary>
        private readonly string FilePath = string.Empty;

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
        private FileIniDataParser configParser;

        /// <summary>
        /// The contents of the "User.ini" file
        /// </summary>
        private IniData userConfigData;

        /// <summary>
        /// The contents of the "UT2004.ini" file
        /// </summary>
        private IniData ut2004ConfigData;

        public ConfigEditor(string filePath)
        {
            FilePath = filePath;

            var parserConfig = new IniParserConfiguration
            {
                AllowDuplicateKeys = true,
                AssigmentSpacer = string.Empty
            };

            configParser = new FileIniDataParser(new IniDataParser(parserConfig));

            userConfigData = configParser.ReadFile(Path.Combine(FilePath, UserConfigFile));
            ut2004ConfigData = configParser.ReadFile(Path.Combine(FilePath, UT2004ConfigFile));
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
        /// Write the config changes to disk
        /// </summary>
        public void SaveChanges()
        {
            configParser.WriteFile(Path.Combine(FilePath, UserConfigFile), userConfigData);
            configParser.WriteFile(Path.Combine(FilePath, UT2004ConfigFile), ut2004ConfigData);
        }
    }
}
