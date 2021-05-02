using IniParser;
using IniParser.Model.Configuration;
using IniParser.Parser;
using Ookii.Dialogs.Wpf;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using UnrealTournamentHotfixer.Command;

namespace UnrealTournamentHotfixer.ViewModel
{
    public class UTHotfixViewModel : INotifyPropertyChanged
    {
        private string filePath;

        public string FilePath
        {
            get { return filePath; }
            set 
            {
                if (!string.IsNullOrEmpty(value))
                {
                    filePath = value;
                    OnPropertyChanged(nameof(FilePath));
                    OnPropertyChanged(nameof(CanApplyHotfix));
                    ApplyHotfixCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool CanApplyHotfix => !string.IsNullOrEmpty(FilePath);
        public DelegateCommand ApplyHotfixCommand { get; }
        public DelegateCommand BrowseCommand { get; }
        public event PropertyChangedEventHandler PropertyChanged;

        public UTHotfixViewModel()
        {
            ApplyHotfixCommand = new DelegateCommand(ApplyHotfix, () => CanApplyHotfix);
            BrowseCommand = new DelegateCommand(BrowseForGamePath);
        }

        public void BrowseForGamePath()
        {
            VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog
            {
                Description = "Locate the 'System' folder in the Unreal Tournament 2004 folder",
                UseDescriptionForTitle = true
            };

            if ((bool)fbd.ShowDialog())
            {
                FilePath = fbd.SelectedPath;
            }
        }

        public void ApplyHotfix()
        {
            // TODO: Extract these to some kind of constants class
            var userConfig = "User.ini";
            var ut2k4Config = "UT2004.ini";
            var playerNetworkSettings = "Engine.Player";
            var inputSettings = "Engine.Input";
            var netspeed = "netspeed";
            var internetSpeedSetting = "ConfiguredInternetSpeed";
            var firewallSection = "FireWall";
            var ignoreSP2Setting = "IgnoreSP2";
            var properFramerateInternetSpeed = "10001";

            /*
             * Plan of action:
             * 1. Read the User.ini
             * 2. Find the "Engine.Player" section
             * 3. Read the "ConfiguredInternetSpeed" setting
             * 4. If the CIS setting isn't set to 10001, change it
             * 5. Save the changes
             * 6. Read the UT2004.ini
             * 7. See if there's an existing section called "FireWall" before proceeding
             * 8. Add a new section called "FireWall" if it isn't there
             * 9. If the section exists, check that the key "IgnoreSP2" is set to "1"
             * 10. If the "IgnoreSP2" key isn't set to 1, set it
             * 11. If it doesn't exist, add the key-value pair "IgnoreSP2=1"
             * 12. Save the changes
             */

            // The configs have duplicate keys with unique values, and
            // IniParser defaults to "key = value" over UT's "key=value"
            // convention. If the convention isn't followed, it'll break
            // UT2k4 and cause a CD Key error or similar.
            var parserConfig = new IniParserConfiguration
            {
                AllowDuplicateKeys = true,
                AssigmentSpacer = string.Empty
            };

            var iniParser = new FileIniDataParser(new IniDataParser(parserConfig));

            var userCfgData = iniParser.ReadFile(Path.Combine(FilePath, userConfig));

            var internetSpeed = userCfgData[playerNetworkSettings][internetSpeedSetting];

            if (internetSpeed != properFramerateInternetSpeed)
            {
                userCfgData[playerNetworkSettings][internetSpeedSetting] = properFramerateInternetSpeed;
                iniParser.WriteFile(Path.Combine(FilePath, userConfig), userCfgData);
            }

            var ut2k4CfgData = iniParser.ReadFile(Path.Combine(FilePath, ut2k4Config));

            if (ut2k4CfgData[firewallSection] == null)
            {
                ut2k4CfgData.Sections.AddSection(firewallSection);
                ut2k4CfgData[firewallSection].AddKey(ignoreSP2Setting, "1");
            }
            else
            {
                if (ut2k4CfgData[firewallSection][ignoreSP2Setting] != "1")
                {
                    ut2k4CfgData[firewallSection][ignoreSP2Setting] = "1";
                }
            }

            iniParser.WriteFile(Path.Combine(FilePath, ut2k4Config), ut2k4CfgData);

            // This is Not Good™ - replace with a WindowService
            // demonstrated at https://stackoverflow.com/a/47353136
            TaskCompleteWindow complete = new TaskCompleteWindow();
            complete.Show();
            FilePath = string.Empty;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
