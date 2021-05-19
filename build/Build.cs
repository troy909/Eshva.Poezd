using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations(TimeoutInMilliseconds = 2000)]
// [ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Restore);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion(NoFetch = true, Framework = "netcoreapp3.1")] readonly GitVersion GitVersion;

    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    /*
    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            TestsDirectory.GlobDirectories("*#1#bin", "*#1#obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });
        */

    Target Restore => _ => _
        .Executes(() =>
        {
          DotNetRestore(settings => settings
            .SetForce(false)
            .SetUseLockFile(false)
            .SetProjectFile(Solution));
        });

    /*
    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
          DotNetBuild(s => s
            .SetProjectFile(Solution)
            .SetConfiguration(Configuration)
            .SetAssemblyVersion(GitVersion.AssemblySemVer)
            .SetFileVersion(GitVersion.AssemblySemFileVer)
            .SetInformationalVersion(GitVersion.InformationalVersion)
            .EnableNoRestore());
        });

    Target TestUnits => _ => _
      .DependsOn(Compile)
      .Executes(() =>
      {
        DotNetTest(settings => settings
          .SetProjectFile(Solution)
          .SetConfiguration(Configuration)
          // .SetFramework("netcoreapp3.1")
          .EnableNoBuild()
          .SetFilter("FullyQualifiedName~UnitTests")
          .SetVerbosity(DotNetVerbosity.Normal)
          .SetLogger("trx"));
      });

    Target Package => _ => _
      .DependsOn(Compile)
      .After(TestUnits)
      .Executes(() =>
        DotNetPack(settings => settings
          .SetProject(Solution.Directory / "sources" / "Eshva.Poezd.Core")
          .SetConfiguration(Configuration)
          .SetOutputDirectory(ArtifactsDirectory)
          .EnableNoBuild()
          .EnableNoRestore()
          .SetAuthors("Mike Eshva")));
          */

}
