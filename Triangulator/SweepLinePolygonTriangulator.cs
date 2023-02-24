using Framework;
using Framework.Extensions;
using Models.Base;
using System.Diagnostics.CodeAnalysis;
using Validation;

namespace Triangulator
{
    public static class SweepLinePolygonTriangulator
    {
        public static List<List<int>> Triangulate(List<Vertex2D> vertices, List<List<Vertex2D>>? holes = null)
        {
            vertices.HasElementCount(c => c > 3);

            if (vertices.First() == vertices.Last())
                vertices.RemoveAt(vertices.Count - 1);

            var count = vertices.Count;
            var didReverse = false;
            if (!vertices.Select(v => (v.X, v.Y)).ToList().IsCCW())
            {
                vertices.Reverse();
                didReverse = true;
            }

            var polygon = new PolygonData(vertices);
            if (holes != null)
            {
                foreach (var hole in holes)
                    polygon.AddHole(hole);
            }

            var events = polygon.Points.ToList();
            events.Sort();

            var diagonals = CalculateDiagonals(events);
            events.Reverse();
            diagonals.AddRange(CalculateDiagonals(events, false));
            diagonals = diagonals.Distinct().ToList();

            var result = new List<List<int>>();
            var monotonePolygons = SplitIntoPolygons(polygon, diagonals);
            foreach (var monoton in monotonePolygons.Where(m => m != null))
                result.AddRange(TriangulateMonotone(monoton));

            if (didReverse)
            {
                var tempIndicesList = result.ToList();
                result = new List<List<int>>();
                foreach (var triangleIndices in tempIndicesList)
                {
                    var triangle = new List<int>();
                    foreach (var index in triangleIndices)
                        triangle.Add(count - index - 1);

                    result.Add(triangle);
                }
            }

            return result;
        }

        private static List<List<int>> TriangulateMonotone(PolygonData monoton)
        {
            var result = new List<List<int>>();
            var events = new List<PolygonPoint>(monoton.Points);
            events.Sort();

            var pointStack = new Stack<PolygonPoint>();
            pointStack.Push(events[0]);
            pointStack.Push(events[1]);

            var left = (events[0].Next == events[1]) ? events[1] : events[0];
            var right = (events[0].Last == events[1]) ? events[1] : events[0];

            var pointCnt = monoton.Points.Count;
            for (int i = 2; i < pointCnt; i++)
            {
                var newPoint = events[i];
                var top = pointStack.Peek();
                if (!(top.Last == newPoint || top.Next == newPoint))
                {
                    if (left.Next == newPoint)
                        left = newPoint;
                    else if (right.Last == newPoint)
                        right = newPoint;

                    var p2 = top;
                    while (pointStack.Count != 0)
                    {
                        top = pointStack.Pop();
                        p2 = top;
                        if (pointStack.Count != 0)
                        {
                            top = pointStack.Pop();

                            if (left == newPoint)
                                result.Add(new List<int> { newPoint.Index, p2.Index, top.Index });
                            else
                                result.Add(new List<int> { newPoint.Index, top.Index, p2.Index });
                        }
                        if (pointStack.Count != 0)
                            pointStack.Push(top);
                    }

                    pointStack.Push(events[i - 1]);
                    pointStack.Push(newPoint);
                }
                else
                {
                    top = pointStack.Pop();
                    var p2 = top;

                    if (left.Next == newPoint && right.Last == newPoint)
                    {
                        if (top.Last == newPoint)
                            right = newPoint;
                        else if (top.Next == newPoint)
                            left = newPoint;
                        else
                            throw new Exception("Triangulation error");
                    }
                    else if (left.Next == newPoint)
                        left = newPoint;
                    else if (right.Last == newPoint)
                        right = newPoint;

                    while (pointStack.Count != 0)
                    {
                        if (right == newPoint && new List<Vertex2D> { newPoint.Point, p2.Point, pointStack.Peek().Point }.Select(v => (v.X, v.Y)).ToList().IsCCW())
                        {
                            top = pointStack.Pop();
                            result.Add(new List<int> { newPoint.Index, p2.Index, top.Index });
                            p2 = top;
                        }
                        else if (left == newPoint && !new List<Vertex2D> { newPoint.Point, p2.Point, pointStack.Peek().Point }.Select(v => (v.X, v.Y)).ToList().IsCCW())
                        {
                            top = pointStack.Pop();
                            result.Add(new List<int> { newPoint.Index, top.Index, p2.Index });
                            p2 = top;
                        }
                        else
                            break;
                    }

                    pointStack.Push(p2);
                    pointStack.Push(newPoint);
                }
            }

            return result;
        }

