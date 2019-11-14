using System.Reflection;
using System.Runtime.Versioning;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.Rendering;

namespace RenderDemo
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow = new MainWindow();
            base.OnFrameworkInitializationCompleted();
        }

        // TODO: Make this work with GTK/Skia/Cairo depending on command-line args
        // again.
        static void Main(string[] args) 
            => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

        // App configuration, used by the entry point and previewer
        static AppBuilder BuildAvaloniaApp()
           => AppBuilder.Configure<App>()
               .With(new Win32PlatformOptions
               {
                   OverlayPopups = true,
               })
                .UsePlatformDetect()
                .Use60FpsRendererHackForDotnetFramework47plus()
                .UseReactiveUI()
                .LogToTrace();
    }

    public static class Ext
    {
        public static AppBuilder Use60FpsRendererHackForDotnetFramework47plus(this AppBuilder b)
        {
            var dotnetVer = Assembly.GetEntryAssembly().GetCustomAttribute<TargetFrameworkAttribute>().FrameworkName?.ToLowerInvariant() ?? "";

            if (dotnetVer.StartsWith(".netframework"))
            {
                var intit = b.WindowingSubsystemInitializer;
                b.UseWindowingSubsystem(() =>
                {
                    intit();
                    AvaloniaLocator.CurrentMutable.Bind<IRenderTimer>().ToConstant(new DefaultRenderTimer(65));
                });
            }
            return b;
        }
    }
}
