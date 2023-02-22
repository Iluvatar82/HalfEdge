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
                for(var idx = 0; idx < _outputMesh.PolygonCount; idx++)
                {
                    var indices = _outputMesh.Indices[idx];
                    var polygon = _outputMesh.Polygons[idx];

                    //TODO NEW!!!! (to avoid odd vertices created multiple times!)
                    //Add "Number of Edges" new (odd) vertices "in the right position"
                    //Move the existing "event vertices according to the rules
                    //calculate the new triangle Indices
                    //Create the mesh


                    /*var firstNewIndex = subdividedMeshVertices.Count;
                    if (polygon.HalfEdges[0].IsBorder)
                        subdividedMeshVertices.Add(Vertex.Average(polygon.HalfEdges[0].Start, polygon.HalfEdges[0].End));
                    else
                    {
                        var relevantVertices = polygon.HalfEdges[0].Polygon?.Vertices ?? Enumerable.Empty<Vertex>();
                        relevantVertices = relevantVertices.Union(polygon.HalfEdges[0].Opposite?.Polygon?.Vertices ?? Enumerable.Empty<Vertex>()).ToList();
                        var directVertices = relevantVertices.Where(v => v == polygon.HalfEdges[0].Start || v == polygon.HalfEdges[0].End).ToList();
                        var indirectVertices = relevantVertices.Where(v => v != polygon.HalfEdges[0].Start && v != polygon.HalfEdges[0].End).ToList();

                        subdividedMeshVertices.Add(
                            new Vertex(
                                new Vertex(directVertices[0], directVertices[1], (ff, fs) => (ff + fs) * .375),
                                new Vertex(indirectVertices[0], indirectVertices[1], (sf, ss) => (sf + ss) * .125),
                            (f, s) => f + s));
                    };

                    if (polygon.HalfEdges[1].IsBorder)
                        subdividedMeshVertices.Add(Vertex.Average(polygon.HalfEdges[1].Start, polygon.HalfEdges[1].End));
                    else
                    {
                        var relevantVertices = polygon.HalfEdges[1].Polygon?.Vertices ?? Enumerable.Empty<Vertex>();
                        relevantVertices = relevantVertices.Union(polygon.HalfEdges[1].Opposite?.Polygon?.Vertices ?? Enumerable.Empty<Vertex>()).ToList();
                        var directVertices = relevantVertices.Where(v => v == polygon.HalfEdges[1].Start || v == polygon.HalfEdges[1].End).ToList();
                        var indirectVertices = relevantVertices.Where(v => v != polygon.HalfEdges[1].Start && v != polygon.HalfEdges[1].End).ToList();

                        subdividedMeshVertices.Add(
                            new Vertex(
                                new Vertex(directVertices[0], directVertices[1], (ff, fs) => (ff + fs) * .375),
                                new Vertex(indirectVertices[0], indirectVertices[1], (sf, ss) => (sf + ss) * .125),
                            (f, s) => f + s));
                    };

                    if (polygon.HalfEdges[2].IsBorder)
                        subdividedMeshVertices.Add(Vertex.Average(polygon.HalfEdges[2].Start, polygon.HalfEdges[2].End));
                    else
                    {
                        var relevantVertices = polygon.HalfEdges[2].Polygon?.Vertices ?? Enumerable.Empty<Vertex>();
                        relevantVertices = relevantVertices.Union(polygon.HalfEdges[2].Opposite?.Polygon?.Vertices ?? Enumerable.Empty<Vertex>()).ToList();
                        var directVertices = relevantVertices.Where(v => v == polygon.HalfEdges[2].Start || v == polygon.HalfEdges[2].End).ToList();
                        var indirectVertices = relevantVertices.Where(v => v != polygon.HalfEdges[2].Start && v != polygon.HalfEdges[2].End).ToList();

                        subdividedMeshVertices.Add(
                            new Vertex(
                                new Vertex(directVertices[0], directVertices[1], (ff, fs) => (ff + fs) * .375),
                                new Vertex(indirectVertices[0], indirectVertices[1], (sf, ss) => (sf + ss) * .125),
                            (f, s) => f + s));
                    };

                    subdividedMeshIndices.Add(new List<int> { indices[0], firstNewIndex, firstNewIndex + 2 });
                    subdividedMeshIndices.Add(new List<int> { indices[1], firstNewIndex + 1, firstNewIndex });
                    subdividedMeshIndices.Add(new List<int> { indices[2], firstNewIndex + 2, firstNewIndex + 1 });
                    subdividedMeshIndices.Add(new List<int> { firstNewIndex, firstNewIndex + 1, firstNewIndex + 2 });*/
                }

                _outputMesh = MeshFactory.CreateMesh(subdividedMeshVertices, subdividedMeshIndices);

                /*//Modify even Vertices
                for (var vIdx = 0; vIdx < existingVerticesCount; vIdx++)
                {
                    var vertex = _outputMesh.Vertices[vIdx] with { };
                    if (vertex.IsBorder)
                    {
                        var borderNeighbors = vertex.VertexNeighbors.Where(n => n.IsBorder).ToList();
                        vertex.X = borderNeighbors.Sum(v => v.X) * .25 + vertex.X * .75;
                        vertex.Y = borderNeighbors.Sum(v => v.Y) * .25 + vertex.Y * .75;
                        vertex.Z = borderNeighbors.Sum(v => v.Z) * .25 + vertex.Z * .75;
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
                }*/
            }
        }
    }
}