        private static List<(int, int)> CalculateDiagonals(List<PolygonPoint> events, bool sweepDown = true)
        {
            var diagonals = new List<(int, int)>();
            var statusAndHelper = new StatusHelper();

            for (int i = 0; i < events.Count; i++)
            {
                var ev = events[i];
                var evClass = ev.PointClass(!sweepDown);

                StatusHelperElement? she;
                switch (evClass)
                {
                    case PolygonPointClass.Start:
                        statusAndHelper.Add(new StatusHelperElement(sweepDown ? ev.EdgeTwo : ev.EdgeOne, ev));
                        break;

                    case PolygonPointClass.Stop:
                        statusAndHelper.Remove(sweepDown ? ev.EdgeOne : ev.EdgeTwo);
                        break;

                    case PolygonPointClass.Regular:
                        if (ev.Last is not null && ev.Next is not null && ev.Last > ev.Next)
                        {
                            statusAndHelper.Remove(sweepDown ? ev.EdgeOne : ev.EdgeTwo);
                            statusAndHelper.Add(new StatusHelperElement(sweepDown ? ev.EdgeTwo : ev.EdgeOne, ev));
                        }
                        else
                        {
                            she = statusAndHelper.SearchLeft(ev);
                            if (she != null)
                                she.Helper = ev;
                        }
                        break;

                    case PolygonPointClass.Merge:
                        statusAndHelper.Remove(sweepDown ? ev.EdgeOne : ev.EdgeTwo);
                        she = statusAndHelper.SearchLeft(ev);
                        if (she != null)
                            she.Helper = ev;

                        break;

                    case PolygonPointClass.Split:
                        she = statusAndHelper.SearchLeft(ev);
                        if (she != null)
                        {
                            var minP = Math.Min(she.Helper.Index, ev.Index);
                            var maxP = Math.Max(she.Helper.Index, ev.Index);
                            var diagonal = (minP, maxP);
                            diagonals.Add(diagonal);

                            she.Helper = ev;
                            statusAndHelper.Add(new StatusHelperElement(sweepDown ? ev.EdgeTwo : ev.EdgeOne, ev));
                        }
                        break;
                }
            }
            return diagonals;
        }

        private static List<PolygonData> SplitIntoPolygons(PolygonData poly, List<(int, int)> diagonals)
        {
            if (diagonals.Count == 0)
                return new List<PolygonData>() { poly };

            diagonals = diagonals.OrderBy(d => d.Item1).ThenBy(d => d.Item2).ToList();
            var edges = new SortedDictionary<int, List<PolygonEdge>>();
            foreach (var edge in poly.Points.Select(p => p.EdgeTwo)
                .Union(diagonals.Select(d => new PolygonEdge(poly.Points[d.Item1], poly.Points[d.Item2])))
                .Union(diagonals.Select(d => new PolygonEdge(poly.Points[d.Item2], poly.Points[d.Item1]))))
            {
                if (edge is null)
                    continue;

                if (!edges.ContainsKey(edge.PointOne.Index))
                    edges.Add(edge.PointOne.Index, new List<PolygonEdge>() { edge });
                else
                    edges[edge.PointOne.Index].Add(edge);
            }


            var cnt = 0;
            foreach (var edge in edges)
                cnt += edge.Value.Count;

            var subPolygons = new List<PolygonData>();
            while (edges.Count > 0)
            {
                var currentPoint = edges.First().Value.First().PointOne;
                PolygonEdge? nextEdge = null;
                var subPolygonPoints = new List<PolygonPoint>();
                do
                {
                    subPolygonPoints.Add(currentPoint);
                    var possibleEdges = edges[currentPoint.Index].ToList();
                    nextEdge = BestEdge(nextEdge, possibleEdges);
                    edges[currentPoint.Index].Remove(nextEdge);
                    if (edges[currentPoint.Index].Count == 0)
                        edges.Remove(currentPoint.Index);

                    currentPoint = nextEdge.PointTwo;
                }
                while (subPolygonPoints[0].Index != currentPoint.Index);

                subPolygons.Add(new PolygonData(subPolygonPoints));
            }

            return subPolygons;
        }

