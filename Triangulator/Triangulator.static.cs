using HalfEdge.Models;

namespace Triangulator
{
    public static class Triangulator<T> where T : struct
    {
        public static List<Polygon<T>> Triangulate(Polygon<T> polygon)
        {
            //Assumption: all Points are on one Plane!

            //Project to "best" Axis Aligned Plane

            //Triangulate

            throw new NotImplementedException();
        }
    }
}
