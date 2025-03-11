using System.Linq;
using System.Windows.Media;

namespace NCMillComposer.Models
{
    public class Polygon
    {
        private PointCollection _points = [];
        private char _objectType = 'U'; // (U - Undefined) неопределенный тип объекта

        /// <summary>
        /// Коллекция точек объекта
        /// </summary>
        public PointCollection Points 
        { 
            get => _points;
            set
            {
                _points = new PointCollection(value);
            }
        }
        
        /// <summary>
        /// Тип объекта (открытый/закрытый)
        /// </summary>
        public char ObjectType
        {
            get => _objectType;
            set => _objectType = value;
        }

        /// <summary>
        /// Максимальная координата по X
        /// </summary>
        public double MaxX => _points.Max(x => x.X);

        /// <summary>
        /// Максимальная координата по Y
        /// </summary>
        public double MaxY => _points.Max(x => x.Y);

        /// <summary>
        /// Минимальная координата по X
        /// </summary>
        public double MinX => _points.Min(x => x.X);

        /// <summary>
        /// Минимальная координата по Y
        /// </summary>
        public double MinY => _points.Min(x => x.Y);
    }
}