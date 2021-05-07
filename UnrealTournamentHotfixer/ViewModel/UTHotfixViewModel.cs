using Ookii.Dialogs.Wpf;
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
            ConfigEditor configEditor = new(FilePath);

            configEditor.AdjustInternetSpeed();
            configEditor.CreateNetspeedToggleBind();
            configEditor.AdjustMaxClientFrameRate();
            configEditor.AddFirewallSectionIfNotPresent();

            configEditor.SaveChanges();

            MessageBox.Show(@"Hotfixes Applied.

If you join a server and you're stuck at 85 fps, press the 'p' key to unlock the frame rate. You'll have to do this every round.
", "Success");

            FilePath = string.Empty;
        }
    }
}
