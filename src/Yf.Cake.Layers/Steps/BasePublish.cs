using Cake.Common.Tools.DotNet.Publish;

namespace Yf.Cake.Layers.Steps
{
    public abstract class BasePublish : FrostingTask<BuildContext>
    {
        public abstract string Runtime { get; }
        public virtual bool UseTimestamp => true;

        public override void Run(BuildContext context)
        {
            var settings = new DotNetPublishSettings()
            {
                Configuration = context.BuildConfiguration,
                NoLogo = true,
                Runtime = Runtime,
                OutputDirectory = context.OutputDirectory,
                MSBuildSettings = new DotNetMSBuildSettings()
                {
                    Version = UseTimestamp ? context.Timestamp : null
                }
            };

            context.DotNetPublish(context.TargetProject, settings);
        }
    }
}
