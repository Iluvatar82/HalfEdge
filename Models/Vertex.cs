namespace HalfEdge.Models
{
    public class Vertex<T>
    {
        public T X { get; set; }
        public T Y { get; set; }
        public T Z { get; set; }
        public HalfEdge<T> OutHalfEdges { get; set; }
    }
}
