using Validation;

namespace Models.Base
{
    public record class HalfEdge
    {
        private Vertex _start;
        private Vertex _end;
        private HalfEdge? _opposite;
        private HalfEdge? _next;
        private HalfEdge? _previous;
        private Polygon? _polygon;

        public Vertex Start
        {
            get => _start;
            init
            {
                _start = value;
                _start.HalfEdges.Add(this);
            }
        }
        public Vertex End
        {
            get => _end;
            init { _end = value; }
        }
        public HalfEdge? Opposite { get => _opposite; set => _opposite = value; }
        public HalfEdge? Next
        {
            get => _next;
            set
            {
                if (value is not null)
                    value._previous = this;

                _next = value;
            }
        }
        public HalfEdge? Previous
        {
            get => _previous;
            set
            {
                if (value is not null)
                    value._next = this;

                _previous = value;
            }
        }
        public Polygon? Polygon
        {
            get => _polygon;
            set => _polygon = value;
        }
        public bool IsBorder => Opposite is null;


        public HalfEdge(Vertex start, Vertex end)
        {
            Start = start;
            End = end;

            _start.NotNull();
            _end.NotNull();
        }

        public HalfEdge(Vertex start, Vertex end, HalfEdge opposite) : this(start, end)
        {
            Opposite = opposite;
        }

        public void Deconstruct(out Vertex start, out Vertex end)
        {
            start = Start;
            end = End;
        }


        public HalfEdge CreateOpposite()
        {
            var oppositeHalfEdge = this with { Start = End, End = Start, Opposite = this, Previous = null, Next = null };
            Opposite = oppositeHalfEdge;
            return oppositeHalfEdge;
        }


        public static implicit operator HalfEdge((Vertex Start, Vertex End) vertices) => new(vertices.Start, vertices.End);


        public override string ToString() => $"Start: [{_start}], End: [{_end}], Opposite: {_opposite is not null}, Previous: {_previous is not null}, Next: {_next is not null}";
    }
}
