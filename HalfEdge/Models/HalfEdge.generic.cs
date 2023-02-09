using Validation;

namespace HalfEdge.Models
{
    public record class HalfEdge<T> where T : struct
    {
        private Vertex<T> _start;
        private Vertex<T> _end;
        private HalfEdge<T>? _opposite;
        private HalfEdge<T>? _next;
        private HalfEdge<T>? _previous;
        private Polygon<T>? _polygon;

        public Vertex<T> Start
        {
            get => _start; init
            {
                _start = value;
                _start.HalfEdges.Add(this);
            }
        }
        public Vertex<T> End
        {
            get => _end;
            init { _end = value; }
        }
        public HalfEdge<T>? Opposite { get => _opposite; set => _opposite = value; }
        public HalfEdge<T>? Next
        {
            get => _next;
            set
            {
                if (value is null && _next is not null)
                    _next._previous = null;
                else if (value is not null)
                    value._previous = this;

                _next = value;
            }
        }
        public HalfEdge<T>? Previous
        {
            get => _previous;
            set
            {
                if (value is null && _previous is not null)
                    _previous._next = null;
                else if (value is not null)
                    value._next = this;

                _previous = value;
            }
        }
        public Polygon<T>? Polygon
        {
            get => _polygon;
            set => _polygon = value;
        }
        public bool IsBorder => Opposite is null;


        public HalfEdge()
        {
            _start = new Vertex<T>();
            _end = new Vertex<T>();
        }

        public HalfEdge(Vertex<T> start, Vertex<T> end, HalfEdge<T>? opposite = default) : this()
        {
            start.NotNull();
            end.NotNull();

            Start = start;
            End = end;
            Opposite = opposite;
        }

        public void Deconstruct(out Vertex<T> start, out Vertex<T> end)
        {
            start = Start;
            end = End;
        }


        public HalfEdge<T> CreateOpposite()
        {
            var oppositeHalfEdge = this with { Start = this.End, End = this.Start, Opposite = this, Previous = null, Next = null };
            Opposite = oppositeHalfEdge;
            return oppositeHalfEdge;
        }


        public static implicit operator HalfEdge<T>((Vertex<T> Start, Vertex<T> End) vertices) => new(vertices.Start, vertices.End);


        public override string ToString() => $"Start: [{_start}], End: [{_end}], Opposite: {_opposite is not null}, Previous: {_previous is not null}, Next: {_next is not null}";
    }
}
