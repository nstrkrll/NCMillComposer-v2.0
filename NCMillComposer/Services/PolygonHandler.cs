using NCMillComposer.Models;
using System;
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
        /// Ищет калибровочные точки (перекрестия)
        /// </summary>
        /// <param name="polygons">Список объектов</param>
        private static void FindCallibraitingPoints(List<Polygon> polygons)
        {
            foreach (var polygon1 in polygons)
            {
                if (polygon1.ObjectType != 'L' || polygon1.ObjectType == 'X') // если объект не открытый - точно не то, что нужно
                {
                    continue;
                }

                if (polygon1.Points.First().Y == polygon1.Points.Last().Y && polygon1.Points.Count == 2) // объект - прямая горизонтальная линия
                {
                    var centerX = (polygon1.MaxX + polygon1.MinX) / 2;
                    foreach (var polygon2 in polygons)
                    {
                        if (polygon2.ObjectType != 'L' || polygon2.ObjectType == 'X') // если объект не открытый - точно не то, что нужно
                        {
                            continue;
                        }

                        if (polygon2.Points.First().X == polygon2.Points.Last().X && polygon2.Points.Count == 2) // объект - прямая вертикальная линия
                        {
                            var centerY = (polygon2.MaxY + polygon2.MinY) / 2;
                            var deviationX = Math.Abs(centerX - polygon2.MaxX);
                            var deviationY = Math.Abs(centerY - polygon1.MaxY);
                            if (deviationX <= 1 && deviationY <= 1)
                            {
                                polygon1.ObjectType = 'X';
                                polygon2.ObjectType = 'X';
                                break;
                            }
                        }
                    }
                }
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
                    //polygon.Points.RemoveAt(polygon.Points.Count - 1);
                    continue;
                }

                polygon.ObjectType = 'L';
            }

            FindCallibraitingPoints(polygons);
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
            float conversionFactor = 25.4f * (Settings.Scale / 100f) / Settings.PlotterUnitsPerInch;
            foreach (var polygon in polygons)
            {
                for (var j = 0; j < polygon.Points.Count; j++)
                {
                    polygon.Points[j] = new Point
                    {
                        X = polygon.Points[j].X * conversionFactor,
                        Y = polygon.Points[j].Y * conversionFactor,
                    };
                }
            }
        }

        /// <summary>
        /// Приводит координаты всех объектов к нулю в нижнем левом углу
        /// </summary>
        /// <param name="polygons">Список объектов</param>
        public static void NormalizePolygonsCoordinates(List<Polygon> polygons)
        {
            var minX = polygons.Min(x => x.MinX); // находим минимальный X среди всех объектов
            var minY = polygons.Min(x => x.MinY); // находим минимальный Y среди всех объектов
            foreach (var polygon in polygons)
            {
                for (var j = 0; j < polygon.Points.Count; j++)
                {
                    polygon.Points[j] = new Point
                    {
                        X = polygon.Points[j].X - minX,
                        Y = polygon.Points[j].Y - minY,
                    };
                }
            }
        }

        /// <summary>
        /// Рассчитывает точки для последующей отрисовки на экране
        /// </summary>
        /// <param name="polygons">Список объектов</param>
        /// <param name="width">Ширина холста</param>
        /// <param name="height">Высота холста</param>
        /// <returns>Список объектов для отрисовки</returns>
        public static List<Polygon> GetPolygonsForDraw(List<Polygon> polygons, double width, double height)
        {
            var polygonsForDraw = new List<Polygon>();
            var polygonsForDrawCounter = 0;
            var maxX = polygons.Max(x => x.MaxX);
            var maxY = polygons.Max(x => x.MaxY);
            var scaleX = width / maxX;
            var scaleY = height / maxY;
            var scale = Math.Min(scaleX, scaleY);
            var shiftX = (width - maxX * scale) / 2;
            var shiftY = (height - maxY * scale) / 2;
            var centerX = 0d;
            var centerY = 0d;
            for (var i = 0; i < polygons.Count; i++)
            {
                polygonsForDraw.Add(new Polygon() { ObjectType = polygons[i].ObjectType });
                foreach (var point in polygons[i].Points)
                {
                    polygonsForDraw[polygonsForDrawCounter].Points.Add(new Point
                    {
                        X = point.X * scale + shiftX,
                        Y = height - point.Y * scale - shiftY,
                    });
                }

                polygonsForDrawCounter++;
                if (polygons[i].ObjectType == 'P')
                {
                    centerX = polygons[i].MaxX - (polygons[i].MaxX - polygons[i].MinX) / 2;
                    centerY = polygons[i].MaxY - (polygons[i].MaxY - polygons[i].MinY) / 2;
                    polygonsForDraw.Add(new Polygon() { ObjectType = 'C' }); // добавляем новый объект и указываем его тип (С - центр)
                    polygonsForDraw[polygonsForDrawCounter].Points.Add(new Point
                    {
                        X = centerX * scale + shiftX - 2,
                        Y = height - centerY * scale - shiftY,
                    });

                    polygonsForDraw[polygonsForDrawCounter].Points.Add(new Point
                    {
                        X = centerX * scale + shiftX + 2,
                        Y = height - centerY * scale - shiftY,
                    });

                    polygonsForDrawCounter++;
                    polygonsForDraw.Add(new Polygon() { ObjectType = 'C' }); // добавляем новый объект и указываем его тип (С - центр)
                    polygonsForDraw[polygonsForDrawCounter].Points.Add(new Point
                    {
                        X = centerX * scale + shiftX,
                        Y = height - centerY * scale - shiftY - 2,
                    });

                    polygonsForDraw[polygonsForDrawCounter].Points.Add(new Point
                    {
                        X = centerX * scale + shiftX,
                        Y = height - centerY * scale - shiftY + 2,
                    });

                    polygonsForDrawCounter++;
                }
            }

            return polygonsForDraw;
        }
    }
}