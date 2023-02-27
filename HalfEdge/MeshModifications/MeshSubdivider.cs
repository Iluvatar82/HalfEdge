using Framework.Extensions;
using HalfEdge.Enumerations;
using HalfEdge.MeshModifications.Base;
using Models;
using Models.Base;
using System.Linq;
using Validation;

namespace HalfEdge.MeshModifications
{
    /// <summary>
    /// <see cref="https://www.cs.cmu.edu/afs/cs/academic/class/15462-s14/www/lec_slides/Subdivision.pdf"/>
    /// </summary>
    public class MeshSubdivider : MeshModifyBase
    {
        public SubdivisionType SubdivisionType { get; set; }
        public int Iterations { get; set; }

        protected override void CreateOutputMesh()
        {
            SubdivisionType.NotNull();
            Iterations.Satisfies(it => it > 0);

            switch (SubdivisionType)
            {
                case SubdivisionType.Loop:
                    CreateLoopSubdivision();
                    break;

                case SubdivisionType.ModifiedButterfly:
                    CreateModifiedButterflySubdivision();
                    break;

                case SubdivisionType.CatmullClark:
                    CreateCatmullClarkflySubdivision();
                    break;
            }
        }

        private void CreateCatmullClarkflySubdivision()
        {
            _inputMesh.PolygonCount.Satisfies(c => c == _inputMesh.Indices.Count);

            _outputMesh = _inputMesh with { };
            var currentIteration = 0;
            while (currentIteration++ < Iterations)
            {
                var subdividedMeshVertices = _outputMesh.Vertices.Select(v => v with { HalfEdges = new List<Models.Base.HalfEdge>() }).ToList();
                var vertexEdgeMidpoints = _outputMesh.Vertices.Select(v => Vertex.Average(v.VertexNeighbors)).ToList();
                var existingVerticesCount = subdividedMeshVertices.Count;
                var subdividedMeshIndices = new List<List<int>>();
                var existingPolygons = _outputMesh.Polygons.ToList();
                var existingPolygonCount = existingPolygons.Count;
                var existingEdges = _outputMesh.Edges.ToList();

                var polygonIndexInformation = new Dictionary<Polygon, int>();
                var halfEdgeIndexInformation = new Dictionary<(Vertex Start, Vertex End), int>();
                for (var idx = 0; idx < existingPolygons.Count; idx++)
                {
                    var polygon = existingPolygons[idx];
                    subdividedMeshVertices.Add(Vertex.Average(polygon.Vertices));
                    polygonIndexInformation.Add(polygon, idx + existingVerticesCount);
                }

                for (var idx = 0; idx < existingEdges.Count; idx++)
                {
                    var edge = existingEdges[idx];
                    halfEdgeIndexInformation.Add((edge.Start, edge.End), idx + existingVerticesCount + existingPolygonCount);
                    halfEdgeIndexInformation.Add((edge.End, edge.Start), idx + existingVerticesCount + existingPolygonCount);
                    if (edge.IsBorder)
                        subdividedMeshVertices.Add(Vertex.Average(edge.Start, edge.End));
                    else
                    {
                        edge.Polygon.NotNull();
                        edge.Opposite.NotNull();
                        edge.Opposite.Polygon.NotNull();

                        var directVertices = new[] { edge.Start, edge.End };
                        var indirectVertices = new[] { subdividedMeshVertices[polygonIndexInformation[edge.Polygon]], subdividedMeshVertices[polygonIndexInformation[edge.Opposite.Polygon]] };

                        subdividedMeshVertices.Add((directVertices[0] + directVertices[1]) * .375 + (indirectVertices[0] + indirectVertices[1]) * .125);
                    };
                }

                for (var idx = 0; idx < _outputMesh.PolygonCount; idx++)
                {
                    var indices = _outputMesh.Indices[idx];
                    var polygon = _outputMesh.Polygons[idx];
                    var newPolygonIndex = polygonIndexInformation[polygon];
                    var newEdgeIndices = polygon.HalfEdges.Select(h => halfEdgeIndexInformation[(h.Start, h.End)]).ToList();

                    for(var edgeIndex = 0;  edgeIndex < newEdgeIndices.Count; edgeIndex++)
                    {
                        var newEdgeIndexStart = newEdgeIndices[edgeIndex];
                        var newEdgeIndexEnd = newEdgeIndices[(edgeIndex + newEdgeIndices.Count - 1) % newEdgeIndices.Count];
                        subdividedMeshIndices.Add(new List<int> { indices[edgeIndex], newEdgeIndexStart, newPolygonIndex, newEdgeIndexEnd });
                    }
                }

                _outputMesh = MeshFactory.CreateMesh(subdividedMeshVertices, subdividedMeshIndices);
                
                for (var vIdx = 0; vIdx < existingVerticesCount; vIdx++)
                {
                    var vertex = _outputMesh.Vertices[vIdx];
                    if (vertex.IsBorder)
                    {
                        var borderNeighbors = vertex.VertexNeighbors.Where(n => n.IsBorder).ToList();
                        var newVertex = vertex * .75 + Vertex.Sum(borderNeighbors) * .125;
                        vertex.X = newVertex.X;
                        vertex.Y = newVertex.Y;
                        vertex.Z = newVertex.Z;
                    }
                    else
                    {
                        var newPolygonPoints = vertex.Polygons.Select(p => p.Vertices.ElementAt(2)).ToList();
                        var originalEdgeMidpoint = vertexEdgeMidpoints[vIdx];
                        var newVertex = (Vertex.Average(newPolygonPoints) + 2 * originalEdgeMidpoint + vertex * (newPolygonPoints.Count - 3)) / newPolygonPoints.Count;
                        vertex.X = newVertex.X;
                        vertex.Y = newVertex.Y;
                        vertex.Z = newVertex.Z;
                    }
                }
            }
        }

