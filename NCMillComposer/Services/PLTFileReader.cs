using NCMillComposer.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace NCMillComposer.Services
{
    public static class PLTFileReader
    {
        public static List<Polygon> ReadFile(string filePath)
        {
            var vectorObjects = new List<Polygon>();
            var currentVectorObject = new PointCollection();
            Point? point = null;
            var lines = System.IO.File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                var trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine[0] != 'P' && (trimmedLine[1] != 'U' || trimmedLine[1] != 'D'))
                {
                    continue;
                }

                if (trimmedLine[1] == 'U')
                {
                    if (currentVectorObject.Count > 1)
                    {
                        vectorObjects.Add(new Polygon { Points = currentVectorObject });
                        currentVectorObject = [];
                    }
                }

                point = ParseCoordinates(trimmedLine.Substring(2));
                if (point.HasValue)
                {
                    currentVectorObject.Add(point.Value);
                }
            }

            if (currentVectorObject.Count > 1)
            {
                vectorObjects.Add(new Polygon { Points = currentVectorObject });
            }

            return vectorObjects;
        }

        private static Point? ParseCoordinates(string command)
        {
            string[] parts = command.Split([' ', ','], System.StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2 && float.TryParse(parts[0], out float x) && float.TryParse(parts[1].TrimEnd(';'), out float y))
            {
                return new Point(x, y);
            }

            return null;
        }
    }
}