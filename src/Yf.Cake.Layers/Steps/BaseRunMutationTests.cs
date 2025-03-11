using System.Text.RegularExpressions;

namespace Yf.Cake.Layers.Steps
{
    public abstract class BaseRunMutationTests : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.ExecuteIn("tests", () =>
            {
                context.DotNetTool("tool install dotnet-stryker --create-manifest-if-needed");
                context.DotNetTool("tool run dotnet-stryker " +
                    "--config-file=stryker-config.yaml " +
                    "--skip-version-check " +
                    $"--output=.{context.TestResultsDirectory}");
            });

            var markdownFilePath = FilePath.FromString($"{context.TestResultsDirectory}/reports/mutation-report.md");
            var markdownFile = File.ReadAllText(markdownFilePath.FullPath);
            var mutationScoreMatch = Regex.Match(markdownFile, @"(?<=## The final mutation score is )[.0-9]+");
            var mutationScore = double.Parse(mutationScoreMatch.Value, CultureInfo.InvariantCulture);
            var mutationScorePercent = mutationScore.ToPercent();
            var statusGistJson = new StatusGistJson()
            {
                Label = "mutation score",
                Message = mutationScorePercent,
                Color = mutationScore.MapToColor(70, 85),
            };

            context.CreateStatusGistJson("mutation-score", statusGistJson);
            context.SetStepSummaryOutput("Mutation score", mutationScorePercent);
        }
    }
}
