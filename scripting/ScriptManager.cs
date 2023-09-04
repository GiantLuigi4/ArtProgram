using ArtProgram.scripting.lib;
using ArtProgram.scripting.obj;
using MoonSharp.Interpreter;

namespace ArtProgram.script {
    public class ScriptManager {
        public static ScriptManager INSTANCE = new ScriptManager();

        public static void DoPrint(dynamic text) {
            Console.WriteLine(text);
        }

        static ScriptManager() {
            UserData.RegisterType<LuaTexture>();
            UserData.RegisterType<LuaShader>();
            UserData.RegisterType<GlLib>();
        }

        Table Include(Script script, string name) {
            int keyCount = script.Globals.Keys.Count();
            DynValue obj = parse(script, ResourceManager.ReadStr("scripts/" + name.Replace(".", "/") + ".lua"));
            if (keyCount != script.Globals.Keys.Count()) {
                throw new Exception("Globals were added by included script " + name + ". Use the local keyword to prevent this.");
            }
            return obj.Table;
        }

        Script ImprintGlobals() {
            Script scr = new Script(
                CoreModules.Table | 
                CoreModules.Math | 
                CoreModules.TableIterators | 
                CoreModules.String | 
                CoreModules.TableIterators | 
                CoreModules.ErrorHandling |
                CoreModules.Basic |
                CoreModules.Debug
            );
            scr.DebuggerEnabled = false;

            scr.Globals["Gl"] = DynValue.FromObject(scr, GlLib.INSTANCE);

            scr.Globals["print"] = DoPrint;
            scr.Globals["require"] = Include;
            return scr;
        }

        public DynValue parse(Script script, string str) {
            return script.DoString(str);
        }

        public DynValue parse(string str, out Script script) {
            script = ImprintGlobals();
            return script.DoString(str);
        }
    }
}
