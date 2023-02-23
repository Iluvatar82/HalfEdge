using Framework.Extensions;
using HalfEdge.Enumerations;
using HalfEdge.MeshModifications.Base;
using Models.Base;
using Validation;

namespace HalfEdge.MeshModifications
{
    /// <summary>
    /// <see cref="https://www.cs.cmu.edu/afs/cs/academic/class/15462-s14/www/lec_slides/Subdivision.pdf"/>
    /// </summary>
    public class SubdivideMesh_Modifier : MeshModifyBase
    {
        public SubdivisionType SubdivisionType { get; set; }
        public int Iterations { get; set; }

        protected override void CreateOutputMesh()
        {
            SubdivisionType.NotNull();
            Iterations.Satisfies(it => it > 0);

            switch(SubdivisionType)
            {
                case SubdivisionType.Loop:
                    CreateLoopSubdivision();
                    break;

                case SubdivisionType.ModifiedButterfly:

                    break;

                case SubdivisionType.CatmullClark:
                    
                    break;
            }
        }

        private void CreateLoopSubdivision()
        {
            _inputMesh.Indices.ForEach(indices => indices.HasElementCount(c => c == 3));
            _inputMesh.PolygonCount.Satisfies(c => c == _inputMesh.Indices.Count);

            _outputMesh = _inputMesh with { };
            var currentIteration = 0;
            while(currentIteration++ < Iterations)
            {
                var subdividedMeshVertices = _outputMesh.Vertices.Select(v => v with { HalfEdges = new List<Models.Base.HalfEdge>() }).ToList();
                var existingVerticesCount = subdividedMeshVertices.Count;
                var subdividedMeshIndices = new List<List<int>>();
                var existingEdges = _outputMesh.Edges.ToList();
                for(var idx = 0; idx < existingEdges.Count; idx++)
                {
                    var edge = existingEdges[idx];
                    if (edge.IsBorder)
                        subdividedMeshVertices.Add(Vertex.Average(edge.Start, edge.End));
                    else
                    {
                        edge.Next.NotNull();
                        edge.Opposite.NotNull();
                        edge.Opposite.Next.NotNull();

                        var directVertices = new[] { edge.Start, edge.End };
                        var indirectVertices = new[] { edge.Next.End, edge.Opposite.Next.End };

                        subdividedMeshVertices.Add(
                            new Vertex(
                                new Vertex(directVertices[0], directVertices[1], (ff, fs) => (ff + fs) * .375),
                                new Vertex(indirectVertices[0], indirectVertices[1], (sf, ss) => (sf + ss) * .125),
                            (f, s) => f + s));
                    };
                }


                for (var idx = 0; idx < _outputMesh.PolygonCount; idx++)
                {
                    var indices = _outputMesh.Indices[idx];
                    var polygon = _outputMesh.Polygons[idx];
                    var polygonHalfEdgeOrderInfo = polygon.HalfEdges.SelectMany(h => new[] { (h.Start, h.End), (h.End, h.Start) }).ToList();
                    var polygonEdges = existingEdges.Where(e => polygon.Vertices.Contains(e.Start) && polygon.Vertices.Contains(e.End))
                        .OrderBy(e => polygonHalfEdgeOrderInfo.IndexOf((e.Start, e.End))).ToList();
                    var newIndices = polygonEdges.Select(e => existingEdges.IndexOf(e) + existingVerticesCount).ToList();

                    subdividedMeshIndices.Add(new List<int> { indices[0], newIndices[0], newIndices[2] });
                    subdividedMeshIndices.Add(new List<int> { indices[1], newIndices[1], newIndices[0] });
                    subdividedMeshIndices.Add(new List<int> { indices[2], newIndices[2], newIndices[1] });
                    subdividedMeshIndices.Add(new List<int> { newIndices[0], newIndices[1], newIndices[2] });
                }

                _outputMesh = MeshFactory.CreateMesh(subdividedMeshVertices, subdividedMeshIndices);

                for (var vIdx = 0; vIdx < existingVerticesCount; vIdx++)
                {
                    var vertex = _outputMesh.Vertices[vIdx] with { };
                    if (vertex.IsBorder)
                    {
                        var borderNeighbors = vertex.VertexNeighbors.Where(n => n.IsBorder).ToList();
                        vertex.X = borderNeighbors.Sum(v => v.X) * .125 + vertex.X * .75;
                        vertex.Y = borderNeighbors.Sum(v => v.Y) * .125 + vertex.Y * .75;
                        vertex.Z = borderNeighbors.Sum(v => v.Z) * .125 + vertex.Z * .75;
                        _outputMesh.UpdateVertex(vertex, vIdx);
                    }
                    else
                    {
                        var neighbors = vertex.VertexNeighbors.ToList();
                        var neighborsCount = neighbors.Count;
                        var beta = (neighborsCount) switch { 3 => .1875, > 3 => .375 / neighborsCount, _ => throw new NotImplementedException() };
                        var vertexFactor = 1 - neighborsCount * beta;
                        vertex.X = neighbors.Sum(v => v.X) * beta + vertex.X * vertexFactor;
                        vertex.Y = neighbors.Sum(v => v.Y) * beta + vertex.Y * vertexFactor;
                        vertex.Z = neighbors.Sum(v => v.Z) * beta + vertex.Z * vertexFactor;
                        _outputMesh.UpdateVertex(vertex, vIdx);
                    }
                }
            }
        }
    }
}
