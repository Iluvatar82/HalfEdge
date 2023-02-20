using Framework;
using Framework.Extensions;
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
        public bool IsOpenMesh => !_halfEdges.Any() || _halfEdges.Any(h => h.Opposite is null);
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


        public virtual void AddVertex(Vertex vertex) => _vertices.Add(vertex);

        public virtual void AddVertices(IEnumerable<Vertex> vertices) => _vertices.AddRange(vertices);

        public virtual void RemoveVertex(Vertex vertex) => _vertices.Remove(vertex);
        
        public virtual void RemoveVertices(IEnumerable<Vertex> vertices) => vertices.ForEach(RemoveVertex);

        public virtual void AddIndices(List<int> indices) => _indices.Add(indices);

        public virtual void AddIndices(List<List<int>> indicesList) => _indices.AddRange(indicesList);

        public virtual void RemoveIndices(List<int> indices) => _indices.Remove(indices);

        public virtual void RemoveIndices(List<List<int>> indicesList) => indicesList.ForEach(RemoveIndices);

        public virtual void AddHalfEdge(HalfEdge halfEdge) => _halfEdges.Add(halfEdge);
        
        public virtual void AddHalfEdges(IEnumerable<HalfEdge> halfEdges) => _halfEdges.AddRange(halfEdges);
        
        public virtual void RemoveHalfEdge(HalfEdge halfEdge) => _halfEdges.Remove(halfEdge);
        
        public virtual void RemoveHalfEdges(IEnumerable<HalfEdge> halfEdges) => halfEdges.ForEach(RemoveHalfEdge);

        public virtual void AddPolygon(Polygon polygon) => _polygons.Add(polygon);
        
        public virtual void AddPolygons(IEnumerable<Polygon> polygons) => _polygons.AddRange(polygons);

        public virtual void RemovePolygon(Polygon polygon) => _polygons.Remove(polygon);
        
        public virtual void RemovePolygons(IEnumerable<Polygon> polygons) => polygons.ForEach(RemovePolygon);

        public int GetVertexIndex(Vertex vertex)
        {
            var directMatch = _vertices.IndexOf(vertex);
            if (directMatch > -1)
                return directMatch;

            var matches = _vertices.Where(v => v.SquaredDistanceTo(vertex) < Constants.Epsilon).ToList();
            if (!matches.Any())
                return -1;
            else if (matches.Count == 1)
                return _vertices.IndexOf(matches[0]);

            var min = matches.MinBy(v => v.SquaredDistanceTo(vertex));
            if (min is null)
                return -1;

            return _vertices.IndexOf(min);
        }
    }
}
