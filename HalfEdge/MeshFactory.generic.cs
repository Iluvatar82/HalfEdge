using Models;
using Models.Base;
using Validation;

namespace HalfEdge
{
    public static class MeshFactory
    {
        public static Mesh<T> CreateMesh<T>(IEnumerable<Vertex<T>> positions, IEnumerable<List<int>>? indices) where T : struct
        {
            positions.NotNull();
            indices.NotNull();

            var mesh = new Mesh<T>(positions.ToList(), (indices ?? Enumerable.Empty<List<int>>()).ToList());
            foreach (var polygonIndices in mesh.Indices)
                AddPolygon(mesh, polygonIndices);

            return mesh;
        }

        private static void AddPolygon<T>(Mesh<T> mesh, List<int> polygonIndices) where T : struct
        {
            var halfEdges = new List<HalfEdge<T>>();
            var pointCount = polygonIndices.Count;
            for(var i = 0; i < pointCount; i++)
            {
                var existingOpposite = mesh.HalfEdges.SingleOrDefault(o => o.End == mesh.Vertices[polygonIndices[i]] && o.Start == mesh.Vertices[polygonIndices[(i + 1) % pointCount]]);
                if (existingOpposite != default)
                    halfEdges.Add(existingOpposite.CreateOpposite());
                else
                    halfEdges.Add(new HalfEdge<T>(mesh.Vertices[polygonIndices[i]], mesh.Vertices[polygonIndices[(i + 1) % pointCount]]));
            }

            halfEdges.HasElementCount(c => c > 2);

            mesh.AddHalfEdges(halfEdges);
            mesh.AddPolygon(new Polygon<T>(halfEdges));
        }
    }
}
