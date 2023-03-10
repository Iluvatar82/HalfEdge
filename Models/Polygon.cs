using Models.Base;
using System;
using Validation;

namespace Models
{
    public record class Polygon
    {
        private List<HalfEdge> _halfEdges;


        public IEnumerable<Vertex> Vertices => _halfEdges.Select(h => h.Start);
        public List<HalfEdge> HalfEdges => _halfEdges;
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
            _halfEdges = halfEdges;

            for (var idx = 0; idx < _halfEdges.Count; idx++)
            {
                var nextIdx = idx + 1;
                if (nextIdx == _halfEdges.Count)
                    nextIdx = 0;

                if (_halfEdges[idx].End != _halfEdges[nextIdx].Start)
                    throw new ArgumentOutOfRangeException(nameof(halfEdges));

                _halfEdges[idx].Polygon = this;
                _halfEdges[idx].Next = _halfEdges[nextIdx];
            }
        }

        public Polygon(List<HalfEdge> halfEdges)
        {
            halfEdges.NotNullOrEmpty();

            _halfEdges = halfEdges.ToList();
            for (var idx = 0; idx < _halfEdges.Count; idx++)
            {
                var nextIdx = idx + 1;
                if (nextIdx == _halfEdges.Count)
                    nextIdx = 0;

                if (_halfEdges[idx].End != _halfEdges[nextIdx].Start)
                    throw new ArgumentOutOfRangeException(nameof(halfEdges));

                _halfEdges[idx].Polygon = this;
                _halfEdges[idx].Next = _halfEdges[nextIdx];
            }
        }

        public void Deconstruct(out IEnumerable<Vertex> vertices, out IEnumerable<HalfEdge> halfEdges)
        {
            vertices = Vertices;
            halfEdges = _halfEdges;
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
