using Models;
using Models.Base;

namespace HalfEdge.Converter
{
    public static class MeshConverter
    {
        public static TriangleMesh ConvertToTriangleMesh(Mesh mesh)
        {
            var newMesh = MeshFactory.CreateMesh(
                mesh.Vertices.Select(v => v with { HalfEdges = new List<Models.Base.HalfEdge>() }),
                mesh.Indices.ToList());

            var result = new TriangleMesh(
                newMesh.Vertices.Select(v => v with { }).ToList(),
                newMesh.Indices.ToList(),
                newMesh.HalfEdges.Select(h => h with { }).ToList(),
                newMesh.Polygons.Select(p => p with { }).ToList());

            foreach (var polygon in result.Polygons.ToList())
            {
                switch (polygon.HalfEdges.Count)
                {
                    case 3:
                        break;

                    case > 3:
                        var polygonVertices = polygon.Vertices.ToList();

                        var triangulationIndices = Triangulator.Triangulator.Triangulate(polygon);
                        var meshTriangleIndices = triangulationIndices.Select(triangleIndices => triangleIndices.Select(index => result.Vertices.IndexOf(polygonVertices[index])).ToList()).ToList();

                        MeshFactory.RemovePolygonFromMesh(result, polygon);
                        foreach (var triangleIndices in meshTriangleIndices)
                            MeshFactory.AddPolygonToMesh(result, triangleIndices, true);

                        break;
                }
            }

            return result;
        }
    }
}
