using Models;
using Models.Base;

namespace HalfEdge.Converter
{
    public static class MeshConverter
    {
        public static TriangleMesh ConvertToTriangleMesh(Mesh mesh)
        {
            var result = new TriangleMesh(mesh.Vertices.ToList());
            result.AddHalfEdges(mesh.HalfEdges);
            foreach (var polygon in mesh.Polygons)
            {
                switch(polygon.HalfEdges.Count)
                {
                    case 3:
                        result.AddPolygon(polygon);
                        break;

                    case > 3:
                        var polygonVertices = polygon.Vertices.ToList();

                        var triangulationIndices = Triangulator.Triangulator.Triangulate(polygon);
                        var meshTriangleIndices = triangulationIndices.Select(triangleIndices => triangleIndices.Select(index => mesh.Vertices.IndexOf(polygonVertices[index])).ToList()).ToList();

                        MeshFactory.RemovePolygon(mesh, polygon);
                        foreach (var triangleIndices in meshTriangleIndices)
                            MeshFactory.AddPolygon(mesh, triangleIndices);

                        break;
                }
            }

            return result;
        }
    }
}
