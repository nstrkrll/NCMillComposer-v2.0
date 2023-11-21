using System.Globalization;
using System;

namespace NCMillComposer
{
    class AppSettings
    {
        public static string Version = "2.0";
        public static char Separator = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0]; // Разделитель: точка или запятая
        // _programDirectory
        public static string ProgDir = Environment.CurrentDirectory; // Директория откуда запускалась программа
        // _programName
        public static string ProgName = "Nc Mill Composer"; // Название программы
        public static string FileMask; // Маска для открытия файла
        public static string FileExtension; // Расширение для поиска и открытия файла
        public static string FileName; // Имя открытого файла
        // _currentFileExtension
        public static string FileDim; // Расширение открытого файла
        public static string OpenDialogTitle; // Название окна выбора файла
        // _openFirstDirectoryTitle
        public static string OpenDir1Title; // Название окна выбора первой папки
        // _openSecondDirectoryTitle
        public static string OpenDir2Title; // Название окна выбора второй папки
        public static string SaveDialogTitle; // Название окна выбора директории для сохранения
        public static string OpenDirectory; // Директория для открытия (рабочая)
        public static string SaveDirectory; // Директория для сохранения
        // _firstSearchDirectory
        public static string OpenDir1; // Первая директория для поиска файлов
        // _secondSearchDirectory
        public static string OpenDir2; // Вторая директория для поиска файлов
        public static string TempString; // Временная символьная переменная
        public static float PluInch; // Сколько в миллиметре шагов плоттера указано в дюймах
        public static float PluPercent; // Масштаб при экспорте, сохраненный в настройках программы
        // _currentPluPercent
        public static float PluPercentAuto; // Текущий масштаб при экспорте
        public static float MaxTableSizeX; // Максимальный размер стола по Х (в миллиметрах)
        // _cuttingDepth
        public static float MillZ; // Глубина обработки
        // _cuttingDepthInOnePath
        public static float MillZ1; // Глубина обработки за один проход
        // _initCuttingHeight
        public static float MillZStart; // Начальная высота резки
        // _cutterDiameter
        public static float MillD; // Диаметр фрезы
        // _hiddenSmallObjectsSize
        public static float HideSmall; // Размер скрываемых мелких объектов
        // _safetyHeight
        public static float SafeZ; // Высота безопасности
        // _openVectorProcessingStrategy
        public static short MillVectOpen; // Стратегия обработки открытых векторов
        // _firstClosedVectorProcessingStrategy
        public static short MillVectClose1; // Первая стратегия обработки закрытых векторов
        // _secondClosedVectorProcessingStrategy
        public static short MillVectClose2; // Вторая стратегия обработки закрытых векторов
        // _firstXPoint
        public static float MillX1; // Координата Х первой точки - результирующая, которая пойдет в файл и которая будет отрисовываться на экране за вычетом нулевой точки
        // _firstYPoint
        public static float MillY1; // Координата Y первой точки - результирующая, которая пойдет в файл и которая будет отрисовываться на экране за вычетом нулевой точки
        // _secondXPoint
        public static float MillX2; // Координата Х второй точки - результирующая, которая пойдет в файл и которая будет отрисовываться на экране за вычетом нулевой точки
        // _secondYPoint
        public static float MillY2; // Координата Y второй точки - результирующая, которая пойдет в файл и которая будет отрисовываться на экране за вычетом нулевой точки
        // _zeroPointDeterminationStrategy
        public static short MillXY0; // Порядок определения нулевой точки
        // _laserOffset
        public static float LaserY; // Смещение лазера
        // _cutterSpeed
        public static int MillS; // Скорость шпинделя
        // _cutterAccelerationDelay
        public static int MillP; // Задержка на разгон шпинделя
        // _shortCutterAccelerationDelay
        public static int MillPs; // Задержка на разгон шпинделя (короткая)
        // _cuttingFeed
        public static int MillF; // Подача рабочая
        // _loweringFeed
        public static int MillFZ; // Подача опускания по Z
        // _isNecessaryToNumberLines
        public static byte GLineNumber; // Необходимо ли нумеровать строки в выходном файле
        // _isAutoScalingEnabled
        public static byte AutoSizePercent; // Включено ли автоматическое масштабирование
        public static int CurrentLineNumber; // Номер текущей строки

