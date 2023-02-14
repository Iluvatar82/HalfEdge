using Models.Base;

namespace HalfEdge.MeshModifications.Base
{
    public interface IMeshModify
    {
        Mesh InputMesh { get; }
        Mesh OutputMesh { get; }
        void Modify(Mesh mesh);
    }
}
