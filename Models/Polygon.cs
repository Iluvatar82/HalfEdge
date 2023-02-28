using Models.Base;
using Validation;

namespace Models
{
    public record class Polygon
    {
        private List<HalfEdge> _halfEdges;


        public IEnumerable<Vertex> Vertices => _halfEdges.Select(h => h.Start);
        public List<HalfEdge> HalfEdges
        {
            get => _halfEdges;
            init
            {
                _halfEdges = value;
                var count = _halfEdges.Count;
                for (var i = 0; i < count; i++)
                {
                    var index = i;
                    var nextIndex = i + 1;
                    if (nextIndex == count)
                        nextIndex = 0;

                    _halfEdges[index].Polygon = this;
                    _halfEdges[index].Next = _halfEdges[nextIndex];
                }
            }
        }
        public IEnumerable<Polygon> Neighbors
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


        public Polygon(List<Vertex> vertices)
        {
            vertices.NotNullOrEmpty();
            vertices.HasElementCount(e => e > 2);

            var halfEdges = new List<HalfEdge>();
            Vertex? first = null;
            foreach (var vertex in vertices)
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
            _halfEdges.NotNull();
        }

        public Polygon(IEnumerable<HalfEdge> halfEdges)
        {
            halfEdges.NotNullOrEmpty();
            halfEdges.Select(h => (h.Start, h.End)).FormsLoop();

            HalfEdges = halfEdges.ToList();
            _halfEdges.NotNull();
        }

        public void Deconstruct(out IEnumerable<Vertex> vertices, out IEnumerable<HalfEdge> halfEdges)
        {
            vertices = Vertices;
            halfEdges = HalfEdges;
        }


        public static implicit operator Vertex[](Polygon polygon) => polygon.Vertices.ToArray();
        public static implicit operator HalfEdge[](Polygon polygon) => polygon._halfEdges.ToArray();
        public static implicit operator Polygon(Vertex[] vertices)
        {
            vertices.NotNullOrEmpty();
            vertices.HasElementCount(e => e > 2);

            return new(vertices.ToList());
        }

        public override string ToString() => $"{_halfEdges.Count} Vertices, On Border: {_halfEdges.Any(h => h.Opposite is null)}";
    }
}
