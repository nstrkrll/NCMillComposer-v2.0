using System.Collections.Generic;
using System.Windows;

namespace NCMillComposer.Models
{
    public class Contour
    {
        public List<Point> Points { get; set; } = [];

        public Contour() { }

        public Contour(IEnumerable<Point> points)
        {
            Points.AddRange(points);
        }
    }
}