using ArtProgram.display;
using ArtProgram.script;
using ArtProgram.scripting.obj;
using Gtk;
using MoonSharp.Interpreter;
using OpenGL;
using System.Net.Http.Headers;

namespace ArtProgram {
    public class ArtApp {
        public List<Canvas> CanvasList = new List<Canvas>();
        public Canvas CurrentCanvas;

        Script script;
        DynValue PenScript;
        public ArtApp() {
            PenScript = ScriptManager.INSTANCE.parse(ResourceManager.ReadStr("scripts/tools/pen.lua"), out script);
        }

        public bool NeedsSaving() {
            foreach (Canvas cvas in CanvasList)
                if (cvas.Unsaved) return true;
            return false;
        }

        public ApplicationWindow? Window;
        public HBox? CanvasTabs;
        public GLArea? Display;

        public void Init() {
            Display.MakeCurrent();
            RenderHelper.Create();

            Display.MakeCurrent();
            CurrentCanvas = new Canvas(800, 800);
            CanvasList.Add(CurrentCanvas);
        }

        bool Dragging = false;
        bool DraggingR = false;

        public void MouseAction(int mx, int my, int action, int button) {
            double w = Display.Allocation.Width;
            double h = Display.Allocation.Height;

            if (action == 0 && button == 1) {
                Dragging = true;

                double x = mx;
                double y = my;
                CurrentCanvas.Transform(w, h, ref x, ref y);

                CurrentCanvas.Enqueue((a, b) => {
                    Canvas cv = (Canvas) a;

                    cv.GetDC().BindWrite();
                    Gl.ClearColor(0, 0, 0, 0f);
                    Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                    cv.Layers[0].Interlace(cv.GetDC());

                    script.Call(script.Globals["onClick"], 
                        x, y,
                        new LuaTexture(cv.Layers[0].texture),
                        new LuaTexture(cv.GetDC()),
                        0, 10, true
                    );
                });

                Display.QueueDraw();
                Window.QueueDraw();
            } else if (action == 1 && button == 1) {
                Dragging = false;

                CurrentCanvas.Enqueue((a, b) => {
                    Canvas cv = (Canvas) a;

                    double x = mx;
                    double y = my;
                    cv.Transform(w, h, ref x, ref y);

                    script.Call(script.Globals["onRelease"], 
                        x, y,
                        new LuaTexture(cv.Layers[0].texture),
                        new LuaTexture(cv.GetDC()),
                        0, 10, true
                    );

                    cv.Layers[0].MergeInterlace();
                    cv.Layers[0].Interlace(null);
                });
                Display.QueueDraw();
                Window.QueueDraw();
            }
            
            if (action == 0 && button == 3) DraggingR = true;
            else if (action == 1 && button == 3) DraggingR = false;

            PrevMX = mx;
            PrevMY = my;
        }

        int PrevMX = 0;
        int PrevMY = 0;

        public void MouseMoved(int mx, int my) {
            if (DraggingR) {
                CurrentCanvas.Pan(mx - PrevMX, my - PrevMY);
                PrevMX = mx;
                PrevMY = my;
                Display.QueueDraw();
                Window.QueueDraw();
            }
            if (Dragging) {
                double w = Display.Allocation.Width;
                double h = Display.Allocation.Height;

                CurrentCanvas.Enqueue((a, b) => {
                    Canvas cv = (Canvas) a;

                    double x = mx;
                    double y = my;
                    cv.Transform(w, h, ref x, ref y);

                    script.Call(script.Globals["onDrag"], 
                        x, y,
                        new LuaTexture(cv.Layers[0].texture),
                        new LuaTexture(cv.GetDC()),
                        0, 10, true
                    );
                });
                Display.QueueDraw();
                Window.QueueDraw();
            }
        }

        public void Scroll(double delta) {
            CurrentCanvas.HandleZoom(delta);
            Display.QueueDraw();
            Window.QueueDraw();
        }
    }
}
