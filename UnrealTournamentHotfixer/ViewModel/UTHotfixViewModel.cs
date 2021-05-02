using Ookii.Dialogs.Wpf;
using System.ComponentModel;
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
            
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
