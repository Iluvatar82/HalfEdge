using ObjectTK.Tools.Cameras;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace UI.DemoApp.Render
{
    public class NewGimbalBehavior : ThirdPersonBehavior
    {
        public override void MouseMove(CameraState state, Vector2 delta, MouseState? mouseState = null)
        {
            var leftRight = Vector3.Cross(state.Up, state.LookAt);
            var forward = Vector3.Cross(leftRight, state.Up);

            var rot = Matrix3.CreateFromAxisAngle(state.Up, delta.X) * Matrix3.CreateFromAxisAngle(leftRight, delta.Y);
            Vector3.Transform(state.LookAt, Quaternion.FromMatrix(rot), out state.LookAt);
            state.LookAt.Normalize();
            if (Vector3.Dot(state.LookAt, forward) < 0)
                state.Up *= -1;

            state.Position = Origin - (state.Position - Origin).Length * state.LookAt;

            leftRight = Vector3.Cross(state.Up, state.LookAt);
            Vector3.Cross(state.LookAt, leftRight, out state.Up);
            state.Up.Normalize();
        }
    }
}
