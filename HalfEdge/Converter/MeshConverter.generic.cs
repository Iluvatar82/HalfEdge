using HalfEdge.Models;
using HalfEdge.Models.Base;

namespace HalfEdge.Converter
{
    public static class MeshConverter<T> where T : struct
    {
        public static TriangleMesh<T> ConvertToTriangleMesh(Mesh<T> mesh)
        {
            var result = new TriangleMesh<T>(mesh.Vertices.ToList());
            result.AddHalfEdges(mesh.HalfEdges);
            foreach (var polygon in mesh.Polygons)
            {
                switch(polygon.HalfEdges.Count)
                {
                    case 3:
                        result.AddPolygon(polygon);
                        break;

                    case > 3:
                        //TODO: add additional HalfEdges!
                        result.AddPolygons(PolygonConverter<T>.ConvertToTriangles(polygon));
                        break;
                }
            }

            return result;
        }
    }
}
