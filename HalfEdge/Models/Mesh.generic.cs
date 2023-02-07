namespace HalfEdge.Models
{
    public class Mesh<T>
    {
        private List<HalfEdge<T>> _halfEdges;
        private List<Polygon<T>> _polygons;

        public List<HalfEdge<T>> HalfEdges { get => _halfEdges; init => _halfEdges = value; }
        public List<Polygon<T>> Polygons { get => _polygons; init => _polygons = value; }
        public IEnumerable<List<HalfEdge<T>>> Borders
        {
            get
            {
                var allBorderHalfEdges = _halfEdges.Where(h => h.Opposite is null).ToList();
                if (!allBorderHalfEdges.Any())
                    yield break;

                while (allBorderHalfEdges.Any())
                {
                    var borderLoop = new List<HalfEdge<T>>();
                    var currentHalfEdge = allBorderHalfEdges.First();
                    allBorderHalfEdges.Remove(currentHalfEdge);
                    while (true)
                    {
                        borderLoop.Add(currentHalfEdge);
                        currentHalfEdge = currentHalfEdge.End.OutHalfEdges.Single(h => h != currentHalfEdge && h.Opposite is null);
                        allBorderHalfEdges.Remove(currentHalfEdge);
                        if (borderLoop[0] == currentHalfEdge)
                            break;
                    }

                    yield return borderLoop;
                }
            }
        }
        public bool IsOpenMesh => Borders.Any();


        public Mesh()
        {
            HalfEdges = new List<HalfEdge<T>>();
            Polygons = new List<Polygon<T>>();
        }
    }
}
