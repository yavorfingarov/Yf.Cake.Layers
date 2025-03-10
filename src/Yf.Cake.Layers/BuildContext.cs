using System.Text.Json;
using Cake.Common.Build;
using Cake.Common.Build.GitHubActions;
using Cake.Core;
using Cake.Core.IO;

namespace Yf.Cake.Layers
{
    public class BuildContext : FrostingContext
    {
        private static readonly JsonSerializerOptions _JsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public string Timestamp { get; }
        public string BuildConfiguration { get; }
        public string RootDirectory { get; set; }
        public string OutputDirectory { get; }
        public string TestResultsDirectory { get; }
        public string TmpDirectory { get; }
        public string TargetProject { get; }

        private readonly ICakeContext _Context;
        private readonly IGitHubActionsProvider _GitHubActions;

        public BuildContext(ICakeContext context)
            : base(context)
        {
            context.Environment.WorkingDirectory = context.Environment.WorkingDirectory.GetParent();
            BuildConfiguration = "Release";
            TargetProject = context
                .GetFiles("./src/*/*.csproj")
                .Single()
                .FullPath;

            RootDirectory = context.Environment.WorkingDirectory.FullPath;
            OutputDirectory = "./output";
            TestResultsDirectory = "./tmp/test-results";
            TmpDirectory = "./tmp";
            _Context = context;
            _GitHubActions = context.GitHubActions();
            var now = DateTime.UtcNow;
            Timestamp = $"{now:yyyy}.{now:MM}.{now:dd}.{now:HH}{now:mm}";
        }

        public void CreateStatusGistJson(string filename, StatusGistJson statusGistJson)
        {
            var content = JsonSerializer.Serialize(statusGistJson, _JsonSerializerOptions);
            File.WriteAllText($"{TmpDirectory}/{filename}.json", content);
        }

        public void SetEnvironmentVariable(string key, string value)
        {
            if (_GitHubActions.IsRunningOnGitHubActions)
            {
                _GitHubActions.Commands.SetEnvironmentVariable(key, value);
            }
            else
            {
                Log.Information($"[Environment] {key}={value}");
            }
        }

        public void SetStepSummaryOutput(string label, string value)
        {
            if (_GitHubActions.IsRunningOnGitHubActions)
            {
                _GitHubActions.Commands.SetStepSummary($"* {label}: **{value}**");
            }
            else
            {
                Log.Information($"[StepSummary] {label}: {value}");
            }
        }

        public void UploadArtifact(DirectoryPath directory, string filename)
        {
            if (_GitHubActions.IsRunningOnGitHubActions)
            {
                _GitHubActions.Commands
                    .UploadArtifact(directory, filename)
                    .GetAwaiter()
                    .GetResult();
            }
            else
            {
                Log.Information($"[Upload] {directory} -> {filename}");
            }
        }

        public void ExecuteIn(string directory, Action action)
        {
            var originalWorkingDirectory = _Context.Environment.WorkingDirectory;
            _Context.Environment.WorkingDirectory = _Context.Environment.WorkingDirectory.Combine(directory);
            try
            {
                action.Invoke();
            }
            finally
            {
                _Context.Environment.WorkingDirectory = originalWorkingDirectory;
            }
        }
    }
}
