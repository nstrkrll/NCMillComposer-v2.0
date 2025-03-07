using NCMillComposer.Components;
using NCMillComposer.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace NCMillComposer.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Contour> _contours = [];
        public ICommand LoadCommand { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Contour> Contours
        {
            get => _contours;
            set
            {
                _contours = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            LoadCommand = new RelayCommand(LoadPLT);
        }

        public void LoadPLT(object parameter)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "PLT files (*.plt)|*.plt"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var contours = PLTParser.Parse(openFileDialog.FileName);
                Contours.Clear();
                foreach (var contour in contours)
                {
                    Contours.Add(contour);
                }
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}