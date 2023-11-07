using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NCMillComposer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Получить в массив номера всех дочерних объектов и длину этого массива
        private void GetChildList(int objectNumber)
        {
            Array.Resize(ref AppSettings.ChildList, 2); // Очищаем массив дочерних объектов
            AppSettings.ChildListLen = 0; // Обнуляем количество дочерних объектов
            if (AppSettings.ObjectType[objectNumber] != 'P' && objectNumber > 0) // Если не действительный объект
            {
                return;
            }

            if (AppSettings.ObjChilds[objectNumber] == "") // Если нет дочерних объектов
            {
                return;
            }

            var stringCursor = 1; // Указатель при переборе списка дочерних объектов
            while (stringCursor <= AppSettings.ObjChilds[objectNumber].Length) // В поисках номера Перебираем все символы, пока не дойдем до конца строки
            {
                string tempStringLine = "";
                while (stringCursor <= AppSettings.ObjChilds[objectNumber].Length) // В поисках цифры Перебираем все символы, пока не дойдем до конца строки
                {
                    if (AppSettings.ObjChilds[objectNumber].Substring(stringCursor, 1) != " ") // Если считали символ-цифру
                    {
                        tempStringLine += AppSettings.ObjChilds[objectNumber].Substring(stringCursor, 1);
                        stringCursor++; // переходим на считывание следующего символа из строки
                        continue;
                    }

                    // Считали пробел, то переходим к следующей строке и выходим из поиска символов
                    stringCursor++; // переходим на считывание следующего символа из строки
                    break;
                } // Конец поиска символов по достижению конца строки

                if (tempStringLine == "") // Если уже нет больше дочерних объектов то выходим
                {
                    break;
                }

                AppSettings.ChildListLen++; // Увеличиваем количество дочерних объектов
                Array.Resize(ref AppSettings.ChildList, AppSettings.ChildListLen + 1); // расширяем массив
                AppSettings.ChildList[AppSettings.ChildListLen] = Convert.ToInt32(tempStringLine);
            }
        }

        private void FindAllPoints() // Поиск среди объектов точек 0 и 1
        {
            DebugTextBox.Text += "Поиск среди объектов перекрестий для точек 0, 1 и 2...\n";
            AppSettings.FoundP0 = false; // Точка 0 не найдена
            AppSettings.FoundP1 = false; // Точка 1 не найдена
            AppSettings.FoundP2 = false; // Точка 2 не найдена
            AppSettings.Point0X = 0;
            AppSettings.Point0Y = 0;
            AppSettings.Point1X = AppSettings.PolygonMaxX;
            AppSettings.Point1Y = AppSettings.PolygonMaxY;
            AppSettings.MillX1 = AppSettings.PolygonMaxX;
            AppSettings.MillY1 = AppSettings.PolygonMaxY;
            AppSettings.Point2X = AppSettings.PolygonMaxX;
            AppSettings.Point2Y = AppSettings.PolygonMaxY;
            AppSettings.MillX2 = AppSettings.PolygonMaxX;
            AppSettings.MillY2 = AppSettings.PolygonMaxY;
            if (FindPoint()) // Ищем точку 0 Поиск перекрестия среди объектов
            {
                AppSettings.FoundP0 = true;
                AppSettings.Point0X = AppSettings.PointX;
                AppSettings.Point0Y = AppSettings.PointY;
                if (FindPoint()) // Ищем точку 1 Поиск перекрестия среди объектов
                {
                    AppSettings.FoundP1 = true;
                    // меняем местами значения 0 и 1 точек, т.к. скорее все точку 1 рисовали позже и она нашлась первее
                    AppSettings.Point1X = AppSettings.Point0X; // поменяем местами нулевую и первую точки
                    AppSettings.Point1Y = AppSettings.Point0Y; // поменяем местами нулевую и первую точки
                    AppSettings.Point0X = AppSettings.PointX; // поменяем местами нулевую и первую точки
                    AppSettings.Point0Y = AppSettings.PointY; // поменяем местами нулевую и первую точки
                    AppSettings.MillX1 = AppSettings.Point1X - AppSettings.Point0X; // сделаем значение относительно нулевой точки
                    AppSettings.MillY1 = AppSettings.Point1Y - AppSettings.Point0Y; // сделаем значение относительно нулевой точки
                    if (FindPoint()) // Ищем точку 2 - поиск очередного перекрестия
                    {
                        AppSettings.FoundP2 = true;
                        // Меняем местами точки 0 и 1 и 2 т.к. скорее всего это будет правильная расстановка
                        AppSettings.Point2X = AppSettings.Point1X; // поменяем местами вторую и первую точки
                        AppSettings.Point2Y = AppSettings.Point1Y; // поменяем местами вторую и первую точки
                        AppSettings.Point1X = AppSettings.Point0X; // поменяем местами вторую и первую точки
                        AppSettings.Point1Y = AppSettings.Point0Y; // поменяем местами вторую и первую точки
                        AppSettings.Point0X = AppSettings.PointX; // найденное значение занесем в нулевую точку
                        AppSettings.Point0Y = AppSettings.PointY; // найденное значение занесем в нулевую точку
                        AppSettings.MillX1 = AppSettings.Point1X - AppSettings.Point0X; // сделаем значение относительно нулевой точки
                        AppSettings.MillY1 = AppSettings.Point1Y - AppSettings.Point0Y; // сделаем значение относительно нулевой точки
                        AppSettings.MillX2 = AppSettings.Point2X - AppSettings.Point0X; // сделаем значение относительно нулевой точки
                        AppSettings.MillY2 = AppSettings.Point2Y - AppSettings.Point0Y; // сделаем значение относительно нулевой точки
                    }
                }
            }

            if (AppSettings.FoundP0)
            {
                DebugTextBox.Text += $"Найдена Точка =0= Координаты XY: {AppSettings.Point0X} : {AppSettings.Point0Y} \n";
                if (AppSettings.FoundP1)
                {
                    DebugTextBox.Text += $"Найдена Точка -1- Координаты XY: {AppSettings.Point1X} : {AppSettings.Point1Y} Относительные координаты XY: {AppSettings.MillX1} : {AppSettings.MillY1} \n";
                    Point1XTextBox.Text = TrimFormat09(AppSettings.MillX1.ToString());
                    Point1YTextBox.Text = TrimFormat09(AppSettings.MillY1.ToString());
                    if (AppSettings.FoundP2)
                    {
                        DebugTextBox.Text += $"Найдена Точка -2- Координаты XY: {AppSettings.Point2X} : {AppSettings.Point2Y} Относительные координаты XY: {AppSettings.MillX2} : {AppSettings.MillY2} \n";
                        Point2XTextBox.Text = TrimFormat09(AppSettings.MillX2.ToString());
                        Point2YTextBox.Text = TrimFormat09(AppSettings.MillY2.ToString());
                    }
                    else
                    {
                        DebugTextBox.Text += "Точка -2- не найдена, значение можно ввести вручную \n";
                        Point2XTextBox.Text = ""; // обнуляем значения точки2
                        Point2YTextBox.Text = "";
                        AppSettings.MillX2 = 0;
                        AppSettings.MillY2 = 0;
                    }
                }
                else
                {
                    DebugTextBox.Text += "Точки -1- и -2- не найдены, значения можно ввести вручную \n";
                    Point1XTextBox.Text = ""; // обнуляем значения точки1
                    Point1YTextBox.Text = "";
                    AppSettings.MillX1 = 0;
                    AppSettings.MillY1 = 0;
                    Point2XTextBox.Text = ""; // обнуляем значения точки2
                    Point2YTextBox.Text = "";
                    AppSettings.MillX2 = 0;
                    AppSettings.MillY2 = 0;
                }
            }
            else
            {
                DebugTextBox.Text += "Перекрестия среди векторов отсутсвуют. Точки =0= -1- -2- не найдены, за =0= принят нижний левый угол, значения -1- и -2- можно ввести вручную \n";
                Point1XTextBox.Text = ""; // обнуляем значения точки1
                Point1YTextBox.Text = "";
                AppSettings.MillX1 = 0;
                AppSettings.MillY1 = 0;
                Point2XTextBox.Text = ""; // обнуляем значения точки2
                Point2YTextBox.Text = "";
                AppSettings.MillX2 = 0;
                AppSettings.MillY2 = 0;
            }
        }

        private bool FindPoint() // Поиск перекрестия среди объектов
        {
            float aX, bX, abY, cdX, cY, dY;
            for (var objCur = 1; objCur <= AppSettings.ObjLen; objCur++) // Перебираем все вектора в поисках горизонтального незакрытого вектора состоящего из одного элемента
            {
                if (AppSettings.ObjectType[objCur] == 'L' && AppSettings.PolygonY[AppSettings.ObjFirst[objCur]] == AppSettings.PolygonY[AppSettings.ObjLast[objCur]] && (AppSettings.ObjLast[objCur] - AppSettings.ObjFirst[objCur]) == 1)
                {
                    aX = AppSettings.PolygonX[AppSettings.ObjFirst[objCur]];
                    bX = AppSettings.PolygonX[AppSettings.ObjLast[objCur]];
                    abY = AppSettings.PolygonY[AppSettings.ObjFirst[objCur]];
                    if (bX < aX)
                    {
                        bX = AppSettings.PolygonX[AppSettings.ObjFirst[objCur]];
                        aX = AppSettings.PolygonX[AppSettings.ObjLast[objCur]];
                    }

                    // нашли кандидата на первый горизонтальный вектор
                    for (var objCur2 = 1; objCur2 <= AppSettings.ObjLen; objCur2++) // Опять с самого начала перебираем все незакрытые вектора и ищем вертикальный
                    {
                        if (objCur2 != objCur && AppSettings.ObjectType[objCur2] == 'L' && AppSettings.PolygonX[AppSettings.ObjFirst[objCur2]] == AppSettings.PolygonX[AppSettings.ObjLast[objCur2]] && (AppSettings.ObjLast[objCur2] - AppSettings.ObjFirst[objCur2]) == 1)
                        {
                            cY = AppSettings.PolygonY[AppSettings.ObjFirst[objCur2]];
                            dY = AppSettings.PolygonY[AppSettings.ObjLast[objCur2]];
                            cdX = AppSettings.PolygonX[AppSettings.ObjFirst[objCur2]];
                            if (dY < cY)
                            {
                                dY = AppSettings.PolygonY[AppSettings.ObjFirst[objCur2]];
                                cY = AppSettings.PolygonY[AppSettings.ObjLast[objCur2]];
                            }

                            if (((bX - aX) / (dY - cY)) > 0.9f && ((bX - aX) / (dY - cY)) < 1.1f) // если их длины очень близки и...
                            {
                                if ((((bX - aX) / 2f + aX) / cdX) > 0.9f && (((bX - aX) / 2f + aX) / cdX) < 1.1f) // и горизонтальная  палка почти по центру горизонтальной
                                {
                                    if ((((dY - cY) / 2f + cY) / abY) > 0.9f && (((dY - cY) / 2f + cY) / abY) < 1.1f) // и вертикальная палка тоже почти по центру вертикальной
                                    {
                                        AppSettings.PointX = cdX; // CDx - координата по X
                                        AppSettings.PointY = abY; // ABy - координата по Y
                                        AppSettings.ObjectType[objCur] = 'X'; // Промечаем объект как точку
                                        AppSettings.ObjectType[objCur2] = 'X'; // Промечаем объект как точку
                                        return true; // Бинго!!! Мы нашли перекрестие!!! Нет смысла дальше перебирать объекты
                                    } // и вертикальная не по центру горизонтальной
                                } // и горизонтальная не по центру горизонтальной
                            } // Если длины не равны
                        } // Если вектор оказался не подходящим на роль второго вертикального

                        // Переход к обработке следующего вектора на роль второго вертикального
                    }
                } // Если вектор оказался не подходящим для горизонтального первого

                // Переход к обработке следующего первого вектора
            }

            return false;
        }

        private void AllObjChildTest() // Находим все вложенные друг в друга объекты
        {
            DebugTextBox.Text += "Поиск вложенных в друг друга объектов ";
            for (var objCur = 1; objCur <= AppSettings.ObjLen; objCur++)
            {
                AppSettings.ObjChilds[objCur] = ""; // Обнуляем список дочерних объектов
                AppSettings.ObjRoot[objCur] = 0; // Обнуляем его уровень вложенности
                AppSettings.ObjParent[objCur] = 0; // Обнуляем номер вышестоящего родительского объекта
                AppSettings.ObjRootMax = 0; // Обнуляем максимальный уровень вложенности
            }

            AppSettings.ObjNumCur = 0; // Обнуляем первый объект
            for (var objCur = 1; objCur <= AppSettings.ObjLen; objCur++)
            {
                if (objCur != AppSettings.ObjNumCur && AppSettings.ObjectType[objCur] == 'P') // Если нашли закрытый объект
                {
                    DebugTextBox.Text += ".";
                    AppSettings.ObjNumCur = objCur; // делаем его текущим для родительского
                    AppSettings.ObjNumCur2 = 0; // Обнуляем второй объект
                    for (var objCur2 = 1; objCur2 <= AppSettings.ObjLen; objCur2++) // Перебираем все объекты как вторые
                    {
                        if (objCur2 != AppSettings.ObjNumCur) // Если это не тот же объект что и родительский
                        {
                            if (ChildTest(objCur, objCur2))
                            {
                                AppSettings.ObjChilds[objCur] = AppSettings.ObjChilds[objCur] + " " + objCur2; // Делаем запись в родительском объекте
                                AppSettings.ObjRoot[objCur2] = AppSettings.ObjRoot[objCur2] + 1; // и увеличиваем уровень вложенности для дочернего
                                if (AppSettings.ObjRoot[objCur2] > AppSettings.ObjRootMax)
                                {
                                    AppSettings.ObjRootMax = AppSettings.ObjRoot[objCur2]; // и обновляем максимальный уровень вложенности
                                }
                            }
                        }
                    }

                    AppSettings.ObjChilds[objCur] = AppSettings.ObjChilds[objCur].Trim();
                } //  Конец если нашли первый объект
            } // конец цикла поисков первых объектов

            DebugTextBox.Text += $"\n Максимальный уровень вложенности среди объектов: {AppSettings.ObjRootMax} \n";
            // Поиск непосредственно вложенности объектов произведен, теперь удалим из дочерних объектов объекты не их родного уровня вложенности
            for (var objCur = 1; objCur <= AppSettings.ObjLen; objCur++) // переберем все объекты
            {
                GetChildList(objCur); // Получить в массив номера всех дочерних объектов и длину этого массива
                if (AppSettings.ChildListLen != 0) // Если у этого объекта есть дочерние
                {
                    var tempObjChilds = ""; // создадим временную строку для записи его прямых потомков
                    for (var objCur2 = 1; objCur2 <= AppSettings.ChildListLen; objCur2++) // Переберем весь массив с этими объектами
                    {
                        if ((AppSettings.ObjRoot[objCur] + 1) == AppSettings.ObjRoot[AppSettings.ChildList[objCur2]]) // Этот объект прямой потомок
                        {
                            AppSettings.ObjParent[AppSettings.ChildList[objCur2]] = objCur; // Впишем дочернему объекту родителя
                            tempObjChilds = tempObjChilds + AppSettings.ChildList[objCur2] + " "; // и сохраним у родителя потомка в его списке
                        } // конец проверки прямого потомка
                    } // переходим к проверке следующего потомка

                    AppSettings.ObjChilds[objCur] = tempObjChilds.Trim(); // Обновим список потомков у родительского объекта
                } // конец проверки на наличие потомков
            } // Переход к обработке следующего родительского объекта

            // а теперь занесем в нувой элемент массива список всех объектов верхнего уровня
            for (var objCur = 1; objCur <= AppSettings.ObjLen; objCur++)
            {
                if (AppSettings.ObjRoot[objCur] == 0 && (AppSettings.ObjectType[objCur] == 'P' || AppSettings.ObjectType[objCur] == 'L'))
                {
                    AppSettings.ObjChilds[0] = AppSettings.ObjChilds[0] + objCur + " "; // Занесем верхний объект в список детей нулевого
                }
            }

            AppSettings.ObjChilds[0] = AppSettings.ObjChilds[0].Trim();
        }

        private bool ChildTest(int parentObj, int childObj)
        {
            if (AppSettings.ObjectType[parentObj] != 'P') // Если родительский объект не закрытый то выходим сразу из функции
            {
                return false;
            }

            var childTestValue = 0; // Счетчик вложенных точек
            for (var polyCur2 = AppSettings.ObjFirst[childObj]; polyCur2 <= AppSettings.ObjLast[childObj]; polyCur2++) // Перебираем все точки дочернего объекта
            {
                var intersectValue = 0; // Обнуляем счетчик пересечений
                for (AppSettings.PolyCur = AppSettings.ObjFirst[parentObj] + 1; AppSettings.PolyCur <= AppSettings.ObjLast[parentObj]; AppSettings.PolyCur++) // Перебираем все вектора родительского объекта, начиная со второй точки
                {
                    // начало корректировки попадания луча на узел родительского объекта
                    AppSettings.PolyXIntersect1 = AppSettings.PolygonX[AppSettings.PolyCur - 1]; // заносим реальное значение точки Х №1 родительского вектора
                    AppSettings.PolyXIntersect2 = AppSettings.PolygonX[AppSettings.PolyCur]; // заносим реальное значение точки Х №2 родительского вектора
                    // если выпущенный нами луч из точки дочернего объекта проходит сквозь узел родительского объекта, то ту точку родительского объекта смещаем на ничтожно малое значение
                    if (AppSettings.PolygonX[polyCur2] == AppSettings.PolyXIntersect1)
                    {
                        AppSettings.PolyXIntersect1 += 0.001f;
                    }

                    if (AppSettings.PolygonX[polyCur2] == AppSettings.PolyXIntersect2)
                    {
                        AppSettings.PolyXIntersect2 += 0.001f;
                    }

                    // конец корректировки попадания луча на узел родительского объекта
                    if (CalcVectorIntersect(AppSettings.PolygonX[polyCur2], AppSettings.PolygonY[polyCur2], AppSettings.PolygonX[polyCur2], AppSettings.PolygonMaxY + 10, AppSettings.PolyXIntersect1, AppSettings.PolygonY[AppSettings.PolyCur - 1], AppSettings.PolyXIntersect2, AppSettings.PolygonY[AppSettings.PolyCur]))
                    {
                        intersectValue += 1; // Суммируем количество пересечений с векторами родительского объекта
                    }
                }

                AppSettings.PolyCur--; // Возвращаемся на последнее значение
                // начало корректировки попадания луча на узел родительского объекта
                AppSettings.PolyXIntersect1 = AppSettings.PolygonX[AppSettings.PolyCur]; // заносим реальное значение точки Х №1 родительского вектора
                AppSettings.PolyXIntersect2 = AppSettings.PolygonX[AppSettings.ObjFirst[parentObj]]; // заносим реальное значение точки Х №2 родительского вектора
                // если выпущенный нами луч из точки дочернего объекта проходит сквозь узел родительского объекта, то ту точку родительского объекта смещаем на ничтожно малое значение
                if (AppSettings.PolygonX[polyCur2] == AppSettings.PolyXIntersect1)
                {
                    AppSettings.PolyXIntersect1 += 0.001f;
                }

                if (AppSettings.PolygonX[polyCur2] == AppSettings.PolyXIntersect2)
                {
                    AppSettings.PolyXIntersect2 += 0.001f;
                }

                // конец корректировки попадания луча на узел родительского объекта
                if (CalcVectorIntersect(AppSettings.PolygonX[polyCur2], AppSettings.PolygonY[polyCur2], AppSettings.PolygonX[polyCur2], AppSettings.PolygonMaxY + 10, AppSettings.PolyXIntersect1, AppSettings.PolygonY[AppSettings.PolyCur], AppSettings.PolyXIntersect2, AppSettings.PolygonY[AppSettings.ObjFirst[parentObj]]))
                {
                    intersectValue += 1; // Суммируем еще вектор между последней и первой точкой
                }

                if (intersectValue % 2 != 0) // Если значение нечетное
                {
                    childTestValue++; // то точка дочернего объекта находится внутри родительского
                }
            }

            if ((childTestValue / (AppSettings.ObjLast[childObj] - AppSettings.ObjFirst[childObj] + 1f)) > 0.55f) // если количество входящих точек объекта деленное на кол-во векторов
            {
                return true;
            }

            return false;
        }

        private float CalcTriangleArea(float aX, float aY, float bX, float bY, float cX, float cY)
        {
            // Вычисляет Ориентированную удвоенную площадь треугольника
            return (bX - aX) * (cY - aY) - (bY - aY) * (cX - aX);
        }

        private bool CalcVectorIntersect2(float aX, float bX, float cX, float dX)
        {
            // Возвращает True если bounding box - оба отрезка лежат на одной прямой - вызывается 2 раза для X и Y по 1 разу
            if (aX > bX)
            {
                (aX, bX) = (bX, aX);
            }

            if (cX > dX)
            {
                (cX, dX) = (dX, cX);
            }

            if (cX > aX)
            {
                aX = cX;
            }

            if (dX < bX)
            {
                bX = dX;
            }

            if (aX <= bX)
            {
                return true;
            }

            return false;
        }

        private bool CalcVectorIntersect(float aX, float aY, float bX, float bY, float cX, float cY, float dX, float dY)
        {
            // Возвращает True если два вектора пересекаются
            if (CalcVectorIntersect2(aX, bX, cX, dX) && CalcVectorIntersect2(aY, bY, cY, dY) && (CalcTriangleArea(aX, aY, bX, bY, cX, cY) * CalcTriangleArea(aX, aY, bX, bY, dX, dY)) <= 0 && (CalcTriangleArea(cX, cY, dX, dY, aX, aY) * CalcTriangleArea(cX, cY, dX, dY, bX, bY)) <= 0)
            {
                return true;
            }

            return false;
        }

        private void GetObjRandomColor()
        {
            var allColor = 0; // сбрасываем цвет по умолчанию
            for (var objCur = 1; objCur <= AppSettings.ObjLen; objCur++) // Перебираем все объекты
            {
                if (AppSettings.ObjectColor[objCur] != allColor && AppSettings.ObjectColor[objCur] != 1 && AppSettings.ObjectColor[objCur] != 0)
                {
                    allColor = AppSettings.ObjectColor[objCur]; // если объекты разные по цвету, то отмечаем это
                }
            }

            if (allColor == 1 || allColor == 0)
            {
                DebugTextBox.Text += "Почему только черный??? - В мире столько ярких красок!!! - Разукрашиваем объекты в разные цвета ;) ...\n";
            }

            if (allColor == 1 || allColor == 0 || allColor == -1) // Если все оказались одного цыета, то заново Перебираем все объекты
            {
                var random = new Random();
                for (var objCur = 1; objCur <= AppSettings.ObjLen; objCur++)
                {
                    AppSettings.ObjectColor[objCur] = random.Next(1, 9); // Задаем каждому объекту свой случайный цвет :-)
                }
            }
        }

        private void GetObjColorAsOriented()
        {
            DebugTextBox.Text += "Устанавливаем цвет в соответствии с направлением ...\n";
            for (var objCur = 1; objCur <= AppSettings.ObjLen; objCur++) // Перебираем все объекты
            {
                if (AppSettings.ObjectType[objCur] == 'P') // Если это закрытые объекты
                {
                    if (AppSettings.ObjOrient[objCur] == 1 && CloseVectorProcessingCombobox1.SelectedIndex == 0) // Направление обьекта попутное, и в настройках тоже попутное
                    {
                        AppSettings.ObjectColor[objCur] = 1; // Задаем черный цвет :)
                    }
                    else if (AppSettings.ObjOrient[objCur] == 0 && CloseVectorProcessingCombobox1.SelectedIndex == 1) // Направление обьекта встречное, и в настройках тоже встречное
                    {
                        AppSettings.ObjectColor[objCur] = 1; // Задаем черный цвет :)
                    }
                    else
                    {
                        AppSettings.ObjectColor[objCur] = 3; // Задаем красный цвет :)
                    }
                }

                if (AppSettings.ObjectType[objCur] == 'L') // Если это незакрытые объекты
                {
                    if (AppSettings.ObjOrient[objCur] == 1 && OpenVectorsCombobox.SelectedIndex == 0) // Направление обьекта попутное, и в настройках тоже попутное
                    {
                        AppSettings.ObjectColor[objCur] = 2; // Задаем синий цвет :)
                    }
                    else if (AppSettings.ObjOrient[objCur] == 0 && OpenVectorsCombobox.SelectedIndex == 1) // Направление обьекта встречное, и в настройках тоже встречное
                    {
                        AppSettings.ObjectColor[objCur] = 2; // Задаем синий цвет :)
                    }
                    else
                    {
                        AppSettings.ObjectColor[objCur] = 4; // Задаем зеленый цвет :)
                    }
                }
            }
        }

        private void AlignAllObjorientation() // Делаем обход всех объектов попутным
        {
            DebugTextBox.Text += "Установка направления обхода всех объектов попутным... ";
            var polyXTmp = new float[AppSettings.PolyLen + 1]; // Координата X - временный массив
            var polyYTmp = new float[AppSettings.PolyLen + 1]; // Координата Y - временный массив
            var changeCount = 0;
            for (var objCur = 1; objCur <= AppSettings.ObjLen; objCur++) // Перебираем все объекты
            {
                if (AppSettings.ObjOrient[objCur] == -1) // Обход против часовой стрелки - встречный для фрезы
                {
                    for (AppSettings.PolyCur = AppSettings.ObjFirst[objCur]; AppSettings.PolyCur <= AppSettings.ObjLast[objCur]; AppSettings.PolyCur++) // Проходим по всем значениям массива
                    {
                        polyXTmp[AppSettings.PolyCur] = AppSettings.PolygonX[AppSettings.ObjFirst[objCur] + AppSettings.ObjLast[objCur] - AppSettings.PolyCur]; // Заносим во временное значение, данные с противоположного конца массива
                        polyYTmp[AppSettings.PolyCur] = AppSettings.PolygonY[AppSettings.ObjFirst[objCur] + AppSettings.ObjLast[objCur] - AppSettings.PolyCur]; // Заносим во временное значение, данные с противоположного конца массива
                    }

                    for (AppSettings.PolyCur = AppSettings.ObjFirst[objCur]; AppSettings.PolyCur <= AppSettings.ObjLast[objCur]; AppSettings.PolyCur++) // Проходим по всем значениям массива
                    {
                        AppSettings.PolygonX[AppSettings.PolyCur] = polyXTmp[AppSettings.PolyCur]; // Заносим в текущее значение, данные с временного массива
                        AppSettings.PolygonY[AppSettings.PolyCur] = polyYTmp[AppSettings.PolyCur]; // Заносим в текущее значение, данные с временного массива
                    }

                    AppSettings.ObjOrient[objCur] = 1; // Устанавливаем для текущего объекта обход по часовой стрелке - попутный для фрезы
                    changeCount++;
                }
            }

            DebugTextBox.Text += $"Изменено направление на попутное у : {changeCount} объектов \n";
        }

        private void FindAllObjOrientation() // Ищем Ориентированную площадь многоугольника всех объектов
        {
            DebugTextBox.Text += "Поиск ориентированной площади полигонов и определение направления обхода векторов... ";
            var objOrientSumm = 0f; // Сумма ориентированных площадей треугольника
            var forwardCount = 0;
            var backwardCount = 0;
            for (var objCur = 1; objCur <= AppSettings.ObjLen; objCur++) // Перебираем все объекты
            {
                objOrientSumm = 0f; // Обнуляем сумму ориентированных площадей треугольника текущего объекта
                for (AppSettings.PolyCur = AppSettings.ObjFirst[objCur] + 1; AppSettings.PolyCur <= AppSettings.ObjLast[objCur]; AppSettings.PolyCur++) // перебираем все строки объекта, начиная со второй
                {
                    objOrientSumm += CalcTriangleArea(0, 0, AppSettings.PolygonX[AppSettings.PolyCur - 1], AppSettings.PolygonY[AppSettings.PolyCur - 1], AppSettings.PolygonX[AppSettings.PolyCur], AppSettings.PolygonY[AppSettings.PolyCur]);
                }

                AppSettings.PolyCur--; // Возвращаемся на последнее значение
                if (AppSettings.ObjectType[objCur] == 'P')
                {
                    objOrientSumm += CalcTriangleArea(0, 0, AppSettings.PolygonX[AppSettings.PolyCur], AppSettings.PolygonY[AppSettings.PolyCur], AppSettings.PolygonX[AppSettings.ObjFirst[objCur]], AppSettings.PolygonY[AppSettings.ObjFirst[objCur]]);
                }

                if (objOrientSumm < 0)
                {
                    AppSettings.ObjOrient[objCur] = 1; // Обход по часовой стрелке - попутный для фрезы
                    forwardCount++;
                }
                else
                {
                    AppSettings.ObjOrient[objCur] = -1; // Обход против часовой стрелке - встречный для фрезы
                    backwardCount++;
                }
            }

            DebugTextBox.Text += $"Найдено объектов попутных : {forwardCount} и встречных: {backwardCount} \n";
        }

        private void FindObjCenterPoint() // Ищем среднюю точку для всех объектов и их размер
        {
            DebugTextBox.Text += "Вычисление средней точки всех объектов... \n";
            var objXLeftPoint = 0f; // Самая Левая точка X объекта
            var objYBottomPoint = 0f; // Самая Нижняя точка X объекта
            var objXRightPoint = 0f; // Самая Правая точка X объекта
            var objYTopPoint = 0f; // Самая Верхняя точка X объекта
            for (var objCur = 1; objCur <= AppSettings.ObjLen; objCur++) // Перебираем все объекты
            {
                objXLeftPoint = AppSettings.PolygonX[AppSettings.ObjFirst[objCur]]; // Сбрасываем самую Левую точку X объекта
                objYBottomPoint = AppSettings.PolygonY[AppSettings.ObjFirst[objCur]]; // Сбрасываем самую Нижнюю точку Y объекта
                objXRightPoint = AppSettings.PolygonX[AppSettings.ObjFirst[objCur]]; // Сбрасываем самую Правую точку X объекта
                objYTopPoint = AppSettings.PolygonY[AppSettings.ObjFirst[objCur]]; // Сбрасываем самую Верхнюю точку Y объекта
                AppSettings.PolyXFirst = 0; // Накопитель по X
                AppSettings.PolyYFirst = 0; // Накопитель по Y
                for (AppSettings.PolyCur = AppSettings.ObjFirst[objCur]; AppSettings.PolyCur <= AppSettings.ObjLast[objCur]; AppSettings.PolyCur++) // перебираем все строки объекта
                {
                    if (AppSettings.PolygonX[AppSettings.PolyCur] < objXLeftPoint)
                    {
                        objXLeftPoint = AppSettings.PolygonX[AppSettings.PolyCur]; // Нашли более левую точку X
                    }

                    if (AppSettings.PolygonY[AppSettings.PolyCur] < objYBottomPoint)
                    {
                        objYBottomPoint = AppSettings.PolygonY[AppSettings.PolyCur]; // Нашли более нижнюю точку Y
                    }

                    if (AppSettings.PolygonX[AppSettings.PolyCur] > objXRightPoint)
                    {
                        objXRightPoint = AppSettings.PolygonX[AppSettings.PolyCur]; // Нашли более правую точку X
                    }

                    if (AppSettings.PolygonY[AppSettings.PolyCur] > objYTopPoint)
                    {
                        objYTopPoint = AppSettings.PolygonY[AppSettings.PolyCur]; // Нашли более верхнюю точку Y
                    }

                    AppSettings.PolyXFirst += AppSettings.PolygonX[AppSettings.PolyCur]; // Сумма всех точек X
                    AppSettings.PolyYFirst += AppSettings.PolygonY[AppSettings.PolyCur]; // Сумма всех точек Y
                }

                AppSettings.ObjXCenter[objCur] = AppSettings.PolyXFirst / (AppSettings.ObjLast[objCur] - AppSettings.ObjFirst[objCur] + 1); // Среднеарифметическое по X
                AppSettings.ObjYCenter[objCur] = AppSettings.PolyYFirst / (AppSettings.ObjLast[objCur] - AppSettings.ObjFirst[objCur] + 1); // Среднеарифметическое по Y
                AppSettings.ObjXCenterBox[objCur] = (objXRightPoint - objXLeftPoint) / 2f + objXLeftPoint; // Средняя точка X по коробочке
                AppSettings.ObjYCenterBox[objCur] = (objYTopPoint - objYBottomPoint) / 2f + objYBottomPoint; // Средняя точка Y по коробочке
                AppSettings.ObjXDimension[objCur] = objXRightPoint - objXLeftPoint; // размер объекта по X
                AppSettings.ObjYDimension[objCur] = objYTopPoint - objYBottomPoint; // размер объекта по Y
            }
        }

        private void HiddenSmallObjects(float sizeToHidden) // Скрываем все объекты меньше указанных
        {
            for (var objCur = 1; objCur <= AppSettings.ObjLen; objCur++) // Перебираем все объекты и делаем их большими
            {
                if (AppSettings.ObjectType[objCur] == 'p')
                {
                    AppSettings.ObjectType[objCur] = 'P';
                }

                if (AppSettings.ObjectType[objCur] == 'l')
                {
                    AppSettings.ObjectType[objCur] = 'L';
                }
            }

            if (sizeToHidden != 0) // Если значение равно 0 - то все объекты будут большими и будут отображены
            {
                for (var objCur = 1; objCur <= AppSettings.ObjLen; objCur++) // Перебираем все объекты и делаем маленькие невидимыми
                {
                    if (AppSettings.ObjectType[objCur] == 'P' && AppSettings.ObjXDimension[objCur] < sizeToHidden && AppSettings.ObjYDimension[objCur] < sizeToHidden)
                    {
                        AppSettings.ObjectType[objCur] = 'p';
                    }

                    if (AppSettings.ObjectType[objCur] == 'L' && AppSettings.ObjXDimension[objCur] < sizeToHidden && AppSettings.ObjYDimension[objCur] < sizeToHidden)
                    {
                        AppSettings.ObjectType[objCur] = 'l';
                    }
                }
            }
        }

        private void DrawOnScreen() // Вывод на экран объектов по индексу
        {
            if (!AppSettings.IsFileLoaded)
            {
                return;
            }

            Canvas.Children.Clear();
            var drawable = new Drawable(Canvas);
            var color = new SolidColorBrush();
            var strokeThickness = 1;
            float drawXRatio = (Convert.ToSingle(Canvas.ActualWidth) - 1f) / AppSettings.PolygonMaxX;
            float drawYRatio = (Convert.ToSingle(Canvas.ActualHeight) - 1f) / AppSettings.PolygonMaxY;
            if (drawYRatio < drawXRatio)
            {
                drawXRatio = drawYRatio;
            }
            else
            {
                drawYRatio = drawXRatio;
            }

            float drawXShift = (Convert.ToSingle(Canvas.ActualWidth) - AppSettings.PolygonMaxX * drawXRatio) / 2f;
            float drawYShift = (Convert.ToSingle(Canvas.ActualHeight) - AppSettings.PolygonMaxY * drawYRatio) / 2f;
            for (var objCur = 1; objCur <= AppSettings.ObjLen; objCur++)
            {
                color = AppSettings.ObjectColor[objCur] switch // Решаем вопрос с цветом объекта
                {
                    1 => Brushes.Black, // Черный
                    2 => Brushes.Blue, // Синий
                    3 => Brushes.Red, // Красный
                    4 => Brushes.Green, // Зеленый
                    5 => Brushes.Purple, // Фиолетовый
                    6 => Brushes.Orange, // Оранжевый
                    7 => Brushes.DarkCyan, // Темно-голубой
                    8 => Brushes.Brown, // Коричневый
                    _ => Brushes.Black, // Черный для остальных случаев
                };

                for (AppSettings.PolyCur = AppSettings.ObjFirst[objCur] + 1; AppSettings.PolyCur <= AppSettings.ObjLast[objCur]; AppSettings.PolyCur++) // Перебираем весь массив данных объекта начиная со второй строки
                {
                    // и рисуем все линии у объектов L и P
                    if (AppSettings.ObjectType[objCur] == 'L' || AppSettings.ObjectType[objCur] == 'P')
                    {
                        drawable.DrawLine(color, strokeThickness, drawXShift + AppSettings.PolygonX[AppSettings.PolyCur - 1] * drawXRatio, Convert.ToSingle(Canvas.ActualHeight) - AppSettings.PolygonY[AppSettings.PolyCur - 1] * drawYRatio - drawYShift - 1, drawXShift + AppSettings.PolygonX[AppSettings.PolyCur] * drawXRatio, Convert.ToSingle(Canvas.ActualHeight) - AppSettings.PolygonY[AppSettings.PolyCur] * drawYRatio - drawYShift - 1);
                    }
                }

                AppSettings.PolyCur--; // Возвращаемся к последнему значению, т.к. Next вывел его за край
                if (AppSettings.ObjectType[objCur] == 'P') // Если объект замкнутый
                {
                    // то дорисовываем еще одну линию - от конца к началу
                    drawable.DrawLine(color, strokeThickness, drawXShift + AppSettings.PolygonX[AppSettings.PolyCur] * drawXRatio, Convert.ToSingle(Canvas.ActualHeight) - AppSettings.PolygonY[AppSettings.PolyCur] * drawYRatio - drawYShift - 1, drawXShift + AppSettings.PolygonX[AppSettings.ObjFirst[objCur]] * drawXRatio, Convert.ToSingle(Canvas.ActualHeight) - AppSettings.PolygonY[AppSettings.ObjFirst[objCur]] * drawYRatio - drawYShift - 1);
                    // и серый крестик в центре него
                    AppSettings.PolyXFirst = 2f; // Размер перекрестия в центрах закрытых объектов
                    color = Brushes.Gray;
                    drawable.DrawLine(color, strokeThickness, (drawXShift + AppSettings.ObjXCenter[objCur] * drawXRatio) - AppSettings.PolyXFirst, (Convert.ToSingle(Canvas.ActualHeight) - AppSettings.ObjYCenter[objCur] * drawYRatio - drawYShift - 1), (drawXShift + AppSettings.ObjXCenter[objCur] * drawXRatio) + AppSettings.PolyXFirst, (Convert.ToSingle(Canvas.ActualHeight) - AppSettings.ObjYCenter[objCur] * drawYRatio - drawYShift - 1));
                    drawable.DrawLine(color, strokeThickness, (drawXShift + AppSettings.ObjXCenter[objCur] * drawXRatio), (Convert.ToSingle(Canvas.ActualHeight) - AppSettings.ObjYCenter[objCur] * drawYRatio - drawYShift - 1) + AppSettings.PolyXFirst, (drawXShift + AppSettings.ObjXCenter[objCur] * drawXRatio), (Convert.ToSingle(Canvas.ActualHeight) - AppSettings.ObjYCenter[objCur] * drawYRatio - drawYShift - 1) - AppSettings.PolyXFirst);
                }

                if (AppSettings.ObjectType[objCur] == 'L') // Если объект Незамкнутый
                {
                    AppSettings.PolyXFirst = 2f; // Размер перекрестия в центрах закрытых объектов
                    color = Brushes.Gray;
                }
            }

            if (AppSettings.FoundP0) // Если найдена нулевая точка
            {
                AppSettings.PolyXFirst = 14; // Размер перекрестия
                color = Brushes.Gray;
                drawable.DrawLine(color, strokeThickness, (drawXShift + AppSettings.Point0X * drawXRatio) - AppSettings.PolyXFirst, (Convert.ToSingle(Canvas.ActualHeight) - AppSettings.Point0Y * drawYRatio - drawYShift - 1), (drawXShift + AppSettings.Point0X * drawXRatio) + AppSettings.PolyXFirst, (Convert.ToSingle(Canvas.ActualHeight) - AppSettings.Point0Y * drawYRatio - drawYShift - 1));
                drawable.DrawLine(color, strokeThickness, (drawXShift + AppSettings.Point0X * drawXRatio), (Convert.ToSingle(Canvas.ActualHeight) - AppSettings.Point0Y * drawYRatio - drawYShift - 1) + AppSettings.PolyXFirst, (drawXShift + AppSettings.Point0X * drawXRatio), (Convert.ToSingle(Canvas.ActualHeight) - AppSettings.Point0Y * drawYRatio - drawYShift - 1) - AppSettings.PolyXFirst);
                // и точку в центре его
                drawable.DrawEllipse(color, strokeThickness, drawXShift + AppSettings.Point0X * drawXRatio - 6, Convert.ToSingle(Canvas.ActualHeight) - AppSettings.Point0Y * drawYRatio - drawYShift - 1 - 6, 12, 12);
                // пишем номер объекта рядом с центром объекта
                AppSettings.PolyTmpLine = "0";
                color = Brushes.Red;
                drawable.DrawText(AppSettings.PolyTmpLine, 6, "Sans-Serif", color, (drawXShift + AppSettings.Point0X * drawXRatio) - 12, (Convert.ToSingle(Canvas.ActualHeight) - AppSettings.Point0Y * drawYRatio - drawYShift - 1) - 10);
            }

            if (AppSettings.FoundP1) // Если найдена первая точка
            {
                AppSettings.PolyXFirst = 14; // Размер перекрестия
                color = Brushes.Gray;
                drawable.DrawLine(color, strokeThickness, (drawXShift + (AppSettings.MillX1 + AppSettings.Point0X) * drawXRatio) - AppSettings.PolyXFirst, (Convert.ToSingle(Canvas.ActualHeight) - (AppSettings.MillY1 + AppSettings.Point0Y) * drawYRatio - drawYShift - 1), (drawXShift + (AppSettings.MillX1 + AppSettings.Point0X) * drawXRatio) + AppSettings.PolyXFirst, (Convert.ToSingle(Canvas.ActualHeight) - (AppSettings.MillY1 + AppSettings.Point0Y) * drawYRatio - drawYShift - 1));
                drawable.DrawLine(color, strokeThickness, (drawXShift + (AppSettings.MillX1 + AppSettings.Point0X) * drawXRatio), (Convert.ToSingle(Canvas.ActualHeight) - (AppSettings.MillY1 + AppSettings.Point0Y) * drawYRatio - drawYShift - 1) + AppSettings.PolyXFirst, (drawXShift + (AppSettings.MillX1 + AppSettings.Point0X) * drawXRatio), (Convert.ToSingle(Canvas.ActualHeight) - (AppSettings.MillY1 + AppSettings.Point0Y) * drawYRatio - drawYShift - 1) - AppSettings.PolyXFirst);
                // и точку в центре его
                drawable.DrawEllipse(color, strokeThickness, drawXShift + (AppSettings.MillX1 + AppSettings.Point0X) * drawXRatio - 6, Convert.ToSingle(Canvas.ActualHeight) - (AppSettings.MillY1 + AppSettings.Point0Y) * drawYRatio - drawYShift - 1 - 6, 12, 12);
                // у первой напишем индекс
                AppSettings.PolyTmpLine = "1";
                drawable.DrawText(AppSettings.PolyTmpLine, 6, "Sans-Serif", color, (drawXShift + (AppSettings.MillX1 + AppSettings.Point0X) * drawXRatio) - 12, (Convert.ToSingle(Canvas.ActualHeight) - (AppSettings.MillY1 + AppSettings.Point0Y) * drawYRatio - drawYShift - 1) - 10);
                if (AppSettings.FoundP2) // Если найдена вторая точка
                {
                    // нарисуем вторую точку
                    drawable.DrawLine(color, strokeThickness, (drawXShift + (AppSettings.MillX2 + AppSettings.Point0X) * drawXRatio) - AppSettings.PolyXFirst, (Convert.ToSingle(Canvas.ActualHeight) - (AppSettings.MillY2 + AppSettings.Point0Y) * drawYRatio - drawYShift - 1), (drawXShift + (AppSettings.MillX2 + AppSettings.Point0X) * drawXRatio) + AppSettings.PolyXFirst, (Convert.ToSingle(Canvas.ActualHeight) - (AppSettings.MillY2 + AppSettings.Point0Y) * drawYRatio - drawYShift - 1));
                    drawable.DrawLine(color, strokeThickness, (drawXShift + (AppSettings.MillX2 + AppSettings.Point0X) * drawXRatio), (Convert.ToSingle(Canvas.ActualHeight) - (AppSettings.MillY2 + AppSettings.Point0Y) * drawYRatio - drawYShift - 1) + AppSettings.PolyXFirst, (drawXShift + (AppSettings.MillX2 + AppSettings.Point0X) * drawXRatio), (Convert.ToSingle(Canvas.ActualHeight) - (AppSettings.MillY2 + AppSettings.Point0Y) * drawYRatio - drawYShift - 1) - AppSettings.PolyXFirst);
                    // и точку в центре его
                    drawable.DrawEllipse(color, strokeThickness, drawXShift + (AppSettings.MillX2 + AppSettings.Point0X) * drawXRatio - 6, Convert.ToSingle(Canvas.ActualHeight) - (AppSettings.MillY2 + AppSettings.Point0Y) * drawYRatio - drawYShift - 1 - 6, 12, 12);
                    AppSettings.PolyTmpLine = "2";
                    drawable.DrawText(AppSettings.PolyTmpLine, 6, "Sans-Serif", color, (drawXShift + (AppSettings.MillX2 + AppSettings.Point0X) * drawXRatio) - 12, (Convert.ToSingle(Canvas.ActualHeight) - (AppSettings.MillY2 + AppSettings.Point0Y) * drawYRatio - drawYShift - 1) - 10);
                }
            }
        }

        private void readFilePLT() // Открываем, считываем все вектора в массив и закрываем файл
        {
            using (var streamReader = new StreamReader(AppSettings.OpenDirectory + "\\" + AppSettings.FileName + AppSettings.FileDim)) // Открываем файл на построчное чтение
            {
                AppSettings.FoundP0 = false; // Не обнаружен + в файле для нулевой точки
                AppSettings.FoundP1 = false; // Не обнаружен х в файле для точки1
                AppSettings.PolyLen = 0; // Длина массива объектов
                AppSettings.PolyTmpColor = 0; // Текущий цвет объекта
                AppSettings.ObjLen = 0; // Количество объектов в массиве (Длина индекса объектов)
                Array.Resize(ref AppSettings.PolygonX, 2); // Массив Данных координат X объектов
                Array.Resize(ref AppSettings.PolygonY, 2); // Массив Данных координат Y объектов
                Array.Resize(ref AppSettings.ObjectType, 2); // Тип объекта D, L, P, +, X, l, p
                Array.Resize(ref AppSettings.ObjFirst, 2); // Номер первой строки объекта
                Array.Resize(ref AppSettings.ObjLast, 2); // Номер последней строки объекта
                Array.Resize(ref AppSettings.ObjectColor, 2); // Цвет объекта
                Array.Resize(ref AppSettings.ObjXCenter, 2); // Координаты средней точки Х
                Array.Resize(ref AppSettings.ObjYCenter, 2); // Координаты средней точки Y
                Array.Resize(ref AppSettings.ObjXCenterBox, 2); // Координаты средней точки Х по коробочке
                Array.Resize(ref AppSettings.ObjYCenterBox, 2); // Координаты средней точки Y по коробочке
                Array.Resize(ref AppSettings.ObjXDimension, 2); // Размер объекта по X
                Array.Resize(ref AppSettings.ObjYDimension, 2); // Размер объекта по Y
                Array.Resize(ref AppSettings.ObjOrient, 2); // Направление обхода объекта
                Array.Resize(ref AppSettings.ObjChilds, 2); // Список номеров дочерних объектов
                Array.Resize(ref AppSettings.ObjRoot, 2); // Уровень вложенности объекта, 0 - объект наружный
                Array.Resize(ref AppSettings.ObjParent, 2); // Родительский объект
                var dontReadLine = false; // Запрет чтения строки из файла в случае перехода на другой объект
                while (!streamReader.EndOfStream)
                {
                    if (!dontReadLine)
                    {
                        AppSettings.PolyTmpLine = streamReader.ReadLine().Trim(); // Считываем строку из файла
                    }

                    if (AppSettings.PolyTmpLine.Substring(0, 2) == "SP") // Ищем цвет в файле и ничего пока не делаем
                    {
                        AppSettings.PolyTmpColor = Convert.ToInt16(AppSettings.PolyTmpLine.Substring(2, 1)); // сохраняем цвет объекта
                    }

                    if (AppSettings.PolyTmpLine.Substring(0, 2) == "PU") // Ищем перемещение к первой точке объекта
                    {
                        AppSettings.PolyLen++; // Новая строка в массиве;
                        AppSettings.ObjLen++; // Новый объект
                        Array.Resize(ref AppSettings.PolygonX, AppSettings.PolyLen + 1); // Массив Данных координат X объектов
                        Array.Resize(ref AppSettings.PolygonY, AppSettings.PolyLen + 1); // Массив Данных координат Y объектов
                        Array.Resize(ref AppSettings.ObjectType, AppSettings.ObjLen + 1); // Тип объекта D, L, P, +, X, l, p
                        Array.Resize(ref AppSettings.ObjFirst, AppSettings.ObjLen + 1); // Номер первой строки объекта
                        Array.Resize(ref AppSettings.ObjLast, AppSettings.ObjLen + 1); // Номер последней строки объекта
                        Array.Resize(ref AppSettings.ObjectColor, AppSettings.ObjLen + 1); // Цвет объекта
                        Array.Resize(ref AppSettings.ObjXCenter, AppSettings.ObjLen + 1); // Координаты средней точки Х
                        Array.Resize(ref AppSettings.ObjYCenter, AppSettings.ObjLen + 1); // Координаты средней точки Y
                        Array.Resize(ref AppSettings.ObjXCenterBox, AppSettings.ObjLen + 1); // Координаты средней точки Х по коробочке
                        Array.Resize(ref AppSettings.ObjYCenterBox, AppSettings.ObjLen + 1); // Координаты средней точки Y по коробочке
                        Array.Resize(ref AppSettings.ObjXDimension, AppSettings.ObjLen + 1); // Размер объекта по X
                        Array.Resize(ref AppSettings.ObjYDimension, AppSettings.ObjLen + 1); // Размер объекта по Y
                        Array.Resize(ref AppSettings.ObjOrient, AppSettings.ObjLen + 1); // Направление обхода объекта
                        Array.Resize(ref AppSettings.ObjChilds, AppSettings.ObjLen + 1); // Список номеров дочерних объектов
                        Array.Resize(ref AppSettings.ObjRoot, AppSettings.ObjLen + 1); // Уровень вложенности объекта, 0 - объект наружный
                        Array.Resize(ref AppSettings.ObjParent, AppSettings.ObjLen + 1); // Родительский объект
                        AppSettings.ObjectColor[AppSettings.ObjLen] = Convert.ToInt32(AppSettings.PolyTmpColor); // Сохраняем цвет объекта в массив
                        AppSettings.ObjFirst[AppSettings.ObjLen] = AppSettings.PolyLen; // Сохраняем номер первой строки объекта
                                                                                            // Получаем из считанной строки X координату
                        AppSettings.PolygonX[AppSettings.PolyLen] = Convert.ToSingle(TrimFormat09(AppSettings.PolyTmpLine.Substring(2, AppSettings.PolyTmpLine.IndexOf(" ") - 2))); // Сохраняем X координату в массив
                                                                                                                                                                                        // Получаем из считанной строки Y координату
                        AppSettings.PolygonY[AppSettings.PolyLen] = Convert.ToSingle(TrimFormat09(AppSettings.PolyTmpLine.Substring(AppSettings.PolyTmpLine.IndexOf(" "), AppSettings.PolyTmpLine.IndexOf(";") - AppSettings.PolyTmpLine.IndexOf(" ")))); // Сохраняем Y координату в массив
                        while (!streamReader.EndOfStream) // Начинаем обрабатывать все следующие за первой строки
                        {
                            AppSettings.PolyTmpLine = streamReader.ReadLine().Trim(); // Считываем строку из файла
                            if (AppSettings.PolyTmpLine.Substring(0, 2) != "PD") // Если считанная строка не является уже текущими перемещениями с опущенным пером внутри объекта
                            {
                                dontReadLine = true; // Запрещаем еще раз читать лишнюю строку, т.к. эта уже содержит начало нового объекта или его цвет.
                                AppSettings.ObjLast[AppSettings.ObjLen] = AppSettings.PolyLen; // Сохраняем номер последней строки объекта
                                break;
                            }
                            else // Считываем тело текущего объекта
                            {
                                AppSettings.PolyLen++; // Переходим к следующей строке данных массива точек
                                Array.Resize(ref AppSettings.PolygonX, AppSettings.PolyLen + 1); // одна из координат по Х объекта
                                Array.Resize(ref AppSettings.PolygonY, AppSettings.PolyLen + 1); // одна из координат по Y объекта
                                                                                             // Получаем из считанной строки X координату
                                AppSettings.PolygonX[AppSettings.PolyLen] = Convert.ToSingle(TrimFormat09(AppSettings.PolyTmpLine.Substring(2, AppSettings.PolyTmpLine.IndexOf(" ") - 2))); // Сохраняем X координату в массив
                                                                                                                                                                                                // Получаем из считанной строки Y координату
                                AppSettings.PolygonY[AppSettings.PolyLen] = Convert.ToSingle(TrimFormat09(AppSettings.PolyTmpLine.Substring(AppSettings.PolyTmpLine.IndexOf(" "), AppSettings.PolyTmpLine.IndexOf(";") - AppSettings.PolyTmpLine.IndexOf(" ")))); // Сохраняем Y координату в массив
                            }
                        } // конец обработки тела файла, всвязи с окончанием файла
                    } // Конец обработки найденного первого прохода с поднятым пером
                } // конец обработки файла, всвязи с его окончанием.
            }
            
            TextBox18.Text = "";
        }

        private string TrimFormat09(string frmStringVal)
        {
            var result = "";
            frmStringVal = frmStringVal.Trim();
            var tmpCur = "";
            for (var tmpLineNumber = 0; tmpLineNumber < frmStringVal.Length; tmpLineNumber++)
            {
                tmpCur = frmStringVal.Substring(tmpLineNumber, 1);
                if (tmpCur == "-" || tmpCur == "1" || tmpCur == "2" || tmpCur == "3" || tmpCur == "4" || tmpCur == "5" || tmpCur == "6" || tmpCur == "7" || tmpCur == "8" || tmpCur == "9" || tmpCur == "0" || tmpCur == "." || tmpCur == ",")
                {
                    result += tmpCur;
                }
            }

            result = result.Replace('.', AppSettings.Separator);
            result = result.Replace(',', AppSettings.Separator);
            if (result == "" || result == "-" || result == "." || result == ",")
            {
                result = "0";
            }

            if (result.IndexOf(AppSettings.Separator) != -1)
            {
                result = result.Substring(0, result.IndexOf(AppSettings.Separator)) + result.Substring(result.IndexOf(AppSettings.Separator) + 1, 3);
            }

            return result;
        }

        private void FileOpenAndReadObjects() // Открываем и загружаем в массив файл
        {
            AppSettings.IsFileLoaded = false;
            var fileDialog = new OpenFileDialog();
            fileDialog.Filter = AppSettings.FileMask;
            fileDialog.Title = AppSettings.OpenDialogTitle;
            fileDialog.InitialDirectory = AppSettings.OpenDirectory;
            if (TextBox18.Text != "")
            {
                fileDialog.FileName = $"*{TextBox18.Text}*";
            }

            if (fileDialog.ShowDialog() == true) // Если выбрали файл
            {
                AppSettings.FileName = fileDialog.SafeFileName.Substring(0, fileDialog.SafeFileName.IndexOf('.')); // Имя файла без расширения
                AppSettings.FileDim = fileDialog.SafeFileName.Substring(fileDialog.SafeFileName.IndexOf('.')); // Расширение файла (например: .plt)
                AppSettings.OpenDirectory = fileDialog.FileName.Substring(0, fileDialog.FileName.IndexOf(AppSettings.FileName) - 1);
                SaveIniString("OpenDir", AppSettings.OpenDirectory); // Сохраним текущую директорию как основную для последующего открытия
                AppSettings.IsFileLoaded = true;
                RotateButton.IsEnabled = true; // Активируем кнопку поворота на 90 градусов
                // Если высота безопасности равна нулю тогда отключим кнопку сохранить
                if (AppSettings.SafeZ == 0)
                {
                    FileSaveAndCloseButton.IsEnabled = false;
                }
                else
                {
                    FileSaveAndCloseButton.IsEnabled = true; // и соответсвенно включим, если указана высота безопасности
                }

                DebugTextBox.Text += $"\n\n Открывается файл: {AppSettings.OpenDirectory}\\{AppSettings.FileName}{AppSettings.FileDim}\n";
                readFilePLT(); // Открываем, считываем все вектора в массив и закрываем файл
                if (AppSettings.PolyLen < 2) // Если в файле не содержалось объектов
                {
                    App.Current.MainWindow.Title = $"{AppSettings.ProgName} {AppSettings.Version}";
                    DebugTextBox.Text += $"Файл {AppSettings.OpenDirectory}\\{AppSettings.FileName}{AppSettings.FileDim} не содержит информации о векторах!";
                    AppSettings.IsFileLoaded = false;
                    FileSaveAndCloseButton.IsEnabled = false;
                    RotateButton.IsEnabled = false;
                    Array.Resize(ref AppSettings.PolygonX, 2); // Массив Данных координат X объектов
                    Array.Resize(ref AppSettings.PolygonY, 2); // Массив Данных координат Y объектов
                    Array.Resize(ref AppSettings.ObjectType, 2); // Тип объекта D, L, P, +, X, l, p
                    Array.Resize(ref AppSettings.ObjFirst, 2); // Номер первой строки объекта
                    Array.Resize(ref AppSettings.ObjLast, 2); // Номер последней строки объекта
                    Array.Resize(ref AppSettings.ObjectColor, 2); // Цвет объекта
                    Array.Resize(ref AppSettings.ObjXCenter, 2); // Координаты средней точки Х
                    Array.Resize(ref AppSettings.ObjYCenter, 2); // Координаты средней точки Y
                    Array.Resize(ref AppSettings.ObjXCenterBox, 2); // Координаты средней точки Х по коробочке
                    Array.Resize(ref AppSettings.ObjYCenterBox, 2); // Координаты средней точки Y по коробочке
                    Array.Resize(ref AppSettings.ObjXDimension, 2); // Размер объекта по X
                    Array.Resize(ref AppSettings.ObjYDimension, 2); // Размер объекта по Y
                    Array.Resize(ref AppSettings.ObjOrient, 2); // Направление обхода объекта
                    Array.Resize(ref AppSettings.ObjChilds, 2); // Список номеров дочерних объектов
                    Array.Resize(ref AppSettings.ObjRoot, 2); // Уровень вложенности объекта, 0 - объект наружный
                    Array.Resize(ref AppSettings.ObjParent, 2); // Родительский объект
                    AppSettings.FoundP0 = false;
                    AppSettings.FoundP1 = false;
                    AppSettings.PolyLen = 0; // Длина массива объектов
                    AppSettings.ObjLen = 0; // Длина массива объектов
                    return;
                }

                DebugTextBox.Text += $"Загружено Строк: {AppSettings.PolyLen} Объектов: {AppSettings.ObjLen} \n";
            }
        }

        private void FileOpen_ButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }

        private void OpenFile()
        {
            FileOpenAndReadObjects(); // Открываем и загружаем в массив файл
            if (!AppSettings.IsFileLoaded)
            {
                return;
            }

            App.Current.MainWindow.Title = $"{AppSettings.ProgName} {AppSettings.Version}";
            FormatArrayLP(); // Поиск открытых и закрытых объектов
            AppSettings.ConnectCount = 1; // Устанавливаем счетчик соединений, чтобы запустить первый проход
            while (AppSettings.ConnectCount != 0)
            {
                FormatArrayConnect(); // Находим и соединяем вектора разных объектов в один
                FormatArrayLP(); // Поиск открытых и закрытых объектов
                FormatArrayDefrag(); // Дефрагментация и упорядочивание массива объектов
            }

            FormatArrayPercent(); // Приведение координат всех объектов к нулю в нижнем левом углу
            FindAllObjOrientation(); // Ищем Ориентированную площадь многоугольника всех объектов
            AlignAllObjorientation(); // Делаем обход всех объектов попутным
            FindAllPoints(); // Поиск среди объектов точек 0 и 1
            AllObjChildTest(); // Поиск всех дочерних объектов
            FindObjCenterPoint(); // Ищем среднюю точку для всех объектов и их размер
            HiddenSmallObjects(AppSettings.HideSmall); // Скрываем все объекты меньше указанных
            ReverseInnerPolygons(); // Делаем внутренние вложенные нечетные полигоны встречными по отношению к основному движению
            GetObjColorAsOriented(); // Разукрашиваем объекты в соответствии с направлением
            DebugTextBox.Text += "Вывод на экран...";
            Statistic(); // Рассчет суммы длин всех векторов
            DrawOnScreen(); // Вывод на экран объектов по индексу
        }

        

        private void FormatArrayLP() // Поиск открытых и закрытых объектов
        {
            DebugTextBox.Text += "Поиск и маркировка закрытых объектов и открытых векторов ... ";
            // поиск закрытых объектов внутри массива и их маркировка с удалением последней координаты
            var closedObjects = 0; // временный счетчик закрытых объектов
            for (var objCur = 1; objCur <= AppSettings.ObjLen; objCur++)
            {
                if (AppSettings.ObjectType[objCur] != 'P' && AppSettings.ObjectType[objCur] != 'D') // Работаем только с действительными объектами и незакрытыми объектами
                {
                    if (AppSettings.PolygonX[AppSettings.ObjFirst[objCur]] == AppSettings.PolygonX[AppSettings.ObjLast[objCur]] && AppSettings.PolygonY[AppSettings.ObjFirst[objCur]] == AppSettings.PolygonY[AppSettings.ObjLast[objCur]]) // Если первая и последняя кординаты объекта равны
                    {
                        AppSettings.ObjectType[objCur] = 'P'; // Он отмечается как закрытый
                        AppSettings.PolygonX[AppSettings.ObjLast[objCur]] = 0; // Cтирается из массива данных координата X
                        AppSettings.PolygonY[AppSettings.ObjLast[objCur]] = 0; // Cтирается из массива данных координата Y
                        AppSettings.ObjLast[objCur] = AppSettings.ObjLast[objCur] - 1; // и переносится указатель последней строки объекта на одну строку к началу
                    }
                    else
                    {
                        AppSettings.ObjectType[objCur] = 'L'; // Он отмечается как открытый
                    }
                }

                if (AppSettings.ObjectType[objCur] == 'P')
                {
                    closedObjects++;
                }
            }

            DebugTextBox.Text += $"Определено Закрытых: {closedObjects} Открытых: {AppSettings.ObjLen - closedObjects} \n";
        }

        private void FormatArrayConnect() // Находим и соединяем вектора разных объектов в один
        {
            DebugTextBox.Text += "Поиск и стыковка соседних векторов ";
            AppSettings.ConnectCount = 0; // Сбрасываем все счетчики соединений
            var usl1 = 0;
            var usl2 = 0;
            var usl3 = 0;
            var usl4 = 0;
            var polyRecords = 0;
            AppSettings.ObjTmpNum = 0; // устанавливаем номер объекта1 ниже 1 чтобы отличить его от первого попадущегося объекта
            for (var objCur = 1; objCur <= AppSettings.ObjLen; objCur++) // Перебираем построчно весь массив в поиске незакрытого объекта1
            {
                if (AppSettings.ObjectType[objCur] == 'L' && objCur != AppSettings.ObjTmpNum) // Нашли незакрытый объект1
                {
                    DebugTextBox.Text += ".";
                    AppSettings.ObjTmpNum = objCur; // Делаем этот объект текущим
                    AppSettings.Obj2TmpNum = 0; // Устанавливаем начальный номер объекта2 ниже 1 чтобы отличить его от первого попадущегося объекта
                    for (var objCur2 = 1; objCur2 <= AppSettings.ObjLen; objCur2++) // Начинаем поиск объекта2 c начала массива объектов
                    {
                        if (AppSettings.ObjectType[objCur2] == 'L' && objCur2 != AppSettings.Obj2TmpNum && objCur2 != objCur) // Нашли незакрытый объект2
                        {
                            AppSettings.Obj2TmpNum = objCur2; // Делаем этот объект текущим
                            // начинаем сравнение векторов найденых объектов и их объединение
                            if (AppSettings.PolygonX[AppSettings.ObjLast[objCur]] == AppSettings.PolygonX[AppSettings.ObjFirst[objCur2]] && AppSettings.PolygonY[AppSettings.ObjLast[objCur]] == AppSettings.PolygonY[AppSettings.ObjFirst[objCur2]]) // Объект2 становится непосредственно в хвост Объекта1
                            {
                                // Раздвинуть после Объекта1 на длину Объекта2 без одной записи и записать Объект2 со второй его строки
                                polyRecords = AppSettings.ObjLast[objCur2] - AppSettings.ObjFirst[objCur2]; // Длина Объекта2 без одной строки
                                FormatArrayAddAfter(objCur, polyRecords); // Раздвигаем после Объекта1
                                usl1++; // Увеличиваем счетчик стыковок
                                for (AppSettings.PolyCur = 1; AppSettings.PolyCur <= polyRecords; AppSettings.PolyCur++) // Количество переписываемых с объекта2 в объект1 строк
                                {
                                    AppSettings.PolygonX[AppSettings.ObjLast[objCur] - polyRecords + AppSettings.PolyCur] = AppSettings.PolygonX[AppSettings.ObjFirst[objCur2] + AppSettings.PolyCur]; // перенос с Объекта2 в Объект1
                                    AppSettings.PolygonY[AppSettings.ObjLast[objCur] - polyRecords + AppSettings.PolyCur] = AppSettings.PolygonY[AppSettings.ObjFirst[objCur2] + AppSettings.PolyCur]; // перенос с Объекта2 в Объект1
                                }

                                AppSettings.ObjectType[objCur2] = 'D'; // Удаляем Объект2
                            }
                            else if (AppSettings.PolygonX[AppSettings.ObjLast[objCur]] == AppSettings.PolygonX[AppSettings.ObjLast[objCur2]] && AppSettings.PolygonY[AppSettings.ObjLast[objCur]] == AppSettings.PolygonY[AppSettings.ObjLast[objCur2]]) // Объект2 становится непосредственно в хвост Объекта1 c переворотом
                            {
                                // Раздвинуть после Объекта1 на длину Объекта2 без одной записи и записать Объект2 в обратном порядке от предпоследней строки
                                polyRecords = AppSettings.ObjLast[objCur2] - AppSettings.ObjFirst[objCur2]; // Длина Объекта2 без одной строки
                                FormatArrayAddAfter(objCur, polyRecords); // Раздвигаем после Объекта1
                                usl2++; // Увеличиваем счетчик стыковок
                                for (AppSettings.PolyCur = 1; AppSettings.PolyCur <= polyRecords; AppSettings.PolyCur++) // Количество переписываемых с объекта2 в объект1 строк
                                {
                                    AppSettings.PolygonX[AppSettings.ObjLast[objCur] - polyRecords + AppSettings.PolyCur] = AppSettings.PolygonX[AppSettings.ObjLast[objCur2] - AppSettings.PolyCur]; // перенос с Объекта2 в Объект1
                                    AppSettings.PolygonY[AppSettings.ObjLast[objCur] - polyRecords + AppSettings.PolyCur] = AppSettings.PolygonY[AppSettings.ObjLast[objCur2] - AppSettings.PolyCur]; // перенос с Объекта2 в Объект1
                                }

                                AppSettings.ObjectType[objCur2] = 'D'; // Удаляем Объект2
                            }
                            else if (AppSettings.PolygonX[AppSettings.ObjFirst[objCur]] == AppSettings.PolygonX[AppSettings.ObjFirst[objCur2]] && AppSettings.PolygonY[AppSettings.ObjFirst[objCur]] == AppSettings.PolygonY[AppSettings.ObjFirst[objCur2]]) // Объект2 становится непосредственно в хвост Объекта1 c переворотом
                            {
                                // Перед Объектом1 добавить строк на длину Объекта2 без одной записи и записать Объект2 в обратном порядке до второй строки включительно
                                polyRecords = AppSettings.ObjLast[objCur2] - AppSettings.ObjFirst[objCur2]; // Длина Объекта2 без одной строки
                                FormatArrayAddBefore(objCur, polyRecords); // Раздвигаем перед Объектом1
                                usl3++; // Увеличиваем счетчик стыковок
                                for (AppSettings.PolyCur = 1; AppSettings.PolyCur <= polyRecords; AppSettings.PolyCur++) // Количество переписываемых с объекта2 в объект1 строк
                                {
                                    AppSettings.PolygonX[AppSettings.ObjFirst[objCur] - 1 + AppSettings.PolyCur] = AppSettings.PolygonX[AppSettings.ObjLast[objCur2] + 1 - AppSettings.PolyCur]; // перенос с Объекта2 в Объект1
                                    AppSettings.PolygonY[AppSettings.ObjFirst[objCur] - 1 + AppSettings.PolyCur] = AppSettings.PolygonY[AppSettings.ObjLast[objCur2] + 1 - AppSettings.PolyCur]; // перенос с Объекта2 в Объект1
                                }

                                AppSettings.ObjectType[objCur2] = 'D'; // Удаляем Объект2
                            }
                            else if (AppSettings.PolygonX[AppSettings.ObjFirst[objCur]] == AppSettings.PolygonX[AppSettings.ObjLast[objCur2]] && AppSettings.PolygonY[AppSettings.ObjFirst[objCur]] == AppSettings.PolygonY[AppSettings.ObjLast[objCur2]]) // Объект2 становится непосредственно перед Объектом1
                            {
                                // Перед Объектом1 добавить строк на длину Объекта2 без одной записи и записать Объект2 до предпоследней строки
                                polyRecords = AppSettings.ObjLast[objCur2] - AppSettings.ObjFirst[objCur2]; // Длина Объекта2 без одной строки
                                FormatArrayAddBefore(objCur, polyRecords); // Раздвигаем перед Объектом1
                                usl4++; // Увеличиваем счетчик стыковок
                                for (AppSettings.PolyCur = 1; AppSettings.PolyCur <= polyRecords; AppSettings.PolyCur++) // Количество переписываемых с объекта2 в объект1 строк
                                {
                                    AppSettings.PolygonX[AppSettings.ObjFirst[objCur] - 1 + AppSettings.PolyCur] = AppSettings.PolygonX[AppSettings.ObjFirst[objCur2] - 1 + AppSettings.PolyCur]; // перенос с Объекта2 в Объект1
                                    AppSettings.PolygonY[AppSettings.ObjFirst[objCur] - 1 + AppSettings.PolyCur] = AppSettings.PolygonY[AppSettings.ObjFirst[objCur2] - 1 + AppSettings.PolyCur]; // перенос с Объекта2 в Объект1
                                }

                                AppSettings.ObjectType[objCur2] = 'D'; // Удаляем Объект2
                            } // Конец сравнения координат векторов и их стыковки
                        } // Конец поиска незакрытого объекта2
                    } // Переход к следующей строке массива для поиска следующего объекта2
                } // Конец поиска незакрытого объекта1
            } // Переход к следующей строке массива для поиска следующего объекта1

            DebugTextBox.Text += $"\n Произведено всего стыковок: После вектора: {usl1} После с переворотом: {usl2} Впереди вектора: {usl3} Впереди с переворотом: {usl3}";
            DebugTextBox.Text += $" Обработано Строк: {AppSettings.PolyLen} Объектов: {AppSettings.ObjLen} \n";
            AppSettings.ConnectCount = usl1 + usl2 + usl3 + usl4;
        }

        private void FormatArrayAddAfter(int objBegin, int polyRecords) // Раздвигаем массив после
        {
            AppSettings.PolyLen += polyRecords; // Новая длина массива координат
            Array.Resize(ref AppSettings.PolygonX, AppSettings.PolyLen + 1); // Расширяем массив
            Array.Resize(ref AppSettings.PolygonY, AppSettings.PolyLen + 1); // Расширяем массив
            for (var polyCur = AppSettings.PolyLen - polyRecords; polyCur >= AppSettings.ObjLast[objBegin] + 1; polyCur--)
            {
                AppSettings.PolygonX[polyCur + polyRecords] = AppSettings.PolygonX[polyCur];
                AppSettings.PolygonY[polyCur + polyRecords] = AppSettings.PolygonY[polyCur];
            }

            for (var polyCur = AppSettings.ObjLast[objBegin] + 1; polyCur <= AppSettings.ObjLast[objBegin] + polyRecords; polyCur++)
            {
                AppSettings.PolygonX[polyCur] = 0;
                AppSettings.PolygonY[polyCur] = 0;
            }

            AppSettings.ObjLast[objBegin] = AppSettings.ObjLast[objBegin] + polyRecords;
            for (var objCur = objBegin + 1; objCur <= AppSettings.ObjLen; objCur++)
            {
                AppSettings.ObjFirst[objCur] = AppSettings.ObjFirst[objCur] + polyRecords;
                AppSettings.ObjLast[objCur] = AppSettings.ObjLast[objCur] + polyRecords;
            }
        }

        private void FormatArrayAddBefore(int objBegin, int polyRecords) // Раздвигаем массив перед
        {
            AppSettings.PolyLen += polyRecords; // Новая длина массива координат
            Array.Resize(ref AppSettings.PolygonX, AppSettings.PolyLen + 1); // Расширяем массив
            Array.Resize(ref AppSettings.PolygonY, AppSettings.PolyLen + 1); // Расширяем массив
            for (var polyCur = AppSettings.PolyLen - polyRecords; polyCur >= AppSettings.ObjFirst[objBegin]; polyCur--)
            {
                AppSettings.PolygonX[polyCur + polyRecords] = AppSettings.PolygonX[polyCur];
                AppSettings.PolygonY[polyCur + polyRecords] = AppSettings.PolygonY[polyCur];
            }

            for (var polyCur = AppSettings.ObjFirst[objBegin]; polyCur <= AppSettings.ObjFirst[objBegin] + polyRecords - 1; polyCur++)
            {
                AppSettings.PolygonX[polyCur] = 0;
                AppSettings.PolygonY[polyCur] = 0;
            }

            AppSettings.ObjLast[objBegin] = AppSettings.ObjLast[objBegin] + polyRecords;
            for (var objCur = objBegin + 1; objCur <= AppSettings.ObjLen; objCur++)
            {
                AppSettings.ObjFirst[objCur] = AppSettings.ObjFirst[objCur] + polyRecords;
                AppSettings.ObjLast[objCur] = AppSettings.ObjLast[objCur] + polyRecords;
            }
        }

        private void FormatArrayDefrag() // Дефрагментация и упорядочивание массива объектов
        {
            DebugTextBox.Text += "Дефрагментация и упорядочивание массива объектов... ";
            var polyX2 = new float[2]; // Координата X
            var polyY2 = new float[2]; // Координата Y
            var objType2 = new char[2]; // Тип объекта N, L, P, +, X, l, p
            var objFirst2 = new int[2]; // Номер первой строки объекта
            var objLast2 = new int[2]; // Номер последней строки объекта
            var objColor2 = new int[2]; // Цвет объекта
            var objXCenter2 = new float[2]; // Координаты средней точки Х объекта
            var objYCenter2 = new float[2]; // Координаты средней точки Y объекта
            var objXCenterBox2 = new float[2]; // Координаты средней точки Х прямоугольника в который вписан объект
            var objYCenterBox2 = new float[2]; // Координаты средней точки Y прямоугольника в который вписан объект
            var objXDimension2 = new float[2]; // Размер объекта по X
            var objYDimension2 = new float[2]; // Размер объекта по Y
            var objOrient2 = new int[2]; // Направление обхода объекта
            var objChilds2 = new string[2]; // Список номеров дочерних объектов
            var objRoot2 = new int[2]; // Уровень вложенности объекта, 0 - объект наружный
            var objParent2 = new int[2]; // Родительский объект
            var objLen2 = 0; // Временный счетчик объектов
            var polyLen2 = 0; // Временный счетчик строк
            for (var objCur = 1; objCur <= AppSettings.ObjLen; objCur++) // Перебираем весь индекс объектов
            {
                if (AppSettings.ObjectType[objCur] != 'D' && (AppSettings.ObjLast[objCur] - AppSettings.ObjFirst[objCur]) > 0) // Если в индексе идет действительный объект
                {
                    // Расширяем временный индекс
                    objLen2++;
                    Array.Resize(ref objType2, objLen2 + 1); // Тип объекта N, L, P, +, X, l, p
                    Array.Resize(ref objFirst2, objLen2 + 1); // Номер первой строки объекта
                    Array.Resize(ref objLast2, objLen2 + 1); // Номер последней строки объекта
                    Array.Resize(ref objColor2, objLen2 + 1); // Цвет объекта
                    Array.Resize(ref objXCenter2, objLen2 + 1); // Координаты средней точки Х объекта
                    Array.Resize(ref objYCenter2, objLen2 + 1); // Координаты средней точки Y объекта
                    Array.Resize(ref objXCenterBox2, objLen2 + 1); // Координаты средней точки Х прямоугольника в который вписан объект
                    Array.Resize(ref objYCenterBox2, objLen2 + 1); // Координаты средней точки Y прямоугольника в который вписан объект
                    Array.Resize(ref objXDimension2, objLen2 + 1); // Размер объекта по X
                    Array.Resize(ref objYDimension2, objLen2 + 1); // Размер объекта по Y
                    Array.Resize(ref objOrient2, objLen2 + 1); // Направление обхода объекта
                    Array.Resize(ref objChilds2, objLen2 + 1); // Список номеров дочерних объектов
                    Array.Resize(ref objRoot2, objLen2 + 1); // Уровень вложенности объекта, 0 - объект наружный
                    Array.Resize(ref objParent2, objLen2 + 1); // Родительский объект
                    // и записываем туда все значения со старого, кроме номеров строк
                    objType2[objLen2] = AppSettings.ObjectType[objCur];
                    objColor2[objLen2] = AppSettings.ObjectColor[objCur];
                    objXCenter2[objLen2] = AppSettings.ObjXCenter[objCur];
                    objYCenter2[objLen2] = AppSettings.ObjYCenter[objCur];
                    objXCenterBox2[objLen2] = AppSettings.ObjXCenterBox[objCur];
                    objYCenterBox2[objLen2] = AppSettings.ObjYCenterBox[objCur];
                    objXDimension2[objLen2] = AppSettings.ObjXDimension[objCur];
                    objYDimension2[objLen2] = AppSettings.ObjYDimension[objCur];
                    objOrient2[objLen2] = AppSettings.ObjOrient[objCur];
                    objChilds2[objLen2] = AppSettings.ObjChilds[objCur];
                    objRoot2[objLen2] = AppSettings.ObjRoot[objCur];
                    objParent2[objLen2] = AppSettings.ObjParent[objCur];
                    objFirst2[objLen2] = polyLen2 + 1; // Запишем в начальное значение номера строки данных следующее значение
                    // расширяем массив с координатами на длину координат текущего объекта
                    Array.Resize(ref polyX2, polyLen2 + AppSettings.ObjLast[objCur] - AppSettings.ObjFirst[objCur] + 2); // Координата X
                    Array.Resize(ref polyY2, polyLen2 + AppSettings.ObjLast[objCur] - AppSettings.ObjFirst[objCur] + 2); // Координата Y
                    for (AppSettings.PolyCur = AppSettings.ObjFirst[objCur]; AppSettings.PolyCur <= AppSettings.ObjLast[objCur]; AppSettings.PolyCur++)
                    {
                        polyLen2++; // Увеличиваем номер временной строки с координатами
                        polyX2[polyLen2] = AppSettings.PolygonX[AppSettings.PolyCur];
                        polyY2[polyLen2] = AppSettings.PolygonY[AppSettings.PolyCur];
                    }

                    objLast2[objLen2] = polyLen2; // Запишем в последнее значение номера строки данных объекта полученное значение счетчика строк
                }
            }

            // Очистим и подгоним размеры нового массива
            AppSettings.ObjLen = objLen2;
            Array.Resize(ref AppSettings.ObjectType, AppSettings.ObjLen + 1); // Тип объекта N, L, P, +, X, l, p
            Array.Resize(ref AppSettings.ObjFirst, AppSettings.ObjLen + 1); // Номер первой строки объекта
            Array.Resize(ref AppSettings.ObjLast, AppSettings.ObjLen + 1); // Номер последней строки объекта
            Array.Resize(ref AppSettings.ObjectColor, AppSettings.ObjLen + 1); // Цвет объекта
            Array.Resize(ref AppSettings.ObjXCenter, AppSettings.ObjLen + 1); // Координаты средней точки Х объекта
            Array.Resize(ref AppSettings.ObjYCenter, AppSettings.ObjLen + 1); // Координаты средней точки Y объекта
            Array.Resize(ref AppSettings.ObjXCenterBox, AppSettings.ObjLen + 1); // Координаты средней точки Х прямоугольника в который вписан объект
            Array.Resize(ref AppSettings.ObjYCenterBox, AppSettings.ObjLen + 1); // Координаты средней точки Y прямоугольника в который вписан объект
            Array.Resize(ref AppSettings.ObjXDimension, AppSettings.ObjLen + 1); // Размер объекта по X
            Array.Resize(ref AppSettings.ObjYDimension, AppSettings.ObjLen + 1); // Размер объекта по Y
            Array.Resize(ref AppSettings.ObjOrient, AppSettings.ObjLen + 1); // Направление обхода объекта
            Array.Resize(ref AppSettings.ObjChilds, AppSettings.ObjLen + 1); // Список номеров дочерних объектов
            Array.Resize(ref AppSettings.ObjRoot, AppSettings.ObjLen + 1); // Уровень вложенности объекта, 0 - объект наружный
            Array.Resize(ref AppSettings.ObjParent, AppSettings.ObjLen + 1); // Родительский объект
            for (var i = 1; i <= objLen2; i++)
            {
                AppSettings.ObjectType[i] = objType2[i];
                AppSettings.ObjFirst[i] = objFirst2[i];
                AppSettings.ObjLast[i] = objLast2[i];
                AppSettings.ObjectColor[i] = objColor2[i];
                AppSettings.ObjXCenter[i] = objXCenter2[i];
                AppSettings.ObjYCenter[i] = objYCenter2[i];
                AppSettings.ObjXCenterBox[i] = objXCenterBox2[i];
                AppSettings.ObjYCenterBox[i] = objYCenterBox2[i];
                AppSettings.ObjXDimension[i] = objXDimension2[i];
                AppSettings.ObjYDimension[i] = objYDimension2[i];
                AppSettings.ObjOrient[i] = objOrient2[i];
                AppSettings.ObjChilds[i] = objChilds2[i];
                AppSettings.ObjRoot[i] = objRoot2[i];
                AppSettings.ObjParent[i] = objParent2[i];
            }

            AppSettings.PolyLen = polyLen2; // количество строк в массиве координат
            // Очистим и подгоним размеры нового массива координат
            Array.Resize(ref AppSettings.PolygonX, AppSettings.PolyLen + 1); // Координата X
            Array.Resize(ref AppSettings.PolygonY, AppSettings.PolyLen + 1); // Координата Y
            for (var i = 1; i <= polyLen2; i++)
            {
                AppSettings.PolygonX[i] = polyX2[i];
                AppSettings.PolygonY[i] = polyY2[i];
            }

            DebugTextBox.Text += $"В результате - Строк: {AppSettings.PolyLen} Объектов: {AppSettings.ObjLen} \n";
        }

        private void FormatArrayPercent() // Приведение координат всех объектов к нулю в нижнем левом углу И применение автомасштаба
        {
            if (AppSettings.AutoSizePercent == 1)
            {
                AppSettings.PluPercentAuto = 1000;
            }
            else
            {
                AppSettings.PluPercentAuto = AppSettings.PluPercent;
            }

            FormatArrayToZero();
            while (AppSettings.PolygonMaxX > AppSettings.MaxTableSizeX && AppSettings.AutoSizePercent == 1)
            {
                AppSettings.PluPercentAuto /= 10;
                readFilePLT(); // Открываем, считываем все вектора в массив и закрываем файл
                FormatArrayLP(); // Поиск открытых и закрытых объектов
                AppSettings.ConnectCount = 1; // Устанавливаем счетчик соединений, чтобы запустить первый проход
                while (AppSettings.ConnectCount != 0)
                {
                    FormatArrayConnect(); // Находим и соединяем вектора разных объектов в один
                    FormatArrayLP(); // Поиск открытых и закрытых объектов
                    FormatArrayDefrag(); // Дефрагментация и упорядочивание массива объектов
                }

                FormatArrayToZero();
            }

            if (AppSettings.AutoSizePercent == 1)
            {
                AppSettings.PluPercent = AppSettings.PluPercentAuto;
                SaveIniString("PluPercent", AppSettings.PluPercent.ToString());
                ScaleTextBox.Text = AppSettings.PluPercent + "%";
            }
        }

        private void FormatArrayToZero() // Приведение координат всех объектов к нулю в нижнем левом углу
        {
            DebugTextBox.Text += "Приведение координат объектов к нижнему левому углу... ";
            // Переведем сначала все длины в реальные миллиметры с учетом обратной пропорции масштаба при импорте
            for (AppSettings.PolyCur = 1; AppSettings.PolyCur <= AppSettings.PolyLen; AppSettings.PolyCur++)
            {
                AppSettings.PolygonX[AppSettings.PolyCur] = AppSettings.PolygonX[AppSettings.PolyCur] / AppSettings.PluInch * 25.4f * (AppSettings.PluPercentAuto / 100f);
                AppSettings.PolygonY[AppSettings.PolyCur] = AppSettings.PolygonY[AppSettings.PolyCur] / AppSettings.PluInch * 25.4f * (AppSettings.PluPercentAuto / 100f);
            }

            float polyXMin = AppSettings.PolygonX[1]; // Будем считать первую координату минимальной
            float polyYMin = AppSettings.PolygonY[1]; // Будем считать первую координату минимальной
            AppSettings.PolygonMaxX = AppSettings.PolygonX[2]; // Будем считать первую координату максимальной
            AppSettings.PolygonMaxY = AppSettings.PolygonY[2]; // Будем считать первую координату максимальной
            for (var objCur = 1; objCur <= AppSettings.ObjLen; objCur++) // Перебираем все объекты
            {
                if (AppSettings.ObjectType[objCur] != 'D') // И только действительные из них
                {
                    for (AppSettings.PolyCur = AppSettings.ObjFirst[objCur]; AppSettings.PolyCur <= AppSettings.ObjLast[objCur]; AppSettings.PolyCur++) // Перебираем все действительные значения координат объекта
                    {
                        if (AppSettings.PolygonX[AppSettings.PolyCur] < polyXMin)
                        {
                            polyXMin = AppSettings.PolygonX[AppSettings.PolyCur];
                        }

                        if (AppSettings.PolygonY[AppSettings.PolyCur] < polyYMin)
                        {
                            polyYMin = AppSettings.PolygonY[AppSettings.PolyCur];
                        }

                        if (AppSettings.PolygonX[AppSettings.PolyCur] > AppSettings.PolygonMaxX)
                        {
                            AppSettings.PolygonMaxX = AppSettings.PolygonX[AppSettings.PolyCur];
                        }

                        if (AppSettings.PolygonY[AppSettings.PolyCur] > AppSettings.PolygonMaxY)
                        {
                            AppSettings.PolygonMaxY = AppSettings.PolygonY[AppSettings.PolyCur];
                        }
                    }
                }
            }

            AppSettings.PolygonMaxX -= polyXMin; // Приводим к норме максимальное значение
            AppSettings.PolygonMaxY -= polyYMin; // Приводим к норме максимальное значение
            for (var objCur = 1; objCur <= AppSettings.ObjLen; objCur++) // Перебираем опять все объекты
            {
                if (AppSettings.ObjectType[objCur] != 'D') // И только действительные из них
                {
                    for (AppSettings.PolyCur = AppSettings.ObjFirst[objCur]; AppSettings.PolyCur <= AppSettings.ObjLast[objCur]; AppSettings.PolyCur++) // Перебираем все действительные значения координат объекта
                    {
                        AppSettings.PolygonX[AppSettings.PolyCur] = AppSettings.PolygonX[AppSettings.PolyCur] - polyXMin; // Вычитаем из текущего значения минимально возможное
                        AppSettings.PolygonY[AppSettings.PolyCur] = AppSettings.PolygonY[AppSettings.PolyCur] - polyYMin; // Вычитаем из текущего значения минимально возможное
                    }
                }
            }

            DebugTextBox.Text += $"MaxX: {AppSettings.PolygonMaxX} MaxY: {AppSettings.PolygonMaxY} \n";
        }

        private bool SaveIniString(string iniString, string iniValue) // Записываем в строку ини-файла значение
        {
            iniValue = iniValue.Replace(',', '.').Trim();
            var iniArray = new string[1];
            var iniLineNumber = 0;
            using (var streamReader = new StreamReader($"{AppSettings.ProgDir}\\settings.ini"))
            {
                while (!streamReader.EndOfStream)
                {
                    iniLineNumber++;
                    Array.Resize(ref iniArray, iniLineNumber);
                    iniArray[iniLineNumber - 1] = streamReader.ReadLine().Trim();
                }
            }

            int iniLineMax = iniLineNumber;
            var iniLineSaved = false;
            using (var streamWriter = new StreamWriter($"{AppSettings.ProgDir}\\settings.ini"))
            {
                for (iniLineNumber = 0; iniLineNumber < iniLineMax; iniLineNumber++)
                {
                    if (iniArray[iniLineNumber].Substring(0, iniArray[iniLineNumber].IndexOf(':')) == iniString)
                    {
                        streamWriter.WriteLine($"{iniString}: {iniValue}");
                        iniLineSaved = true;
                    }
                    else
                    {
                        streamWriter.WriteLine(iniArray[iniLineNumber]);
                    }
                }

                if (!iniLineSaved)
                {
                    streamWriter.WriteLine($"{iniString}: {iniValue}");
                }
            }

            return true;
        }

        private void ReverseInnerPolygons() // Делаем внутренние вложенные нечетные полигоны встречными по отношению к основному движению
        {
            var polyXTmp = new float[AppSettings.PolyLen + 1]; // Координата X - временный массив
            var polyYTmp = new float[AppSettings.PolyLen + 1]; // Координата Y - временный массив
            for (var objCur = 1; objCur <= AppSettings.ObjLen; objCur++)
            {
                if (AppSettings.ObjectType[objCur] == 'P' && (AppSettings.ObjRoot[objCur] % 2) != 0) // Если это закрытые объекты с нечетным уровнем вложенности
                {
                    if (AppSettings.ObjOrient[objCur] == 1) // Если Обход по часовой стрелке - попутный для фрезы
                    {
                        AppSettings.ObjOrient[objCur] = 0; // То делаем его встречным
                    }
                    else
                    {
                        AppSettings.ObjOrient[objCur] = 1; // Если он был встречным, то делаем его попутным
                    }

                    for (AppSettings.PolyCur = AppSettings.ObjFirst[objCur]; AppSettings.PolyCur <= AppSettings.ObjLast[objCur]; AppSettings.PolyCur++) // Проходим по всем значениям массива
                    {
                        polyXTmp[AppSettings.PolyCur] = AppSettings.PolygonX[AppSettings.ObjFirst[objCur] + AppSettings.ObjLast[objCur] - AppSettings.PolyCur]; // Заносим во временное значение, данные с противоположного конца массива
                        polyYTmp[AppSettings.PolyCur] = AppSettings.PolygonY[AppSettings.ObjFirst[objCur] + AppSettings.ObjLast[objCur] - AppSettings.PolyCur]; // Заносим во временное значение, данные с противоположного конца массива
                    }

                    for (AppSettings.PolyCur = AppSettings.ObjFirst[objCur]; AppSettings.PolyCur <= AppSettings.ObjLast[objCur]; AppSettings.PolyCur++) // Проходим по всем значениям массива
                    {
                        AppSettings.PolygonX[AppSettings.PolyCur] = polyXTmp[AppSettings.PolyCur]; // Заносим в текущее значение, данные с временного массива
                        AppSettings.PolygonY[AppSettings.PolyCur] = polyYTmp[AppSettings.PolyCur]; // Заносим в текущее значение, данные с временного массива
                    }
                }
            }
        }

        private void Statistic() // Полная статистика и вывод ее на экран
        {
            // Рассчет длин векторов
            var perimeterFull = 0f; // Длина всех объектов
            var objectCount = 0; // Количество действительных объектов
            for (var objCur = 1; objCur <= AppSettings.ObjLen; objCur++)
            {
                if (AppSettings.ObjectType[objCur] == 'P' || AppSettings.ObjectType[objCur] == 'L')
                {
                    objectCount++; // Увеличим количество действительных объектов
                    for (AppSettings.PolyCur = AppSettings.ObjFirst[objCur] + 1; AppSettings.PolyCur <= AppSettings.ObjLast[objCur]; AppSettings.PolyCur++)
                    {
                        perimeterFull += Convert.ToSingle(Math.Sqrt((AppSettings.PolygonY[AppSettings.PolyCur] - AppSettings.PolygonY[AppSettings.PolyCur - 1]) * (AppSettings.PolygonY[AppSettings.PolyCur] - AppSettings.PolygonY[AppSettings.PolyCur - 1]) + (AppSettings.PolygonX[AppSettings.PolyCur] - AppSettings.PolygonX[AppSettings.PolyCur - 1]) * (AppSettings.PolygonX[AppSettings.PolyCur] - AppSettings.PolygonX[AppSettings.PolyCur - 1])));
                    }

                    AppSettings.PolyCur--; // вернемся назад на 1 для компенсации цикла
                    if (AppSettings.ObjectType[objCur] == 'P') // И посчитаем замыкающий вектор еще до кучи
                    {
                        perimeterFull += Convert.ToSingle(Math.Sqrt((AppSettings.PolygonY[AppSettings.PolyCur] - AppSettings.PolygonY[AppSettings.ObjFirst[objCur]]) * (AppSettings.PolygonY[AppSettings.PolyCur] - AppSettings.PolygonY[AppSettings.ObjFirst[objCur]]) + (AppSettings.PolygonX[AppSettings.PolyCur] - AppSettings.PolygonX[AppSettings.ObjFirst[objCur]]) * (AppSettings.PolygonX[AppSettings.PolyCur] - AppSettings.PolygonX[AppSettings.ObjFirst[objCur]])));
                    }
                }
            }

            DebugTextBox.Text += $"\n Сводная информация по файлу: {AppSettings.OpenDirectory}\\{AppSettings.FileName}{AppSettings.FileDim} \n";
            DebugTextBox.Text += $"ВСЕГО: Строк: {AppSettings.PolyLen} Объектов: {objectCount} Суммарная длина всех векторов: {ToNCLight(perimeterFull)} мм или {ToNCLight(perimeterFull / 1000)} метров \n Наружный размер: Ширина: {AppSettings.PolygonMaxX} мм Высота: {AppSettings.PolygonMaxY} мм";
            if (AppSettings.AutoSizePercent == 1 && AppSettings.PluPercentAuto != 100)
            {
                DebugTextBox.Text += $"АВТОМАСШТАБ: {AppSettings.PluPercentAuto}%";
            }
        }

        private void FileSaveAndCloseButton_Click(object sender, EventArgs e) // Сохранение файла
        {
            // ReverseInnerPolygons() ' Делаем внутренние вложенные нечетные полигоны встречными по отношению к основному движению
            // Обращение к функции вынесено в функцию открытия файла, чтобы все объекты были подписаны при выводе на экран
            // --------------------------------------------------------------------------------------------------------------------------
            // вероятно в верхней функции притаилась ошибка, т.к. нужно делать встречными по отношению к основномнуму движению не внутренние нечетные полигоны, а все внутренние полигоны первого уровня вложенности.
            // либо все полигоны, а потом слуедующущую вложенность еще раз перекрутить.
            // --------------------------------------------------------------------------------------------------------------------------
            if (OpenVectorsCombobox.SelectedIndex == 1) // Если выбрано что линии режутся от конца к началу, то меняем направление всех линий.
            {
                ReverseLines(); // Меняем у всех открытых Объектов направление движения на противоположное
            }

            if (CloseVectorProcessingCombobox1.SelectedIndex == 1) // Если выбрано что закрытые объекты режутся встречно, то меняем направление всех полигонов.
            {
                ReversePolygons(); // Меняем у всех Закрытых полигонов направление движения на противоположное
            }

            if (AppSettings.MillXY0 == 6 || AppSettings.MillXY0 == 8) // Если выбраны режимы с резкой лицом вниз
            {
                ReverseObjects(); // Меняем у всех действительных  объектов направление движения на противоположное
            }

            if (AppSettings.MillZ1 == 0 || AppSettings.MillZ1 > Math.Abs(AppSettings.MillZStart - AppSettings.MillZ))
            {
                AppSettings.MillZ1 = Math.Abs(AppSettings.MillZStart - AppSettings.MillZ); // Если глубина за 1 проход = 0, то пилим сразу все за один проход
                if (AppSettings.MillZ1 == 0)
                {
                    AppSettings.MillZ1 = 10000; // Бесконечно большое значение, чтобы цикл отработал только 1 раз
                }

                AppSettings.MillZStart = AppSettings.MillZ;
            }
            else
            {
                float millZFull = Convert.ToSingle(Math.Round(Math.Abs(AppSettings.MillZStart - AppSettings.MillZ), 3)); // Реальная общая глубина резки от начала до конца
                int millZSteps = Convert.ToInt32(Math.Ceiling(millZFull / AppSettings.MillZ1)); // Количество целых проходов округленное в сторону увеличения.
                AppSettings.MillZ1 = Convert.ToSingle(Math.Round(Math.Abs(millZFull / millZSteps), 3)); // Реальный новый дробный шаг одного прохода
                float millZRest = millZFull - (AppSettings.MillZ1 * (millZSteps - 1)); // Верхний остаток при осуществлении (всех проходов -1)
                AppSettings.MillZStart -= millZRest; // Начало первого прохода с учетом первого погружения по верхнему остатку
            }

            DebugTextBox.Text += $"\n Очищаем папку: {AppSettings.SaveDirectory}\\ \n";
            foreach (string file in Directory.GetFiles(AppSettings.SaveDirectory, "*.g")) // Очищаем текущую директорию от всех файлов *.g*
            {
                File.Delete(file);
            }

            foreach (string file in Directory.GetFiles(AppSettings.SaveDirectory, "*.nc")) // Очищаем текущую директорию от всех файлов *.nc*
            {
                File.Delete(file);
            }

            var iRand = 0;
            var rand = new Random(DateTime.Now.Millisecond);
            iRand = rand.Next(100, 999); // Создаем случайное число для добавление в имя файла, чтобы не было повторов
            DebugTextBox.Text += $"Записывается: {AppSettings.SaveDirectory}\\{AppSettings.FileName}.{iRand}.g \n";
            var streamWriter = new StreamWriter($"{AppSettings.SaveDirectory}\\{AppSettings.FileName}.{iRand}.g");
            streamWriter.WriteLine($"{LineNumber()}%\n"); // начало файла NC
            var laserOffset = 0f;
            if (AppSettings.MillXY0 > 2) // если по лазеру, учитываем смещение лазера по Y
            {
                laserOffset = AppSettings.LaserY;
            }
            else
            {
                laserOffset = 0f;
            }

            if (AppSettings.MillXY0 == 0 || AppSettings.MillXY0 == 1 || AppSettings.MillXY0 == 3 || AppSettings.MillXY0 == 4)
            {
                // если по фрезе, по фрезе и 0, по лазеру или по лазеру и 0
                streamWriter.WriteLine($"{LineNumber()}T01 M06 M08\n"); // Выбираем инструмент 01, включаем охлаждение
                streamWriter.WriteLine($"{LineNumber()}G00 Z{ToNCString(AppSettings.SafeZ)} S{AppSettings.MillS} M03\n"); // Поднимем фрезу и Запустим шпиндель с его скоростью
                streamWriter.WriteLine($"{LineNumber()}G92 X0.000 Y{ToNCString(laserOffset)} Z{ToNCString(AppSettings.SafeZ)}\n"); // установка текущих координат
                streamWriter.WriteLine($"{LineNumber()}M801 \"|W| = Waiting {AppSettings.MillP / 1000} seconds for start spindel... =\"\n"); // Выводим надпись на экран
                streamWriter.WriteLine($"{LineNumber()}G04 P{AppSettings.MillP}\n"); // Задержка на разгон шпинделя
                streamWriter.WriteLine($"{LineNumber()}M801 \"\"\n"); // Выводим надпись на экран
            }
            else if (AppSettings.MillXY0 == 5) // По лазеру и точкам 1 и 0
            {
                streamWriter.WriteLine($"{LineNumber()}'----- И  Н  С  Т  Р  У  К  Ц  И  Я -----\n");
                streamWriter.WriteLine($"{LineNumber()}' 1. ПРОВЕРИТЬ или ВВЕСТИ значения:\n");
                streamWriter.WriteLine($"{LineNumber()}'(Файл=>Править Загруженный Файл)\n");
                streamWriter.WriteLine($"{LineNumber()}#1={ToNCString(AppSettings.MillX1)} ' =X= (миллиметры)\n");
                streamWriter.WriteLine($"{LineNumber()}#2={ToNCString(AppSettings.MillY1)} ' =Y= (миллиметры)\n");
                streamWriter.WriteLine($"{LineNumber()}'(Файл=>Сохранить и Загрузить)\n");
                streamWriter.WriteLine($"{LineNumber()}' 2. ОТКАЛИБРОВАТЬ поверхность (7-я иконка)\n");
                streamWriter.WriteLine($"{LineNumber()}' 3. Положить ЛИЦОМ ВВЕРХ, ШАПКОЙ ОТ СЕБЯ\n");
                streamWriter.WriteLine($"{LineNumber()}' 4. поставить ТОЧКУ =0= ПОД ЛАЗЕР и закрепить\n");
                streamWriter.WriteLine($"{LineNumber()}' 5. НАЖАТЬ =СТАРТ= (F9)\n");
                streamWriter.WriteLine($"{LineNumber()}' 6. Подвинуть ТОЧКУ =1= ПОД ЛАЗЕР и закрепить\n");
                streamWriter.WriteLine($"{LineNumber()}' 7. Через \" & MillP / 1000 & \" секунд НАЧНЕТСЯ РЕЗКА.\n");
                streamWriter.WriteLine($"{LineNumber()}'------- ЭТО ПРОГРАММА - ЕЁ НЕ МЕНЯТЬ ---------\n");
                streamWriter.WriteLine($"{LineNumber()}M801 \"\"\n"); // Выводим надпись на экран
                streamWriter.WriteLine($"{LineNumber()}T01 M06 M08\n"); // Выбираем инструмент 01, включаем охлаждение
                streamWriter.WriteLine($"{LineNumber()}G00 Z{ToNCString(AppSettings.SafeZ)} S{AppSettings.MillS} M03\n"); // Поднимем фрезу и Запустим шпиндель с его скоростью
                streamWriter.WriteLine($"{LineNumber()}G92 X0.000 Y{ToNCString(laserOffset)} Z{ToNCString(AppSettings.SafeZ)}\n"); // установка текущих координат
                streamWriter.WriteLine($"{LineNumber()}G00 X=#1 Y=#2+{ToNCString(laserOffset)} Z{ToNCString(AppSettings.SafeZ)}\n"); // едем к точке 1
                streamWriter.WriteLine($"{LineNumber()}M801 \"|W| = Waiting {AppSettings.MillP / 1000} seconds for start spindel... =\"\n"); // Выводим надпись на экран
                streamWriter.WriteLine($"{LineNumber()}G04 P{AppSettings.MillP}\n"); // Задержка на разгон шпинделя
                streamWriter.WriteLine($"{LineNumber()}M801 \"\"\n"); // Выводим надпись на экран
            }
            else if (AppSettings.MillXY0 == 2) // По фрезе и точкам 1 и 0
            {
                streamWriter.WriteLine($"{LineNumber()}'----- И  Н  С  Т  Р  У  К  Ц  И  Я -----\n");
                streamWriter.WriteLine($"{LineNumber()}' 1. ПРОВЕРИТЬ или ВВЕСТИ значения:\n");
                streamWriter.WriteLine($"{LineNumber()}'(Файл=>Править Загруженный Файл)\n");
                streamWriter.WriteLine($"{LineNumber()}#1={ToNCString(AppSettings.MillX1)} ' =X= (миллиметры)\n");
                streamWriter.WriteLine($"{LineNumber()}#2={ToNCString(AppSettings.MillY1)} ' =Y= (миллиметры)\n");
                streamWriter.WriteLine($"{LineNumber()}'(Файл=>Сохранить и Загрузить)\n");
                streamWriter.WriteLine($"{LineNumber()}' 2. ОТКАЛИБРОВАТЬ поверхность (7-я иконка)\n");
                streamWriter.WriteLine($"{LineNumber()}' 3. Положить ЛИЦОМ ВВЕРХ, ШАПКОЙ ОТ СЕБЯ\n");
                streamWriter.WriteLine($"{LineNumber()}' 4. поставить ФРЕЗУ НА ТОЧКОЙ =0= и закрепить\n");
                streamWriter.WriteLine($"{LineNumber()}' 5. НАЖАТЬ =СТАРТ= (F9)\n");
                streamWriter.WriteLine($"{LineNumber()}' 6. Подвинуть ТОЧКУ =1= ПОД ФРЕЗУ и закрепить\n");
                streamWriter.WriteLine($"{LineNumber()}' 7. Через {AppSettings.MillP / 1000 / 2} секунд запуститься ШПИНДЕЛЬ.\n");
                streamWriter.WriteLine($"{LineNumber()}' 8. Через {AppSettings.MillP / 1000} секунд НАЧНЕТСЯ РЕЗКА.\n");
                streamWriter.WriteLine($"{LineNumber()}'------- ЭТО ПРОГРАММА - ЕЁ НЕ МЕНЯТЬ ---------\n");
                streamWriter.WriteLine($"{LineNumber()}M801 \"\"\n"); // Выводим надпись на экран
                streamWriter.WriteLine($"{LineNumber()}T01 M06 M08\n"); // Выбираем инструмент 01, включаем охлаждение
                streamWriter.WriteLine($"{LineNumber()}G00 Z{ToNCString(AppSettings.SafeZ)}\n"); // Поднимем фрезу
                streamWriter.WriteLine($"{LineNumber()}M05\n"); // остановка шпинделя
                streamWriter.WriteLine($"{LineNumber()}G92 X0.000 Y{ToNCString(laserOffset)} Z{ToNCString(AppSettings.SafeZ)}\n"); // установка текущих координат
                streamWriter.WriteLine($"{LineNumber()}G00 X=#1 Y=#2 Z{ToNCString(AppSettings.SafeZ)}\n"); // едем к точке 1
                streamWriter.WriteLine($"{LineNumber()}M801 \"|W| = Warning!!! after {AppSettings.MillP / 1000 / 2} seconds spindel start ... =\"\n"); // Выводим надпись на экран
                streamWriter.WriteLine($"{LineNumber()}G04 P{AppSettings.MillP / 2}\n"); // Задержка на позиционирование по фрезе перед запуском шпинделя
                streamWriter.WriteLine($"{LineNumber()}G00 Z{ToNCString(AppSettings.SafeZ)} S{AppSettings.MillS} M03\n"); // Поднимем фрезу и Запустим шпиндель с его скоростью
                streamWriter.WriteLine($"{LineNumber()}M801 \"|W| = Waiting {AppSettings.MillP / 1000} seconds for start spindel... =\"\n"); // Выводим надпись на экран
                streamWriter.WriteLine($"{LineNumber()}G04 P{AppSettings.MillP}\n"); // Задержка на разгон шпинделя
                streamWriter.WriteLine($"{LineNumber()}M801 \"\"\n"); // Выводим надпись на экран
            }
            else if (AppSettings.MillXY0 == 6) // По лазеру ЛИЦОМ ВНИЗ и точкам 1 и 0
            {
                streamWriter.WriteLine($"{LineNumber()}'----- И  Н  С  Т  Р  У  К  Ц  И  Я -----\n");
                streamWriter.WriteLine($"{LineNumber()}' 1. ПРОВЕРИТЬ или ВВЕСТИ значения:\n");
                streamWriter.WriteLine($"{LineNumber()}'(Файл=>Править Загруженный Файл)\n");
                streamWriter.WriteLine($"{LineNumber()}#1={ToNCString(AppSettings.MillX1)} ' =X= (миллиметры)\n");
                streamWriter.WriteLine($"{LineNumber()}#2={ToNCString(AppSettings.MillY1)} ' =Y= (миллиметры)\n");
                streamWriter.WriteLine($"{LineNumber()}'(Файл=>Сохранить и Загрузить)\n");
                streamWriter.WriteLine($"{LineNumber()}' 2. ОТКАЛИБРОВАТЬ поверхность (7-я иконка)\n");
                streamWriter.WriteLine($"{LineNumber()}' 3. Положить ЛИЦОМ ВНИЗ, ШАПКОЙ К СЕБЕ\n");
                streamWriter.WriteLine($"{LineNumber()}' 4. поставить ТОЧКУ =0= ПОД ЛАЗЕР и закрепить\n");
                streamWriter.WriteLine($"{LineNumber()}' 5. НАЖАТЬ =СТАРТ= (F9)\n");
                streamWriter.WriteLine($"{LineNumber()}' 6. Подвинуть вторую ТОЧКУ ПОД ЛАЗЕР и закрепить\n");
                streamWriter.WriteLine($"{LineNumber()}' 7. Через {AppSettings.MillP / 1000} секунд НАЧНЕТСЯ РЕЗКА.\n");
                streamWriter.WriteLine($"{LineNumber()}'------- ЭТО ПРОГРАММА - ЕЁ НЕ МЕНЯТЬ ---------\n");
                streamWriter.WriteLine($"{LineNumber()}M801 \"\"\n"); // Выводим надпись на экран
                streamWriter.WriteLine($"{LineNumber()}T01 M06 M08\n"); // Выбираем инструмент 01, включаем охлаждение
                streamWriter.WriteLine($"{LineNumber()}G00 Z{ToNCString(AppSettings.SafeZ)} S{AppSettings.MillS} M03\n"); // Поднимем фрезу и Запустим шпиндель с его скоростью
                streamWriter.WriteLine($"{LineNumber()}G92 X0.000 Y{ToNCString(laserOffset)} Z{ToNCString(AppSettings.SafeZ)}\n"); // установка текущих координат
                streamWriter.WriteLine($"{LineNumber()}G51 J0 P-1\n"); // Отразим зеркально систему координат по Y
                streamWriter.WriteLine($"{LineNumber()}G00 X=#1 Y=#2-{ToNCString(laserOffset)} Z{ToNCString(AppSettings.SafeZ)}\n"); // едем к точке 1
                streamWriter.WriteLine($"{LineNumber()}M801 \"|W| = Waiting {AppSettings.MillP / 1000} seconds for start spindel... =\"\n"); // Выводим надпись на экран
                streamWriter.WriteLine($"{LineNumber()}G04 P{AppSettings.MillP}\n"); // Задержка на разгон шпинделя
                streamWriter.WriteLine($"{LineNumber()}M801 \"\"\n"); // Выводим надпись на экран
            }
            else if (AppSettings.MillXY0 == 7) // По лазеру и точкам 0, 1 и 2
            {
                streamWriter.WriteLine($"{LineNumber()}'----- И  Н  С  Т  Р  У  К  Ц  И  Я -----\n");
                streamWriter.WriteLine($"{LineNumber()}' 1. ОТКАЛИБРОВАТЬ поверхность (7-я иконка)\n");
                streamWriter.WriteLine($"{LineNumber()}' 2. Положить ЛИЦОМ ВВЕРХ, ШАПКОЙ ОТ СЕБЯ\n");
                streamWriter.WriteLine($"{LineNumber()}' 3. ЛАЗЕР на ТОЧКУ =0=, закрепить и НАЖАТЬ =СТАРТ= (F9)\n");
                streamWriter.WriteLine($"{LineNumber()}' 4. ТОЧКУ -1- макс. подвинуть под лазер, потом (Операции => Покачивать)\n");
                streamWriter.WriteLine($"{LineNumber()}' 5. Перемещая ЛАЗЕР, точно установить его на ТОЧКУ -1- и =СТАРТ= (F9)\n");
                streamWriter.WriteLine($"{LineNumber()}' 6. (Операции => Покачивать) установить ЛАЗЕР точно на ТОЧКУ -2-\n");
                streamWriter.WriteLine($"{LineNumber()}' 7. ЗАКРЕПИТЬ! По выходу шпинделя на обороты нажать =СТАРТ= (F9)\n");
                streamWriter.WriteLine($"{LineNumber()}' 8. РЕЗКА начнется НЕЗАМЕДЛИТЕЛЬНО!\n");
                streamWriter.WriteLine($"{LineNumber()}'------- ЭТО ПРОГРАММА - ЕЁ НЕ МЕНЯТЬ ---------\n");
                streamWriter.WriteLine($"{LineNumber()}M801 \"\"\n"); // Очищаем область сообщений
                streamWriter.WriteLine($"{LineNumber()}T01 M06 M08\n"); // Выбираем инструмент 01, включаем охлаждение
                streamWriter.WriteLine($"{LineNumber()}G00 Z{ToNCString(AppSettings.SafeZ)} M05\n"); // Поднимем фрезу и остановим шпиндель
                streamWriter.WriteLine($"{LineNumber()}#1={ToNCString(AppSettings.MillX1)} ' =X1=\n");
                streamWriter.WriteLine($"{LineNumber()}#2={ToNCString(AppSettings.MillY1)} ' =Y1=\n");
                streamWriter.WriteLine($"{LineNumber()}#3={ToNCString(AppSettings.MillX2)} ' =X2=\n");
                streamWriter.WriteLine($"{LineNumber()}#4={ToNCString(AppSettings.MillY2)} ' =Y2=\n");
                streamWriter.WriteLine($"{LineNumber()}M801 \"\"\n"); // Очищаем область сообщений
                streamWriter.WriteLine($"{LineNumber()}G92 X0.000 Y{ToNCString(laserOffset)} Z{ToNCString(AppSettings.SafeZ)}\n"); // установка текущих координат
                streamWriter.WriteLine($"{LineNumber()}G00 X=#1 Y=#2+{ToNCString(laserOffset)} Z{ToNCString(AppSettings.SafeZ)}\n"); // едем к точке 1
                streamWriter.WriteLine($"{LineNumber()}G906\n"); // Синхронизация
                streamWriter.WriteLine($"{LineNumber()}M801 \"|W| -=< Correct Point -1- & START(F9) again to Continue >=-\"\n"); // Сообщение о коррекции точки 1
                streamWriter.WriteLine($"{LineNumber()}M00\n"); // Останов на паузу для коррекции
                streamWriter.WriteLine($"{LineNumber()}M801 \"\"\n"); // Очищаем область сообщений
                streamWriter.WriteLine($"{LineNumber()}G906\n"); // Синхронизация
                streamWriter.WriteLine($"{LineNumber()}IF (#1) #5=#SSCURWORKCOOR0/#1\n"); // Получаем первичный масштаб по Х
                streamWriter.WriteLine($"{LineNumber()}IF (!#1) #5=(#SSCURWORKCOOR0+0.001)/(#1+0.001)\n"); // Получаем первичный масштаб по Х, если X равен нулю
                streamWriter.WriteLine($"{LineNumber()}IF (#2+{ToNCString(laserOffset)}) #6=#SSCURWORKCOOR1/(#2+{ToNCString(laserOffset)})\n"); // Получаем первичный масштаб по Y
                streamWriter.WriteLine($"{LineNumber()}IF (!(#2+{ToNCString(laserOffset)})) #6=(#SSCURWORKCOOR1+0.001)/(#2+{ToNCString(laserOffset)}+0.001)\n"); // Получаем первичный масштаб по Y, если Y равен нулю
                streamWriter.WriteLine($"{LineNumber()}S{AppSettings.MillS} M03\n"); // Запустим шпиндель с его скоростью
                streamWriter.WriteLine($"{LineNumber()}G00 X=(#3*#5) Y=((#4+{ToNCString(laserOffset)})*#6)\n"); // едем к точке 2 уже с учетом полученного масштаба
                streamWriter.WriteLine($"{LineNumber()}G906\n"); // Синхронизация
                streamWriter.WriteLine($"{LineNumber()}M801 \"|W| -=< Correct Point -2- & START(F9) again to Continue >=-\"\n"); // Сообщение о коррекции точки 2
                streamWriter.WriteLine($"{LineNumber()}M00\n"); // Останов на паузу для коррекции
                streamWriter.WriteLine($"{LineNumber()}M801 \"\"\n"); // Очищаем область сообщений
                streamWriter.WriteLine($"{LineNumber()}G906\n"); // Синхронизация
                streamWriter.WriteLine($"{LineNumber()}IF (#3) #7=#SSCURWORKCOOR0/#3\n"); // Получаем результирующий масштаб по Х
                streamWriter.WriteLine($"{LineNumber()}IF (!#3) #7=(#SSCURWORKCOOR0+0.001)/(#3+0.001)\n"); // Получаем результирующий масштаб по Х, если X равен нулю
                streamWriter.WriteLine($"{LineNumber()}IF (#4+{ToNCString(laserOffset)}) #8=#SSCURWORKCOOR1/(#4+{ToNCString(laserOffset)})\n"); // Получаем результирующий масштаб по Y
                streamWriter.WriteLine($"{LineNumber()}IF (!(#4+{ToNCString(laserOffset)})) #8=(#SSCURWORKCOOR1+0.001)/(#4+{ToNCString(laserOffset)}+0.001)\n"); // Получаем результирующий масштаб по Y, если Y равен нулю
                streamWriter.WriteLine($"{LineNumber()}M801 \"|W| = Waiting {AppSettings.MillPs / 1000} seconds for start spindel... =\"\n"); // Выводим надпись на экран
                streamWriter.WriteLine($"{LineNumber()}G04 P{AppSettings.MillPs}\n"); // Задержка на разгон шпинделя
            }

            var curOperateObject = 0; // Текущий обрабатываемый объект
            AppSettings.ObjParent[0] = -1; // Для определения что вылезли выше верхнего уровня
            var curXPoint = 0f; // Текущая точка X
            var curYPoint = 0f + laserOffset; // Текущая точка Y

            if (AppSettings.MillXY0 > 1 && AppSettings.MillXY0 != 3 && AppSettings.MillXY0 != 4) // если выбрана привязка к точкам 1 или 1 и 2
            {
                if (AppSettings.MillX1 != 0 && AppSettings.MillY1 != 0) // если есть точка 1
                {
                    curXPoint = AppSettings.MillX1; // Текущая точка Х равна точке 1
                    curYPoint = AppSettings.MillY1 + laserOffset; // Текущая точка Y равна точке 1
                    if (AppSettings.MillXY0 == 6 || AppSettings.MillXY0 == 8)
                    {
                        curYPoint = AppSettings.MillY1 - laserOffset;
                    }
                }

                if (AppSettings.MillX2 != 0 && AppSettings.MillY2 != 0) // если есть точка 2
                {
                    curXPoint = AppSettings.MillX2; // Текущая точка Х равна точке 2
                    curYPoint = AppSettings.MillY2 + laserOffset; // Текущая точка Y равна точке 2
                    if (AppSettings.MillXY0 == 6 || AppSettings.MillXY0 == 8)
                    {
                        curYPoint = AppSettings.MillY1 - laserOffset;
                    }
                }
            }

            if (AppSettings.MillXY0 != 0 && AppSettings.MillXY0 != 3) // Если обработка пойдет не от нижнего левого угла
            {
                for (var objCur = 1; objCur <= AppSettings.ObjLen; objCur++) // перебираем все объекты
                {
                    if (AppSettings.ObjectType[objCur] == 'P' || AppSettings.ObjectType[objCur] == 'L') // и только действительные их них
                    {
                        for (AppSettings.PolyCur = AppSettings.ObjFirst[objCur]; AppSettings.PolyCur <= AppSettings.ObjLast[objCur]; AppSettings.PolyCur++) // перебираем все поля каждого объекта
                        {
                            AppSettings.PolygonX[AppSettings.PolyCur] = AppSettings.PolygonX[AppSettings.PolyCur] - AppSettings.Point0X; // Смещаем коорждинаты объекта на положение точки =0=
                            AppSettings.PolygonY[AppSettings.PolyCur] = AppSettings.PolygonY[AppSettings.PolyCur] - AppSettings.Point0Y; // Смещаем коорждинаты объекта на положение точки =0=
                        }
                    }
                }
            } // Конец условия если обработка пойдет не от нижнего левого угла

            // теперь идет непосредственно сам вывод координат в файл
            while (curOperateObject >= 0) // Крутим цикл пока есть хоть один ближайший объект на родительском уровне 0
            {
                GetChildList(curOperateObject); // Считываем всех потомков и заносим их в массивчик
                if (AppSettings.ChildListLen != 0) // Если в массивчике есть объекты
                {
                    float curShortestDistance = PointsDistance(0, 0, AppSettings.PolygonMaxX, AppSettings.PolygonMaxY); // текущая наименьшая дистанция
                    var newShortestDistance = 0f;
                    for (var objCur = 1; objCur <= AppSettings.ChildListLen; objCur++)
                    {
                        newShortestDistance = ShortestDistanceToObject(curXPoint, curYPoint, AppSettings.ChildList[objCur]);
                        if ((AppSettings.ObjectType[AppSettings.ChildList[objCur]] == 'P' || AppSettings.ObjectType[AppSettings.ChildList[objCur]] == 'L') && newShortestDistance < curShortestDistance)
                        {
                            curShortestDistance = newShortestDistance;
                            curOperateObject = AppSettings.ChildList[objCur]; // Наиближайший объект становится текущим
                        }
                    }

                    if (curShortestDistance == PointsDistance(0, 0, AppSettings.PolygonMaxX, AppSettings.PolygonMaxY)) // Если массивчик есть но в нем так и не нашли валидных объектов
                    {
                        // то очистим у текущего объекта список его дочерних, т.к. он уже весь отработан.
                        AppSettings.ObjChilds[curOperateObject] = "";
                        // и при следующем проходе окажется что массивчика уже нет
                    }
                }
                else // если в массивчике нет объектов
                {
                    // выводим тот объект в файл что есть, тоесть текущий
                    if (AppSettings.ObjectType[curOperateObject] == 'P' || AppSettings.ObjectType[curOperateObject] == 'L') // eсли это действительные объекты
                    {
                        RotateBeginOfObjectToPoint(curXPoint, curYPoint, curOperateObject); // Повернем объект ближней точкой к текущей
                        for (var millZCur = AppSettings.MillZStart; millZCur <= AppSettings.MillZ; millZCur += AppSettings.MillZ1 * -1) // Цикл количества проходов
                        {
                            if (millZCur == AppSettings.MillZStart) // первое погружение при многопроходной резке
                            {
                                if (AppSettings.MillXY0 == 7 || AppSettings.MillXY0 == 8) // Если по Лазеру и 0 1 2 точкам, то
                                {
                                    streamWriter.WriteLine($"{LineNumber()}G00 X={ToNCString(AppSettings.PolygonX[AppSettings.ObjFirst[curOperateObject]])}*#7 Y={ToNCString(AppSettings.PolygonY[AppSettings.ObjFirst[curOperateObject]])}*#8 Z{ToNCString(AppSettings.SafeZ)}\n");
                                    streamWriter.WriteLine($"{LineNumber()}G01 Z{ToNCString(millZCur)} F{AppSettings.MillFZ}\n");
                                }
                                else // если просто тогда
                                {
                                    streamWriter.WriteLine($"{LineNumber()}G00 X{ToNCString(AppSettings.PolygonX[AppSettings.ObjFirst[curOperateObject]])} Y{ToNCString(AppSettings.PolygonY[AppSettings.ObjFirst[curOperateObject]])} Z{ToNCString(AppSettings.SafeZ)}\n");
                                    streamWriter.WriteLine($"{LineNumber()}G01 Z{ToNCString(millZCur)} F{AppSettings.MillFZ}\n");
                                }
                            }
                            else // не первое погружение при мнопрохододовом режиме
                            {
                                streamWriter.WriteLine($"{LineNumber()}G01 Z{ToNCString(millZCur)} F{AppSettings.MillFZ}\n");
                            }

                            for (AppSettings.PolyCur = AppSettings.ObjFirst[curOperateObject] + 1; AppSettings.PolyCur <= AppSettings.ObjLast[curOperateObject]; AppSettings.PolyCur++) // проходим по всем строкам начиная со второй
                            {
                                if (AppSettings.PolyCur == (AppSettings.ObjFirst[curOperateObject] + 1)) // если это первая строка
                                {
                                    if (AppSettings.MillXY0 == 7 || AppSettings.MillXY0 == 8) // Если по Лазеру и 0 1 2 точкам, то
                                    {
                                        streamWriter.WriteLine($"{LineNumber()}G01 X={ToNCString(AppSettings.PolygonX[AppSettings.PolyCur])}*#7 Y={ToNCString(AppSettings.PolygonY[AppSettings.PolyCur])}*#8 F{ToNCString(AppSettings.MillF)}\n"); // то дописываем еще в конце подачу
                                    }
                                    else // если просто тогда
                                    {
                                        streamWriter.WriteLine($"{LineNumber()}G01 X{ToNCString(AppSettings.PolygonX[AppSettings.PolyCur])} Y{ToNCString(AppSettings.PolygonY[AppSettings.PolyCur])} F{ToNCString(AppSettings.MillF)}\n"); // то дописываем еще в конце подачу
                                    }
                                }
                                else
                                {
                                    if (AppSettings.MillXY0 == 7 || AppSettings.MillXY0 == 8) // Если по Лазеру и 0 1 2 точкам, то
                                    {
                                        streamWriter.WriteLine($"{LineNumber()}X={ToNCString(AppSettings.PolygonX[AppSettings.PolyCur])}*#7 Y={ToNCString(AppSettings.PolygonY[AppSettings.PolyCur])}*#8\n"); // просто бежим по значениям
                                    }
                                    else // если просто тогда
                                    {
                                        streamWriter.WriteLine($"{LineNumber()}X{ToNCString(AppSettings.PolygonX[AppSettings.PolyCur])} Y{ToNCString(AppSettings.PolygonY[AppSettings.PolyCur])}\n"); // просто бежим по значениям
                                    }
                                }
                            } // конец вывода в файл всех строк объекта

                            if (AppSettings.ObjectType[curOperateObject] == 'P') // если текущий объект был закрытым
                            {
                                // то дописываем в конце еще раз первую строку чтобы его закрыть
                                if (AppSettings.MillXY0 == 7 || AppSettings.MillXY0 == 8) // Если по Лазеру и 0 1 2 точкам, то
                                {
                                    streamWriter.WriteLine($"{LineNumber()}X={ToNCString(AppSettings.PolygonX[AppSettings.ObjFirst[curOperateObject]])}*#7 Y={ToNCString(AppSettings.PolygonY[AppSettings.ObjFirst[curOperateObject]])}*#8\n");
                                }
                                else // если просто тогда
                                {
                                    streamWriter.WriteLine($"{LineNumber()}X{ToNCString(AppSettings.PolygonX[AppSettings.ObjFirst[curOperateObject]])} Y{ToNCString(AppSettings.PolygonY[AppSettings.ObjFirst[curOperateObject]])}\n");
                                }
                            } // конец проверки закрытости объекта
                        } // конец цикла количества проходов
                    } // конец проверки действительности объекта

                    AppSettings.ObjectType[curOperateObject] = 'N';
                    curXPoint = AppSettings.PolygonX[AppSettings.ObjFirst[curOperateObject]]; // Текущая точка Х
                    curYPoint = AppSettings.PolygonY[AppSettings.ObjFirst[curOperateObject]]; // Текущая точка Y
                    curOperateObject = AppSettings.ObjParent[curOperateObject]; // Перейдем к вышестоящему обекту
                } // конец проверки массивчика на наличие в нем объектов
            }

            // финальные строки NC файла
            streamWriter.WriteLine($"{LineNumber()}G00 Z{ToNCString(AppSettings.SafeZ)}\n"); // поднимаем фрезу
            streamWriter.WriteLine($"{LineNumber()}M05 M09\n"); // остановка шпинделя и охлаждения
            if (AppSettings.MillXY0 == 6 || AppSettings.MillXY0 == 8) // Если был переворот по оси Y
            {
                streamWriter.WriteLine($"{LineNumber()}G00 X0.000 Y-{ToNCString(laserOffset)} Z{ToNCString(AppSettings.SafeZ)}\n"); // возвращение в начало
            }
            else if (AppSettings.MillXY0 == 7) // По лазеру и точкам 0, 1 и 2
            {
                streamWriter.WriteLine($"{LineNumber()}G00 X0.000 Y{ToNCString(AppSettings.PolygonMaxY + laserOffset)} Z{ToNCString(AppSettings.SafeZ)}\n"); // возвращение в начало
            }
            else // если не было переворота
            {
                streamWriter.WriteLine($"{LineNumber()}G00 X0.000 Y{ToNCString(laserOffset)} Z{ToNCString(AppSettings.SafeZ)}\n"); // возвращение в начало
            }

            streamWriter.WriteLine($"{LineNumber()}M30\n");
            streamWriter.WriteLine($"{LineNumber()}'-= Created by <{AppSettings.ProgName} {AppSettings.Version}> =-\n");
            streamWriter.Close();
            AppSettings.MillZStart = Convert.ToSingle(TrimFormat09(StartZTextBox.Text));
            AppSettings.MillZ1 = Convert.ToSingle(TrimFormat09(PassTextBox.Text));
            AppSettings.MillZ = Convert.ToSingle(TrimFormat09(ResultZTextBox.Text));
            DebugTextBox.Text += $"Закрыт: {AppSettings.SaveDirectory}\\{AppSettings.FileName}.{iRand}.g \n";
            DebugTextBox.Text += $"Закрыт: {AppSettings.OpenDirectory}\\{AppSettings.FileName}{AppSettings.FileDim}";
            TextBox18.Text = "";
            AppSettings.IsFileLoaded = false;
            App.Current.MainWindow.Title = $"{AppSettings.ProgName} {AppSettings.Version}";
            FileSaveAndCloseButton.IsEnabled = false;
            RotateButton.IsEnabled = false;
            AppSettings.FoundP0 = false;
            AppSettings.FoundP1 = false;
            AppSettings.PolyLen = 0; // Длина массива объектов
        }

        private void ReverseLines() // Меняем у всех открытых Объектов направление движения на противоположное
        {
            var polyXTmp = new float[AppSettings.PolyLen + 1]; // Координата X - временный массив
            var polyYTmp = new float[AppSettings.PolyLen + 1]; // Координата Y - временный массив
            for (var objCur = 1; objCur <= AppSettings.ObjLen; objCur++) // Перебираем все объекты
            {
                if (AppSettings.ObjectType[objCur] == 'L') // Если это закрытые объекты
                {
                    for (AppSettings.PolyCur = AppSettings.ObjFirst[objCur]; AppSettings.PolyCur <= AppSettings.ObjLast[objCur]; AppSettings.PolyCur++) // Проходим по всем значениям массива
                    {
                        polyXTmp[AppSettings.PolyCur] = AppSettings.PolygonX[AppSettings.ObjFirst[objCur] + AppSettings.ObjLast[objCur] - AppSettings.PolyCur]; // Заносим во временное значение, данные с противоположного конца массива
                        polyYTmp[AppSettings.PolyCur] = AppSettings.PolygonY[AppSettings.ObjFirst[objCur] + AppSettings.ObjLast[objCur] - AppSettings.PolyCur]; // Заносим во временное значение, данные с противоположного конца массива
                    }

                    for (AppSettings.PolyCur = AppSettings.ObjFirst[objCur]; AppSettings.PolyCur <= AppSettings.ObjLast[objCur]; AppSettings.PolyCur++) // Проходим по всем значениям массива
                    {
                        AppSettings.PolygonX[AppSettings.PolyCur] = polyXTmp[AppSettings.PolyCur]; // Заносим в текущее значение, данные с временного массива
                        AppSettings.PolygonY[AppSettings.PolyCur] = polyYTmp[AppSettings.PolyCur]; // Заносим в текущее значение, данные с временного массива
                    }
                }
            }
        }

        private void ReversePolygons() // Меняем у всех закрытых полигонов направление движения на противоположное
        {
            var polyXTmp = new float[AppSettings.PolyLen + 1]; // Координата X - временный массив
            var polyYTmp = new float[AppSettings.PolyLen + 1]; // Координата Y - временный массив
            for (var objCur = 1; objCur <= AppSettings.ObjLen; objCur++) // Перебираем все объекты
            {
                if (AppSettings.ObjectType[objCur] == 'P') // Если это закрытые объекты
                {
                    for (AppSettings.PolyCur = AppSettings.ObjFirst[objCur]; AppSettings.PolyCur <= AppSettings.ObjLast[objCur]; AppSettings.PolyCur++) // Проходим по всем значениям массива
                    {
                        polyXTmp[AppSettings.PolyCur] = AppSettings.PolygonX[AppSettings.ObjFirst[objCur] + AppSettings.ObjLast[objCur] - AppSettings.PolyCur]; // Заносим во временное значение, данные с противоположного конца массива
                        polyYTmp[AppSettings.PolyCur] = AppSettings.PolygonY[AppSettings.ObjFirst[objCur] + AppSettings.ObjLast[objCur] - AppSettings.PolyCur]; // Заносим во временное значение, данные с противоположного конца массива
                    }

                    for (AppSettings.PolyCur = AppSettings.ObjFirst[objCur]; AppSettings.PolyCur <= AppSettings.ObjLast[objCur]; AppSettings.PolyCur++) // Проходим по всем значениям массива
                    {
                        AppSettings.PolygonX[AppSettings.PolyCur] = polyXTmp[AppSettings.PolyCur]; // Заносим в текущее значение, данные с временного массива
                        AppSettings.PolygonY[AppSettings.PolyCur] = polyYTmp[AppSettings.PolyCur]; // Заносим в текущее значение, данные с временного массива
                    }
                }
            }
        }

        private void ReverseObjects() // Меняем у всех действительных  объектов направление движения на противоположное
        {
            var polyXTmp = new float[AppSettings.PolyLen + 1]; // Координата X - временный массив
            var polyYTmp = new float[AppSettings.PolyLen + 1]; // Координата Y - временный массив
            for (var objCur = 1; objCur <= AppSettings.ObjLen; objCur++) // Перебираем все объекты
            {
                if (AppSettings.ObjectType[objCur] == 'L' || AppSettings.ObjectType[objCur] == 'P') // Если это действительные валидные объекты
                {
                    for (AppSettings.PolyCur = AppSettings.ObjFirst[objCur]; AppSettings.PolyCur <= AppSettings.ObjLast[objCur]; AppSettings.PolyCur++) // Проходим по всем значениям массива
                    {
                        polyXTmp[AppSettings.PolyCur] = AppSettings.PolygonX[AppSettings.ObjFirst[objCur] + AppSettings.ObjLast[objCur] - AppSettings.PolyCur]; // Заносим во временное значение, данные с противоположного конца массива
                        polyYTmp[AppSettings.PolyCur] = AppSettings.PolygonY[AppSettings.ObjFirst[objCur] + AppSettings.ObjLast[objCur] - AppSettings.PolyCur]; // Заносим во временное значение, данные с противоположного конца массива
                    }

                    for (AppSettings.PolyCur = AppSettings.ObjFirst[objCur]; AppSettings.PolyCur <= AppSettings.ObjLast[objCur]; AppSettings.PolyCur++) // Проходим по всем значениям массива
                    {
                        AppSettings.PolygonX[AppSettings.PolyCur] = polyXTmp[AppSettings.PolyCur]; // Заносим в текущее значение, данные с временного массива
                        AppSettings.PolygonY[AppSettings.PolyCur] = polyYTmp[AppSettings.PolyCur]; // Заносим в текущее значение, данные с временного массива
                    }
                }
            }
        }

        private string LineNumber()
        {
            var result = "";
            if (AppSettings.GLineNumber == 0)
            {
                return result;
            }

            result = AppSettings.CurrentLineNumber.ToString();
            if (result.Length < 6)
            {
                for (var i = 1; i <= 6 - result.Length; i++)
                {
                    result = "0" + result;
                }
            }

            AppSettings.CurrentLineNumber++;
            return $"N{result}   ";
        }

        private string ToNCString(float inSingle)
        {
            var result = "";
            inSingle = Convert.ToSingle(Math.Round(inSingle, 3));
            string inString = inSingle.ToString();
            inString = inString.Replace(',', '.');
            if (inString.IndexOf('.') == -1)
            {
                return inString + ".000";
            }

            result = result.Substring(0, result.IndexOf("."));
            int lastLen = inString.Length - inString.IndexOf(".");
            if (lastLen > 3)
            {
                lastLen = 3;
            }

            for (var i = inString.IndexOf(".") + 1; i <= inString.IndexOf(".") + lastLen; i++)
            {
                result += inString.Substring(i, 1);
            }

            if (lastLen < 3)
            {
                result += "0";
            }

            if (lastLen < 2)
            {
                result += "0";
            }

            return result;
        }

        private float PointsDistance(float aX, float aY, float bX, float bY)
        {
            return Convert.ToSingle(Math.Sqrt((bY - aY) * (bY - aY) + (bX - aX) * (bX - aX)));
        }

        private float ShortestDistanceToObject(float aX, float aY, int objectNumber)
        {
            float result = PointsDistance(aX, aY, AppSettings.PolygonX[AppSettings.ObjFirst[objectNumber]], AppSettings.PolygonY[AppSettings.ObjFirst[objectNumber]]);
            if (AppSettings.ObjectType[objectNumber] == 'P')
            {
                for (var polyCurLine = AppSettings.ObjFirst[objectNumber] + 1; polyCurLine <= AppSettings.ObjLast[objectNumber]; polyCurLine++)
                {
                    if (PointsDistance(aX, aY, AppSettings.PolygonX[polyCurLine], AppSettings.PolygonY[polyCurLine]) < result)
                    {
                        result = PointsDistance(aX, aY, AppSettings.PolygonX[polyCurLine], AppSettings.PolygonY[polyCurLine]);
                    }
                }
            }

            return result;
        }

        private void RotateBeginOfObjectToPoint(float aX, float aY, int objectNumber)
        {
            // Переворачиваем все строки с координатами объекта таким образом, чтобы ближайшая точка была первой
            if (AppSettings.ObjectType[objectNumber] != 'P') // Если объект не закрытый, то сразу выходим
            {
                return;
            }

            float shortestDistanceToObject = PointsDistance(aX, aY, AppSettings.PolygonX[AppSettings.ObjFirst[objectNumber]], AppSettings.PolygonY[AppSettings.ObjFirst[objectNumber]]);
            int pointToBeFirst = AppSettings.ObjFirst[objectNumber]; // номер строки с самым близким значением
            for (var polyCurLine = AppSettings.ObjFirst[objectNumber]; polyCurLine <= AppSettings.ObjLast[objectNumber]; polyCurLine++)
            {
                if (PointsDistance(aX, aY, AppSettings.PolygonX[polyCurLine], AppSettings.PolygonY[polyCurLine]) < shortestDistanceToObject)
                {
                    shortestDistanceToObject = PointsDistance(aX, aY, AppSettings.PolygonX[polyCurLine], AppSettings.PolygonY[polyCurLine]);
                    pointToBeFirst = polyCurLine;
                }
            }

            if (pointToBeFirst != AppSettings.ObjFirst[objectNumber]) // Если ближайшая точка не является и так уже первой
            {
                var polyXTemp = new float[AppSettings.ObjLast[objectNumber] - AppSettings.ObjFirst[objectNumber] + 2]; // временный массив
                var polyYTemp = new float[AppSettings.ObjLast[objectNumber] - AppSettings.ObjFirst[objectNumber] + 2]; // временный массив
                var polyCurNewLine = 1; // Счетчик строк в новом массиве
                for (var polyCurLine = pointToBeFirst; polyCurLine <= AppSettings.ObjLast[objectNumber]; polyCurLine++) // Переписываем часть массива, начиная с ближайшей точки
                {
                    polyXTemp[polyCurNewLine] = AppSettings.PolygonX[polyCurLine];
                    polyYTemp[polyCurNewLine] = AppSettings.PolygonY[polyCurLine];
                    polyCurNewLine++; // Увеличиваем счетчик нового архива
                }

                for (var polyCurLine = AppSettings.ObjFirst[objectNumber]; polyCurLine <= pointToBeFirst - 1; polyCurLine++) // Переписываем часть массива от первой точки и до ближайшей точки - 1
                {
                    polyXTemp[polyCurNewLine] = AppSettings.PolygonX[polyCurLine];
                    polyYTemp[polyCurNewLine] = AppSettings.PolygonY[polyCurLine];
                    polyCurNewLine++; // Увеличиваем счетчик нового архива
                }

                polyCurNewLine = 1; // Обнуляем Счетчик строк в новом массиве
                for (var polyCurLine = AppSettings.ObjFirst[objectNumber]; polyCurLine <= AppSettings.ObjLast[objectNumber]; polyCurLine++) // Переписываем новый массив назад в старый
                {
                    polyXTemp[polyCurLine] = AppSettings.PolygonX[polyCurNewLine];
                    polyYTemp[polyCurLine] = AppSettings.PolygonY[polyCurNewLine];
                    polyCurNewLine++; // Увеличиваем счетчик нового архива
                }
            }
        }

        private int GetNumberOfObjectsByRoot(int rootNumber)
        {
            var result = 0;
            for (var objCur = 1; objCur <= AppSettings.ObjLen; objCur++)
            {
                if (AppSettings.ObjRoot[objCur] == rootNumber && (AppSettings.ObjectType[objCur] == 'P' || AppSettings.ObjectType[objCur] == 'L'))
                {
                    result++;
                }
            }

            return result;
        }

        private string ToNCLight(float inSingle)
        {
            string result = inSingle.ToString();
            result = result.Replace(',', '.');
            if (result.IndexOf('.') != -1)
            {
                result = result.Substring(0, result.IndexOf('.')) + result.Substring(result.IndexOf(".") + 1, 3);
            }

            return result;
        }


        private void OnLoad(object sender, RoutedEventArgs e)
        {
            App.Current.MainWindow.Title = $"{AppSettings.ProgName} {AppSettings.Version}";
            // Проверяем наличие файла со всеми установками программы и если его нет то создаем дефолтные установки
            if (!File.Exists(AppSettings.ProgDir + "\\settings.ini"))
            {
                using (var streamWriter = new StreamWriter(AppSettings.ProgDir + "\\settings.ini"))
                {
                    streamWriter.Write($"Version: {AppSettings.ProgName} {AppSettings.Version}"); // Создаем файл настроек
                }
            }

            AppSettings.TempString = FindIniString("Version");
            if (AppSettings.TempString != $"{AppSettings.ProgName} {AppSettings.Version}")
            {
                SaveIniString("Version", $"{AppSettings.ProgName} {AppSettings.Version}");
            }

            AppSettings.OpenDirectory = FindIniString("OpenDir");
            if (AppSettings.OpenDirectory == "N")
            {
                AppSettings.OpenDirectory = AppSettings.ProgDir;
                SaveIniString("OpenDir", AppSettings.OpenDirectory);
            }

            AppSettings.OpenDir1 = FindIniString("OpenDir1");
            if (AppSettings.OpenDir1 == "N")
            {
                AppSettings.OpenDir1 = AppSettings.ProgDir;
                SaveIniString("OpenDir1", AppSettings.OpenDir1);
            }

            AppSettings.OpenDir2 = FindIniString("OpenDir2");
            if (AppSettings.OpenDir2 == "N")
            {
                AppSettings.OpenDir2 = AppSettings.ProgDir;
                SaveIniString("OpenDir2", AppSettings.OpenDir2);
            }

            AppSettings.SaveDirectory = FindIniString("SaveDir");
            if (AppSettings.SaveDirectory == "N")
            {
                AppSettings.SaveDirectory = AppSettings.ProgDir;
                SaveIniString("SaveDir", AppSettings.SaveDirectory);
            }

            AppSettings.FileMask = FindIniString("FileMask");
            if (AppSettings.FileMask == "N")
            {
                AppSettings.FileMask = "HPGL CorelDraw файлы|*.plt";
                SaveIniString("FileMask", AppSettings.FileMask);
            }

            AppSettings.FileExtension = FindIniString("FileExt");
            if (AppSettings.FileExtension == "N")
            {
                AppSettings.FileExtension = "plt";
                SaveIniString("FileExt", AppSettings.FileExtension);
            }

            AppSettings.OpenDialogTitle = FindIniString("OpenDialogTitle");
            if (AppSettings.OpenDialogTitle == "N")
            {
                AppSettings.OpenDialogTitle = "Выберите HPGL CorelDraw файл для обработки";
                SaveIniString("OpenDialogTitle", AppSettings.OpenDialogTitle);
            }

            AppSettings.OpenDir1Title = FindIniString("OpenDir1Title");
            if (AppSettings.OpenDir1Title == "N")
            {
                AppSettings.OpenDir1Title = "Выберите директорию №1 для поиска HPGL файлов";
                SaveIniString("OpenDir1Title", AppSettings.OpenDir1Title);
            }

            AppSettings.OpenDir2Title = FindIniString("OpenDir2Title");
            if (AppSettings.OpenDir2Title == "N")
            {
                AppSettings.OpenDir2Title = "Выберите директорию №2 для поиска HPGL файлов";
                SaveIniString("OpenDir2Title", AppSettings.OpenDir2Title);
            }

            AppSettings.SaveDialogTitle = FindIniString("SaveDialogTitle");
            if (AppSettings.SaveDialogTitle == "N")
            {
                AppSettings.SaveDialogTitle = "Выберите директорию для сохранения NC файлов";
                SaveIniString("SaveDialogTitle", AppSettings.SaveDialogTitle);
            }

            AppSettings.TempString = FindIniString("VectorsClose1Case0");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "Попутно";
                SaveIniString("VectorsClose1Case0", AppSettings.TempString);
            }

            CloseVectorProcessingCombobox1.Items.Add(AppSettings.TempString);
            AppSettings.TempString = FindIniString("VectorsClose1Case1");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "Встречно";
                SaveIniString("VectorsClose1Case1", AppSettings.TempString);
            }

            CloseVectorProcessingCombobox1.Items.Add(AppSettings.TempString);
            AppSettings.TempString = FindIniString("VectorsOpenCase0");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "Начало => Конец";
                SaveIniString("VectorsOpenCase0", AppSettings.TempString);
            }

            OpenVectorsCombobox.Items.Add(AppSettings.TempString);
            AppSettings.TempString = FindIniString("VectorsOpenCase1");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "Конец => Начало";
                SaveIniString("VectorsOpenCase1", AppSettings.TempString);
            }

            OpenVectorsCombobox.Items.Add(AppSettings.TempString);
            AppSettings.TempString = FindIniString("VectorsClose2Case0");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "Вдоль";
                SaveIniString("VectorsClose2Case0", AppSettings.TempString);
            }

            CloseVectorProcessingCombobox2.Items.Add(AppSettings.TempString);
            AppSettings.TempString = FindIniString("VectorsClose2Case1");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "Снаружи";
                SaveIniString("VectorsClose2Case1", AppSettings.TempString);
            }

            CloseVectorProcessingCombobox2.Items.Add(AppSettings.TempString);
            AppSettings.TempString = FindIniString("VectorsClose2Case2");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "Внутри";
                SaveIniString("VectorsClose2Case2", AppSettings.TempString);
            }

            CloseVectorProcessingCombobox2.Items.Add(AppSettings.TempString);
            AppSettings.TempString = FindIniString("Point0Case0");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "Фреза нижний левый";
                SaveIniString("Point0Case0", AppSettings.TempString);
            }

            PointsBindingCombobox.Items.Add(AppSettings.TempString);
            AppSettings.TempString = FindIniString("Point0Case1");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "Фреза только =0=";
                SaveIniString("Point0Case1", AppSettings.TempString);
            }

            PointsBindingCombobox.Items.Add(AppSettings.TempString);
            AppSettings.TempString = FindIniString("Point0Case2");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "Фреза =0= и -1-";
                SaveIniString("Point0Case2", AppSettings.TempString);
            }

            PointsBindingCombobox.Items.Add(AppSettings.TempString);
            AppSettings.TempString = FindIniString("Point0Case3");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "Лазер нижний левый";
                SaveIniString("Point0Case3", AppSettings.TempString);
            }

            PointsBindingCombobox.Items.Add(AppSettings.TempString);
            AppSettings.TempString = FindIniString("Point0Case4");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "Лазер только =0=";
                SaveIniString("Point0Case4", AppSettings.TempString);
            }

            PointsBindingCombobox.Items.Add(AppSettings.TempString);
            AppSettings.TempString = FindIniString("Point0Case5");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "Лазер =0= и -1-";
                SaveIniString("Point0Case5", AppSettings.TempString);
            }

            PointsBindingCombobox.Items.Add(AppSettings.TempString);
            AppSettings.TempString = FindIniString("Point0Case6");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "Лазер =0= и -1- вниз лицом";
                SaveIniString("Point0Case6", AppSettings.TempString);
            }

            PointsBindingCombobox.Items.Add(AppSettings.TempString);
            AppSettings.TempString = FindIniString("Point0Case7");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "Лазер =0= -1- -2-";
                SaveIniString("Point0Case7", AppSettings.TempString);
            }

            PointsBindingCombobox.Items.Add(AppSettings.TempString);
            AppSettings.TempString = FindIniString("PluInch");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "1016";
                SaveIniString("PluInch", AppSettings.TempString);
            }

            AppSettings.PluInch = Convert.ToSingle(TrimFormat09(AppSettings.TempString));
            AppSettings.TempString = FindIniString("PluPercent");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "100";
                SaveIniString("PluPercent", AppSettings.TempString);
            }

            AppSettings.PluPercent = Convert.ToSingle(TrimFormat09(AppSettings.TempString));
            ScaleTextBox.Text = AppSettings.PluPercent + "%";
            AppSettings.PluPercentAuto = AppSettings.PluPercent;
            AppSettings.TempString = FindIniString("MaxTableSizeX");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "1370";
                SaveIniString("MaxTableSizeX", AppSettings.TempString);
            }

            AppSettings.MaxTableSizeX = Convert.ToSingle(TrimFormat09(AppSettings.TempString));
            AppSettings.TempString = FindIniString("MillZ");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "0";
                SaveIniString("MillZ", AppSettings.TempString);
            }

            AppSettings.MillZ = Convert.ToSingle(TrimFormat09(AppSettings.TempString));
            ResultZTextBox.Text = AppSettings.MillZ.ToString();
            AppSettings.TempString = FindIniString("MillZ1");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "0";
                SaveIniString("MillZ1", AppSettings.TempString);
            }

            AppSettings.MillZ1 = Convert.ToSingle(TrimFormat09(AppSettings.TempString));
            PassTextBox.Text = AppSettings.MillZ1.ToString();
            AppSettings.TempString = FindIniString("MillZStart");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "0";
                SaveIniString("MillZStart", AppSettings.TempString);
            }

            AppSettings.MillZStart = Convert.ToSingle(TrimFormat09(AppSettings.TempString));
            StartZTextBox.Text = AppSettings.MillZStart.ToString();
            AppSettings.TempString = FindIniString("SafeZ");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "25";
                SaveIniString("SafeZ", AppSettings.TempString);
            }

            AppSettings.SafeZ = Convert.ToSingle(TrimFormat09(AppSettings.TempString));
            SafeZTextBox.Text = AppSettings.SafeZ.ToString();
            AppSettings.TempString = FindIniString("MillD");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "0";
                SaveIniString("MillD", AppSettings.TempString);
            }

            AppSettings.MillD = Convert.ToSingle(TrimFormat09(AppSettings.TempString));
            MillDTextBox.Text = AppSettings.MillD.ToString();
            AppSettings.TempString = FindIniString("HideSmall");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "4";
                SaveIniString("HideSmall", AppSettings.TempString);
            }

            AppSettings.HideSmall = Convert.ToSingle(TrimFormat09(AppSettings.TempString));
            HideTextBox.Text = AppSettings.HideSmall.ToString();
            AppSettings.TempString = FindIniString("MillS");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "24000";
                SaveIniString("MillS", AppSettings.TempString);
            }

            AppSettings.MillS = Convert.ToInt32(TrimFormat09(AppSettings.TempString));
            MillTextBox.Text = AppSettings.MillS.ToString();
            AppSettings.TempString = FindIniString("MillP");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "18000";
                SaveIniString("MillP", AppSettings.TempString);
            }

            AppSettings.MillP = Convert.ToInt32(TrimFormat09(AppSettings.TempString));
            AppSettings.TempString = FindIniString("MillPs");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "5000";
                SaveIniString("MillPs", AppSettings.TempString);
            }

            AppSettings.MillPs = Convert.ToInt32(TrimFormat09(AppSettings.TempString));
            AppSettings.TempString = FindIniString("MillF");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "4000";
                SaveIniString("MillF", AppSettings.TempString);
            }

            AppSettings.MillF = Convert.ToInt32(TrimFormat09(AppSettings.TempString));
            G01TextBox.Text = AppSettings.MillF.ToString();
            AppSettings.TempString = FindIniString("MillFZ");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "1500";
                SaveIniString("MillFZ", AppSettings.TempString);
            }

            AppSettings.MillFZ = Convert.ToInt32(TrimFormat09(AppSettings.TempString));
            MinZTextBox.Text = AppSettings.MillFZ.ToString();
            AppSettings.TempString = FindIniString("GLineNumber");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "0";
                SaveIniString("GLineNumber", AppSettings.TempString);
            }

            AppSettings.GLineNumber = Convert.ToByte(TrimFormat09(AppSettings.TempString));
            AppSettings.TempString = FindIniString("LaserY");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "65";
                SaveIniString("LaserY", AppSettings.TempString);
            }

            AppSettings.LaserY = Convert.ToInt32(TrimFormat09(AppSettings.TempString));
            LaserYTextBox.Text = AppSettings.LaserY.ToString();
            AppSettings.TempString = FindIniString("MillVectClose1");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "0";
                SaveIniString("MillVectClose1", AppSettings.TempString);
            }

            AppSettings.MillVectClose1 = Convert.ToInt16(TrimFormat09(AppSettings.TempString));
            CloseVectorProcessingCombobox1.SelectedIndex = AppSettings.MillVectClose1;
            AppSettings.TempString = FindIniString("MillVectClose2");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "0";
                SaveIniString("MillVectClose2", AppSettings.TempString);
            }

            AppSettings.MillVectClose2 = Convert.ToInt16(TrimFormat09(AppSettings.TempString));
            CloseVectorProcessingCombobox2.SelectedIndex = AppSettings.MillVectClose2;
            AppSettings.TempString = FindIniString("MillVectOpen");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "0";
                SaveIniString("MillVectOpen", AppSettings.TempString);
            }

            AppSettings.MillVectOpen = Convert.ToInt16(TrimFormat09(AppSettings.TempString));
            OpenVectorsCombobox.SelectedIndex = AppSettings.MillVectOpen;
            AppSettings.TempString = FindIniString("MillXY0");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "7";
                SaveIniString("MillXY0", AppSettings.TempString);
            }

            AppSettings.MillXY0 = Convert.ToInt16(TrimFormat09(AppSettings.TempString));
            PointsBindingCombobox.SelectedIndex = AppSettings.MillXY0;
            AppSettings.TempString = FindIniString("AutoSizePercent");
            if (AppSettings.TempString == "N")
            {
                AppSettings.TempString = "1";
                SaveIniString("AutoSizePercent", AppSettings.TempString);
            }

            AppSettings.AutoSizePercent = Convert.ToByte(TrimFormat09(AppSettings.TempString));
            ScaleAutoCheckBox.IsChecked = Convert.ToBoolean(AppSettings.AutoSizePercent);
            if (AppSettings.AutoSizePercent == 1)
            {
                AppSettings.PluPercent = 100;
                SaveIniString("PluPercent", AppSettings.PluPercent.ToString());
                ScaleTextBox.IsEnabled = false; // Отключим редактирование процентов
            }
            else
            {
                ScaleTextBox.IsEnabled = true; // Включим редактирование процентов
            }

            AppSettings.MillX1 = 0;
            AppSettings.MillY1 = 0;
            AppSettings.IsFileLoaded = false;
            FileSaveAndCloseButton.IsEnabled = false;
            RotateButton.IsEnabled = false;
            // Если диаметр фрезы равен нулю тогда Обработку закрытых векторов сделаем по вектору и неактивной
            if (AppSettings.MillD == 0)
            {
                CloseVectorProcessingCombobox2.SelectedIndex = 0;
                CloseVectorProcessingCombobox2.IsEnabled = false;
                MillDTextBox.IsEnabled = false; // Временно отключим редактирование диаметра фрезы
            }
            else
            {
                CloseVectorProcessingCombobox2.IsEnabled = true; // и соответсвенно включим, если указан диаметр фрезы
            }

            DebugTextBox.Text += $"{AppSettings.ProgName} {AppSettings.Version}";
        }

        private string FindIniString(string iniString) // Поиск значения в ини-файле
        {
            var result = "N";
            using (var streamReader = new StreamReader(AppSettings.ProgDir + "\\settings.ini"))
            {
                while (!streamReader.EndOfStream)
                {
                    string iniLineCurrent = streamReader.ReadLine().Trim();
                    if (iniLineCurrent.Substring(0, iniLineCurrent.IndexOf(':')) == iniString)
                    {
                        result = iniLineCurrent.Substring(iniLineCurrent.IndexOf(':') + 1).Trim();
                    }
                }
            }

            return result;
        }

        private void AnyChanges() // Полное сохранение переформатирование и сохранение всех полей ввода одним махом
        {
            // Начальная глубина обработки
            AppSettings.MillZStart = Convert.ToSingle(TrimFormat09(StartZTextBox.Text));
            if (AppSettings.MillZ > AppSettings.MillZStart)
            {
                AppSettings.MillZStart = AppSettings.MillZ;
            }

            StartZTextBox.Text = AppSettings.MillZStart.ToString();
            SaveIniString("MillZStart", AppSettings.MillZStart.ToString());
            // Глубина обработки за 1 проход
            AppSettings.MillZ1 = Convert.ToSingle(TrimFormat09(PassTextBox.Text));
            if (AppSettings.MillZ1 < 0)
            {
                AppSettings.MillZ1 *= -1;
            }

            PassTextBox.Text = AppSettings.MillZ1.ToString();
            SaveIniString("MillZ1", AppSettings.MillZ1.ToString());
            // Глубина обработки - всегда отрицательное значение
            AppSettings.MillZ = Convert.ToSingle(TrimFormat09(ResultZTextBox.Text));
            if (AppSettings.MillZ > 0)
            {
                AppSettings.MillZ *= -1;
            }

            if (AppSettings.MillZ > AppSettings.MillZStart)
            {
                AppSettings.MillZStart = AppSettings.MillZ;
            }

            ResultZTextBox.Text = AppSettings.MillZ.ToString();
            SaveIniString("MillZ", AppSettings.MillZ.ToString());
            // координаты точки 1 по Х - несохраняемое значение - не выходит за края максимальных значений
            AppSettings.MillX1 = Convert.ToSingle(TrimFormat09(Point1XTextBox.Text));
            if (AppSettings.MillX1 > AppSettings.PolygonMaxX - AppSettings.Point0X)
            {
                AppSettings.MillX1 = AppSettings.PolygonMaxX - AppSettings.Point0X;
            }

            if (AppSettings.MillX1 < 0 - AppSettings.Point0X)
            {
                AppSettings.MillX1 = 0 - AppSettings.Point0X;
            }

            if (AppSettings.MillX1 != 0)
            {
                AppSettings.FoundP1 = true;
            }

            Point1YTextBox.Text = TrimFormat09(AppSettings.MillY1.ToString());
            // Если точка 1 в нуле - тогда поля делаем пустыми
            if ((AppSettings.MillX1 + AppSettings.Point0X) == AppSettings.Point0X && (AppSettings.MillY1 + AppSettings.Point0Y) == AppSettings.Point0Y)
            {
                AppSettings.FoundP0 = false;
                Point1XTextBox.Text = "";
                Point1YTextBox.Text = "";
                Point2XTextBox.Text = "";
                Point2YTextBox.Text = "";
            }

            // координаты точки 2 по Х - несохраняемое значение - не выходит за края максимальных значений
            AppSettings.MillX2 = Convert.ToSingle(TrimFormat09(Point2XTextBox.Text));
            if (AppSettings.MillX2 > (AppSettings.PolygonMaxX - AppSettings.Point0X))
            {
                AppSettings.MillX2 = AppSettings.PolygonMaxX - AppSettings.Point0X;
            }

            if (AppSettings.MillX2 < (0 - AppSettings.Point0X))
            {
                AppSettings.MillX2 = 0 - AppSettings.Point0X;
            }

            if (AppSettings.MillX2 != 0)
            {
                AppSettings.FoundP2 = true;
            }

            Point2XTextBox.Text = TrimFormat09(AppSettings.MillX2.ToString());
            // координаты точки 2 по Y - несохраняемое значение - не выходит за края максимальных значений
            AppSettings.MillY2 = Convert.ToSingle(TrimFormat09(Point2YTextBox.Text));
            if (AppSettings.MillY2 > (AppSettings.PolygonMaxX - AppSettings.Point0Y))
            {
                AppSettings.MillY2 = AppSettings.PolygonMaxY - AppSettings.Point0Y;
            }

            if (AppSettings.MillY2 < (0 - AppSettings.Point0Y))
            {
                AppSettings.MillY2 = 0 - AppSettings.Point0Y;
            }

            if (AppSettings.MillY2 != 0)
            {
                AppSettings.FoundP2 = true;
            }

            Point2YTextBox.Text = TrimFormat09(AppSettings.MillY2.ToString());
            // Если точка 1 в нуле - тогда поля делаем пустыми
            if ((AppSettings.MillX2 + AppSettings.Point0X) == AppSettings.Point0X && (AppSettings.MillY2 + AppSettings.Point0Y) == AppSettings.Point0Y)
            {
                AppSettings.FoundP2 = false;
                Point2XTextBox.Text = "";
                Point2YTextBox.Text = "";
            }

            // Смещение Лазера по Y - всегда положительное значение
            AppSettings.LaserY = Convert.ToSingle(TrimFormat09(LaserYTextBox.Text));
            if (AppSettings.LaserY < 0)
            {
                AppSettings.LaserY *= -1;
            }

            LaserYTextBox.Text = AppSettings.LaserY.ToString();
            SaveIniString("LaserY", AppSettings.LaserY.ToString());
            // Диаметр фрезы - всегда положительное значение
            AppSettings.MillD = Convert.ToSingle(TrimFormat09(MillDTextBox.Text));
            if (AppSettings.MillD < 0)
            {
                AppSettings.MillD *= -1;
            }

            MillDTextBox.Text = AppSettings.MillD.ToString();
            SaveIniString("MillD", AppSettings.MillD.ToString());
            // Если диаметр фрезу равен нулю тогда Обработку закрытых векторов сделаем по вектору и неактивной
            if (AppSettings.MillD == 0)
            {
                CloseVectorProcessingCombobox2.SelectedIndex = 0;
                CloseVectorProcessingCombobox2.IsEnabled = false;
            }
            else
            {
                CloseVectorProcessingCombobox2.IsEnabled = true; // и соответсвенно включим, если указан диаметр фрезы
            }

            // Высота безопасности - всегда положительное значение
            AppSettings.SafeZ = Convert.ToSingle(TrimFormat09(SafeZTextBox.Text));
            if (AppSettings.SafeZ < 0)
            {
                AppSettings.SafeZ *= -1;
            }

            if (AppSettings.SafeZ < AppSettings.MillZStart)
            {
                AppSettings.SafeZ = AppSettings.MillZStart + 5;
            }

            SafeZTextBox.Text = AppSettings.SafeZ.ToString();
            SaveIniString("SafeZ", AppSettings.SafeZ.ToString());
            // Если высота безопасности равена нулю тогда отключим кнопку сохранить
            if (AppSettings.SafeZ == 0)
            {
                FileSaveAndCloseButton.IsEnabled = false;
            }

            if (AppSettings.SafeZ > 0 && AppSettings.IsFileLoaded == true)
            {
                FileSaveAndCloseButton.IsEnabled = true; // и соответсвенно включим, если указана высота безопасности и файл загружен
            }

            // Подача G0 - всегда положительна
            AppSettings.MillF = Convert.ToInt32(TrimFormat09(G01TextBox.Text));
            if (AppSettings.MillF < 0)
            {
                AppSettings.MillF *= -1;
            }

            G01TextBox.Text = AppSettings.MillF.ToString();
            SaveIniString("MillF", AppSettings.MillF.ToString());
            // Подача по Z - всегда положительна
            AppSettings.MillFZ = Convert.ToInt32(TrimFormat09(MinZTextBox.Text));
            if (AppSettings.MillFZ < 0)
            {
                AppSettings.MillFZ *= -1;
            }

            MinZTextBox.Text = AppSettings.MillFZ.ToString();
            SaveIniString("MillFZ", AppSettings.MillFZ.ToString());
            // Скрыть мелкие объекты - всегда положительна
            AppSettings.HideSmall = Convert.ToSingle(TrimFormat09(HideTextBox.Text));
            if (AppSettings.HideSmall < 0)
            {
                AppSettings.HideSmall *= -1;
            }

            HideTextBox.Text = AppSettings.HideSmall.ToString();
            SaveIniString("HideSmall", AppSettings.HideSmall.ToString());
            // Скорость вращения шпинделя - всегда положительна
            AppSettings.MillS = Convert.ToInt32(TrimFormat09(MillTextBox.Text));
            if (AppSettings.MillS < 0)
            {
                AppSettings.MillS *= -1;
            }

            MillTextBox.Text = AppSettings.MillS.ToString();
            SaveIniString("MillS", AppSettings.MillS.ToString());
            // % - масштаб при экспорте - всегда положительна
            AppSettings.PluPercent = Convert.ToSingle(TrimFormat09(ScaleTextBox.Text));
            if (AppSettings.PluPercent < 0)
            {
                AppSettings.PluPercent *= -1;
            }

            ScaleTextBox.Text = AppSettings.PluPercent.ToString() + "%";
            SaveIniString("PluPercent", AppSettings.PluPercent.ToString());
        }
    }
}