using System.Numerics;

namespace HalfEdge.Models
{
    public class HalfEdge<T>
    {
        public Vertex<T> Start { get; set; }
        public Vertex<T> End{ get; set; }
        public HalfEdge<T> Opposite { get; set; }
    }
}
