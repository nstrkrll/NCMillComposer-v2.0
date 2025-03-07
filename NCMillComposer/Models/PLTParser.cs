using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace NCMillComposer.Models
{
    public static class PLTParser
    {
        private static float _plotterUnitsToMillimiters = 25.4f / Settings.PlotterUnitsPerInch;

        public static List<Contour> Parse(string filePath)
        {
            var contours = new List<Contour>();
            var currentContour = new List<Point>(1000);
            Point? lastPoint = null;
            var lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                var trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine[0] != 'P')
                {
                    continue;
                }

                if (trimmedLine[1] == 'U')
                {
                    if (currentContour.Count > 1)
                    {
                        contours.Add(new Contour(currentContour));
                        currentContour = new List<Point>(1000);
                    }

                    lastPoint = ParseCoordinates(trimmedLine.Substring(2));
                    if (lastPoint.HasValue)
                    {
                        currentContour.Add(ConvertToMm(lastPoint.Value));
                    }
                }
                else if (trimmedLine[1] == 'D')
                {
                    var point = ParseCoordinates(trimmedLine.Substring(2));
                    if (point.HasValue && lastPoint.HasValue)
                    {
                        if (currentContour.Count == 0)
                        {
                            currentContour.Add(ConvertToMm(lastPoint.Value));
                        }

                        currentContour.Add(ConvertToMm(point.Value));
                    }

                    lastPoint = point;
                }
            }

            if (currentContour.Count > 1)
                contours.Add(new Contour(currentContour));

            return contours;
        }

        private static Point? ParseCoordinates(string command)
        {
            string[] parts = command.Split([' ', ','], StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2 && float.TryParse(parts[0], out float x) && float.TryParse(parts[1].TrimEnd(';'), out float y))
            {
                return new Point(x, y);
            }

            return null;
        }

        private static Point ConvertToMm(Point point)
        {
            return new Point(point.X * _plotterUnitsToMillimiters, point.Y * _plotterUnitsToMillimiters);
        }
    }
}