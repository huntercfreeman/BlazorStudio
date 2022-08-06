using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;

namespace BlazorStudio.RazorLib.SyntaxRootRender;

public partial class SyntaxRootDisplay : ComponentBase
{
    [Parameter]
    public SyntaxNode SyntaxNode { get; set; } = null!;
}