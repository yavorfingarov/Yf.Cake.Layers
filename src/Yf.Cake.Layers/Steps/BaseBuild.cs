namespace Yf.Cake.Layers.Steps
{
    public abstract class BaseBuild : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            var settings = new DotNetBuildSettings()
            {
                Configuration = context.BuildConfiguration,
                NoLogo = true,
                NoRestore = true
            };

            context.DotNetBuild(context.RootDirectory, settings);
        }
    }
}