        internal static PolygonEdge BestEdge(PolygonEdge? lastEdge, List<PolygonEdge> possibleEdges)
        {
            if ((lastEdge?.PointOne == null && lastEdge?.PointTwo == null) || possibleEdges.Count == 1)
                return possibleEdges.First();

            var bestEdge = possibleEdges[0];
            var bestAngle = (float)Math.PI * 2;
            Vector2D lastVector = lastEdge.PointTwo.Point - lastEdge.PointOne.Point;
            lastVector.Normalize();

            var insideVector = new Vertex2D(-lastVector.Y, lastVector.X);
            foreach (var possibleEdge in possibleEdges)
            {
                Vector2D edgeVector = possibleEdge.PointTwo.Point - possibleEdge.PointOne.Point;
                edgeVector.Normalize();

                var dot = insideVector.X * edgeVector.X + insideVector.Y * edgeVector.Y;
                var cos = lastVector.X * edgeVector.X + lastVector.Y * edgeVector.Y;
                var angle = 0f;
                if ((insideVector.X * edgeVector.X + insideVector.Y * edgeVector.Y) > 0)
                    angle = (float)Math.PI - (float)Math.Acos(cos);
                else
                    angle = (float)Math.PI + (float)Math.Acos(cos);

                if (angle < bestAngle)
                {
                    bestAngle = angle;
                    bestEdge = possibleEdge;
                }
            }

            return bestEdge;
        }
    }


    internal enum PolygonPointClass : byte
    {
        Start,
        Stop,
        Split,
        Merge,
        Regular
    }


    file class StatusHelper
    {
        internal List<StatusHelperElement> EdgesHelpers { get; set; }


        internal StatusHelper()
        {
            EdgesHelpers = new List<StatusHelperElement>();
        }


        internal void Add(StatusHelperElement element) => EdgesHelpers.Add(element);

        internal void Remove(PolygonEdge edge) => EdgesHelpers.RemoveAll(she => she.Edge == edge);

        internal StatusHelperElement? SearchLeft(PolygonPoint point)
        {
            StatusHelperElement? result = null;
            var dist = double.PositiveInfinity;

            var px = point.X;
            var py = point.Y;
            foreach (var she in EdgesHelpers)
            {
                if (she.MinX > px)
                    continue;

                var xValue = she.Edge.PointOne.X + (py - she.Edge.PointOne.Y) * she.Factor;
                if (xValue <= (px + (float)Constants.Epsilon))
                {
                    var sheDist = px - xValue;
                    if (sheDist < dist)
                    {
                        dist = sheDist;
                        result = she;
                    }
                }
            }

            return result;
        }
    }


    file class StatusHelperElement
    {
        private double _factor;


        internal double Factor => _factor;

        internal PolygonEdge Edge { get; set; }

        internal PolygonPoint Helper { get; set; }


        internal double MinX { get; private set; }


        internal StatusHelperElement(PolygonEdge edge, PolygonPoint point)
        {
            Edge = edge;
            Helper = point;
            Vector2D vector = edge.PointTwo.Point - edge.PointOne.Point;
            _factor = vector.X / vector.Y;
            MinX = Math.Min(edge.PointOne.X, edge.PointTwo.X);
        }
    }


    internal class PolygonPoint : IComparable<PolygonPoint>
    {
        private int _index;
        private Vertex2D _point;
        private PolygonEdge? _edgeOne;
        private PolygonEdge? _edgeTwo;

        internal int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        internal Vertex2D Point
        {
            get => _point;
            set => _point = value;
        }

        internal double X => _point.X;

        internal double Y => _point.Y;

        internal PolygonEdge EdgeOne
        {
            get
            {
                _edgeOne.NotNull();
                return _edgeOne;
            }

            set => _edgeOne = value;
        }

        internal PolygonEdge EdgeTwo
        {
            get
            {
                _edgeTwo.NotNull();
                return _edgeTwo;
            }

            set => _edgeTwo = value;
        }

        internal PolygonPoint? Last
        {
            get
            {
                if (_edgeOne != null && _edgeOne.PointOne != null)
                    return _edgeOne.PointOne;
                else
                    return null;
            }
        }

        internal PolygonPoint? Next
        {
            get
            {
                if (_edgeTwo != null && _edgeTwo.PointTwo != null)
                    return _edgeTwo.PointTwo;
                else
                    return null;
            }
        }


        internal PolygonPoint(Vertex2D p)
        {
            _point = p;
            _index = -1;
        }


        public static bool operator <(PolygonPoint first, PolygonPoint second) => first.CompareTo(second) == 1;
        public static bool operator >(PolygonPoint first, PolygonPoint second) => first.CompareTo(second) == -1;


        internal PolygonPointClass PointClass(bool reverse = false)
        {
            if (Next == null || Last == null)
                throw new Exception("No closed Polygon");

            if (!reverse)
            {
                if (Last < this && Next < this && IsConvexPoint())
                    return PolygonPointClass.Start;
                else if (Last > this && Next > this && IsConvexPoint())
                    return PolygonPointClass.Stop;
                else if (Last < this && Next < this)
                    return PolygonPointClass.Split;
                else if (Last > this && Next > this)
                    return PolygonPointClass.Merge;
                else
                    return PolygonPointClass.Regular;
            }
            else
            {
                if (Last < this && Next < this && IsConvexPoint())
                    return PolygonPointClass.Stop;
                else if (Last > this && Next > this && IsConvexPoint())
                    return PolygonPointClass.Start;
                else if (Last < this && Next < this)
                    return PolygonPointClass.Merge;
                else if (Last > this && Next > this)
                    return PolygonPointClass.Split;
                else
                    return PolygonPointClass.Regular;
            }
        }

