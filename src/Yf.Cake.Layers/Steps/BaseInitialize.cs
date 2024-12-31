using Cake.Git;

namespace Yf.Cake.Layers.Steps
{
    public abstract class BaseInitialize : FrostingTask<BuildContext>
    {
        public virtual string[] AdditionalDirectoriesToClean => Array.Empty<string>();

        public override void Run(BuildContext context)
        {
            Clean(context);
            CountLinesOfCode(context);
            CheckSrc(context);
        }

        private void Clean(BuildContext context)
        {
            var binObjDirectories =
                context.GetDirectories("**/node_modules") +
                context.GetDirectories("**/bin") +
                context.GetDirectories("**/obj");

            var additionalDirectories = AdditionalDirectoriesToClean.SelectMany(x => context.GetDirectories(x));
            var directories = binObjDirectories
                .Where(x => !x.FullPath.Contains("/build/"))
                .Append(context.TmpDirectory)
                .Append(context.OutputDirectory)
                .Concat(additionalDirectories);

            foreach (var directory in directories)
            {
                context.Log.Information($"Cleaning: {directory}");
                context.CleanDirectory(directory);
            }
        }

        private static void CountLinesOfCode(BuildContext context)
        {
            context.Log.Information("Counting lines of code...");
            var linesOfCode = 0;
            var files = context
                .GetFiles("./src/**/*.{cs,sql,cshtml,html,css,js}")
                .Select(x => x.FullPath);

            foreach (var file in files)
            {
                var lines = File.ReadAllLines(file);
                context.Log.Information($"{file} -> {lines.Length}");
                linesOfCode += lines.Length;
            }

            var linesOfCodeShort = linesOfCode.Shorten();
            var statusGistJson = new StatusGistJson()
            {
                Label = "loc",
                Message = linesOfCodeShort,
                Color = "blue"
            };

            context.CreateStatusGistJson("lines-of-code", statusGistJson);
            context.SetStepSummaryOutput("Lines of code", linesOfCodeShort);
        }

        private static void CheckSrc(BuildContext context)
        {
            context.Log.Information($"Checking 'src/' for changes...");
            var srcChanges = true;
            if (context.GitIsValidRepository(context.RootDirectory))
            {
                srcChanges = context
                    .GitDiff(context.RootDirectory, "HEAD~1", "HEAD")
                    .Any(x => x.Path.StartsWith("src/", StringComparison.Ordinal));
            }

            context.SetEnvironmentVariable("SRC_CHANGED", srcChanges.ToString().ToLowerInvariant());
        }
    }
}
