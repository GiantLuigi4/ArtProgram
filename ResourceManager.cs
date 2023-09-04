using System.Reflection;
using System.Text;

namespace ArtProgram {
    public class ResourceManager {
        public static byte[] ReadInternal(string name) {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream("ArtProgram.assets." + name.Replace("/", "."))) {
                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                stream.Close();
                return data;
            }
        }

        public static string ReadInternalStr(string name) {
            return Encoding.Default.GetString(ReadInternal(name));
        }

        public static byte[] Read(string name) {
            using (FileStream fs = new FileStream("resources/" + name, FileMode.Open)) {
                byte[] data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                fs.Close();
                return data;
            }
        }
        
        public static string ReadStr(string name) {
            return Encoding.Default.GetString(Read(name));
        }

        static ResourceManager() {
            string text = Encoding.Default.GetString(ReadInternal("assets.txt"));

            foreach (string eeeeeeeeeeeeeeeeeeeeeeee in text.Split("\n")) {
                string line = eeeeeeeeeeeeeeeeeeeeeeee.Trim();
                if (line.Length == 0)
                    continue;
                if (line.StartsWith("--")) continue;

                byte[] data = ReadInternal(line);

                if (!File.Exists("resources/" + line)) {
                    Directory.CreateDirectory(Directory.GetParent("resources/" + line).ToString());

                    using (FileStream fs = new FileStream("resources/" + line, FileMode.CreateNew)) {
                        fs.Write(data, 0, data.Length);
                        fs.Close();
                    }
                }
            }
        }
    }
}
