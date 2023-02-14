using Validation;

namespace Models.Base
{
    public record class Vertex2D
    {
        private double _x;
        private double _y;


        public double X { get => _x; init => _x = value; }
        public double Y { get => _y; init => _y = value; }
        
        
        public Vertex2D()
        {
            _x = default;
            _y = default;
        }

        public Vertex2D(double x, double y)
        {
            _x = x;
            _y = y;
        }

        public Vertex2D(Vertex2D first, Vertex2D second, Func<double, double, double> valueFunction) : this()
        {
            _x = valueFunction(first.X, second.X);
            _y = valueFunction(first.Y, second.Y);
        }


        public static implicit operator Vertex2D(Vertex vertex) => new(vertex.X, vertex.Y);
        public static implicit operator Vertex2D(Vector2D vector) => new(vector.X, vector.Y);
        public static implicit operator double[](Vertex2D vertex) => new[] { vertex._x, vertex._y };
        public static implicit operator Vertex2D(double[] vertexData)
        {
            vertexData.NotNullOrEmpty();
            vertexData.HasElementCountEqualTo(2);

            return new(vertexData[0], vertexData[1]);
        }

        public static Vertex2D operator -(Vertex vertex, Vertex2D other) => new(vertex, other, (f, s) => f - s);
        public static Vertex2D operator +(Vertex vertex, Vertex2D other) => new(vertex, other, (f, s) => f + s);
    }
}
