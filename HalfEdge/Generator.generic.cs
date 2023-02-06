using HalfEdge.Models;

namespace HalfEdge
{
    public class Generator<T>
    {
        public List<Vertex<T>> Vertices { get; set; }
        public List<List<int>> Indices { get; set; }
        public List<HalfEdge<T>> Mesh { get; set; }
        public int PolygonCount => Indices.Count;

        public Generator(IEnumerable<Vertex<T>> positions, IEnumerable<List<int>> indices)
        {
            Vertices = positions.ToList();
            Indices = indices.ToList();
            Mesh = new List<HalfEdge<T>>();
            foreach (var polygonIndices in Indices)
                AddPolygon(polygonIndices);
        }

        private void AddPolygon(List<int> polygonIndices)
        {
            var halfEdges = new List<HalfEdge<T>>();
            var pointCount = polygonIndices.Count;
            for(var i = 0; i < pointCount; i++)
            {
                var existingOpposite = Mesh.SingleOrDefault(o => o.End == Vertices[polygonIndices[i]] && o.Start == Vertices[polygonIndices[(i + 1) % pointCount]]);
                if (existingOpposite != default)
                    halfEdges.Add(existingOpposite.CreateOpposite());
                else
                    halfEdges.Add(new HalfEdge<T>(Vertices[polygonIndices[i]], Vertices[polygonIndices[(i + 1) % pointCount]]));
            }

            Mesh.AddRange(halfEdges);
        }
    }
}
