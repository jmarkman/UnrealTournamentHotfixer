using Ookii.Dialogs.Wpf;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnrealTournamentHotfixer.Command;
using UnrealTournamentHotfixer.Services;

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
            ConfigEditor configEditor = new ConfigEditor(FilePath);

            configEditor.AdjustInternetSpeed();
            configEditor.CreateNetspeedToggleBind();
            configEditor.AdjustMaxClientFrameRate();
            configEditor.AddFirewallSectionIfNotPresent();

            configEditor.SaveChanges();

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
