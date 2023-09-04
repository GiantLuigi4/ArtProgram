using Gtk;

using OpenGL;
using ArtProgram.display;
using ArtProgram.display.shader;
using ArtProgram.layer;
using MoonSharp.Interpreter;
using ArtProgram.script;
using ArtProgram.scripting.obj;
using ArtProgram.scripting.lib;

namespace ArtProgram {
    public class Canvas {
        public bool Unsaved = false;
        public int width = 100, height = 100;
        float Zoom = 1;
        double Px = 0, Py = 0;

        public List<TextureLayer> Layers = new();

        Texture swap0, swap1, s0, s1;
        Texture drawContent;

        public List<EventHandler> handlers = new();

        Script script;
        DynValue Merge;

        bool firstFrame = true;
        uint canvasFbo;

        public Canvas(int width, int height) {
            Gl.GetInteger(GetPName.DrawFramebufferBinding, out canvasFbo);
      
            this.width = width;
            this.height = height;

            swap0 = new Texture(width, height, true);
            swap1 = new Texture(width, height, true);
            s0 = new Texture(width, height, true);
            s1 = new Texture(width, height, true);

            Layers.Add(new TextureLayer(width, height));
            // Layers.Add(new TextureLayer(width, height));
            Gl.Viewport(0, 0, width, height);
            Layers[0].Clear(1, 1, 1, 1);
            // Layers[1].Clear(1, 1, 1, 0.5f);

            Merge = ScriptManager.INSTANCE.parse(ResourceManager.ReadStr("scripts/merging/merge.lua"), out script);

            drawContent = new Texture(width, height, true);
        }

        public Texture GetDC() { // hey look! I'm encapsulating stuff!
            return drawContent;
        }

        public void Render(ArtApp app, GLArea area) {
            area.MakeCurrent();

            if (firstFrame) firstFrame = false;
            else Gl.GetInteger(GetPName.DrawFramebufferBinding, out canvasFbo);

            Gl.Disable(EnableCap.CullFace);

            Gl.Viewport(0, 0, width, height);
            Gl.ClearColor(0f, 0, 0, 0);
            this.s0.BindWrite();
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            this.s1.BindWrite();
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            this.swap0.BindWrite();
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            this.swap1.BindWrite();
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            GlLib.width = width;
            GlLib.height = height;
            GlLib.ShaderSetup = (sdr) => {
                sdr.setUniformF("Resolution", width, height);
            };
            foreach (EventHandler handler in handlers) handler.Invoke(this, EventArgs.Empty);
            handlers.Clear();

            RenderHelper helper = RenderHelper.INSTANCE;
            
            Texture res = this.swap0;
            Texture resSwap = this.swap1;
            
            Texture s0 = this.s0;
            Texture s1 = this.s1;
            
            foreach (TextureLayer layer in Layers) {
                Texture dst = layer.GetTexture(s0, s1);
                script.Call(script.Globals["blit"], new object[]{
                    new LuaTexture(res),
                    new LuaTexture(dst),
                    new LuaTexture(resSwap),
                    1f, 0
                });
                Texture tmp = res;
                res = resSwap;
                resSwap = tmp;
            }

            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, canvasFbo);
            Gl.Viewport(0, 0, area.Allocation.Width, area.Allocation.Height);

            Gl.ClearColor(0.09019607843f, 0.09019607843f, 0.09019607843f, 1f);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            BaseShader shader = helper.UI_SHADER;

            shader.Bind();
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture2d, res.tex);
            Gl.Uniform1(shader.GetUniform("Texture"), 0);

            Gl.Uniform2(shader.GetUniform("CanvasSize"), width, height);
            Gl.Uniform2(shader.GetUniform("Resolution"), area.Allocation.Width, area.Allocation.Height);
            Gl.Uniform2(shader.GetUniform("Offset"), (float) Px, (float) Py);
            Gl.Uniform1(shader.GetUniform("Scale"), Zoom);

            helper.BeginDrawing(PrimitiveType.Triangles);
            helper.Vertex(-1, -1, 0);
            helper.Vertex(-1, 1, 0);
            helper.Vertex(1, 1, 0);
            helper.Vertex(-1, -1, 0);
            helper.Vertex(1, -1, 0);
            helper.Vertex(1, 1, 0);
            helper.FinishDraw();

            Gl.BindVertexArray(0);
            Gl.Flush();
        }

        public void Pan(int x, int y) {
            Px += -x;
            Py += y;
        }

        public void HandleZoom(double Delta) {
            Px /= Zoom;
            Py /= Zoom;

            for (int i = 0; i < Math.Abs(Delta); i++) {
                float mul = 1.1f;
                if (Delta < 0) Zoom *= mul;
                else Zoom /= mul;
            }
            if (Zoom < 1f / 2000) Zoom = 1f / 2000;

            Px *= Zoom;
            Py *= Zoom;
        }

        public void Enqueue(EventHandler handler) {
            handlers.Add(handler);
        }

        public void Transform(double width, double height, ref double x, ref double y) {
            x -= width / 2;
            y -= height / 2;

            x += Px;
            y -= Py;

            x *= 2;
            y *= -2;

            x /= Zoom;
            y /= Zoom;
        }
    }
}
