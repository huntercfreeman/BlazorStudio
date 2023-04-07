using BlazorStudio.ClassLib.DotNet;
using BlazorStudio.ClassLib.FileSystem.Classes.FilePath;
using BlazorStudio.ClassLib.FileSystem.Classes.Local;
using BlazorStudio.ClassLib.Namespaces;

namespace BlazorStudio.Tests.Basics.DotNet;

public class DotNetSolutionParserTests
{
    [Fact]
    public void AdhocTest()
    {
	    var localEnvironmentProvider = new LocalEnvironmentProvider();
	    
	    var solutionAbsoluteFilePathString = @"C:\Users\hunte\Repos\BlazorApp1\BlazorApp1.sln";

	    var solutionAbsoluteFilePath = new AbsoluteFilePath(
		    solutionAbsoluteFilePathString,
		    false,
		    localEnvironmentProvider);
	    
	    var solution = DotNetSolutionParser.Parse(
		    SOLUTION_TEST_DATA,
		    new NamespacePath("", solutionAbsoluteFilePath),
		    localEnvironmentProvider);
    }

    private const string SOLUTION_TEST_DATA = @"
Microsoft Visual Studio Solution File, Format Version 12.00
Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = ""BlazorApp1"", ""BlazorApp1\BlazorApp1.csproj"", ""{510BA6A0-B1B5-4D57-AB74-EA7ED2127ED4}""
EndProject
Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = ""BlazorCommon.RazorLib"", ""..\BlazorCommon\BlazorCommon.RazorLib\BlazorCommon.RazorLib.csproj"", ""{2F763B00-22EC-4566-B27A-C3D6367AC8F0}""
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{510BA6A0-B1B5-4D57-AB74-EA7ED2127ED4}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{510BA6A0-B1B5-4D57-AB74-EA7ED2127ED4}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{510BA6A0-B1B5-4D57-AB74-EA7ED2127ED4}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{510BA6A0-B1B5-4D57-AB74-EA7ED2127ED4}.Release|Any CPU.Build.0 = Release|Any CPU
		{2F763B00-22EC-4566-B27A-C3D6367AC8F0}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{2F763B00-22EC-4566-B27A-C3D6367AC8F0}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{2F763B00-22EC-4566-B27A-C3D6367AC8F0}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{2F763B00-22EC-4566-B27A-C3D6367AC8F0}.Release|Any CPU.Build.0 = Release|Any CPU
	EndGlobalSection
EndGlobal
";
}