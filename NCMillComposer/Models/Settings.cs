namespace NCMillComposer.Models
{
    /// <summary>
    /// Класс настроек программы
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// Кол-во шагов плоттера в 1 дюйме
        /// </summary>
        public static int PlotterUnitsPerInch = 1016;

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