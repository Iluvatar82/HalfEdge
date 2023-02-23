using ObjectTK.Shaders;
using ObjectTK.Shaders.Sources;
using ObjectTK.Shaders.Variables;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;


namespace UI.DemoApp.Models.Programs
{
    [VertexShaderSource("Simple.Vertex")]
    [FragmentShaderSource("Simple.Fragment")]
    public class SimpleProgram : Program
    {
        [VertexAttrib(3, VertexAttribPointerType.Float)]
        public VertexAttrib InPosition { get; protected set; }
        
        [VertexAttrib(4, VertexAttribPointerType.UnsignedByte, true)]
        public VertexAttrib InColor { get; protected set; }

        public Uniform<Matrix4> ModelViewProjectionMatrix { get; protected set; }
    }
}