        private void CreateModifiedButterflySubdivision()
        {
            _inputMesh.Indices.ForEach(indices => indices.HasElementCount(c => c == 4));
            _inputMesh.PolygonCount.Satisfies(c => c == _inputMesh.Indices.Count);

            _outputMesh = _inputMesh with { };
            var currentIteration = 0;
            while (currentIteration++ < Iterations)
            {
                //TODO
            }
        }

        private void CreateLoopSubdivision()
        {
            _inputMesh.Indices.ForEach(indices => indices.HasElementCount(c => c == 3));
            _inputMesh.PolygonCount.Satisfies(c => c == _inputMesh.Indices.Count);

            _outputMesh = _inputMesh with { };
            var currentIteration = 0;
            while (currentIteration++ < Iterations)
            {
                var subdividedMeshVertices = _outputMesh.Vertices.Select(v => v with { HalfEdges = new List<Models.Base.HalfEdge>() }).ToList();
                var existingVerticesCount = subdividedMeshVertices.Count;
                var subdividedMeshIndices = new List<List<int>>();
                var existingEdges = _outputMesh.Edges.ToList();
                var halfEdgeIndexInformation = new Dictionary<(Vertex Start, Vertex End), int>();
                for (var idx = 0; idx < existingEdges.Count; idx++)
                {
                    var edge = existingEdges[idx];
                    halfEdgeIndexInformation.Add((edge.Start, edge.End), idx + existingVerticesCount);
                    halfEdgeIndexInformation.Add((edge.End, edge.Start), idx + existingVerticesCount);
                    if (edge.IsBorder)
                        subdividedMeshVertices.Add(Vertex.Average(edge.Start, edge.End));
                    else
                    {
                        edge.Next.NotNull();
                        edge.Opposite.NotNull();
                        edge.Opposite.Next.NotNull();

                        var directVertices = new[] { edge.Start, edge.End };
                        var indirectVertices = new[] { edge.Next.End, edge.Opposite.Next.End };

                        subdividedMeshVertices.Add((directVertices[0] + directVertices[1]) * .375 + (indirectVertices[0] + indirectVertices[1]) * .125);
                    };
                }

                for (var idx = 0; idx < _outputMesh.PolygonCount; idx++)
                {
                    var indices = _outputMesh.Indices[idx];
                    var polygon = _outputMesh.Polygons[idx];
                    var newIndices = polygon.HalfEdges.Select(h => halfEdgeIndexInformation[(h.Start, h.End)]).ToList();

                    var triangleIndices = new List<int> { indices[0], newIndices[0], newIndices[2] };
                    subdividedMeshIndices.Add(triangleIndices);

                    triangleIndices = new List<int> { indices[1], newIndices[1], newIndices[0] };
                    subdividedMeshIndices.Add(triangleIndices);

                    triangleIndices = new List<int> { indices[2], newIndices[2], newIndices[1] };
                    subdividedMeshIndices.Add(triangleIndices);

                    triangleIndices = new List<int> { newIndices[0], newIndices[1], newIndices[2] };
                    subdividedMeshIndices.Add(triangleIndices);
                }

                _outputMesh = MeshFactory.CreateMesh(subdividedMeshVertices, subdividedMeshIndices);

                for (var vIdx = 0; vIdx < existingVerticesCount; vIdx++)
                {
                    var vertex = _outputMesh.Vertices[vIdx] with { };
                    var triangles = subdividedMeshIndices.Where(il => il.Any(i => i == vIdx));
                    if (vertex.IsBorder)
                    {
                        var neighbors = vertex.VertexNeighbors.Where(n => n.IsBorder).ToList();
                        var newVertex = Vertex.Sum(neighbors) * .125 + vertex * .75;
                        vertex.X = newVertex.X;
                        vertex.Y = newVertex.Y;
                        vertex.Z = newVertex.Z;

                        _outputMesh.UpdateVertex(vertex, vIdx);
                    }
                    else
                    {
                        var neighbors = vertex.VertexNeighbors.ToList();
                        var neighborsCount = neighbors.Count;
                        var beta = (neighborsCount) switch { 3 => .1875, > 3 => .375 / neighborsCount, _ => throw new NotImplementedException() };
                        var vertexFactor = 1 - neighborsCount * beta;
                        var newVertex = Vertex.Sum(neighbors) * beta + vertex * vertexFactor;
                        vertex.X = newVertex.X;
                        vertex.Y = newVertex.Y;
                        vertex.Z = newVertex.Z;

                        _outputMesh.UpdateVertex(vertex, vIdx);
                    }
                }
            }
        }
    }
}
