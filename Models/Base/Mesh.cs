using System.Collections.ObjectModel;

namespace Models.Base
{
    public class Mesh
    {
        protected List<Vertex> _vertices;
        protected List<List<int>> _indices;
        protected List<HalfEdge> _halfEdges;
        protected List<Polygon> _polygons;

        public ReadOnlyCollection<Vertex> Vertices => _vertices.AsReadOnly<Vertex>();
        public ReadOnlyCollection<List<int>> Indices => _indices.AsReadOnly<List<int>>();
        public ReadOnlyCollection<HalfEdge> HalfEdges => _halfEdges.AsReadOnly<HalfEdge>();
        public ReadOnlyCollection<Polygon> Polygons => _polygons.AsReadOnly<Polygon>();
        public IEnumerable<List<HalfEdge>> Borders
        {
            get
            {
                var allBorderHalfEdges = _halfEdges.Where(h => h.Opposite is null).ToList();
                if (!allBorderHalfEdges.Any())
                    yield break;

                while (allBorderHalfEdges.Any())
                {
                    var borderLoop = new List<HalfEdge>();
                    var currentHalfEdge = allBorderHalfEdges.First();
                    allBorderHalfEdges.Remove(currentHalfEdge);
                    while (true)
                    {
                        borderLoop.Add(currentHalfEdge);
                        currentHalfEdge = currentHalfEdge.End.HalfEdges.Single(h => h != currentHalfEdge && h.Opposite is null);
                        allBorderHalfEdges.Remove(currentHalfEdge);
                        if (borderLoop[0] == currentHalfEdge)
                            break;
                    }

                    yield return borderLoop;
                }
            }
        }
        public bool IsOpenMesh => Borders.Any();
        public int PolygonCount => _polygons.Count;


        public Mesh()
        {
            _vertices = new List<Vertex>();
            _indices = new List<List<int>>();
            _halfEdges = new List<HalfEdge>();
            _polygons = new List<Polygon>();
        }

        public Mesh(List<Vertex> vertices, List<List<int>> indices)
        {
            _vertices = vertices;
            _indices = indices;
            _halfEdges = new List<HalfEdge>();
            _polygons = new List<Polygon>();
        }


        public void AddHalfEdges(IEnumerable<HalfEdge> halfEdges) => _halfEdges.AddRange(halfEdges);

        public virtual void AddPolygon(Polygon polygon) => _polygons.Add(polygon);

        public virtual void AddPolygons(IEnumerable<Polygon> polygons) => _polygons.AddRange(polygons);
    }
}
