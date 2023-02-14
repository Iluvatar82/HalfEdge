using Framework.Extensions;
using Models.Base;
using Validation;

namespace Models
{
    public class TriangleMesh<T> : Mesh<T> where T : struct
    {

        public TriangleMesh(List<Vertex<T>> vertices)
            :base()
        {
            _vertices = vertices;
        }

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
