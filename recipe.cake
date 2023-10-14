#load nuget:?package=Cake.Recipe&version=3.1.1

Environment.SetVariableNames();

BuildParameters.SetParameters(context: Context,
                            buildSystem: BuildSystem,
                            sourceDirectoryPath: "./source/",
                            title: "Cake.ExtendedNuGet",
                            repositoryOwner: "cake-contrib",
                            repositoryName: "Cake.ExtendedNuGet",
                            shouldRunDotNetCorePack: true,
                            shouldRunInspectCode: false,
                            shouldRunIntegrationTests: false,
                            testFilePattern: "DO_NOT_RUN_TESTS",
                            preferredBuildProviderType: BuildProviderType.GitHubActions,
                            preferredBuildAgentOperatingSystem: PlatformFamily.Linux);

BuildParameters.PrintParameters(Context);

ToolSettings.SetToolSettings(context: Context,
                            testCoverageFilter: "+[*]* -[xunit.*]* -[Cake.Core]* -[Cake.Testing]* -[*.Tests]* -[FakeItEasy]*",
                            testCoverageExcludeByAttribute: "*.ExcludeFromCodeCoverage*",
                            testCoverageExcludeByFile: "*/*Designer.cs;*/*.g.cs;*/*.g.i.cs");
Build.RunDotNetCore();
