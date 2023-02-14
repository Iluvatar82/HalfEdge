using Models.Base;

namespace HalfEdge.MeshModifications.Base
{
    public abstract class MeshModifyBase : IMeshModify
    {
        protected Mesh _inputMesh;
        protected Mesh _outputMesh;


        public Mesh InputMesh => _inputMesh;

        public Mesh OutputMesh => _outputMesh;


        public MeshModifyBase()
        {
            _inputMesh = new Mesh();
            _outputMesh = new Mesh();
        }

        public void Modify(Mesh mesh)
        {
            _inputMesh = mesh;
            CreateOutputMesh();
        }

        protected abstract void CreateOutputMesh();
    }
}