        public static bool IsFileLoaded = false; // Загружен ли файл
        // _isFirstZeroPointFounded
        public static bool FoundP0; // Обнаружена ли первая нулевая точка
        // _isSecondZeroPointFounded
        public static bool FoundP1; // Обнаружена ли вторая нулевая точка
        // _isThirdZeroPointFounded
        public static bool FoundP2; // Обнаружена ли третья нулевая точка
        // _foundedPointX
        public static float PointX; // Координата X обнаруженной точки
        // _foundedPointY
        public static float PointY; // Координата Y обнаруженной точки
        // _firstZeroPointX
        public static float Point0X; // Координата X первой нулевой точки
        // _firstZeroPointY
        public static float Point0Y; // Координата Y первой нулевой точки
        // _secondZeroPointX
        public static float Point1X; // Координата X второй нулевой точки
        // _secondZeroPointY
        public static float Point1Y; // Координата Y второй нулевой точки
        // _thirdZeroPointX
        public static float Point2X; // Координата X третьей нулевой точки
        // _thirdZeroPointY
        public static float Point2Y; // Координата Y третьей нулевой точки
        // _polygonArrayLength
        public static int PolyLen; // Длинна массива с векторами
        // _currentPolygonArrayRow
        public static int PolyCur; // Номер текущей строка из массива с векторами для обработок разных

        // _firstTempObjNumber
        public static int ObjTmpNum; // Номер первого считываемого из файла или массива объекта
        // _secondTempObjNumber
        public static int Obj2TmpNum; // Номер второго считываемого из файла или массива объекта

        // _currentPolygonLine
        public static string PolyTmpLine; // Текущая строка из массива с векторами для обработок разных
        // _currentPolygonObjColor
        public static short PolyTmpColor; // Текущий цвет объекта
        // _pointSearchIndex
        public static byte IdxLineXY; // Индекс для перебора символов при поиске X и Y в строке файла
        // _pointSearchTempLine
        public static string TmpLineXY; // Временная строка для сбора символов X и Y из строки файла
        // _pointSearchChar
        public static char TmpCharXY; // Временная символ для сбора символов X и Y из строки файла

        public static float[] PolygonX = new float[2]; // Координата X
        public static float[] PolygonY = new float[2]; // Координата Y
        // _firstPolygonXIntersect
        public static float PolyXIntersect1; // Координата X #1 - временная для исключения прохождения луча через узел
        // _secondPolygonXIntersect
        public static float PolyXIntersect2; // Координата X #2 - временная для исключения прохождения луча через узел

        public static char[] ObjectType = new char[2]; // Тип объекта N, L, P, +, X, l, p
        // _objectFirstLineNumber
        public static int[] ObjFirst = new int[2]; // Номер первой строки объекта
        // _objectLastLineNumber
        public static int[] ObjLast = new int[2]; // Номер последней строки объекта
        public static int[] ObjectColor = new int[2]; // Цвет объекта
        // _objectMiddlePointX
        public static float[] ObjXCenter = new float[2]; // Координаты средней точки Х объекта
        // _objectMiddlePointY
        public static float[] ObjYCenter = new float[2]; // Координаты средней точки Y объекта
        // _boxMiddlePointX
        public static float[] ObjXCenterBox = new float[2]; // Координаты средней точки Х прямоугольника в который вписан объект
        // _boxMiddlePointY
        public static float[] ObjYCenterBox = new float[2]; // Координаты средней точки Y прямоугольника в который вписан объект
        // _objectSizeByX
        public static float[] ObjXDimension = new float[2]; // Размер объекта по X
        // _objectSizeByY
        public static float[] ObjYDimension = new float[2]; // Размер объекта по Y
        // _objectTraversalDirection
        public static int[] ObjOrient = new int[2]; // Направление обхода объекта
        // _childObjectsNumbersList
        public static string[] ObjChilds = new string[2]; // Список номеров дочерних объектов
        // _nestingLevel
        public static int[] ObjRoot = new int[2]; // Уровень вложенности объекта, 0 - объект наружный
        // _parentObjectNumber
        public static int[] ObjParent = new int[2]; // Номер родительского объекта

        // _objectIndexesArrayLength
        public static int ObjLen; // Длина массива с индексом объектов
        // _maxNestingLevel
        public static int ObjRootMax; // Максимальный уровень вложенности среди объектов
        // _middleLineToZeroPointLength
        public static float ObjToZero; // Длина средней линии до нуля координат

        // _currentSearchObjectNumber
        public static int ObjNumCur; // Номер текущего обрабатываемого объекта при поиске объектов
        // _secondSearchObjectNumber
        public static int ObjNumCur2; // Номер второго обрабатываемого объекта при поиске объектов

        // _currentObjectX
        public static float PolyXFirst; // Первая координата Х текущего обрабатываемого объекта
        // _currentObjectY
        public static float PolyYFirst; // Первая координата Y текущего обрабатываемого объекта

        //_objectConnectionsCount
        public static int ConnectCount; // Счетчик количества стыковок объектов

        public static float PolygonMaxX; // Максимальное значение координаты по X в массиве
        public static float PolygonMaxY; // Максимальное значение координаты по Y в массиве

        // _sameNestingLevelObjectNumbers
        public static int[] ChildList; // Массив с номерами объектов одного уровня вложенности
        // _sameNestingLevelObjectNumbersArrayLength
        public static int ChildListLen; // Длина массива с номерами объектов одного уровня вложенности
    }
}
