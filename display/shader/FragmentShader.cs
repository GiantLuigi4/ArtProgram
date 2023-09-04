using System.Text;
using Gdk;
using OpenGL;

namespace ArtProgram.display.shader {
    public class FragmentShader : BaseShader {
        private static readonly uint defaultVert;

        static FragmentShader() {
            defaultVert = Gl.CreateShader(ShaderType.VertexShader);
            Gl.ShaderSource(defaultVert, new string[] { ResourceManager.ReadInternalStr("shaders/vert.vsh") });
            Gl.CompileShader(defaultVert);

            int param;
            Gl.GetShader(defaultVert, ShaderParameterName.CompileStatus, out param);
            if (param == Gl.FALSE) {
                string log = ReadLog(defaultVert);
                Console.WriteLine("FAILED TO COMPILE VERT");
                Console.WriteLine(log.ToString());
                Gl.DeleteShader(defaultVert);
            }
        }

        public FragmentShader(string src) {
            StringBuilder bldr = new StringBuilder();
            bool isVer = src.TrimStart().StartsWith("#version");

            var lin = 0;

            if (isVer) {
                foreach (string ln in src.Split("\n")) {
                    bldr.Append(ln).Append("\n");
                    if (isVer && ln.TrimStart().StartsWith("#version")) {
                        bldr
                            .Append("#define FRAGMENT\n")
                            .Append("#line ").Append(lin + 1).Append("\n");
                        isVer = false;
                    }
                    lin += 1;
                }
            } else {
                bldr.Append("#define FRAGMENT\n").Append(src);
            }

            uint fragment = Gl.CreateShader(ShaderType.FragmentShader);
            Gl.ShaderSource(fragment, new string[] { bldr.ToString() });
            Gl.CompileShader(fragment);

            Gl.GetShader(fragment, ShaderParameterName.CompileStatus, out int param);
            if (param == Gl.FALSE) {
                string log = ReadLog(fragment);
                Console.WriteLine("FAILED TO COMPILE FRAG");
                Console.WriteLine(log.ToString());
                Gl.DeleteShader(fragment);
            }

            progId = Gl.CreateProgram();
            Gl.AttachShader(progId, defaultVert);
            Gl.AttachShader(progId, fragment);
            Gl.DeleteShader(fragment);
        }

        public FragmentShader(string vert, string src) {
            StringBuilder bldr = new StringBuilder();
            bool isVer = src.TrimStart().StartsWith("#version");

            var lin = 0;

            if (isVer) {
                foreach (string ln in src.Split("\n")) {
                    bldr.Append(ln).Append("\n");
                    if (isVer && ln.TrimStart().StartsWith("#version")) {
                        bldr
                            .Append("#define FRAGMENT\n")
                            .Append("#line ").Append(lin + 1).Append("\n");
                        isVer = false;
                    }
                    lin += 1;
                }
            } else {
                bldr.Append("#define FRAGMENT\n").Append(src);
            }

            // vert
            uint vid = Gl.CreateShader(ShaderType.VertexShader);
            Gl.ShaderSource(vid, new string[] { vert });
            Gl.CompileShader(vid);

            Gl.GetShader(vid, ShaderParameterName.CompileStatus, out int param);
            if (param == Gl.FALSE) {
                string log = ReadLog(vid);
                Console.WriteLine("FAILED TO COMPILE FRAG");
                Console.WriteLine(log.ToString());
                Gl.DeleteShader(vid);
            }

            // frag
            uint fragment = Gl.CreateShader(ShaderType.FragmentShader);
            Gl.ShaderSource(fragment, new string[] { bldr.ToString() });
            Gl.CompileShader(fragment);

            Gl.GetShader(fragment, ShaderParameterName.CompileStatus, out param);
            if (param == Gl.FALSE) {
                string log = ReadLog(fragment);
                Console.WriteLine("FAILED TO COMPILE FRAG");
                Console.WriteLine(log.ToString());
                Gl.DeleteShader(fragment);
            }

            progId = Gl.CreateProgram();
            Gl.AttachShader(progId, vid);
            Gl.AttachShader(progId, fragment);
            Gl.BindAttribLocation(progId, 0, "position");
            Gl.LinkProgram(progId);
            Gl.ValidateProgram(progId);
            
            Gl.DeleteShader(vid);
            Gl.DeleteShader(fragment);
        }
    }
}
