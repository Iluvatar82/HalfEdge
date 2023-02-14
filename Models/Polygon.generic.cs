using Models.Base;
using Validation;

namespace Models
{
    public record class Polygon<T> where T : struct
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
                foreach (var h in _halfEdges)
                {
                    if (h.Opposite?.Polygon is not null)
                        yield return h.Opposite.Polygon;
                }

                yield break;
            }
        }

        public bool IsBorder => !_halfEdges.Any() || _halfEdges.Any(h => h.IsBorder);


        public Polygon()
        {
            _halfEdges = new List<HalfEdge<T>>();
        }

        public Polygon(List<Vertex<T>> vertices) : this()
        {
            vertices.NotNullOrEmpty();
            vertices.HasElementCount(e => e > 2);

            var halfEdges = new List<HalfEdge<T>>();
            Vertex<T>? first = null;
            foreach(var vertex in vertices)
            {
                first ??= vertex;
                if (vertex != first)
                {
                    halfEdges.Add((first, vertex));
                    first = vertex;
                }
            }

            halfEdges.Add((halfEdges.Last().End, halfEdges.First().Start));
            HalfEdges = halfEdges;
        }

        public Polygon(IEnumerable<HalfEdge<T>> halfEdges) : this()
        {
            halfEdges.NotNullOrEmpty();
            halfEdges.Select(h => (h.Start, h.End)).FormsLoop();

            HalfEdges = new List<HalfEdge<T>>(halfEdges);
        }

        public void Deconstruct(out IEnumerable<Vertex<T>> vertices, out IEnumerable<HalfEdge<T>> halfEdges)
        { 
            vertices = Vertices; 
            halfEdges = HalfEdges;
        }


        public static implicit operator Vertex<T>[](Polygon<T> polygon) => polygon.Vertices.ToArray();
        public static implicit operator HalfEdge<T>[](Polygon<T> polygon) => polygon._halfEdges.ToArray();
        public static implicit operator Polygon<T>(Vertex<T>[] vertices)
        {
            vertices.NotNullOrEmpty();
            vertices.HasElementCount(e => e > 2);

            return new(vertices.ToList());
        }

        public override string ToString() => $"{_halfEdges.Count} Vertices, On Border: {_halfEdges.Any(h => h.Opposite is null)}";
    }
}
