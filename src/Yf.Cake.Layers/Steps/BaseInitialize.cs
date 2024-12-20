using Cake.Git;

namespace Yf.Cake.Layers.Steps
{
    public abstract class BaseInitialize : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            Clean(context);
            CountLinesOfCode(context);
            CheckSrc(context);
        }

        private static void Clean(BuildContext context)
        {
            var binObjDirectories =
                context.GetDirectories("**/bin") +
                context.GetDirectories("**/obj");

            var directories = binObjDirectories
                .Where(x => !x.FullPath.Contains("/build/"))
                .Append(context.TmpDirectory)
                .Append(context.OutputDirectory);

            foreach (var directory in directories)
            {
                context.Log.Information($"Cleaning: {directory}");
                context.CleanDirectory(directory);
            }
        }

        private static void CountLinesOfCode(BuildContext context)
        {
            context.Log.Information("Counting lines of code...");
            var linesOfCode = context
                .GetFiles("./src/**/*.cs")
                .Select(x => File.ReadAllLines(x.FullPath).Length)
                .Sum()
                .Shorten();

            var statusGistJson = new StatusGistJson()
            {
                Label = "loc",
                Message = linesOfCode,
                Color = "blue"
            };

            context.CreateStatusGistJson("lines-of-code", statusGistJson);
            context.SetStepSummaryOutput("Lines of code", linesOfCode);
        }

        private static void CheckSrc(BuildContext context)
        {
            context.Log.Information($"Checking './src' for changes...");
            var srcChanges = true;
            if (context.GitIsValidRepository(context.Environment.WorkingDirectory))
            {
                srcChanges = context
                    .GitDiff(context.Environment.WorkingDirectory, "HEAD~1", "HEAD")
                    .Any(x => x.Path.Contains("/src/"));
            }

            context.SetOutput("src-changed", srcChanges.ToString().ToLowerInvariant());
        }
    }
}
