using System.Windows.Controls;
using System.Windows.Media;

namespace NCMillComposer
{
    class Drawable
    {
        private Canvas _canvas;
        private SolidColorBrush _strokeColor;
        private int _strokeWidth;

        public SolidColorBrush StrokeColor
        {
            set { _strokeColor = value; }
        }

        public int StrokeWidth
        {
            set { _strokeWidth = value; }
        }

        public Drawable(Canvas canvas) 
        {
            _canvas = canvas;
        }

        public void DrawLine(float x1, float y1, float x2, float y2)
        {

        }
    }
}
