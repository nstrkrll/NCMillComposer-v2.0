using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace NCMillComposer.Helpers
{
    public class ObjectTypeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is char objectType)
            {
                return objectType switch
                {
                    'X' => Brushes.Gray,
                    'L' => Brushes.Blue,
                    'C' => Brushes.Green,
                    _ => Brushes.Black,
                };
            }

            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}