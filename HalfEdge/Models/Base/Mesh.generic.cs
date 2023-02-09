using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HalfEdge.Models.Base
{
    public class Mesh<T> where T : struct
    {
        protected List<HalfEdge<T>> _halfEdges;
        protected List<Polygon<T>> _polygons;

        public ReadOnlyCollection<HalfEdge<T>> HalfEdges { get => _halfEdges.AsReadOnly<HalfEdge<T>>(); }
        public ReadOnlyCollection<Polygon<T>> Polygons { get => _polygons.AsReadOnly<Polygon<T>>(); }
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
                        currentHalfEdge = currentHalfEdge.End.HalfEdges.Single(h => h != currentHalfEdge && h.Opposite is null);
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
            _halfEdges = new List<HalfEdge<T>>();
            _polygons = new List<Polygon<T>>();
        }


        public void AddHalfEdges(IEnumerable<HalfEdge<T>> halfEdges) => _halfEdges.AddRange(halfEdges);

        public virtual void AddPolygon(Polygon<T> polygon) => _polygons.Add(polygon);

        public virtual void AddPolygons(IEnumerable<Polygon<T>> polygons) => _polygons.AddRange(polygons);
    }
}
