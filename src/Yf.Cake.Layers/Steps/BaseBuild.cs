using Cake.Common.Tools.DotNet.Build;
using Cake.Common.Xml;

namespace Yf.Cake.Layers.Steps
{
    public abstract class BaseBuild : FrostingTask<BuildContext>
    {
        public virtual bool RunCalculateMetrics => true;
        public virtual string? MetricsFramework => null;

        public override void Run(BuildContext context)
        {
            var settings = new DotNetBuildSettings()
            {
                Configuration = context.BuildConfiguration,
                NoLogo = true,
                NoRestore = true
            };

            context.DotNetBuild(context.Root, settings);

            if (RunCalculateMetrics)
            {
                CalculateMetrics(context);
            }
        }

        private void CalculateMetrics(BuildContext context)
        {
            var buildSettings = new DotNetBuildSettings()
            {
                Configuration = context.BuildConfiguration,
                NoLogo = true,
                NoRestore = true,
                Framework = MetricsFramework,
                MSBuildSettings = new DotNetMSBuildSettings()
            };

            buildSettings.MSBuildSettings.Targets.Add("Metrics");

            context.DotNetBuild(context.TargetProject, buildSettings);

            var metricsFilePath = context
                .GetFiles("./src/*/*.Metrics.xml")
                .Single();

            var maintainabilityStringValue = context.XmlPeek(
                metricsFilePath,
                "CodeMetricsReport/Targets/Target/Assembly/Metrics/Metric[@Name='MaintainabilityIndex']/@Value");

            var maintainability = double.Parse(maintainabilityStringValue, CultureInfo.InvariantCulture);
            var maintainabilityString = maintainability.ToString(CultureInfo.InvariantCulture);
            var statusGistJson = new StatusGistJson()
            {
                Label = "maintainability",
                Message = maintainabilityString,
                Color = maintainability.MapToColor(9, 19)
            };

            var metricsFilename = metricsFilePath.GetFilename();
            context.MoveFile(metricsFilePath, $"{context.TmpDirectory}/{metricsFilename}");

            context.CreateStatusGistJson("maintainability", statusGistJson);
            context.SetStepSummaryOutput("Maintainability", maintainabilityString);
        }
    }
}
