using Framework.Extensions;
using Models.Base;
using Validation;

namespace Models
{
    public class TriangleMesh : Mesh
    {
        public TriangleMesh(List<Vertex> vertices, List<List<int>> indices, List<HalfEdge> halfEdges, List<Polygon> polygons)
            :base(vertices, indices)
        {
            _halfEdges = halfEdges;
            _polygons = polygons;
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
