namespace Yf.Cake.Layers
{
    public static class Runner
    {
        public static int Run(string[] args)
        {
            var host = new CakeHost();
            host.UseContext<BuildContext>();
            host.InstallTool(new Uri("dotnet:?package=dotnet-reportgenerator-globaltool&version=5.4.4"));
            var exitCode = host.Run(args);

            return exitCode;
        }
    }
}
