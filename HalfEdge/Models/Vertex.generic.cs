using System.Diagnostics.CodeAnalysis;
using Validation;

namespace HalfEdge.Models
{
    public record class Vertex<T>
    {
        private T _x;
        private T _y;
        private T _z;
        private List<HalfEdge<T>> _halfEdges;


        public T X { get => _x; init => _x = value; }
        public T Y { get => _y; init => _y = value; }
        public T Z { get => _z; init => _z = value; }
        public List<HalfEdge<T>> OutHalfEdges { get => _halfEdges; set => _halfEdges = value; }


        public Vertex([NotNull] T x, [NotNull] T y, [NotNull] T z)
        {
            x.NotNull();
            y.NotNull();
            z.NotNull();

            _x = x;
            _y = y;
            _z = z;

            _halfEdges = new List<HalfEdge<T>>();
            _halfEdges.NotNull();
        }


        public override string ToString() => $"X: {_x}, Y: {_y}, Z: {_z}";
    }
}
