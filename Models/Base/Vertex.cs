using Validation;

namespace Models.Base
{
    public partial record class Vertex
    {
        private double _x;
        private double _y;
        private double _z;
        private List<HalfEdge> _halfEdges = new List<HalfEdge>();


        public double X { get => _x; set => _x = value; }
        public double Y { get => _y; set => _y = value; }
        public double Z { get => _z; set => _z = value; }
        public List<HalfEdge> HalfEdges { get => _halfEdges; init => _halfEdges = value; }
        public IEnumerable<HalfEdge> IncomingHalfEdges => VertexNeighbors.Where(v => v.HalfEdges.Any(h => h.End == this)).Select(v => v.HalfEdges.First(h => h.End == this));
        public IEnumerable<Vertex> BorderNeighbors => VertexNeighbors.Where(n => _halfEdges.Any(h => h.End == n && h.IsBorder) || n.HalfEdges.Any(h => h.End == this && h.IsBorder));
        public IEnumerable<Polygon> Polygons
        {
            get
            {
                foreach (var h in _halfEdges)
                {
                    if (h.Polygon is not null)
                        yield return h.Polygon;
                }

                yield break;
            }
        }
        public IEnumerable<Vertex> VertexNeighbors
        {
            get
            {
                foreach (var h in _halfEdges)
                {
                    yield return h.End;
                    if (h.Polygon is not null)
                    {
                        var pH = h.Polygon.HalfEdges.Single(ph => ph.End == this);
                        if (pH.Opposite is null)
                            yield return h.Polygon.HalfEdges.Single(ph => ph.End == this).Start;
                    }
                }

                yield break;
            }
        }
        public bool IsBorder => !_halfEdges.Any() || _halfEdges.Any(h => h.IsBorder);


        public Vertex()
        {
            _x = default;
            _y = default;
            _z = default;
        }

        public Vertex(double x, double y, double z) : this()
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public Vertex(Vertex first, Vertex second, Func<double, double, double> valueFunction) : this()
        {
            _x = valueFunction(first.X, second.X);
            _y = valueFunction(first.Y, second.Y);
            _z = valueFunction(first.Z, second.Z);
        }

        public Vertex(Vertex existing, Func<double, double> valueFunction)
            : this()
        {
            _x = valueFunction(existing.X);
            _y = valueFunction(existing.Y);
            _z = valueFunction(existing.Z);
        }


        public Vertex(Func<IEnumerable<double>, double> aggregateFunction, IEnumerable<Vertex> vertices) : this()
        {
            _x = aggregateFunction(vertices.Select(v => v.X));
            _y = aggregateFunction(vertices.Select(v => v.Y));
            _z = aggregateFunction(vertices.Select(v => v.Z));
        }

        public void Deconstruct(out double x, out double y, out double z)
        {
            x = _x;
            y = _y;
            z = _z;
        }


        public static Vertex Minimum(Vertex first, Vertex second) => new(first, second, Math.Min);
        public static Vertex Maximum(Vertex first, Vertex second) => new(first, second, Math.Max);
        public static Vertex Average(Vertex first, Vertex second) => new(first, second, (f, s) => (f + s) * .5);
        public static Vertex Average(IEnumerable<Vertex> vertices) => new((v) => v.Average(), vertices);
        public static Vertex Sum(IEnumerable<Vertex> vertices) => new((v) => v.Sum(), vertices);

        public static Vertex operator -(Vertex vertex, Vertex other) => new(vertex, other, (f, s) => f - s);
        public static Vertex operator +(Vertex vertex, Vertex other) => new(vertex, other, (f, s) => f + s);
        public static Vertex operator *(Vertex vertex, double factor) => new(vertex, (v) => v * factor);
        public static Vertex operator *(double factor, Vertex vertex) => vertex * factor;
        public static Vertex operator /(Vertex vertex, double divisor) { divisor.Satisfies(d => d != 0); return new(vertex, (v) => v / divisor); }

        public static implicit operator Vertex(Vector vector) => new(vector.X, vector.Y, vector.Z);
        public static implicit operator Vertex(Vertex2D vertex) => new(vertex.X, vertex.Y, 0);
        public static implicit operator double[](Vertex vertex) => new[] { vertex._x, vertex._y, vertex._z };
        public static implicit operator Vertex(double[] vertexData)
        {
            vertexData.NotNullOrEmpty();
            vertexData.HasElementCountEqualTo(3);

            return new(vertexData[0], vertexData[1], vertexData[2]);
        }


        public double SquaredDistanceTo(Vertex other) => (other.X - X) * (other.X - X) + (other.Y - Y) * (other.Y - Y) + (other.Z - Z) * (other.Z - Z);

        public double DistanceTo(Vertex other) => Math.Sqrt(SquaredDistanceTo(other));

        public override string ToString() => $"X: {_x:F2}, Y: {_y:F2}, Z: {_z:F2}";
    }
}
