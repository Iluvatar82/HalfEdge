using HalfEdge.Enumerations;
using HalfEdge.MeshModifications.Base;
using Validation;

namespace HalfEdge.MeshModifications
{
    public class SubdivideMesh_Modifier : MeshModifyBase
    {
        public SubdivisionType SubdivisionType { get; set; }
        public int Iterations { get; set; }

        protected override void CreateOutputMesh()
        {
            SubdivisionType.NotNull();
            Iterations.Satisfies(it => it > 0);

            throw new NotImplementedException();
        }
    }
}
