using Validation;

namespace Models.Base
{
    public record class Vector
    {
        private double _x;
        private double _y;
        private double _z;
        private double _length;

        public double X { get => _x; set => _x = value; }
        public double Y { get => _y; set => _y = value; }
        public double Z { get => _z; set => _z = value; }
        public double Length { get => _length; set => _length = value; }


        public Vector()
        {
            _x = default;
            _y = default;
            _z = default;
            _length = default;
        }

        public Vector(double x, double y, double z)
        {
            _x = x;
            _y = y;
            _z = z;
            _length = Math.Sqrt(_x * _x + _y * _y + _z * _z);
        }

        public Vector(Vector existing, Func<double, double> valueFunction)
            : this()
        {
            _x = valueFunction(existing.X);
            _y = valueFunction(existing.Y);
            _z = valueFunction(existing.Z);
            _length = valueFunction(existing.Length);
        }

        public Vector(Vertex first, Vertex second, Func<double, double, double> valueFunction)
            : this()
        {
            _x = valueFunction(first.X, second.X);
            _y = valueFunction(first.Y, second.Y);
            _z = valueFunction(first.Z, second.Z);
            _length = Math.Sqrt(_x * _x + _y * _y + _z * _z);
        }


        public static implicit operator Vector(Vertex vertex) => new(vertex.X, vertex.Y, vertex.Z);
        public static Vector operator -(Vector vector, Vector other) => new(vector, other, (f, s) => f - s);
        public static Vector operator +(Vector vector, Vector other) => new(vector, other, (f, s) => f + s);
        public static Vector operator *(Vector vector, double factor) => new(vector, (v) => v * factor);
        public static Vector operator *(double factor, Vector vector) => vector * factor;
        public static Vector operator /(Vector vector, double divisor) { divisor.Satisfies(d => d != 0); return new (vector, (v) => v / divisor); }


        public void Normalize()
        {
            if (_length == default)
                return;

            _x /= _length;
            _y /= _length;
            _z /= _length;
            _length = 1;
        }
    }
}
