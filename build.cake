#tool nuget:?package=xunit.runner.console&version=2.2.0
#tool nuget:?package=xunit.runner.visualstudio&version=2.2.0

#load cake/paths.cake

var target = Argument("Target", "Build");
var configuration = Argument("Configuration", "Release");


Task("Restore")
    .Does(() =>
{
    NuGetRestore(Paths.SolutionFile);
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    MSBuild(
        Paths.SolutionFile,
        settings => settings.SetConfiguration(configuration)
                            .WithTarget("Build"));
});



RunTarget(target);
