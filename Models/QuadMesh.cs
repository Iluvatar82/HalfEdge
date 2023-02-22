using Framework.Extensions;
using Models.Base;
using Validation;

namespace Models
{
    public record class QuadMesh : Mesh
    {
        public QuadMesh(List<Vertex> vertices, List<List<int>> indices, List<HalfEdge> halfEdges, List<Polygon> polygons)
            :base(vertices, indices)
        {
            _halfEdges = halfEdges;
            _polygons = polygons;
        }


        public override void AddPolygon(Polygon polygon)
        {
            polygon.HalfEdges.HasElementCountEqualTo(4);

            base.AddPolygon(polygon);
        }

        public override void AddPolygons(IEnumerable<Polygon> polygons)
        {
            polygons.ForEach(p => p.HalfEdges.HasElementCountEqualTo(4));

            base.AddPolygons(polygons);
        }
    }
}
