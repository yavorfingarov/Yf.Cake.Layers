namespace Yf.Cake.Layers.Steps
{
    public abstract class BaseRestoreNuGetPackages : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.DotNetRestore(context.Root);
        }
    }
}
