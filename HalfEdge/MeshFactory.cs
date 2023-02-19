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

            var mesh = new Mesh(positions.ToList(), indices.ToList());
            foreach (var polygonIndices in mesh.Indices)
                AddPolygonToMesh(mesh, polygonIndices);

            return mesh;
        }

        public static void AddPolygonToMesh(Mesh mesh, List<int> polygonIndices, bool addIndices = false)
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

            if (addIndices)
                mesh.AddIndices(polygonIndices);

            mesh.AddHalfEdges(halfEdges);
            mesh.AddPolygon(new Polygon(halfEdges));
        }

        public static void RemovePolygonFromMesh(Mesh mesh, Polygon polygon)
        {
            var vertexIndices = new List<int>();
            foreach (var halfEdge in polygon.HalfEdges)
            {
                if (halfEdge.Opposite != null)
                    halfEdge.Opposite.Opposite = null;

                halfEdge.Start.HalfEdges.Remove(halfEdge);
                vertexIndices.Add(mesh.Vertices.IndexOf(halfEdge.Start));
            }

            mesh.RemoveHalfEdges(polygon.HalfEdges);
            mesh.RemoveIndices(mesh.Indices.First(indexList => indexList.All(index => vertexIndices.Contains(index))));
            mesh.RemovePolygon(polygon);
        }
    }
}
