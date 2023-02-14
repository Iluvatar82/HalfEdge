using Models;
using Models.Base;
using Validation;

namespace HalfEdge
{
    public static class MeshFactory
    {
        public static Mesh CreateMesh(IEnumerable<Vertex> positions, IEnumerable<List<int>>? indices)
        {
            positions.NotNull();
            indices.NotNull();

            var mesh = new Mesh(positions.ToList(), (indices ?? Enumerable.Empty<List<int>>()).ToList());
            foreach (var polygonIndices in mesh.Indices)
                AddPolygon(mesh, polygonIndices);

            return mesh;
        }

        private static void AddPolygon(Mesh mesh, List<int> polygonIndices)
        {
            var halfEdges = new List<Models.Base.HalfEdge>();
            var pointCount = polygonIndices.Count;
            for(var i = 0; i < pointCount; i++)
            {
                var existingOpposite = mesh.HalfEdges.SingleOrDefault(o => o.End == mesh.Vertices[polygonIndices[i]] && o.Start == mesh.Vertices[polygonIndices[(i + 1) % pointCount]]);
                if (existingOpposite != default)
                    halfEdges.Add(existingOpposite.CreateOpposite());
                else
                    halfEdges.Add(new Models.Base.HalfEdge(mesh.Vertices[polygonIndices[i]], mesh.Vertices[polygonIndices[(i + 1) % pointCount]]));
            }

            halfEdges.HasElementCount(c => c > 2);

            mesh.AddHalfEdges(halfEdges);
            mesh.AddPolygon(new Polygon(halfEdges));
        }
    }
}
