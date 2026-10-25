using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;

class App : Form
{
    [DllImport("user32.dll")] static extern bool SetProcessDPIAware();
    [DllImport("user32.dll")] static extern int GetDpiForSystem();
    [DllImport("user32.dll")] static extern bool ReleaseCapture();
    [DllImport("user32.dll")] static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

    App()
    {
        Text = "To-Do List";
        Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(360 * GetDpiForSystem() / 96, 440 * GetDpiForSystem() / 96);

        var web = new WebView2 { Dock = DockStyle.Fill };
        // The page saves tasks to localStorage, which lives in this folder
        web.CreationProperties = new CoreWebView2CreationProperties {
            UserDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "To-Do List")
        };
        // Title bar actions posted from the page
        web.WebMessageReceived += (s, e) => {
            switch (e.TryGetWebMessageAsString()) {
                case "drag":
                    if (MouseButtons != MouseButtons.Left) break; // click already ended
                    ReleaseCapture();
                    SendMessage(Handle, 0xA1, 2, 0); // WM_NCLBUTTONDOWN on HTCAPTION: Windows moves the window
                    break;
                case "min":
                    WindowState = FormWindowState.Minimized;
                    break;
                case "max":
                    // Keep a borderless window from covering the taskbar
                    var screen = Screen.FromControl(this);
                    MaximizedBounds = new Rectangle(screen.WorkingArea.Location - (Size)screen.Bounds.Location, screen.WorkingArea.Size);
                    WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
                    break;
                case "close":
                    Close();
                    break;
            }
        };
        web.Source = new Uri(Path.Combine(Application.StartupPath, "To-Do List.html"));
        Controls.Add(web);
    }

    // Lets clicking the taskbar button minimize and restore a borderless window
    protected override CreateParams CreateParams {
        get { var cp = base.CreateParams; cp.Style |= 0x20000; return cp; } // WS_MINIMIZEBOX
    }

    [STAThread]
    static void Main()
    {
        SetProcessDPIAware();
        Application.Run(new App());
    }
}
