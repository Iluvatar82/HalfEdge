using Models.Base;

namespace HalfEdge.Tests
{
    [TestFixture]
    public class MeshFactoryTests
    {
        [SetUp]
        public void Setup()
        {
        }


        [Test]
        public void CreateGenerator_Ok()
        {
            var mesh = MeshFactory.CreateMesh(new List<Vertex>(), new List<List<int>>());
            Assert.Multiple(() =>
            {
                Assert.That(mesh.Vertices, Is.Empty);
                Assert.That(mesh.Indices, Is.Empty);
                Assert.That(mesh.HalfEdges, Is.Empty);
                Assert.That(mesh.Edges, Is.Empty);
            });
        }

        [Test]
        public void CreateGeneratorIndices_Null()
        {
            Assert.Throws<ArgumentNullException>(() => MeshFactory.CreateMesh(new List<Vertex>(), null));
        }

        [Test]
        public void CreateGenerator_WithVertices()
        {
            var vertices = new List<Vertex> { new Vertex(0, 0, 0), new Vertex(1, 1, 1), new Vertex(0, 1, 1) };
            var mesh = MeshFactory.CreateMesh(vertices, new List<List<int>>());
            Assert.Multiple(() =>
            {
                Assert.That(mesh.Vertices, Is.Not.Empty);
                Assert.That(mesh.Vertices, Has.Count.EqualTo(3));
                Assert.That(mesh.Indices, Is.Empty);
                Assert.That(mesh.HalfEdges, Is.Empty);
                Assert.That(mesh.Edges, Is.Empty);
            });
        }

        [Test]
        public void CreateGenerator_WithOnePolygon()
        {
            var vertices = new List<Vertex> { new Vertex(0, 0, 0), new Vertex(1, 1, 1), new Vertex(0, 1, 1) };
            var indices = new List<List<int>> { new List<int> { 0, 1, 2 } };
            var mesh = MeshFactory.CreateMesh(vertices, indices);
            Assert.Multiple(() =>
            {
                Assert.That(mesh.Vertices, Is.Not.Empty);
                Assert.That(mesh.Vertices, Has.Count.EqualTo(3));
                Assert.That(mesh.Indices, Is.Not.Empty);
                Assert.That(mesh.Indices, Has.Count.EqualTo(1));
                Assert.That(mesh.HalfEdges, Is.Not.Empty);
                Assert.That(mesh.HalfEdges, Has.Count.EqualTo(3));
                Assert.That(mesh.HalfEdges.All(h => h.IsBorder), Is.True);
                Assert.That(mesh.EdgeCount, Is.EqualTo(3));
                Assert.That(mesh.Edges.ToList(), Has.Count.EqualTo(3));
                Assert.That(mesh.Polygons, Is.Not.Empty);
                Assert.That(mesh.Polygons, Has.Count.EqualTo(1));
                Assert.That(mesh.Polygons.All(p => p.IsBorder), Is.True);
                Assert.That(mesh.PolygonCount, Is.EqualTo(1));
                Assert.That(mesh.Polygons[0].Neighbors.ToList(), Has.Count.EqualTo(0));
                Assert.That(mesh.IsOpenMesh, Is.True);
                Assert.That(mesh.Borders.ToList(), Has.Count.EqualTo(1));
            });
        }

