using System;
using System.IO;
using System.Windows;
using WpfApplication = System.Windows.Application;

namespace Cccc.Presentation.Verification
{
    internal static class Program
    {
        [STAThread]
        private static int Main(string[] args)
        {
            string mode = args.Length > 0 ? args[0] : "animation";
            string output = args.Length > 1
                ? args[1]
                : Path.Combine(AppContext.BaseDirectory, "checks");
            if (mode != "preview") Directory.CreateDirectory(output);

            WpfApplication app = new WpfApplication { ShutdownMode = ShutdownMode.OnExplicitShutdown };
            try
            {
                switch (mode)
                {
                    case "preview":
                        PreviewHost.Run(app);
                        break;
                    case "animation":
                        AnimationVerification.Run(output);
                        break;
                    case "taskbar":
                        TaskbarVerification.Run();
                        break;
                    case "shape":
                        ShapeVerification.Run(output);
                        break;
                    case "all":
                        AnimationVerification.Run(output);
                        TaskbarVerification.Run();
                        break;
                    case "material":
                        return MaterialVerification.Run(args.Length > 1 ? args[1..] : Array.Empty<string>());
                    default:
                        throw new ArgumentException("Unknown check mode: " + mode);
                }
                return 0;
            }
            catch (Exception error)
            {
                Console.Error.WriteLine(error);
                return 1;
            }
            finally { app.Shutdown(); }
        }
    }
}
