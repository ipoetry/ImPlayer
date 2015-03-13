using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;

[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
public class App : Application
{
    private bool _contentLoaded;

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
        if (!this._contentLoaded)
        {
            this._contentLoaded = true;
            base.StartupUri = new Uri("lrcwindow.xaml", UriKind.Relative);
            Uri resourceLocator = new Uri("/Lrc;component/app.xaml", UriKind.Relative);
            Application.LoadComponent(this, resourceLocator);
        }
    }

    [STAThread, DebuggerNonUserCode]
    public static void Main()
    {
        App app = new App();
        app.InitializeComponent();
        app.Run();
    }
}
