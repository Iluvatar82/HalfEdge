using System.Diagnostics.CodeAnalysis;
using Validation;

namespace HalfEdge.Models
{
    public record class Polygon<T>
    {
        private List<HalfEdge<T>> _halfEdges;


        public IEnumerable<Vertex<T>> Vertices => _halfEdges.Select(h => h.Start);
        public List<HalfEdge<T>> HalfEdges
        {
            get => _halfEdges;
            init 
            {
                _halfEdges = value;
                for(var i = 0; i < _halfEdges.Count; i++)
                {
                    _halfEdges[i].Polygon = this;
                    var next = (i + 1) % _halfEdges.Count;
                    _halfEdges[i].Next = _halfEdges[next];
                }
            }
        }
        public IEnumerable<Polygon<T>> Neighbors
        {
            get
            {
                foreach (var halfEdge in _halfEdges)
                {
                    if (halfEdge.Opposite?.Polygon is not null)
                        yield return halfEdge.Opposite.Polygon;
                }
            }
        }
        public bool IsBorder => _halfEdges.Any(h => h.IsBorder);


        public Polygon([NotNull] IEnumerable<HalfEdge<T>> halfEdges)
        {
            halfEdges.NotNullOrEmpty();

            HalfEdges = new List<HalfEdge<T>>(halfEdges);
        }


        public override string ToString() => $"{_halfEdges.Count} Vertices, On Border: {_halfEdges.Any(h => h.Opposite is null)}";
    }
}
