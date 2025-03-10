using NCMillComposer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace NCMillComposer.Services
{
    /// <summary>
    /// Класс, предоставляющий методы для обработки объектов
    /// </summary>
    public static class PolygonHandler
    {
        /// <summary>
        /// Добавляет точки второго объекта в конец первого
        /// </summary>
        /// <param name="target">Объект, в который добавляются точки</param>
        /// <param name="source">Объект, из которого берутся точки</param>
        /// <param name="reverse">Флаг переворота второго объекта</param>
        private static void AppendPolygon(Polygon target, Polygon source, bool reverse)
        {
            var pointsToAdd = reverse ? source.Points.Reverse().Skip(1) : source.Points.Skip(1);
            foreach (var point in pointsToAdd)
            {
                target.Points.Add(point);
            }
        }

        /// <summary>
        /// Добавляет точки второго объекта в начало первого
        /// </summary>
        /// <param name="target">Объект, в который добавляются точки</param>
        /// <param name="source">Объект, из которого берутся точки</param>
        /// <param name="reverse">Флаг переворота второго объекта</param>
        private static void PrependPolygon(Polygon target, Polygon source, bool reverse)
        {
            var pointsToAdd = reverse ? source.Points.Reverse().SkipLast(1) : source.Points.SkipLast(1);
            for (var i = pointsToAdd.Count() - 1; i >= 0; i--)
            {
                target.Points.Insert(0, pointsToAdd.ElementAt(i));
            }
        }

        /// <summary>
        /// Устанавливает тип каждого объекта (P - закрытый; L - открытый)
        /// </summary>
        /// <param name="polygons">Список объектов</param>
        public static void SetPolygonsType(List<Polygon> polygons)
        {
            foreach (var polygon in polygons)
            {
                if (polygon.ObjectType == 'P')
                {
                    continue;
                }

                if (polygon.Points.First() == polygon.Points.Last())
                {
                    polygon.ObjectType = 'P';
                    polygon.Points.RemoveAt(polygon.Points.Count - 1);
                    continue;
                }

                polygon.ObjectType = 'L';
            }
        }

        /// <summary>
        /// Соединяет открытые объекты друг с другом
        /// </summary>
        /// <param name="polygons">Список объектов</param>
        /// <returns>Количество соединений объектов</returns>
        public static int ConnectOpenPolygons(List<Polygon> polygons)
        {
            var connectsCount = 0;
            for (var i = 0; i < polygons.Count; i++)
            {
                if (polygons[i].ObjectType != 'L')
                {
                    continue;
                }

                for (var j = 0; j < polygons.Count; j++)
                {
                    if (i == j || polygons[j].ObjectType != 'L')
                    {
                        continue;
                    }

                    if (polygons[i].Points.Last() == polygons[j].Points.First()) // Соединение хвост - голова (второй объект помещается в конец первого)
                    {
                        connectsCount++;
                        AppendPolygon(polygons[i], polygons[j], false);
                        polygons.RemoveAt(j);
                        break;
                    }

                    if (polygons[i].Points.Last() == polygons[j].Points.Last()) // Соединение хвост - хвост (второй объект переворачивается и помещается в конец первого)
                    {
                        connectsCount++;
                        AppendPolygon(polygons[i], polygons[j], true);
                        polygons.RemoveAt(j);
                        break;
                    }

                    if (polygons[i].Points.First() == polygons[j].Points.First()) // Соединение голова - голова (второй объект переворачивается и помещается перед первым)
                    {
                        connectsCount++;
                        PrependPolygon(polygons[i], polygons[j], true);
                        polygons.RemoveAt(j);
                        break;
                    }

                    if (polygons[i].Points.First() == polygons[j].Points.Last()) // Соединение голова - хвост (второй объект помещается перед первым)
                    {
                        connectsCount++;
                        PrependPolygon(polygons[i], polygons[j], false);
                        polygons.RemoveAt(j);
                        break;
                    }
                }
            }

            return connectsCount;
        }

        /// <summary>
        /// Переводит значения координат точек из шагов плоттера в миллиметры c учетом текущего масштаба
        /// </summary>
        /// <param name="polygons">Список объектов</param>
        public static void ConvertCoordinatesToMillimiters(List<Polygon> polygons)
        {
            float conversionFactor = Settings.PlotterUnitsPerInch * 25.4f * (Settings.Scale / 100f);
            foreach (var polygon in polygons)
            {
                for (var j = 0; j < polygon.Points.Count; j++)
                {
                    polygon.Points[j] = new Point
                    {
                        X = polygon.Points[j].X / conversionFactor,
                        Y = polygon.Points[j].Y / conversionFactor,
                    };
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="polygons"></param>
        public static void NormalizePolygonsCoordinates(List<Polygon> polygons)
        {
            var minX = polygons[0].Points[0].X; // указываем, что X первой точки - минимальный X
            var minY = polygons[0].Points[0].Y; // указываем, что Y первой точки - минимальный Y
            for (var i = 0; i < polygons.Count; i++)
            {
                
            }
        }

        /*
        public static void ConvertCoordinatesToMillimiters(System.Collections.Generic.List<Polygon> polygons)
        {
            foreach(var polygon in polygons)
            {
                foreach (var point in polygon.Points)
                {
 
                }
            }
        }

        private static System.Windows.Point ConvertToMm(System.Windows.Point point)
        {
            return new System.Windows.Point(point.X * _plotterUnitsToMillimiters, point.Y * _plotterUnitsToMillimiters);
        }
        */
    }
}