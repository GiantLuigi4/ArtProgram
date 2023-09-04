using OpenGL;

namespace ArtProgram.display {
    public class VStruct {
        public uint vao;
        public uint vbo;
        public int stride;


        public VStruct() {
            vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);

            vbo = Gl.GenBuffer();
            Gl.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        }

        public void Bind() {
            Gl.BindVertexArray(vao);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        }

        public void SetStride(int stride) {
            this.stride = stride;
        }
    }
}
