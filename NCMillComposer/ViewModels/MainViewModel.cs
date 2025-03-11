using Microsoft.Win32;
using NCMillComposer.Components;
using NCMillComposer.Models;
using NCMillComposer.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace NCMillComposer.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Polygon> _polygons = [];
        private double _canvasWidth = 600;
        private double _canvasHeight = 800;
        private double _offsetX;
        private double _offsetY;
        private double _scale = 1.0;

        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand LoadCommand { get; private set; }

        public ObservableCollection<Polygon> Polygons
        {
            get => _polygons;
            set
            {
                _polygons = value;
                OnPropertyChanged();
            }
        }

        public double CanvasWidth
        {
            get => _canvasWidth;
            set
            {
                _canvasWidth = value;
                OnPropertyChanged();
                AdjustContours();
            }
        }

        public double CanvasHeight
        {
            get => _canvasHeight;
            set
            {
                _canvasHeight = value;
                OnPropertyChanged();
                AdjustContours();
            }
        }

        public double OffsetX
        {
            get => _offsetX;
            set 
            { 
                _offsetX = value; 
                OnPropertyChanged(); 
            }
        }

        public double OffsetY
        {
            get => _offsetY;
            set 
            { 
                _offsetY = value; 
                OnPropertyChanged(); 
            }
        }

        public double Scale
        {
            get => _scale;
            set 
            { 
                _scale = value; 
                OnPropertyChanged(); 
            }
        }

        public MainViewModel()
        {
            LoadCommand = new RelayCommand(LoadPLT);
        }

        public void LoadPLT(object parameter)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "PLT files (*.plt)|*.plt"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var polygons = PLTFileReader.ReadFile(openFileDialog.FileName);
                PolygonHandler.SetPolygonsType(polygons);
                PolygonHandler.ConnectOpenPolygons(polygons);
                PolygonHandler.ConvertCoordinatesToMillimiters(polygons);
                PolygonHandler.NormalizePolygonsCoordinates(polygons);
                Polygons.Clear();
                foreach (var polygon in polygons)
                {
                    Polygons.Add(polygon);
                }

                AdjustContours();
            }
        }

        private void AdjustContours()
        {
            if (Polygons.Count == 0)
            {
                return;
            }

            var maxX = Polygons.Max(x => x.MaxX);
            var maxY = Polygons.Max(x => x.MaxY);
            var minX = Polygons.Min(x => x.MinX);
            var minY = Polygons.Min(x => x.MinY);
            double scaleX = CanvasWidth / maxX;
            double scaleY = CanvasHeight / maxY;
            Scale = Math.Min(scaleX, scaleY);
            double centerX = (minX + maxX) / 2;
            double centerY = (minY + maxY) / 2;
            double canvasCenterX = CanvasWidth / 2;
            double canvasCenterY = CanvasHeight / 2;
            OffsetX = (CanvasWidth - maxX * Scale) / 2;
            OffsetY = (CanvasHeight - maxY * Scale) / 2;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}