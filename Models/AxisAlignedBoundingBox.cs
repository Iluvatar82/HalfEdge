using Framework;
using Models.Base;
using Validation;

namespace Models
{
    public record class AxisAlignedBoundingBox
    {
        public Vertex Min { get; set; }
        public Vertex Max { get; init; }

        public AxisAlignedBoundingBox(IEnumerable<Vertex> vertices)
        {
            vertices.NotNullOrEmpty();

            var first = vertices.First();
            Min = new Vertex(first.X, first.Y, first.Z);
            Max = new Vertex(first.X, first.Y, first.Z);

            foreach (var vertex in vertices.Skip(1))
            {
                var minX = Math.Min(Min.X, vertex.X);
            }
        }
    }
}
