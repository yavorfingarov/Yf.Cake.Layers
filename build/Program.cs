using Cake.Frosting;
using Yf.Cake.Layers;
using Yf.Cake.Layers.Steps;

namespace Build
{
    public static class Program
    {
        public static int Main(string[] args) => Runner.Run(args);

        public sealed class Initialize : BaseInitialize
        {
        }

        [IsDependentOn(typeof(Initialize))]
        public sealed class RestoreNuGetPackages : BaseRestoreNuGetPackages
        {
        }

        [IsDependentOn(typeof(RestoreNuGetPackages))]
        public sealed class CheckCodeStyle : BaseCheckCodeStyle
        {
        }

        [IsDependentOn(typeof(CheckCodeStyle))]
        public sealed class Build : BaseBuild
        {
            public override bool RunCalculateMetrics => false;
        }

        [IsDependentOn(typeof(Build))]
        public sealed class CreatePackage : BasePack
        {
        }
    }
}
