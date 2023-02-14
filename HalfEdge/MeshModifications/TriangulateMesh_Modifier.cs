using HalfEdge.Converter;
using HalfEdge.MeshModifications.Base;
using Models;

namespace HalfEdge.MeshModifications
{
    public class TriangulateMesh_Modifier : MeshModifyBase
    {
        protected override void CreateOutputMesh()
        {
            if (_inputMesh.Polygons.All(p => p.HalfEdges.Count == 3))
            {
                _outputMesh = new TriangleMesh(_inputMesh.Vertices.ToList());
                _outputMesh.AddHalfEdges(_inputMesh.HalfEdges);
                _outputMesh.AddPolygons(_inputMesh.Polygons);
            }
            else
                _outputMesh = MeshConverter.ConvertToTriangleMesh(_inputMesh);
        }
    }
}
