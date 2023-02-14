using System.Collections.ObjectModel;

namespace Models.Base
{
    public class Mesh<T> where T : struct
    {
        protected List<Vertex<T>> _vertices;
        protected List<List<int>> _indices;
        protected List<HalfEdge<T>> _halfEdges;
        protected List<Polygon<T>> _polygons;

        public ReadOnlyCollection<Vertex<T>> Vertices => _vertices.AsReadOnly<Vertex<T>>();
        public ReadOnlyCollection<List<int>> Indices => _indices.AsReadOnly<List<int>>();
        public ReadOnlyCollection<HalfEdge<T>> HalfEdges => _halfEdges.AsReadOnly<HalfEdge<T>>();
        public ReadOnlyCollection<Polygon<T>> Polygons => _polygons.AsReadOnly<Polygon<T>>();
        public IEnumerable<List<HalfEdge<T>>> Borders
        {
            get
            {
                var allBorderHalfEdges = _halfEdges.Where(h => h.Opposite is null).ToList();
                if (!allBorderHalfEdges.Any())
                    yield break;

                while (allBorderHalfEdges.Any())
                {
                    var borderLoop = new List<HalfEdge<T>>();
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
            _vertices = new List<Vertex<T>>();
            _indices = new List<List<int>>();
            _halfEdges = new List<HalfEdge<T>>();
            _polygons = new List<Polygon<T>>();
        }

        public Mesh(List<Vertex<T>> vertices, List<List<int>> indices)
        {
            _vertices = vertices;
            _indices = indices;
            _halfEdges = new List<HalfEdge<T>>();
            _polygons = new List<Polygon<T>>();
        }


        public void AddHalfEdges(IEnumerable<HalfEdge<T>> halfEdges) => _halfEdges.AddRange(halfEdges);

        public virtual void AddPolygon(Polygon<T> polygon) => _polygons.Add(polygon);

        public virtual void AddPolygons(IEnumerable<Polygon<T>> polygons) => _polygons.AddRange(polygons);
    }
}
