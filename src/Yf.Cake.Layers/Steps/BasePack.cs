using Cake.Common.Tools.DotNet.Pack;

namespace Yf.Cake.Layers.Steps
{
    public abstract class BasePack : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            var settings = new DotNetPackSettings()
            {
                Configuration = context.BuildConfiguration,
                NoLogo = true,
                NoRestore = true,
                NoBuild = true,
                OutputDirectory = context.OutputDirectory
            };

            context.DotNetPack(context.TargetProject, settings);
        }
    }
}
