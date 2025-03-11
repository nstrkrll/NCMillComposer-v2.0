using NCMillComposer.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NCMillComposer.Helpers
{
    public class DrawingCanvas : Canvas
    {
        private readonly VisualCollection _visuals;
        private readonly Pen _blackPen;
        private readonly Pen _redPen;
        private readonly Pen _grayPen;
        private readonly Pen _bluePen;

        protected override int VisualChildrenCount => _visuals.Count;

        protected override Visual GetVisualChild(int index) => _visuals[index];

        public DrawingCanvas()
        {
            _visuals = new VisualCollection(this);
            _blackPen = new Pen(Brushes.Black, 1);
            _redPen = new Pen(Brushes.Red, 1);
            _grayPen = new Pen(Brushes.Gray, 1);
            _bluePen = new Pen(Brushes.Blue, 1);
        }

        private Pen GetPenForPolygon(Polygon polygon)
        {
            return polygon.ObjectType switch
            {
                'X' => _grayPen,
                'L' => _bluePen,
                'P' => _blackPen,
                _ => _blackPen,
            };
        }

        public void DrawPolygons(ObservableCollection<Polygon> polygons)
        {
            _visuals.Clear();
            if (polygons == null || polygons.Count == 0)
            {
                return;
            }

            var visual = new DrawingVisual();
            using (DrawingContext dc = visual.RenderOpen())
            {
                var workingWidth = ActualWidth - 10;
                var workingHeight = ActualHeight - 10;
                var maxX = polygons.Max(x => x.MaxX);
                var maxY = polygons.Max(x => x.MaxY);
                var scaleX = workingWidth / maxX;
                var scaleY = workingHeight / maxY;
                var scale = scaleX > scaleY ? scaleY : scaleX;
                var shiftX = (workingWidth - maxX * scale) / 2;
                var shiftY = (workingHeight - maxY * scale) / 2;
                foreach (var polygon in polygons)
                {
                    var pen = GetPenForPolygon(polygon);
                    for (int i = 0; i < polygon.Points.Count - 1; i++)
                    {
                        var x1 = polygon.Points[i].X * scale + shiftX;
                        var y1 = workingHeight - polygon.Points[i].Y * scale - shiftY;
                        var x2 = polygon.Points[i + 1].X * scale + shiftX;
                        var y2 = workingHeight - polygon.Points[i + 1].Y * scale - shiftY;
                        dc.DrawLine(pen, new Point(x1, y1), new Point(x2, y2));
                    }

                    /*
                    if (polygon.ObjectType == 'P')
                    {
                        var centerX = (polygon.MaxX - polygon.MinX) / 2 * scale + shiftX;
                        var centerY = workingHeight - (polygon.MaxY - polygon.MinY) / 2 * scale - shiftY;
                        dc.DrawLine(_grayPen, new Point(centerX - 2, centerY), new Point(centerX + 2, centerY));
                        dc.DrawLine(_grayPen, new Point(centerX, centerY - 2), new Point(centerX, centerY + 2));
                    }
                    */
                }
            }

            _visuals.Add(visual);
        }
    }
}