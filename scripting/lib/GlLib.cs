using ArtProgram.display;
using ArtProgram.display.shader;
using ArtProgram.scripting.obj;
using MoonSharp.Interpreter;
using OpenGL;

namespace ArtProgram.scripting.lib {
    [MoonSharpUserData]
    public class GlLib {
        public static readonly GlLib INSTANCE = new GlLib();

        public static bool AllowCompute = false;
        public static double width, height;

        public void begin(string mode) {
            switch (mode) {
                case "polygons":
                    RenderHelper.INSTANCE.BeginDrawing(OpenGL.PrimitiveType.Polygon);
                    break;
                case "quads":
                    RenderHelper.INSTANCE.BeginDrawing(OpenGL.PrimitiveType.Quads);
                    break;
                case "triangles":
                    RenderHelper.INSTANCE.BeginDrawing(OpenGL.PrimitiveType.Triangles);
                    break;
            }
        }

        public void finish() {
            RenderHelper.INSTANCE.FinishDraw();
        }

        [MoonSharpUserDataMetamethod("getShader")]
        public LuaShader getShader(string name) {
            return new LuaShader(ShaderManager.GetShader(name, AllowCompute));
        }

        [MoonSharpUserDataMetamethod("getShader")]
        public LuaShader getShader(string name, bool forceCompute) {
            if (forceCompute) return new LuaShader(ShaderManager.GetShader(name, true));
            return new LuaShader(ShaderManager.GetShader(name, AllowCompute));
        }

        public static Action<LuaShader> ShaderSetup = (sdr) => { };

        public void setupUniforms(LuaShader sdr) {
            ShaderSetup.Invoke(sdr);
        }

        public void blit(LuaShader sdr, LuaTexture dst) {
            if (sdr.actual.GetType() == typeof(FragmentShader)) {
                Gl.Viewport(0, 0, dst.actual.Width, dst.actual.Height);
                
                RenderHelper helper = RenderHelper.INSTANCE;
                helper.BeginDrawing(PrimitiveType.Triangles);
                helper.Vertex(-1, -1, 0);
                helper.Vertex(-1, 1, 0);
                helper.Vertex(1, 1, 0);
                helper.Vertex(1, 1, 0);
                helper.Vertex(1, -1, 0);
                helper.Vertex(-1, -1, 0);
                helper.FinishDraw();
            } else {
                Gl.DispatchCompute((uint) Math.Ceiling(dst.actual.Width / 32d), (uint) Math.Ceiling(dst.actual.Height / 32d), 1);
                Gl.MemoryBarrier(MemoryBarrierMask.AllBarrierBits);
            }
        }

        public void vertex(dynamic x, dynamic y) {
            RenderHelper.INSTANCE.Vertex(x / width, y / height, 0);
        }
    }
}
