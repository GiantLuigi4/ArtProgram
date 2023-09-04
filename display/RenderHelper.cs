using ArtProgram.display.shader;
using OpenGL;
using System;

namespace ArtProgram.display {
    public class RenderHelper {
        public static RenderHelper? INSTANCE;

        protected VStruct VAO = new VStruct();

        public BaseShader UI_SHADER;
        public FragmentShader BLIT_FRAG;
        public FragmentShader MERGE_FRAG;
        public BaseShader MERGE_SHADER;

        private RenderHelper() {
            UI_SHADER = new FragmentShader(ResourceManager.ReadInternalStr("shaders/ui.vsh"), ResourceManager.ReadInternalStr("shaders/ui.fsh"));
            BLIT_FRAG = (FragmentShader) ShaderManager.GetShader("util/blit", false);
            MERGE_FRAG = (FragmentShader) ShaderManager.GetShader("layer/merge", false);
            MERGE_SHADER = (BaseShader) ShaderManager.GetShader("layer/merge", true);
            UI_SHADER.DiscoverUniform("Texture");
            UI_SHADER.DiscoverUniform("Resolution");
            UI_SHADER.DiscoverUniform("CanvasSize");
            UI_SHADER.DiscoverUniform("Offset");
            UI_SHADER.DiscoverUniform("Scale");
            UI_SHADER.ParseAttribs(new string[]{"position"});
        }

        public static void Create() {
            if (INSTANCE == null) {
                INSTANCE = new RenderHelper();
            }
        }


        VStruct? ActiveStruct = null;
        readonly List<float> Floats = new();
        PrimitiveType Mode;
        int Count;

        public void BeginDrawing(PrimitiveType mode) {
            ActiveStruct = VAO;
            Floats.Clear();
            Count = 0;
            Mode = mode;
        }

        public void Vertex(double x, double y, double z) {
            Floats.Add((float) x);
            Floats.Add((float) y);
            Floats.Add((float) z);

            Count += 3;
        }

        public void Color(double r, double g, double b, double a) {
            Floats.Add((float) r);
            Floats.Add((float) g);
            Floats.Add((float) b);
            Floats.Add((float) a);

            Count += 4;
        }

        public void Texture(double u, double v) {
            Floats.Add((float) u);
            Floats.Add((float) v);

            Count += 2;
        }

        public void FinishDraw() {
            VAO.Bind();
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)Count * 4, Floats.ToArray(), BufferUsage.StaticDraw);
            int stride = BaseShader.SetupAttribs();
            Gl.DrawArrays(Mode, 0, Count / stride);
            BaseShader.DisableAttribs();
        }

        public void FinishDraw(Texture target) {
            VAO.Bind();
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)Count * 4, Floats.ToArray(), BufferUsage.StaticDraw);
            int stride = BaseShader.SetupAttribs();
            BaseShader.BindWrite(target, "Output", 0);
            Gl.DrawArrays(Mode, 0, Count / stride);
            BaseShader.DisableAttribs();
        }
    }
}
