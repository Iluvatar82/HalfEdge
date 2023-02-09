using HalfEdge.Models.Base;

namespace HalfEdge.MeshModifications.Base
{
    public abstract class MeshModifyBase<T> : IMeshModify<T> where T : struct
    {
        protected Mesh<T> _inputMesh;
        protected Mesh<T> _outputMesh;


        public Mesh<T> InputMesh => _inputMesh;

        public Mesh<T> OutputMesh => _outputMesh;


        public MeshModifyBase()
        {
            _inputMesh = new Mesh<T>();
            _outputMesh = new Mesh<T>();
        }

        public void Modify(Mesh<T> mesh)
        {
            _inputMesh = mesh;
            CreateOutputMesh();
        }

        protected abstract void CreateOutputMesh();
    }
}
