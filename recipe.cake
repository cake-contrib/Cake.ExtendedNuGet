#load nuget:?package=Cake.Recipe&version=1.0.0

Environment.SetVariableNames();

BuildParameters.SetParameters(context: Context,
                            buildSystem: BuildSystem,
                            sourceDirectoryPath: "./source/",
                            title: "Cake.ExtendedNuGet",
                            repositoryOwner: "redth",
                            repositoryName: "Cake.ExtendedNuGet",
                            appVeyorAccountName: "redth",
                            shouldRunCodecov: false,
                            shouldRunDotNetCorePack: true,
                            shouldRunDupFinder: false,
                            shouldRunInspectCode: false,
                            shouldRunIntegrationTests: false,
                            testFilePattern: "DO_NOT_RUN_TESTS",
                            shouldRunGitVersion: true);

BuildParameters.PrintParameters(Context);

ToolSettings.SetToolSettings(context: Context,
                            dupFinderExcludePattern: new string[] {
                                BuildParameters.RootDirectoryPath + "/Cake.ExtendedNuGet.Tests/*.cs" },
                            testCoverageFilter: "+[*]* -[xunit.*]* -[Cake.Core]* -[Cake.Testing]* -[*.Tests]* -[FakeItEasy]*",
                            testCoverageExcludeByAttribute: "*.ExcludeFromCodeCoverage*",
                            testCoverageExcludeByFile: "*/*Designer.cs;*/*.g.cs;*/*.g.i.cs");
Build.RunDotNetCore();
