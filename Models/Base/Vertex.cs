﻿using Validation;

namespace Models.Base
{
    public partial record class Vertex
    {
        private double _x;
        private double _y;
        private double _z;
        private List<HalfEdge> _halfEdges;


        public double X { get => _x; init => _x = value; }
        public double Y { get => _y; init => _y = value; }
        public double Z { get => _z; init => _z = value; }
        public List<HalfEdge> HalfEdges { get => _halfEdges; init => _halfEdges = value; }
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
        public bool IsBorder => !_halfEdges.Any() || _halfEdges.Any(h => h.IsBorder);


        public Vertex()
        {
            _x = default;
            _y = default;
            _z = default;

            _halfEdges = new List<HalfEdge>();
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

        public Vertex(Func<IEnumerable<double>, double> aggregateFunction, params Vertex[] vertices) : this()
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
        public static Vertex Average(params Vertex[] vertices) => new((v) => v.Sum() / v.Count(), vertices);


        public static implicit operator Vertex(Vertex2D vertex) => new(vertex.X, vertex.Y, 0);
        public static implicit operator double[](Vertex vertex) => new[] { vertex._x, vertex._y, vertex._z };
        public static implicit operator Vertex(double[] vertexData)
        {
            vertexData.NotNullOrEmpty();
            vertexData.HasElementCountEqualTo(3);

            return new(vertexData[0], vertexData[1], vertexData[2]);
        }


        public override string ToString() => $"X: {_x}, Y: {_y}, Z: {_z}";
    }
}