        [Test]
        public void CreateGenerator_WithTwoPolygons()
        {
            var vertices = new List<Vertex> { new Vertex(0, 0, 0), new Vertex(1, 0, 0), new Vertex(1, 1, 1), new Vertex(0, 1, 0) };
            var indices = new List<List<int>> { new List<int> { 0, 1, 2 }, new List<int> { 0, 2, 3 } };
            var mesh = MeshFactory.CreateMesh(vertices, indices);
            Assert.Multiple(() =>
            {
                Assert.That(mesh.Vertices, Is.Not.Empty);
                Assert.That(mesh.Vertices, Has.Count.EqualTo(4));
                Assert.That(mesh.Indices, Is.Not.Empty);
                Assert.That(mesh.Indices, Has.Count.EqualTo(2));
                Assert.That(mesh.HalfEdges, Is.Not.Empty);
                Assert.That(mesh.HalfEdges, Has.Count.EqualTo(6));
                Assert.That(mesh.EdgeCount, Is.EqualTo(5));
                Assert.That(mesh.Edges.ToList(), Has.Count.EqualTo(5));
                Assert.That(mesh.Polygons, Is.Not.Empty);
                Assert.That(mesh.Polygons, Has.Count.EqualTo(2));
                Assert.That(mesh.Polygons.All(p => p.IsBorder), Is.True);
                Assert.That(mesh.PolygonCount, Is.EqualTo(2));
                Assert.That(mesh.Polygons[0].Neighbors.ToList(), Has.Count.EqualTo(1));
                Assert.That(mesh.Polygons[1].Neighbors.ToList(), Has.Count.EqualTo(1));
                Assert.That(mesh.HalfEdges.Count(h => h.Opposite is not null), Is.EqualTo(2));
                Assert.That(mesh.HalfEdges.Count(h => h.IsBorder), Is.EqualTo(4));
                Assert.That(mesh.IsOpenMesh, Is.True);
                Assert.That(mesh.Borders.ToList(), Has.Count.EqualTo(1));
            });
        }


        [Test]
        public void CreateGenerator_WithClosedMesh()
        {
            var vertices = new List<Vertex> { new Vertex(0, 0, 0), new Vertex(1, 0, 0), new Vertex(1, 1, 0), new Vertex(1, 1, 1) };
            var indices = new List<List<int>> { new List<int> { 2, 1, 0 }, new List<int> { 0, 1, 3 }, new List<int> { 1, 2, 3 }, new List<int> { 2, 0, 3 } };
            var mesh = MeshFactory.CreateMesh(vertices, indices);
            Assert.Multiple(() =>
            {
                Assert.That(mesh.Vertices, Is.Not.Empty);
                Assert.That(mesh.Vertices, Has.Count.EqualTo(4));
                Assert.That(mesh.Indices, Is.Not.Empty);
                Assert.That(mesh.Indices, Has.Count.EqualTo(4));
                Assert.That(mesh.HalfEdges, Is.Not.Empty);
                Assert.That(mesh.HalfEdges, Has.Count.EqualTo(12));
                Assert.That(mesh.EdgeCount, Is.EqualTo(6));
                Assert.That(mesh.Edges.ToList(), Has.Count.EqualTo(6));
                Assert.That(mesh.Polygons, Is.Not.Empty);
                Assert.That(mesh.Polygons, Has.Count.EqualTo(4));
                Assert.That(mesh.Polygons.All(p => p.IsBorder), Is.False);
                Assert.That(mesh.PolygonCount, Is.EqualTo(4));
                Assert.That(mesh.Polygons[0].Neighbors.ToList(), Has.Count.EqualTo(3));
                Assert.That(mesh.Polygons[1].Neighbors.ToList(), Has.Count.EqualTo(3));
                Assert.That(mesh.Polygons[2].Neighbors.ToList(), Has.Count.EqualTo(3));
                Assert.That(mesh.Polygons[3].Neighbors.ToList(), Has.Count.EqualTo(3));
                Assert.That(mesh.HalfEdges.Count(h => h.Opposite is null), Is.EqualTo(0));
                Assert.That(mesh.HalfEdges.Count(h => h.IsBorder), Is.EqualTo(0));
                Assert.That(mesh.IsOpenMesh, Is.False);
                Assert.That(mesh.Borders.ToList(), Has.Count.EqualTo(0));
                Assert.That(mesh.Vertices.Select(v => v.Polygons.ToList().Count), Has.All.EqualTo(3));
                Assert.That(mesh.Vertices.Select(v => v.VertexNeighbors.ToList().Count), Has.All.EqualTo(3));
            });
        }

        [Test]
        public void CreateGenerator_Add_Polygon_Ok()
        {
            var vertices = new List<Vertex> { new Vertex(0, 0, 0), new Vertex(1, 1, 1), new Vertex(0, 1, 1) };
            var mesh = MeshFactory.CreateMesh(vertices, new List<List<int>>());
            MeshFactory.AddPolygonToMesh(mesh, new List<int> { 0, 1, 2 }, true);
            Assert.Multiple(() =>
            {
                Assert.That(mesh.Vertices, Is.Not.Empty);
                Assert.That(mesh.Vertices, Has.Count.EqualTo(3));
                Assert.That(mesh.Indices, Is.Not.Empty);
                Assert.That(mesh.Indices, Has.Count.EqualTo(1));
                Assert.That(mesh.HalfEdges, Is.Not.Empty);
                Assert.That(mesh.HalfEdges, Has.Count.EqualTo(3));
            });
        }

