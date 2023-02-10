using HalfEdge.Models;
using HalfEdge.Models.Base;
using Validation;

namespace HalfEdge
{
    public class Generator<T> where T : struct
    {
        public Mesh<T> Mesh { get; set; }
        public int PolygonCount => Mesh.Polygons.Count();

        public Generator(IEnumerable<Vertex<T>> positions, IEnumerable<List<int>>? indices)
        {
            positions.NotNull();
            indices.NotNull();

            Mesh = new Mesh<T>(positions.ToList(), (indices ?? Enumerable.Empty<List<int>>()).ToList());
            foreach (var polygonIndices in Mesh.Indices)
                AddPolygon(polygonIndices);
        }

        private void AddPolygon(List<int> polygonIndices)
        {
            var halfEdges = new List<HalfEdge<T>>();
            var pointCount = polygonIndices.Count;
            for(var i = 0; i < pointCount; i++)
            {
                var existingOpposite = Mesh.HalfEdges.SingleOrDefault(o => o.End == Mesh.Vertices[polygonIndices[i]] && o.Start == Mesh.Vertices[polygonIndices[(i + 1) % pointCount]]);
                if (existingOpposite != default)
                    halfEdges.Add(existingOpposite.CreateOpposite());
                else
                    halfEdges.Add(new HalfEdge<T>(Mesh.Vertices[polygonIndices[i]], Mesh.Vertices[polygonIndices[(i + 1) % pointCount]]));
            }

            halfEdges.HasElementCount(c => c > 2);

            Mesh.AddHalfEdges(halfEdges);
            Mesh.AddPolygon(new Polygon<T>(halfEdges));
        }
    }
}
