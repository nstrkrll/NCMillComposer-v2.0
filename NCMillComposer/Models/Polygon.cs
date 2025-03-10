using System.Windows.Media;

namespace NCMillComposer.Models
{
    public class Polygon
    {
        private PointCollection _points = [];
        private char _objectType = 'U'; // (U - Undefined) неопределенный тип объекта

        public PointCollection Points 
        { 
            get => _points;
            set
            {
                _points = new PointCollection(value);
            }
        }

        public char ObjectType
        {
            get => _objectType;
            set => _objectType = value;
        }
    }
}