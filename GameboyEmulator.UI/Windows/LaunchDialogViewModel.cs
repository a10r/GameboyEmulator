using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GameboyEmulator.UI.Windows
{
    public class LaunchDialogViewModel : INotifyPropertyChanged
    {
        private string _bootromFile;
        private string _romFile;

        public string BootromFile
        {
            get => _bootromFile;
            set
            {
                _bootromFile = value;
                OnPropertyChanged();
            }
        }

        public string RomFile
        {
            get => _romFile;
            set
            {
                _romFile = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}