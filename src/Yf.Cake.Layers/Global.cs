global using System.Globalization;
global using Cake.Common.IO;
global using Cake.Common.Tools.DotNet;
global using Cake.Common.Tools.DotNet.Build;
global using Cake.Common.Tools.DotNet.MSBuild;
global using Cake.Core.Diagnostics;
global using Cake.Core.IO;
global using Cake.Frosting;

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Performance",
    "CA1861:Avoid constant arrays as arguments",
    Justification = "The invocation only runs once.",
    Scope = "member",
    Target = "~M:Yf.Cake.Layers.Steps.BaseRunTests.Run(Yf.Cake.Layers.BuildContext)")]

[assembly: SuppressMessage(
    "Performance",
    "SYSLIB1045:Convert to 'GeneratedRegexAttribute'.",
    Justification = "Regex performance is not an issue.",
    Scope = "member",
    Target = "~M:Yf.Cake.Layers.Steps.BaseRunMutationTests.Run(Yf.Cake.Layers.BuildContext)")]

