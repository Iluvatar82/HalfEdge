using ObjectTK.Buffers;
using ObjectTK.Tools.Shapes;
using OpenTK.Graphics.OpenGL;

namespace UI.DemoApp.Models.Shapes
{
    public abstract class WireframeShape : ColoredShape
    {
        public uint[] WireframeColors { get; protected set; }
        public Buffer<uint> WireframeColorBuffer { get; protected set; }

        public override void UpdateBuffers()
        {
            base.UpdateBuffers();
            WireframeColorBuffer = new Buffer<uint>();
            WireframeColorBuffer.Init(BufferTarget.ArrayBuffer, WireframeColors);
        }

        protected override void Dispose(bool manual)
        {
            base.Dispose(manual);
            if (!manual)
                return;

            if (WireframeColorBuffer != null)
                WireframeColorBuffer.Dispose();
        }
    }
}
