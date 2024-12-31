using Cake.Common.Tools.DotNet.Format;

namespace Yf.Cake.Layers.Steps
{
    public abstract class BaseScanCode : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            CheckStyle(context);
            RunSecurityScan(context);
            CalculateMetrics(context);
        }

        private static void CheckStyle(BuildContext context)
        {
            context.Log.Information("Checking code style...");
            var settings = new DotNetFormatSettings()
            {
                NoRestore = true,
                VerifyNoChanges = true,
                Severity = DotNetFormatSeverity.Info
            };

            context.DotNetFormat(context.RootDirectory, settings);
        }

        public static void RunSecurityScan(BuildContext context)
        {
            context.Log.Information("Running security scan...");
            context.DotNetTool("tool install Microsoft.CST.DevSkim.CLI --create-manifest-if-needed");
            context.DotNetTool("tool run devskim analyze -E " +
                "--file-format=text " +
                "--ignore-globs=\"**/bin/**,**/obj/**,**/Properties/launchSettings.json\" " +
                "--source-code=./src/");
        }

        private static void CalculateMetrics(BuildContext context)
        {
            context.Log.Information("Calculating code metrics...");
            var buildSettings = new DotNetBuildSettings()
            {
                Configuration = context.BuildConfiguration,
                NoLogo = true,
                NoRestore = true,
                Framework = GetFrameworkForMetrics(context),
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

        private static string GetFrameworkForMetrics(BuildContext context)
        {
            var directoryBuildProps = $"{context.RootDirectory}/Directory.Build.props";
            var targetFrameworks =
                context.XmlPeekSafe(context.TargetProject, "Project/PropertyGroup/TargetFrameworks") ??
                context.XmlPeekSafe(directoryBuildProps, "Project/PropertyGroup/TargetFrameworks");

            string? framework;
            if (targetFrameworks == null)
            {
                framework =
                    context.XmlPeekSafe(context.TargetProject, "Project/PropertyGroup/TargetFramework") ??
                    context.XmlPeekSafe(directoryBuildProps, "Project/PropertyGroup/TargetFramework")
                    ?? throw new InvalidOperationException("Could not find a target framework.");
            }
            else
            {
                framework = targetFrameworks.Split(';').Last();
            }

            return framework;
        }
    }
}
