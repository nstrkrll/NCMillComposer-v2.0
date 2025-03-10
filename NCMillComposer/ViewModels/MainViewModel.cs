using NCMillComposer.Components;
using NCMillComposer.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace NCMillComposer.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Models.Polygon> _polygons = [];
        private double _canvasWidth = 800;
        private double _canvasHeight = 600;
        private double _offsetX;
        private double _offsetY;
        private double _scale = 1.0;
        public ICommand LoadCommand { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Models.Polygon> Polygons
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
            set { _offsetX = value; OnPropertyChanged(); }
        }

        public double OffsetY
        {
            get => _offsetY;
            set { _offsetY = value; OnPropertyChanged(); }
        }

        public double Scale
        {
            get => _scale;
            set { _scale = value; OnPropertyChanged(); }
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
                var polygons = PLTFileReader.ReadFile(openFileDialog.FileName);
                PolygonHandler.SetPolygonsType(polygons);
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
            if (Polygons.Count == 0) return;

            // Находим границы чертежа
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            foreach (var polygon in Polygons)
            {
                foreach (var point in polygon.Points)
                {
                    if (point.X < minX) minX = point.X;
                    if (point.Y < minY) minY = point.Y;
                    if (point.X > maxX) maxX = point.X;
                    if (point.Y > maxY) maxY = point.Y;
                }
            }

            double width = maxX - minX;
            double height = maxY - minY;

            // Вычисляем масштаб
            double scaleX = _canvasWidth / width;
            double scaleY = _canvasHeight / height;
            Scale = Math.Min(scaleX, scaleY) * 0.9; // 90% размера для отступов

            // Вычисляем центр чертежа
            double centerX = (minX + maxX) / 2;
            double centerY = (minY + maxY) / 2;

            // Вычисляем центр Canvas
            double canvasCenterX = _canvasWidth / 2;
            double canvasCenterY = _canvasHeight / 2;

            // Вычисляем смещение
            OffsetX = canvasCenterX - centerX * Scale;
            OffsetY = canvasCenterY - centerY * Scale;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}