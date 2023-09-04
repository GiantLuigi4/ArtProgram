using ArtProgram.display;
using MoonSharp.Interpreter;
using OpenGL;

namespace ArtProgram.scripting.obj {
    [MoonSharpUserData]
    public class LuaTexture {
        public Texture actual;

        public LuaTexture(Texture tex) {
            this.actual = tex;
        }

        public void startWrite() {
            actual.BindWrite();
        }

        public void stopWrite() {
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void bindWrite(LuaShader sdr, string uniform, int loc) {
            sdr.actual.BindOutput(actual, uniform, loc);
        }

        public void bind(LuaShader sdr, string uniform, int loc) {
            Gl.ActiveTexture(TextureUnit.Texture0 + loc);
            Gl.BindTexture(TextureTarget.Texture2d, actual.tex);
            Gl.Uniform1(sdr.actual.GetUniform(uniform), loc);
        }

        public void clear() {
            actual.BindWrite();
            Gl.ClearColor(0, 0, 0, 0f);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }
    }
}
