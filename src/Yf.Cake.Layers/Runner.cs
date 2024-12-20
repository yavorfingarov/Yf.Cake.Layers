namespace Yf.Cake.Layers
{
    public static class Runner
    {
        public static int Run(string[] args)
        {
            var host = new CakeHost();
            host.UseContext<BuildContext>();
            var exitCode = host.Run(args);

            return exitCode;
        }
    }
}
