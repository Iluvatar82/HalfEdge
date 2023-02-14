using Models;

namespace Triangulator
{
    public static class Triangulator
    {
        public static List<Polygon> Triangulate(Polygon polygon)
        {
            //Project to "best" Axis Aligned Plane, i.e. the two most "used" dimensions
            var boundingBox = new AxisAlignedBoundingBox(polygon.Vertices);


            //Triangulate

            throw new NotImplementedException();
        }
    }
}
