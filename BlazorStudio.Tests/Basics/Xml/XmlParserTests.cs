using BlazorStudio.ClassLib.DotNet;
using BlazorStudio.ClassLib.FileSystem.Classes.FilePath;
using BlazorStudio.ClassLib.FileSystem.Classes.Local;
using BlazorStudio.ClassLib.Namespaces;

namespace BlazorStudio.Tests.Basics.Xml;

public class XmlParserTests
{
    [Fact]
    public void AdhocTest()
    {
	    var localEnvironmentProvider = new LocalEnvironmentProvider();
	    
	    var projectAbsoluteFilePathString = @"C:\Users\hunte\Repos\Demos\BlazorCrudApp\BlazorCrudApp\BlazorCrudApp.csproj";

	    var projectAbsoluteFilePath = new AbsoluteFilePath(
		    projectAbsoluteFilePathString,
		    false,
		    localEnvironmentProvider);
	    
	    var project = DotNetSolutionParser.Parse(
		    ProjectTestData,
		    new NamespacePath("", projectAbsoluteFilePath),
		    localEnvironmentProvider);
    }

    private const string ProjectTestData = @"<Project Sdk=""Microsoft.NET.Sdk.Web"">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

	<ItemGroup>
		<PackageReference Include=""Fluxor.Blazor.Web"" Version=""5.7.0"" />
	</ItemGroup>
</Project>
";
}