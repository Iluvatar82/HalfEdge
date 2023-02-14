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
                        //TODO: add additional HalfEdges!
                        result.AddPolygons(PolygonConverter.ConvertToTriangles(polygon));
                        break;
                }
            }

            return result;
        }
    }
}
