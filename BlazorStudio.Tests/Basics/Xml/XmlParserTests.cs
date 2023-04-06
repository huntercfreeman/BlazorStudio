using BlazorStudio.ClassLib.DotNet;
using BlazorStudio.ClassLib.FileSystem.Classes.FilePath;
using BlazorStudio.ClassLib.FileSystem.Classes.Local;
using BlazorStudio.ClassLib.Namespaces;
using BlazorStudio.ClassLib.Xml;
using BlazorTextEditor.RazorLib.Analysis.Html.SyntaxActors;

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

	    var htmlLexer = new TextEditorHtmlLexer();

	    var textSpans = htmlLexer.Lex(ProjectTestData);
	    
	    var htmlSyntaxUnit = HtmlSyntaxTree.ParseText(ProjectTestData);

	    var syntaxNodeRoot = htmlSyntaxUnit.RootTagSyntax;

	    var htmlSyntaxWalker = new HtmlSyntaxWalker();

	    htmlSyntaxWalker.Visit(syntaxNodeRoot);
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