        private bool IsConvexPoint()
        {
            if (Next == null || Last == null)
                throw new Exception("No closed Polygon");

            Vector2D vecFromLast = _point - Last.Point;
            vecFromLast.Normalize();

            var vecLeft = new Vertex2D(-vecFromLast.Y, vecFromLast.X);
            Vector2D vecToNext = Next.Point - _point;
            vecToNext.Normalize();

            if ((vecLeft.X * vecToNext.X + vecLeft.Y * vecToNext.Y) >= 0)
                return true;
            else
                return false;
        }

        public override string ToString() => this.Index + " X:" + this.X + " Y:" + this.Y;

        public int CompareTo(PolygonPoint? second)
        {
            if (this == null || second == null)
                return 0;

            if (Y > second.Y || (Y == second.Y && X < second.X))
                return -1;
            else if (Y == second.Y && X == second.X)
                return 0;
            else
                return 1;
        }
    }


    internal class PolygonEdge
    {
        private PolygonPoint _pointOne;
        private PolygonPoint _pointTwo;


        internal PolygonPoint PointOne
        {
            get => _pointOne;
            set => _pointOne = value;
        }

        internal PolygonPoint PointTwo
        {
            get => _pointTwo;
            set => _pointTwo = value;
        }

        internal PolygonEdge? Last
        {
            get
            {
                if (_pointOne != null && _pointOne.EdgeOne != null)
                    return _pointOne.EdgeOne;
                else
                    return null;
            }
        }

        internal PolygonEdge? Next
        {
            get
            {
                if (_pointTwo != null && _pointTwo.EdgeTwo != null)
                    return _pointTwo.EdgeTwo;
                else
                    return null;
            }
        }


        internal PolygonEdge(PolygonPoint one, PolygonPoint two)
        {
            _pointOne = one;
            _pointTwo = two;
        }


        public override string ToString() => "From: {" + _pointOne + "} To: {" + _pointTwo + "}";
    }


    internal class PolygonData
    {
        private List<PolygonPoint> _points;
        private List<List<PolygonPoint>> _holes;
        private int _numBoundaryPoints;


        [NotNull]
        internal List<PolygonPoint> Points
        {
            get => _points;
            set => _points = value;
        }

        internal List<List<PolygonPoint>> Holes => _holes;

        internal bool HasHoles => _holes.Count > 0;



        internal PolygonData(List<Vertex2D> points, List<int>? indices = null)
        {
            _points = new List<PolygonPoint>(points.Select(p => new PolygonPoint(p)));
            _holes = new List<List<PolygonPoint>>();
            _numBoundaryPoints = _points.Count;

            if (indices == null)
            {
                for (int i = 0; i < _points.Count; i++)
                    _points[i].Index = i;
            }
            else
            {
                for (int i = 0; i < _points.Count; i++)
                    _points[i].Index = indices[i];
            }

            var cnt = _points.Count;
            for (int i = 0; i < cnt; i++)
            {
                var lastIdx = (i + cnt - 1) % cnt;
                var edge = new PolygonEdge(_points[lastIdx], _points[i]);
                _points[lastIdx].EdgeTwo = edge;
                _points[i].EdgeOne = edge;
            }
        }

        internal PolygonData(List<PolygonPoint> points)
            : this(points.Select(p => p.Point).ToList(), points.Select(p => p.Index).ToList())
        {
        }


        internal void AddHole(List<Vertex2D> points)
        {
            if (points.Select(p => (p.X, p.Y)).ToList().IsCCW())
                points.Reverse();

            var polyPoints = points.Select(p => new PolygonPoint(p)).ToList();
            if (polyPoints[0].Equals(polyPoints[polyPoints.Count - 1]))
                polyPoints.RemoveAt(polyPoints.Count - 1);

            _holes.Add(polyPoints);

            var cntBefore = _points.Count;
            var pointCount = points.Count;
            _points.AddRange(polyPoints);

            for (int i = cntBefore; i < _points.Count; i++)
                polyPoints[i - cntBefore].Index = i;

            var cnt = _points.Count;
            for (int i = 0; i < pointCount; i++)
            {
                var lastIdx = (i + pointCount - 1) % pointCount;
                var edge = new PolygonEdge(polyPoints[lastIdx], polyPoints[i]);
                polyPoints[lastIdx].EdgeTwo = edge;
                polyPoints[i].EdgeOne = edge;
            }
        }
    }
}