namespace Models.Base
{
    public record class Vector2D
    {
        private double _x;
        private double _y;
        private double _length;

        public double X { get => _x; set => _x = value; }
        public double Y { get => _y; set => _y = value; }
        public double Length { get => _length; set => _length = value; }


        public Vector2D()
        {
            _x = default;
            _y = default;
            _length = default;
        }

        public Vector2D(double x, double y)
        {
            _x = x;
            _y = y;
            _length = Math.Sqrt(_x * _x + _y * _y);
        }

        public Vector2D(Vertex2D first, Vertex2D second, Func<double, double, double> valueFunction) : this()
        {
            _x = valueFunction(first.X, second.X);
            _y = valueFunction(first.Y, second.Y);
            _length = Math.Sqrt(_x * _x + _y * _y);
        }


        public static implicit operator Vector2D(Vertex2D vertex) => new(vertex.X, vertex.Y);


        public static Vector2D operator -(Vector2D vector, Vector2D other) => new(vector, other, (f, s) => f - s);
        public static Vector2D operator +(Vector2D vector, Vector2D other) => new(vector, other, (f, s) => f + s);


        public void Normalize()
        {
            if (_length == default)
                return;

            _x /= _length;
            _y /= _length;
            _length = 1;
        }
    }
}
