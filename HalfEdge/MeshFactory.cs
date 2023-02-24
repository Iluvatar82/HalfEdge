using Framework.Extensions;
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
            for (var i = 0; i < pointCount; i++)
            {
                var startVertex = mesh.Vertices[polygonIndices[i]];
                var endVertex = mesh.Vertices[polygonIndices[(i + 1) % pointCount]];

                mesh.BorderHalfEdgeDictionary.TryGetValue((endVertex, startVertex), out var existingOpposite);
                if (existingOpposite != null)
                {
                    halfEdges.Add(existingOpposite.CreateOpposite());
                    mesh.BorderHalfEdgeDictionary.Remove((endVertex, startVertex));
                }
                else
                {
                    var newHalfEdge = new Models.Base.HalfEdge(startVertex, endVertex);
                    halfEdges.Add(newHalfEdge);
                    mesh.BorderHalfEdgeDictionary.Add((startVertex, endVertex), newHalfEdge);
                }
            }

            halfEdges.HasElementCount(c => c > 2);

            if (addIndices)
                mesh.AddIndices(polygonIndices);

            mesh.AddHalfEdges(halfEdges);
            mesh.AddPolygon(new Polygon(halfEdges));
        }

        public static void AddPolygonToMesh(Mesh mesh, List<Vertex> vertices)
        {
            var halfEdges = new List<Models.Base.HalfEdge>();
            var pointCount = vertices.Count;
            var vertexIndex = vertices.Select(v => (Vertex: v, Index: mesh.GetVertexIndex(v))).ToList();
            for (var i = 0; i < pointCount; i++)
            {
                if (vertexIndex[i].Index > -1)
                    continue;

                vertexIndex[i] = (vertexIndex[i].Vertex, mesh.Vertices.Count);
                mesh.AddVertex(vertexIndex[i].Vertex);
            }

            var polygonIndices = vertexIndex.Select(vi => vi.Index).ToList();
            AddPolygonToMesh(mesh, polygonIndices, true);
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

            polygon.HalfEdges.Where(h => h.Opposite is null).ForEach(h => mesh.BorderHalfEdgeDictionary.Remove((h.Start, h.End)));
            polygon.HalfEdges.Where(h => h.Opposite is not null).Select(h => h.Opposite).ForEach(o => mesh.BorderHalfEdgeDictionary.Add((o.Start, o.End), o));
        }
    }
}
