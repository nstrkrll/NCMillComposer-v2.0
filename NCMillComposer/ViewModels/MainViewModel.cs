using NCMillComposer.Components;
using NCMillComposer.Models;
using NCMillComposer.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;

namespace NCMillComposer.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _openedFilePath;
        private string _fileName;
        private ObservableCollection<FileInfo> _filesList;
        private List<Polygon> _polygons;
        private ObservableCollection<Polygon> _polygonsForDraw;
        private double _canvasWidth;
        private double _canvasHeight;

        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand OpenFileCommand { get; private set; }
        public ICommand OnWindowLoadedCommand { get; private set; }
        public ICommand OnWindowClosingCommand { get; private set; }
        public ICommand SelectFirstDirectoryForSearchCommand { get; private set; }
        public ICommand SelectSecondDirectoryForSearchCommand { get; private set; }
        public ICommand OpenSelectedFileCommand { get; }

        public string Title
        {
            get
            {
                return $"{Settings.ProgramName} {Settings.Version}" + (string.IsNullOrWhiteSpace(_fileName) ? "" : " - " + _fileName);
            }
        }

        public ObservableCollection<FileInfo> FilesList
        {
            get => _filesList;
            set
            {
                _filesList = value;
                OnPropertyChanged(nameof(FilesList));
            }
        }

        public ObservableCollection<Polygon> PolygonsForDraw
        {
            get => _polygonsForDraw;
            set
            {
                if (_polygonsForDraw != value)
                {
                    _polygonsForDraw = value;
                    OnPropertyChanged(nameof(PolygonsForDraw));
                }
            }
        }

        public double CanvasWidth
        {
            get => _canvasWidth;
            set
            {
                if (_canvasWidth != value && value > 0)
                {
                    _canvasWidth = value;
                    OnPropertyChanged(nameof(CanvasWidth));
                    RedrawObjects();
                }
            }
        }

        public double CanvasHeight
        {
            get => _canvasHeight;
            set
            {
                if (_canvasHeight != value && value > 0)
                {
                    _canvasHeight = value;
                    OnPropertyChanged(nameof(CanvasHeight));
                    RedrawObjects();
                }
            }
        }

        public MainViewModel()
        {
            OnPropertyChanged(nameof(Title));
            _polygons = [];
            _polygonsForDraw = [];
            _filesList = [];
            PropertyChanged = delegate { };
            OnWindowLoadedCommand = new RelayCommand(OnWindowLoaded);
            OnWindowClosingCommand = new RelayCommand(OnWindowClosing);
            OpenFileCommand = new RelayCommand(OpenFile);
            SelectFirstDirectoryForSearchCommand = new RelayCommand(SelectFirstDirectoryForSearch);
            SelectSecondDirectoryForSearchCommand = new RelayCommand(SelectSecondDirectoryForSearch);
            OpenSelectedFileCommand = new RelayCommand<FileInfo>(OpenSelectedFile);
        }

        private void OnWindowLoaded(object parameter)
        {
            LoadFiles();
        }

        private void OnWindowClosing(object parameter)
        {
            MessageBox.Show("Закрытие окна");
        }

        private void OpenFile(object parameter)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Filter = "PLT files (*.plt)|*.plt"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _openedFilePath = openFileDialog.FileName;
                _fileName = openFileDialog.SafeFileName;
                OnPropertyChanged(nameof(Title));
                LoadPLT();
            }
        }

        private void OpenSelectedFile(FileInfo parameter)
        {
            _openedFilePath = parameter.FullName;
            _fileName = parameter.Name;
            OnPropertyChanged(nameof(Title));
            LoadPLT();
        }

        private void LoadPLT()
        {
            _polygons = PLTFileReader.ReadFile(_openedFilePath);
            PolygonHandler.SetPolygonsType(_polygons);
            PolygonHandler.ConnectOpenPolygons(_polygons);
            PolygonHandler.ConvertCoordinatesToMillimiters(_polygons);
            PolygonHandler.NormalizePolygonsCoordinates(_polygons);
            RedrawObjects();
        }

        private void SelectFirstDirectoryForSearch(object parameter)
        {
            using var dialog = new FolderBrowserDialog
            {
                ShowNewFolderButton = true,
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Settings.FirstDirectoryForSearch = dialog.SelectedPath;
            }

            LoadFiles();
        }
        
        private void SelectSecondDirectoryForSearch(object parameter)
        {
            using var dialog = new FolderBrowserDialog
            {
                ShowNewFolderButton = true,
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Settings.SecondDirectoryForSearch = dialog.SelectedPath;
            }

            LoadFiles();
        }

        private void LoadFiles()
        {
            FilesList.Clear();
            var filesList = FilesHandler.GetFilesList();
            foreach (var file in filesList)
            {
                FilesList.Add(file);
            }
        }

        private void RedrawObjects()
        {
            if (_polygons != null && _polygons.Count > 0)
            {
                var polygonsForDraw = PolygonHandler.GetPolygonsForDraw(_polygons, CanvasWidth - 10, CanvasHeight - 10);
                PolygonsForDraw.Clear();
                foreach (var polygon in polygonsForDraw)
                {
                    PolygonsForDraw.Add(polygon);
                }
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}