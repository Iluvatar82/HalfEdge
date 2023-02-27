using HalfEdge.Converter;
using HalfEdge.MeshModifications.Base;
using Models;

namespace HalfEdge.MeshModifications
{
    public class MeshTriangulator : MeshModifyBase
    {
        protected override void CreateOutputMesh()
        {
            if (_inputMesh.Polygons.All(p => p.HalfEdges.Count == 3))
                _outputMesh = new TriangleMesh(_inputMesh.Vertices.ToList(), _inputMesh.Indices.ToList(), _inputMesh.HalfEdges.ToList(), _inputMesh.Polygons.ToList());
            else
                _outputMesh = MeshConverter.ConvertToTriangleMesh(_inputMesh);
        }
    }
}
