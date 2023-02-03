using HalfEdge.Models;

namespace HalfEdge
{
    public class Generator<T>
    {
        public List<Vertex<T>> Vertices { get; set; }
        public List<List<int>> Indices { get; set; }
        public List<HalfEdge<T>> Mesh { get; set; }

        public Generator(IEnumerable<Vertex<T>> positions, IEnumerable<List<int>> indices)
        {
            Vertices = positions.ToList();
            Indices = indices.ToList();
            foreach (var polygonIndices in Indices)
                AddPolygon(polygonIndices);
        }

        public void AddPolygon(IEnumerable<int> polygonIndices)
        {

        }
    }
}
