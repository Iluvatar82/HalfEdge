using HalfEdge.Models;
using Validation;

namespace HalfEdge
{
    public class Generator<T> where T : struct
    {
        public List<Vertex<T>> Vertices { get; set; }
        public List<List<int>> Indices { get; set; }
        public Mesh<T> Mesh { get; set; }
        public int PolygonCount => Mesh.Polygons.Count;

        public Generator(IEnumerable<Vertex<T>> positions, IEnumerable<List<int>>? indices)
        {
            positions.NotNull();
            indices.NotNull();

            Vertices = positions.ToList();
            Indices = (indices ?? Enumerable.Empty<List<int>>()).ToList();
            Mesh = new Mesh<T>();
            foreach (var polygonIndices in Indices)
                AddPolygon(polygonIndices);
        }

        private void AddPolygon(List<int> polygonIndices)
        {
            var halfEdges = new List<HalfEdge<T>>();
            var pointCount = polygonIndices.Count;
            for(var i = 0; i < pointCount; i++)
            {
                var existingOpposite = Mesh.HalfEdges.SingleOrDefault(o => o.End == Vertices[polygonIndices[i]] && o.Start == Vertices[polygonIndices[(i + 1) % pointCount]]);
                if (existingOpposite != default)
                    halfEdges.Add(existingOpposite.CreateOpposite());
                else
                    halfEdges.Add(new HalfEdge<T>(Vertices[polygonIndices[i]], Vertices[polygonIndices[(i + 1) % pointCount]]));
            }

            Mesh.HalfEdges.AddRange(halfEdges);
            Mesh.Polygons.Add(new Polygon<T>(halfEdges));
        }
    }
}
