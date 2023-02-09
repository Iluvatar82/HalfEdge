using HalfEdge.MeshModifications.Base;
using HalfEdge.Models;

namespace HalfEdge.MeshModifications
{
    public class TriangulateMesh<T> : MeshModifyBase<T> where T : struct
    {
        protected override void CreateOutputMesh()
        {
            if (_inputMesh.Polygons.All(p => p.HalfEdges.Count == 3))
            {
                _outputMesh = new TriangleMesh<T>();
                _outputMesh.AddHalfEdges(_inputMesh.HalfEdges);
                _outputMesh.AddPolygons(_inputMesh.Polygons);
            }
            else
            {
                //TODO Triangulate the Polygons
            }
        }
    }
}
