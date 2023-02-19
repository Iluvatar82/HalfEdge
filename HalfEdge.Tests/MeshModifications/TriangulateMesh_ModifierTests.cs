using HalfEdge.MeshModifications;
using Models.Base;

namespace HalfEdge.Tests.MeshModifications
{
    [TestFixture]
    public class TriangulateMesh_ModifierTests
    {
        [SetUp] public void SetUp()
        {
        }

        [Test]
        public void CreateOutputMesh_Ok()
        {
            var mesh = MeshFactory.CreateMesh(new List<Vertex> {
                new Vertex(0, 0, 0), new Vertex(1, 0, 0), new Vertex(1, 1, 0), new Vertex(0, 1, 0),
                new Vertex(0, 0, 1), new Vertex(1, 0, 1), new Vertex(1, 1, 1), new Vertex(0, 1, 1) },
                /*new List<List<int>> {
                    new List<int> { 0, 2, 1 }, new List<int> { 0, 3, 2 },
                    new List<int> { 0, 1, 5 }, new List<int> { 0, 5, 4 },
                    new List<int> { 1, 2, 6 }, new List<int> { 1, 6, 5 },
                    new List<int> { 2, 3, 7 }, new List<int> { 2, 7, 6 },
                    new List<int> { 3, 0, 4 }, new List<int> { 3, 4, 7 },
                    new List<int> { 4, 5, 6 }, new List<int> { 4, 6, 7 }
                }*/new List<List<int>> {
                       new List<int> { 0, 3, 2, 1 },
                       new List<int> { 0, 1, 5, 4 }, new List<int> { 1, 2, 6, 5 }, new List<int> { 2, 3, 7, 6 }, new List<int> { 3, 0, 4, 7 },
                       new List<int> { 4, 5, 6, 7 }
                });

            /*Assert.Multiple(() =>
            {
                Assert.That(mesh.Vertices, Has.Count.EqualTo(8));
                Assert.That(mesh.Indices, Has.Count.EqualTo(6));
                Assert.That(mesh.HalfEdges, Has.Count.EqualTo(24));
                Assert.That(mesh.HalfEdges.All(h => h.IsBorder), Is.False);
                Assert.That(mesh.Polygons, Has.Count.EqualTo(6));
                Assert.That(mesh.Polygons.All(p => p.IsBorder), Is.False);
                Assert.That(mesh.PolygonCount, Is.EqualTo(6));
                Assert.That(mesh.IsOpenMesh, Is.False);
                Assert.That(mesh.Borders, Is.Empty);
            });*/

            var meshModifier = new TriangulateMesh_Modifier();
            meshModifier.Modify(mesh);
            var triangleMesh = meshModifier.OutputMesh;

            Assert.Multiple(() =>
            {
                Assert.That(triangleMesh.Vertices, Has.Count.EqualTo(8));
                Assert.That(triangleMesh.Indices, Has.Count.EqualTo(12));
                Assert.That(triangleMesh.HalfEdges, Has.Count.EqualTo(36));
                Assert.That(triangleMesh.HalfEdges.All(h => h.IsBorder), Is.False);
                Assert.That(triangleMesh.Polygons, Has.Count.EqualTo(12));
                Assert.That(triangleMesh.Polygons.All(p => p.IsBorder), Is.False);
                Assert.That(triangleMesh.Polygons.All(p => p.HalfEdges.Count == 3), Is.True);
                Assert.That(triangleMesh.PolygonCount, Is.EqualTo(12));
                Assert.That(triangleMesh.IsOpenMesh, Is.False);
                Assert.That(triangleMesh.Borders, Is.Empty);
            });
        }
    }
}
