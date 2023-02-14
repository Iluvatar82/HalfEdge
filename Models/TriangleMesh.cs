using Framework.Extensions;
using Models.Base;
using Validation;

namespace Models
{
    public class TriangleMesh : Mesh
    {

        public TriangleMesh(List<Vertex> vertices)
            :base()
        {
            _vertices = vertices;
        }

        public override void AddPolygon(Polygon polygon)
        {
            polygon.HalfEdges.HasElementCountEqualTo(3);

            base.AddPolygon(polygon);
        }

        public override void AddPolygons(IEnumerable<Polygon> polygons)
        {
            polygons.ForEach(p => p.HalfEdges.HasElementCountEqualTo(3));

            base.AddPolygons(polygons);
        }
    }
}
