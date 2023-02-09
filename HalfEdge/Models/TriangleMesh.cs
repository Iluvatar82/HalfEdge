using HalfEdge.Extensions;
using HalfEdge.Models.Base;
using Validation;

namespace HalfEdge.Models
{
    public class TriangleMesh<T> : Mesh<T> where T : struct
    {
        public override void AddPolygon(Polygon<T> polygon)
        {
            polygon.HalfEdges.HasElementCountEqualTo(3);

            base.AddPolygon(polygon);
        }

        public override void AddPolygons(IEnumerable<Polygon<T>> polygons)
        {
            polygons.ForEach(p => p.HalfEdges.HasElementCountEqualTo(3));

            base.AddPolygons(polygons);
        }
    }
}
