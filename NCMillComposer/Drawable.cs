using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NCMillComposer
{
    class Drawable
    {
        private readonly Canvas _canvas;

        public Drawable(Canvas canvas) 
        {
            _canvas = canvas;
        }

        public void DrawLine(SolidColorBrush strokeColor, int strokeThickness, float x1, float y1, float x2, float y2)
        {
            var line = new Line
            {
                Stroke = strokeColor,
                StrokeThickness = strokeThickness,
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2
            };

            _canvas.Children.Add(line);
        }

        public void DrawEllipse(SolidColorBrush strokeColor, int strokeThickness, float x, float y, float width, float height)
        {
            var ellipse = new Ellipse
            {
                Stroke = strokeColor,
                StrokeThickness = strokeThickness,
                Width = width,
                Height = height,
                Margin = new System.Windows.Thickness(x, y, 0, 0)
            };

            _canvas.Children.Add(ellipse);
        }

        public void DrawText(string text, int fontSize, string fontFamily, SolidColorBrush fontColor, float x, float y)
        {
            var textBlock = new TextBlock
            {
                Text = text,
                FontFamily = new FontFamily(fontFamily),
                FontSize = fontSize,
                Foreground = fontColor,
                Margin = new System.Windows.Thickness(x, y, 0, 0)
            };

            _canvas.Children.Add(textBlock);
        }
    }
}
