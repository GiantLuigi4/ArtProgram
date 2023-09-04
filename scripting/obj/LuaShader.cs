using ArtProgram.display;
using ArtProgram.display.shader;
using MoonSharp.Interpreter;
using OpenGL;

namespace ArtProgram.scripting.obj {
    [MoonSharpUserData]
    public class LuaShader {
        public BaseShader actual;

        public LuaShader(BaseShader tex) {
            this.actual = tex;
        }

        public void bind() {
            actual.Bind();
        }

        public void unbind() {
            Gl.UseProgram(0);
        }

        [MoonSharpUserDataMetamethod("setUniformF")]
        public void setUniformF(string name, float v0) {
            Gl.Uniform1(actual.GetUniform(name), v0);
        }

        [MoonSharpUserDataMetamethod("setUniformF")]
        public void setUniformF(string name, float v0, float v1) {
            Gl.Uniform2(actual.GetUniform(name), v0, v1);
        }
        
        [MoonSharpUserDataMetamethod("setUniformF")]
        public void setUniformF(string name, float v0, float v1, float v2) {
            Gl.Uniform3(actual.GetUniform(name), v0, v1, v2);
        }

        [MoonSharpUserDataMetamethod("setUniformF")]
        public void setUniformF(string name, float v0, float v1, float v2, float v3) {
            Gl.Uniform4(actual.GetUniform(name), v0, v1, v2, v3);
        }
    }
}
