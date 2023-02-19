using Models;
using Models.Base;

namespace HalfEdge.Converter
{
    public static class MeshConverter
    {
        public static TriangleMesh ConvertToTriangleMesh(Mesh mesh)
        {
            var result = new TriangleMesh(mesh.Vertices.ToList(), mesh.Indices.ToList(), mesh.HalfEdges.ToList(), mesh.Polygons.ToList());
            foreach (var polygon in mesh.Polygons.ToList())
            {
                switch(polygon.HalfEdges.Count)
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
