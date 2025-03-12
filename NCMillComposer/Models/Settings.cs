using System;

namespace NCMillComposer.Models
{
    /// <summary>
    /// Класс настроек программы
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// Версия приложения
        /// </summary>
        public const string Version = "2.0";

        /// <summary>
        /// Название приложения
        /// </summary>
        public const string ProgramName = "NC Mill Composer";

        /// <summary>
        /// Папка для поиска файлов №1
        /// </summary>
        public static string FirstDirectoryForSearch = Environment.CurrentDirectory;

        /// <summary>
        /// Папка для поиска файлов №2
        /// </summary>
        public static string SecondDirectoryForSearch = Environment.CurrentDirectory;

        /// <summary>
        /// Кол-во шагов плоттера в 1 дюйме
        /// </summary>
        public const int PlotterUnitsPerInch = 1016;

        /// <summary>
        /// Масштаб (в процентах)
        /// </summary>
        public static float Scale = 100f;

        /// <summary>
        /// Автомасштаб
        /// </summary>
        public static bool IsAutoScaleEnabled = true;

        /// <summary>
        /// Ширина стола
        /// </summary>
        public static float TableWidth = 1370f;
    }
}