using Framework.Extensions;
using Models;
using Models.Base;

namespace Triangulator
{
    public static class Triangulator
    {
        public static List<List<int>> Triangulate(Polygon polygon)
        {
            var boundingBox = new AxisAlignedBoundingBox(polygon.Vertices);
            var projectedPolygon = ProjectVerticesToAxisAlignedPlane(polygon, boundingBox);

            return SweepLinePolygonTriangulator.Triangulate(projectedPolygon);
        }

        private static List<Vertex2D> ProjectVerticesToAxisAlignedPlane(Polygon polygon, AxisAlignedBoundingBox boundingBox)
        {
            var sizeX = boundingBox.Max.X - boundingBox.Min.X;
            var sizeY = boundingBox.Max.Y - boundingBox.Min.Y;
            var sizeZ = boundingBox.Max.Z - boundingBox.Min.Z;
            var minSize = new[] { sizeX, sizeY, sizeZ }.Min();

            var result = new List<Vertex2D>();
            if (minSize == sizeX)
                result = polygon.Vertices.Select(v => new Vertex2D(v.Y, v.Z)).ToList();
            else if (minSize == sizeY)
                result = polygon.Vertices.Select(v => new Vertex2D(v.X, v.Z)).ToList();
            else
                result = polygon.Vertices.Select(v => new Vertex2D(v.X, v.Y)).ToList();

            if (!result.Select(v => (v.X, v.Y)).ToList().IsCCW())
                result.Reverse();

            return result;
        }
    }
}