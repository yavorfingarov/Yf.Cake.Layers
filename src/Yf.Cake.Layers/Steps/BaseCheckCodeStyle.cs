using Cake.Common.Tools.DotNet.Format;

namespace Yf.Cake.Layers.Steps
{
    public abstract class BaseCheckCodeStyle : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.Log.Information("Checking code style...");
            var settings = new DotNetFormatSettings()
            {
                NoRestore = true,
                VerifyNoChanges = true,
                Severity = DotNetFormatSeverity.Info
            };

            context.DotNetFormat(context.Root, settings);
        }
    }
}
