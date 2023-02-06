namespace HalfEdge.Models
{
    public class Mesh<T>
    {
        public List<HalfEdge<T>> HalfEdges { get; set; }
        public List<Polygon<T>> Polygons { get; set; }


        public Mesh()
        {
            HalfEdges = new List<HalfEdge<T>>();
            Polygons = new List<Polygon<T>>();
        }
    }
}
