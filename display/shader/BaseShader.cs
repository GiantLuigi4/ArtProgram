using OpenGL;
using System.Text;

using ArtProgram.display;

namespace ArtProgram.display.shader {
    public class BaseShader {
        private static BaseShader? BOUND = null;
        bool position, color, texture, normal;

        public void ParseAttribs(string[] attribs) {
            foreach (string ae in attribs) {
                string a = ae.Trim();
                
                if (a == "position") position = true;
                else if (a == "color") color = true;
                else if (a == "texture") texture = true;
                else if (a == "normal") normal = true;
            }

            uint bnd = 0;
            if (position) {
                Gl.BindAttribLocation(progId, bnd, "position");
                bnd++;
            }
            if (color) {
                Gl.BindAttribLocation(progId, bnd, "color");
                bnd++;
            }
            if (texture) {
                Gl.BindAttribLocation(progId, bnd, "texCoord");
                bnd++;
            }
            if (normal) {
                Gl.BindAttribLocation(progId, bnd, "normal");
                bnd++;
            }
            Gl.LinkProgram(progId);
            Gl.ValidateProgram(progId);
        }

        protected uint progId;
        protected Dictionary<string, int> shaderUniforms = new();

        // so, attempting to read the whole log at once caused the program to crash
        // doing it this way, for some reason doesn't
        // so ig I'm doing this
        // I feel like it'd be funny to have people keeping track of how long they spent trying to fix this, so uh
        // time spent on this method: 10 minutes
        protected static String ReadLog(uint id) {
            StringBuilder log = new StringBuilder();
            int len = 0;
            int lastLen = 0;
            while (true) {
                try {
                    Gl.GetShaderInfoLog(id, len + 2, out len, log);
                    if (len == lastLen) break;
                    lastLen = len;
                } catch (Exception e) {
                    break;
                }
            }
            return log.ToString();
        }

        public BaseShader() {
        }

        public void DiscoverUniform(string name) {
            int id = Gl.GetUniformLocation(progId, name);
            shaderUniforms[name] = id;
            if (id == -1) {
                Console.WriteLine("Couldn't find uniform " + name);
            }
        }

        public void Bind() {
            Gl.UseProgram(progId);
            BOUND = this;
        }

        public void Delete() {
            Gl.DeleteProgram(progId);
        }

        public int GetUniform(string name) {
            if (!shaderUniforms.ContainsKey(name)) return -1;
            return shaderUniforms[name];
        }

        public static void BindWrite(Texture tex, string uniform, int loc) {
            BOUND.BindOutput(tex, uniform, loc);
        }

        public virtual void BindOutput(Texture texture, string uniform, int loc) {
            texture.BindWrite();
            // Gl.Uniform1i(GetUniform(uniform), loc, texture);
        }

        public static void DisableAttribs() {
            if (BOUND.position) {
                uint loc = (uint) Gl.GetAttribLocation(BOUND.progId, "position");
                Gl.EnableVertexAttribArray(loc);
            }
            if (BOUND.color) {
                uint loc = (uint) Gl.GetAttribLocation(BOUND.progId, "color");
                Gl.EnableVertexAttribArray(loc);
            }
            if (BOUND.texture) {
                uint loc = (uint) Gl.GetAttribLocation(BOUND.progId, "texCoord");
                Gl.EnableVertexAttribArray(loc);
            }
            if (BOUND.normal) {
                uint loc = (uint) Gl.GetAttribLocation(BOUND.progId, "normal");
                Gl.EnableVertexAttribArray(loc);
            }
        }

        public static int SetupAttribs() {
            int bnd = 0;

            int strd = 0;
            if (BOUND.position) strd += 3;
            if (BOUND.color) strd += 4;
            if (BOUND.texture) strd += 2;
            if (BOUND.normal) strd += 3;

            if (BOUND.position) {
                uint loc = (uint) Gl.GetAttribLocation(BOUND.progId, "position");

                Gl.EnableVertexAttribArray(loc);
                Gl.VertexAttribPointer(loc, 3, VertexAttribType.Float, false, strd * 4, bnd * 4);

                bnd += 3;
            }

            if (BOUND.color) {
                uint loc = (uint) Gl.GetAttribLocation(BOUND.progId, "color");

                Gl.EnableVertexAttribArray(loc);
                Gl.VertexAttribPointer(loc, 4, VertexAttribType.Float, false, strd * 4, bnd * 4);

                bnd += 4;
            }

            if (BOUND.texture) {
                uint loc = (uint) Gl.GetAttribLocation(BOUND.progId, "texCoord");

                Gl.EnableVertexAttribArray(loc);
                Gl.VertexAttribPointer(loc, 2, VertexAttribType.Float, false, strd * 4, bnd * 4);

                bnd += 2;
            }

            if (BOUND.normal) {
                uint loc = (uint) Gl.GetAttribLocation(BOUND.progId, "normal");

                Gl.EnableVertexAttribArray(loc);
                Gl.VertexAttribPointer(loc, 3, VertexAttribType.Float, false, strd * 4, bnd * 4);

                bnd += 3;
            }

            return strd;
        }
    }
}
