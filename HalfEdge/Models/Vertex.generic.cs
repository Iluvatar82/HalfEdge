namespace HalfEdge.Models
{
    public record class Vertex<T>
    {
        public T X { get; init; }
        public T Y { get; init; }
        public T Z { get; init; }
        public List<HalfEdge<T>> OutHalfEdges { get; set; }


        public Vertex(T x, T y, T z)
        {
            X = x;
            Y = y;
            Z = z;

            OutHalfEdges = new List<HalfEdge<T>>();
        }
    }
}
