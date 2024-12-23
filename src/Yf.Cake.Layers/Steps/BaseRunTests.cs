using System.Text.Json;
using Cake.Common.Tools.DotNet.Test;
using Cake.Common.Tools.ReportGenerator;

namespace Yf.Cake.Layers.Steps
{
    public abstract class BaseRunTests : FrostingTask<BuildContext>
    {
        public virtual bool CalculateCoverage => true;
        public virtual string? Filter => null;
        public virtual string[] CoverageReportClassFilter => Array.Empty<string>();

        public override void Run(BuildContext context)
        {
            var settings = new DotNetTestSettings()
            {
                Configuration = context.BuildConfiguration,
                NoLogo = true,
                NoRestore = true,
                NoBuild = true,
                Filter = Filter,
                Loggers = new[] { "html" },
                ResultsDirectory = context.TestResultsDirectory
            };

            if (CalculateCoverage)
            {
                settings.Collectors = new[] { "XPlat Code Coverage" };
            }

            context.DotNetTest(context.RootDirectory, settings);

            if (CalculateCoverage)
            {
                GenerateCoverageReport(context);
            }
        }

        private void GenerateCoverageReport(BuildContext context)
        {
            var coverageFiles = context.GetFiles($"{context.TestResultsDirectory}/**/coverage.cobertura.xml");
            var settings = new ReportGeneratorSettings()
            {
                ClassFilters = CoverageReportClassFilter,
                ReportTypes = new[]
                            {
                    ReportGeneratorReportType.Html,
                    ReportGeneratorReportType.JsonSummary
                }
            };

            context.ReportGenerator(coverageFiles, $"{context.TestResultsDirectory}/reports", settings);

            SetOutputs(context);
        }

        public override void OnError(Exception exception, BuildContext context)
        {
            var snapshotsDirectory = $"{context.TestResultsDirectory}/snapshots";
            context.CreateDirectory(snapshotsDirectory);
            context.CopyFiles("./tests/**/*.received.*", snapshotsDirectory, preserveFolderStructure: true);
            context.UploadArtifact(context.TestResultsDirectory, $"TestResults-{context.Timestamp}");

            throw exception;
        }

        private static void SetOutputs(BuildContext context)
        {
            var metricsFile = File.ReadAllText($"{context.TestResultsDirectory}/reports/Summary.json");
            using var jsonDocument = JsonDocument.Parse(metricsFile);
            var coverage = jsonDocument.RootElement
                .GetProperty("summary")
                .GetProperty("linecoverage")
                .GetDouble();

            var coveragePercent = coverage.ToPercent();
            var statusGistJson = new StatusGistJson()
            {
                Label = "test coverage",
                Message = coveragePercent,
                Color = coverage.MapToColor(70, 85)
            };

            context.CreateStatusGistJson("test-coverage", statusGistJson);
            context.SetStepSummaryOutput("Test coverage", coveragePercent);
        }
    }
}
