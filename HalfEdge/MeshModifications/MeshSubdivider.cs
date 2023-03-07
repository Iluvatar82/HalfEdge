using Framework.Extensions;
using HalfEdge.Enumerations;
using HalfEdge.MeshModifications.Base;
using Models;
using Models.Base;
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
            _inputMesh.PolygonCount.Satisfies(c => c == _inputMesh.IndicesCount);

            _outputMesh = _inputMesh with { };
            var currentIteration = 0;
            while (currentIteration++ < Iterations)
            {
                var existingVertexCount = _outputMesh.VertexCount;
                var existingPolygonCount = _outputMesh.PolygonCount;
                var existingEdges = _outputMesh.Edges.ToList();
                var existingPolygons = _outputMesh.Polygons.ToList();
                var halfEdgeIndexInformation = new Dictionary<(Vertex Start, Vertex End), int>(existingEdges.Count * 2);
                var subdividedMeshVertices = new Vertex[_outputMesh.VertexCount + existingEdges.Count + existingPolygons.Count];
                var subdividedMeshIndices = new List<int>[_outputMesh.HalfEdgeCount];
                var vertexEdgeMidpoints = _outputMesh.Vertices.Select(v => Vertex.Average(v.VertexNeighbors.Select(n => (n + v) / 2))).ToArray();

                _outputMesh.Vertices.ForEach((v, i) => subdividedMeshVertices[i] = v with { HalfEdges = new List<Models.Base.HalfEdge>() });

                var polygonIndexInformation = new Dictionary<Polygon, int>(existingPolygons.Count);
                var newPolygonIndices = _outputMesh.Polygons.Select(p => p.HalfEdges.Count).ToArray();

                for (var idx = 0; idx < existingPolygons.Count; idx++)
                {
                    var polygon = existingPolygons[idx];
                    polygonIndexInformation.Add(polygon, idx + existingVertexCount);
                }

                for (var idx = 0; idx < existingEdges.Count; idx++)
                {
                    halfEdgeIndexInformation.Add((existingEdges[idx].Start, existingEdges[idx].End), idx + existingVertexCount + existingPolygonCount);
                    halfEdgeIndexInformation.Add((existingEdges[idx].End, existingEdges[idx].Start), idx + existingVertexCount + existingPolygonCount);
                }

                //Parallel.For(0, existingPolygons.Count, (idx) =>
                for (var idx = 0; idx < existingPolygons.Count; idx++)
                {
                    subdividedMeshVertices[existingVertexCount + idx] = Vertex.Average(existingPolygons[idx].Vertices);
                }
                //);

                //Parallel.For(0, existingEdges.Count, (idx) =>
                for (var idx = 0; idx < existingEdges.Count; idx++)
                {
                    var edge = existingEdges[idx];
                    if (edge.IsBorder)
                        subdividedMeshVertices[existingVertexCount + existingPolygons.Count + idx] = Vertex.Average(edge.Start, edge.End);
                    else
                    {
                        edge.Polygon.NotNull();
                        edge.Opposite.NotNull();
                        edge.Opposite.Polygon.NotNull();

                        var directVertices = new[] { edge.Start, edge.End };
                        var indirectVertices = new[] { subdividedMeshVertices[polygonIndexInformation[edge.Polygon]], subdividedMeshVertices[polygonIndexInformation[edge.Opposite.Polygon]] };

                        subdividedMeshVertices[existingVertexCount + existingPolygons.Count + idx] = (directVertices[0] + directVertices[1]) * .375 + (indirectVertices[0] + indirectVertices[1]) * .125;
                    }
                }
                //);

                //Parallel.For(0, existingPolygonCount, (idx) =>
                for (var idx = 0; idx < existingPolygonCount; idx++)
                {
                    var polygonFirstIndex = currentIteration == 1 ? newPolygonIndices[..idx].Sum() : idx * 4;
                    var indices = _outputMesh.GetIndices(idx);
                    var polygon = _outputMesh.GetPolygon(idx);
                    var newPolygonIndex = polygonIndexInformation[polygon];
                    var newEdgeIndices = polygon.HalfEdges.Select(h => halfEdgeIndexInformation[(h.Start, h.End)]).ToList();

                    for (var edgeIndex = 0; edgeIndex < newEdgeIndices.Count; edgeIndex++)
                    {
                        var newEdgeIndexStart = newEdgeIndices[edgeIndex];
                        var newEdgeIndexEnd = newEdgeIndices[(edgeIndex + newEdgeIndices.Count - 1) % newEdgeIndices.Count];
                        subdividedMeshIndices[polygonFirstIndex + edgeIndex] = new List<int> { indices[edgeIndex], newEdgeIndexStart, newPolygonIndex, newEdgeIndexEnd };
                    }
                }
                //);

                _outputMesh = MeshFactory.CreateMesh(subdividedMeshVertices, subdividedMeshIndices);

                //Parallel.For(0, existingVertexCount, (vIdx) =>
                for (var vIdx = 0; vIdx < existingVertexCount; vIdx++)
                {
                    var vertex = _outputMesh.GetVertex(vIdx);
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
                //);
            }
        }

        private void CreateModifiedButterflySubdivision()
        {
            _inputMesh.Indices.ForEach(indices => indices.HasElementCount(c => c == 3));
            _inputMesh.PolygonCount.Satisfies(c => c == _inputMesh.IndicesCount);

            _outputMesh = _inputMesh with { };
            var currentIteration = 0;
            while (currentIteration++ < Iterations)
            {
                var existingVertexCount = _outputMesh.VertexCount;
                var existingEdges = _outputMesh.Edges.ToList();
                var halfEdgeIndexInformation = new Dictionary<(Vertex Start, Vertex End), int>(existingEdges.Count * 2);
                var subdividedMeshVertices = new Vertex[existingVertexCount + existingEdges.Count];
                var subdividedMeshIndices = new List<int>[_outputMesh.PolygonCount * 4];

                _outputMesh.Vertices.ForEach((v, i) => subdividedMeshVertices[i] = v with { HalfEdges = new List<Models.Base.HalfEdge>() });

                for (var idx = 0; idx < existingEdges.Count; idx++)
                {
                    halfEdgeIndexInformation.Add((existingEdges[idx].Start, existingEdges[idx].End), idx + existingVertexCount);
                    halfEdgeIndexInformation.Add((existingEdges[idx].End, existingEdges[idx].Start), idx + existingVertexCount);
                }

                //Parallel.For(0, existingEdges.Count, (idx) =>
                for (var idx = 0; idx < existingEdges.Count; idx++)
                {
                    var edge = existingEdges[idx];
                    var directNeighbors = new[] { edge.Start, edge.End };
                    if (edge.IsBorder)
                    {
                        var indirectNeighbors = directNeighbors.Select(n => n.BorderNeighbors.First(bn => !directNeighbors.Contains(bn))).ToList();
                        subdividedMeshVertices[existingVertexCount + idx] = Vertex.Sum(directNeighbors) * .5625 +
                                                                            Vertex.Sum(indirectNeighbors) * -.0625;
                    }
                    else
                    {
                        var directWeight = .5;
                        var butterflyWeight = .0625;

                        var oppositeEdge = edge.Opposite;
                        oppositeEdge.NotNull();
                        edge.Next.NotNull();
                        edge.Previous.NotNull();
                        oppositeEdge.Next.NotNull();
                        oppositeEdge.Previous.NotNull();

                        var indirectNeighbors = new[] { edge.Next.End, oppositeEdge.Next.End };

                        subdividedMeshVertices[existingVertexCount + idx] = Vertex.Sum(directNeighbors) * directWeight;
                        if (!edge.Next.IsBorder)
                        {
                            edge.Next.Opposite.NotNull();
                            edge.Next.Opposite.Next.NotNull();
                            subdividedMeshVertices[existingVertexCount + idx] += indirectNeighbors[0] * butterflyWeight - edge.Next.Opposite.Next.End * butterflyWeight;
                        }

                        if (!edge.Previous.IsBorder)
                        {
                            edge.Previous.Opposite.NotNull();
                            edge.Previous.Opposite.Next.NotNull();
                            subdividedMeshVertices[existingVertexCount + idx] += indirectNeighbors[0] * butterflyWeight - edge.Previous.Opposite.Next.End * butterflyWeight;
                        }

                        if (!oppositeEdge.Next.IsBorder)
                        {
                            oppositeEdge.Next.Opposite.NotNull();
                            oppositeEdge.Next.Opposite.Next.NotNull();
                            subdividedMeshVertices[existingVertexCount + idx] += indirectNeighbors[1] * butterflyWeight - oppositeEdge.Next.Opposite.Next.End * butterflyWeight;
                        }

                        if (!oppositeEdge.Previous.IsBorder)
                        {
                            oppositeEdge.Previous.Opposite.NotNull();
                            oppositeEdge.Previous.Opposite.Next.NotNull();
                            subdividedMeshVertices[existingVertexCount + idx] += indirectNeighbors[1] * butterflyWeight - oppositeEdge.Previous.Opposite.Next.End * butterflyWeight;
                        }
                    }
                }
                //);

                //Parallel.For(0, _outputMesh.PolygonCount, (idx) =>
                for (var idx = 0; idx < _outputMesh.PolygonCount; idx++)
                {
                    var indices = _outputMesh.GetIndices(idx);
                    var polygon = _outputMesh.GetPolygon(idx);
                    var newIndices = polygon.HalfEdges.Select(h => halfEdgeIndexInformation[(h.Start, h.End)]).ToList();

                    var triangleIndices = new List<int> { indices[0], newIndices[0], newIndices[2] };
                    subdividedMeshIndices[idx * 4] = triangleIndices;

                    triangleIndices = new List<int> { indices[1], newIndices[1], newIndices[0] };
                    subdividedMeshIndices[idx * 4 + 1] = triangleIndices;

                    triangleIndices = new List<int> { indices[2], newIndices[2], newIndices[1] };
                    subdividedMeshIndices[idx * 4 + 2] = triangleIndices;

                    triangleIndices = new List<int> { newIndices[0], newIndices[1], newIndices[2] };
                    subdividedMeshIndices[idx * 4 + 3] = triangleIndices;
                }
                //);

                _outputMesh = MeshFactory.CreateMesh(subdividedMeshVertices, subdividedMeshIndices);
            }
        }

        private void CreateLoopSubdivision()
        {
            _inputMesh.Indices.ForEach(indices => indices.HasElementCount(c => c == 3));
            _inputMesh.PolygonCount.Satisfies(c => c == _inputMesh.IndicesCount);

            _outputMesh = _inputMesh with { };
            var currentIteration = 0;
            while (currentIteration++ < Iterations)
            {
                var existingVertexCount = _outputMesh.VertexCount;
                var existingEdges = _outputMesh.Edges.ToList();
                var halfEdgeIndexInformation = new Dictionary<(Vertex Start, Vertex End), int>(existingEdges.Count * 2);
                var subdividedMeshVertices = new Vertex[existingVertexCount + existingEdges.Count];
                var subdividedMeshIndices = new List<int>[_outputMesh.PolygonCount * 4];

                _outputMesh.Vertices.ForEach((v, i) => subdividedMeshVertices[i] = v with { HalfEdges = new List<Models.Base.HalfEdge>() });

                for (var idx = 0; idx < existingEdges.Count; idx++)
                {
                    halfEdgeIndexInformation.Add((existingEdges[idx].Start, existingEdges[idx].End), idx + existingVertexCount);
                    halfEdgeIndexInformation.Add((existingEdges[idx].End, existingEdges[idx].Start), idx + existingVertexCount);
                }

                //Parallel.For(0, existingEdges.Count, (idx) =>
                for (var idx = 0; idx < existingEdges.Count; idx++)
                {
                    var edge = existingEdges[idx];
                    if (edge.IsBorder)
                        subdividedMeshVertices[existingVertexCount + idx] = Vertex.Average(edge.Start, edge.End);
                    else
                    {
                        edge.Next.NotNull();
                        edge.Opposite.NotNull();
                        edge.Opposite.Next.NotNull();

                        var directVertices = new[] { edge.Start, edge.End };
                        var indirectVertices = new[] { edge.Next.End, edge.Opposite.Next.End };

                        subdividedMeshVertices[existingVertexCount + idx] = (directVertices[0] + directVertices[1]) * .375 + (indirectVertices[0] + indirectVertices[1]) * .125;
                    };
                }
                //);

                //Parallel.For(0, _outputMesh.PolygonCount, (idx) =>
                for (var idx = 0; idx < _outputMesh.PolygonCount; idx++)
                {
                    var indices = _outputMesh.GetIndices(idx);
                    var polygon = _outputMesh.GetPolygon(idx);
                    var newIndices = polygon.HalfEdges.Select(h => halfEdgeIndexInformation[(h.Start, h.End)]).ToList();

                    var triangleIndices = new List<int> { indices[0], newIndices[0], newIndices[2] };
                    subdividedMeshIndices[idx * 4] = triangleIndices;

                    triangleIndices = new List<int> { indices[1], newIndices[1], newIndices[0] };
                    subdividedMeshIndices[idx * 4 + 1] = triangleIndices;

                    triangleIndices = new List<int> { indices[2], newIndices[2], newIndices[1] };
                    subdividedMeshIndices[idx * 4 + 2] = triangleIndices;

                    triangleIndices = new List<int> { newIndices[0], newIndices[1], newIndices[2] };
                    subdividedMeshIndices[idx * 4 + 3] = triangleIndices;
                }
                //);

                _outputMesh = MeshFactory.CreateMesh(subdividedMeshVertices, subdividedMeshIndices);

                //Parallel.For(0, existingVertexCount, (vIdx) =>
                for (var vIdx = 0; vIdx < existingVertexCount; vIdx++)
                {
                    var vertex = _outputMesh.GetVertex(vIdx) with { };
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
                //);
            }
        }
    }
}
