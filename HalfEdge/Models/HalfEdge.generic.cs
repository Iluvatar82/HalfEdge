using System.Numerics;

namespace HalfEdge.Models
{
    public record class HalfEdge<T>
    {
        private Vertex<T> _start;
        private Vertex<T> _end;


        public Vertex<T> Start
        {
            get => _start;
            init
            {
                _start = value;
                _start.OutHalfEdges.Add(this);
            }
        }
        public Vertex<T> End
        {
            get => _end;
            init { _end = value; }
        }
        public HalfEdge<T>? Opposite { get; set; }


        public HalfEdge(Vertex<T> start, Vertex<T> end, HalfEdge<T>? opposite = null)
        {
            Start = start;
            End = end;
            Opposite = opposite;
        }


        public HalfEdge<T> CreateOpposite()
        {
            var oppositeHalfEdge = this with { Start = this.End, End = this.Start, Opposite = this };
            Opposite = oppositeHalfEdge;
            return oppositeHalfEdge;
        }
    }
}
