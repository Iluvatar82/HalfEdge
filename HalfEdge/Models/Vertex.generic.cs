using Validation;

namespace HalfEdge.Models
{
    public record class Vertex<T> where T : struct
    {
        private T _x;
        private T _y;
        private T _z;
        private List<HalfEdge<T>> _halfEdges;


        public T X { get => _x; init => _x = value; }
        public T Y { get => _y; init => _y = value; }
        public T Z { get => _z; init => _z = value; }
        public List<HalfEdge<T>> HalfEdges { get => _halfEdges; init => _halfEdges = value; }
        public IEnumerable<Polygon<T>> Polygons
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

            _halfEdges = new List<HalfEdge<T>>();
            _halfEdges.NotNull();
        }

        public Vertex(T x, T y, T z) :this()
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public void Deconstruct(out T x, out T y, out T z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public void Deconstruct(out T[] positionData) => positionData = new[] { _x, _y, _z };


        public static implicit operator T[](Vertex<T> vertex) => vertex;
        public static implicit operator Vertex<T>(T[] vertexData)
        {
            vertexData.NotNullOrEmpty();
            vertexData.HasElementCount(3);

            return new(vertexData[0], vertexData[1], vertexData[2]);
        }


        public override string ToString() => $"X: {_x}, Y: {_y}, Z: {_z}";
    }
}
