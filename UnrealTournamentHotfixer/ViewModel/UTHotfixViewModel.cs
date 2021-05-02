using Ookii.Dialogs.Wpf;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using UnrealTournamentHotfixer.Command;
using UnrealTournamentHotfixer.Services;

namespace UnrealTournamentHotfixer.ViewModel
{
    public class UTHotfixViewModel : BaseViewModel
    {
        private string filePath;

        public string FilePath
        {
            get { return filePath; }
            set
            {
                filePath = value;
                OnPropertyChanged(nameof(FilePath));
                OnPropertyChanged(nameof(CanApplyHotfix));
                ApplyHotfixCommand.RaiseCanExecuteChanged();
            }
        }

        public bool CanApplyHotfix => !string.IsNullOrEmpty(FilePath);
        public DelegateCommand ApplyHotfixCommand { get; }
        public DelegateCommand BrowseCommand { get; }

        public UTHotfixViewModel()
        {
            ApplyHotfixCommand = new DelegateCommand(ApplyHotfix, () => CanApplyHotfix);
            BrowseCommand = new DelegateCommand(BrowseForGamePath);
        }

        public void BrowseForGamePath()
        {
            VistaFolderBrowserDialog fbd = new()
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
            CleanIniFile(FilePath);
            ConfigEditor configEditor = new ConfigEditor(FilePath);

            configEditor.AdjustInternetSpeed();
            configEditor.CreateNetspeedToggleBind();
            configEditor.AdjustMaxClientFrameRate();
            configEditor.AddFirewallSectionIfNotPresent();

            configEditor.SaveChanges();

            MessageBox.Show("Hotfixes applied");

            FilePath = string.Empty;
        }

        /// <summary>
        /// For some godforsaken reason, in the UT2004.ini file, the names for
        /// the four MapListRecord entries have the character \u001b instead of
        /// a normal empty space character. It's JUST for this file and JUST in
        /// those four spots. This performs the prior work to clean that file.
        /// </summary>
        private void CleanIniFile(string path)
        {
            var ini = File.ReadAllLines(Path.Combine(path, "UT2004.ini"));
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

            File.WriteAllText(Path.Combine(path, "UT2004.ini"), string.Empty);
            File.WriteAllLines(Path.Combine(path, "UT2004.ini"), updated);
        }
    }
}
