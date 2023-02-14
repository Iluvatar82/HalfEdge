using Models.Base;

namespace HalfEdge.MeshModifications.Base
{
    public interface IMeshModify<T> where T : struct
    {
        Mesh<T> InputMesh { get; }
        Mesh<T> OutputMesh { get; }
        void Modify(Mesh<T> mesh);
    }
}