        [Test]
        public void CreateGenerator_Add_Polygon_ByVertices_Ok()
        {
            var vertices = new List<Vertex> { new Vertex(0, 0, 0), new Vertex(3, 3, 0), new Vertex(0, 6, 0) };
            var mesh = MeshFactory.CreateMesh(vertices, new List<List<int>> { new List<int> { 0, 1, 2 } });
            MeshFactory.AddPolygonToMesh(mesh, new List<Vertex> { new Vertex(4, 0, 4), new Vertex(3, 3, 0), new Vertex(0, 0, 0) });

            Assert.Multiple(() =>
            {
                Assert.That(mesh.Vertices, Is.Not.Empty);
                Assert.That(mesh.Vertices, Has.Count.EqualTo(4));
                Assert.That(mesh.Indices, Is.Not.Empty);
                Assert.That(mesh.Indices, Has.Count.EqualTo(2));
                Assert.That(mesh.HalfEdges, Is.Not.Empty);
                Assert.That(mesh.HalfEdges, Has.Count.EqualTo(6));
                Assert.That(mesh.EdgeCount, Is.EqualTo(5));
                Assert.That(mesh.Edges.ToList(), Has.Count.EqualTo(5));
                Assert.That(mesh.Polygons, Is.Not.Empty);
                Assert.That(mesh.Polygons, Has.Count.EqualTo(2));
                Assert.That(mesh.PolygonCount, Is.EqualTo(2));
                Assert.That(mesh.HalfEdges.Count(h => h.IsBorder), Is.EqualTo(4));
                Assert.That(mesh.IsOpenMesh, Is.True);
                Assert.That(mesh.Borders.ToList(), Has.Count.EqualTo(1));
            });
        }

        [Test]
        public void CreateGenerator_WithTwoPolygons_RemovePolygon()
        {
            var vertices = new List<Vertex> { new Vertex(0, 0, 0), new Vertex(1, 0, 0), new Vertex(1, 1, 0), new Vertex(0, 1, 0) };
            var indices = new List<List<int>> { new List<int> { 0, 1, 2 }, new List<int> { 0, 2, 3 } };
            var mesh = MeshFactory.CreateMesh(vertices, indices);
            MeshFactory.RemovePolygonFromMesh(mesh, mesh.Polygons.Last());
            Assert.Multiple(() =>
            {
                Assert.That(mesh.Vertices, Is.Not.Empty);
                Assert.That(mesh.Vertices, Has.Count.EqualTo(4));
                Assert.That(mesh.Indices, Is.Not.Empty);
                Assert.That(mesh.Indices, Has.Count.EqualTo(1));
                Assert.That(mesh.HalfEdges, Is.Not.Empty);
                Assert.That(mesh.HalfEdges, Has.Count.EqualTo(3));
                Assert.That(mesh.EdgeCount, Is.EqualTo(3));
                Assert.That(mesh.Edges.ToList(), Has.Count.EqualTo(3));
                Assert.That(mesh.Polygons, Is.Not.Empty);
                Assert.That(mesh.Polygons, Has.Count.EqualTo(1));
                Assert.That(mesh.Polygons.All(p => p.IsBorder), Is.True);
                Assert.That(mesh.PolygonCount, Is.EqualTo(1));
                Assert.That(mesh.Polygons[0].Neighbors, Is.Empty);
                Assert.That(mesh.HalfEdges.Where(h => h.Opposite is not null), Is.Empty);
                Assert.That(mesh.HalfEdges.Count(h => h.IsBorder), Is.EqualTo(3));
                Assert.That(mesh.IsOpenMesh, Is.True);
                Assert.That(mesh.Borders.ToList(), Has.Count.EqualTo(1));
            });
        }
    }
}
