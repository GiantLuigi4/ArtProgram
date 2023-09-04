// See https://aka.ms/new-console-template for more information
// https://www.moonsharp.org/getting_started.html

using Gtk;
using ArtProgram;
using OpenGL;
using System.Diagnostics;
using System.Runtime.InteropServices;

Console.WriteLine("Initializing");
Console.WriteLine("PID: " + Process.GetCurrentProcess().Id);

bool isWindows = false;

switch (Environment.OSVersion.Platform) {
    case PlatformID.Win32S:
    case PlatformID.Win32Windows:
    case PlatformID.Win32NT:
    case PlatformID.WinCE:
        isWindows = true;
        break;
}

Func<string, bool> SetDllDir = null;
if (isWindows) {
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetDllDirectory(string lpPathName);
    SetDllDir = SetDllDirectory;
} else {
    static bool SetDllDirectory(string lp) { return false; }
    SetDllDir = SetDllDirectory;
}

SetDllDir(Directory.GetCurrentDirectory() + "/bin");
Application app = new Application("tfc.ArtProgram", GLib.ApplicationFlags.None);
SetDllDir(Directory.GetCurrentDirectory());
ArtApp artApp = new ArtApp();

Gdk.RGBA color(double r, double g, double b) {
    Gdk.RGBA col = new Gdk.RGBA();
    col.Red = r;
    col.Green = g;
    col.Blue = b;
    col.Alpha = 1;
    return col;
}

void colorWidget(Widget widget, Gdk.RGBA color) {
    widget.OverrideBackgroundColor(StateFlags.Normal, color);
}

int padSize = 5;

int buttonWidth = 48;
int buttonsPerRow = 4;

void padV(VBox widget) {
    widget.PackStart(hsep(), false, false, 0);
    widget.PackEnd(hsep(), false, false, 0);
}
void padH(HBox widget) {
    widget.PackStart(vsep(), false, false, 0);
    widget.PackEnd(vsep(), false, false, 0);
}
double divis = 1 / 1.4;
VSeparator vsep() {
    VSeparator sep = new VSeparator();
    sep.SetSizeRequest(padSize, padSize);
    colorWidget(sep, color(0.11372549019 / divis, 0.11372549019 / divis, 0.11372549019 / divis));
    return sep;
}
HSeparator hsep() {
    HSeparator sep = new HSeparator();
    sep.SetSizeRequest(padSize, padSize);
    colorWidget(sep, color(0.11372549019 / divis, 0.11372549019 / divis, 0.11372549019 / divis));
    return sep;
}

app.Activated += (s, e) => {
    Console.WriteLine("|-Creating UI");

    ApplicationWindow wndo = new ApplicationWindow(app);
    wndo.Title = "Art Program";
    artApp.Window = wndo;
    wndo.SetDefaultSize(800, 800);

    wndo.DeleteEvent += (window, args) => { args.RetVal = artApp.NeedsSaving(); };

    Console.WriteLine("|-|-Tool Panel");
    VBox toolSelection = new VBox();
    colorWidget(toolSelection, color(0.11372549019, 0.11372549019, 0.11372549019));
    padV(toolSelection);
    { // tool panel
        VBox tools = new VBox();

        int width = buttonWidth * buttonsPerRow;
        tools.SetSizeRequest(width, 0);
        for (int i1 = 0; i1 < 5; i1++) {
            HBox row = new HBox();
            row.SetSizeRequest(width / buttonsPerRow, width / buttonsPerRow);
            for (int i = 0; i < buttonsPerRow; i++) {
                Button button = new Button(i + ", " + i1);
                button.SetSizeRequest(width / buttonsPerRow, width / buttonsPerRow);
                row.PackStart(button, false, false, 0);
            }
            tools.PackStart(row, false, false, 0);
        }

        colorWidget(tools, color(0.11372549019, 0.11372549019, 0.11372549019));
        toolSelection.PackStart(tools, false, false, 0);
        toolSelection.PackStart(hsep(), false, false, 0);
    }

    Console.WriteLine("|-|-Central Panel");
    VBox centralPanel = new VBox();
    { // central panel
        padV(centralPanel);
        HBox tabs = new HBox();
        artApp.CanvasTabs = tabs;

        tabs.SetSizeRequest(30, 30);
        colorWidget(tabs, color(0.11372549019, 0.11372549019, 0.11372549019));
        centralPanel.PackStart(tabs, false, false, 0);
        centralPanel.PackStart(vsep(), false, false, 0);

        GLArea canvas = new GLArea();
        canvas.Realized += (widget, ev) => { Gl.Initialize(); };

        int mx = -1;
        int my = -1;

        bool inBounds = false;

        canvas.AddTickCallback((wdgt, clk) => {
            wdgt.GetPointer(out int x, out int y);

            if (mx != x || my != y) {
                mx = x;
                my = y;
                artApp.MouseMoved(x, y);

                if (x < 0 || y < 0 || mx > canvas.Allocation.Width || my > canvas.Allocation.Height)
                    inBounds = false;
                else inBounds = true;
            }

            return true;
        });

        bool hasInit = false;
        canvas.Render += (widget, ev) => { 
            if (!hasInit) {
                artApp.Init();
                hasInit = true;
            }
            artApp.CurrentCanvas.Render(artApp, canvas);
        };
        canvas.ErrorBell();

        artApp.Display = canvas;
        EventBox eb = new EventBox();
        eb.Add(canvas);
        eb.AddEvents((int)Gdk.EventMask.AllEventsMask);
        eb.ButtonPressEvent += (wdgt, ev) => {
            artApp.MouseAction(mx, my, 0, (int) ((Gdk.EventButton) ev.Args[0]).Button);
        };
        eb.ButtonReleaseEvent += (wdgt, ev) => {
            artApp.MouseAction(mx, my, 1, (int) ((Gdk.EventButton) ev.Args[0]).Button);
        };
        eb.ScrollEvent += (wdgt, ev) => {
            artApp.Scroll(((Gdk.EventScroll) ev.Args[0]).DeltaY);
        };
        centralPanel.PackEnd(eb, true, true, 0);
    }

    colorWidget(centralPanel, color(0.09019607843, 0.09019607843, 0.09019607843));

    Console.WriteLine("|-|-Layer Panel");
    VBox layers = new VBox();
    { // layer panel
        padV(layers);
        for (int i = 0; i < 10; i++) {
            Button text = new Button("Layer " + i);
            layers.PackStart(text, false, false, 0);
        }
        layers.SetSizeRequest(300, 0);
        colorWidget(layers, color(0.11372549019, 0.11372549019, 0.11372549019));
    }

    Console.WriteLine("|-|-Pack");
    { // prepare the display
        HBox everything = new HBox();
        padH(everything);

        everything.PackStart(toolSelection, false, false, 0);
        everything.PackStart(vsep(), false, false, 0);

        everything.PackEnd(layers, false, false, 0);
        everything.PackEnd(vsep(), false, false, 0);

        everything.PackStart(centralPanel, true, true, 0);
        wndo.Add(everything);
    }

    Console.WriteLine("|-|-Show");
    wndo.ShowAll();
    wndo.Present();

    Console.WriteLine("|-Ready");
};

app.Run(null, new String[0]);
