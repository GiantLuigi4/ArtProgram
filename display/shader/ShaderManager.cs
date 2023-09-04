using System.Net.Http.Headers;
using System.Text;
using ArtProgram;
using OpenGL;

namespace ArtProgram.display.shader {
    public class ShaderManager {
        private static string[] SplitFirst(string src, char sep) {
            int indx = src.IndexOf(sep);
            return new string[]{
                src.Substring(0, indx).Trim(),
                src.Substring(indx + 1).Trim(),
            };
        }

        private static Dictionary<string, FragmentShader> FRAGMENTS = new();
        private static Dictionary<string, ComputeShader> COMPUTES = new();
        
        public static BaseShader GetShader(string name, bool compute) {
            if (!FRAGMENTS.ContainsKey(name)) {
                string txt = ResourceManager.ReadStr("shaders/" + name + ".properties");

                string file = name;
                string ext = ".fsh";
                string vert = "[[builtin]]";
                string attribs = "position";

                List<string> uniforms = new();

                foreach (string ln in txt.Split("\n")) {
                    if (ln.Trim().Length == 0) continue;
                    if (ln.Trim()[0] == '#') continue;

                    string[] split = SplitFirst(ln, ':');

                    if (split[0] == "shader-name") {
                        file = split[1];
                    } else if (split[0] == "shader-extension") {
                        ext = "." + split[1];
                    } else if (split[0] == "shader-vertex") {
                        vert = split[1];
                    } else if (split[0].StartsWith("uniform")) {
                        uniforms.Add(SplitFirst(split[0], '-')[1]);
                    } else if (split[0] == "shader-attribs") {
                        attribs = split[1];
                    }
                }

                string data = ResourceManager.ReadStr("shaders/" + file + ext);

                bool common = ext == ".glsl";

                BaseShader? sdrL = null;
                BaseShader? sdrR = null;
                if (vert != "[[builtin]]") {
                    if (common || ext == ".comp")
                        throw new Exception("Cannot use vertex shaders with a compute or hybrid shader");
                    
                    sdrL = new FragmentShader(
                        ResourceManager.ReadStr("shaders/" + vert + ".vsh"),
                        data
                    );
                } else {
                    if (common || ext == ".fsh")
                        sdrL = FRAGMENTS[file] = new FragmentShader(data);
                    if (common || ext == ".comp")
                        sdrR = COMPUTES[file] = new ComputeShader(data);
                }

                sdrL?.ParseAttribs(attribs.Split(","));

                foreach (string s in uniforms) {
                    sdrL?.DiscoverUniform(s);
                    sdrR?.DiscoverUniform(s);
                }
            }
            
            if (compute)
                if (COMPUTES.ContainsKey(name)) return COMPUTES[name];

            return FRAGMENTS[name];
        }
    }
}
