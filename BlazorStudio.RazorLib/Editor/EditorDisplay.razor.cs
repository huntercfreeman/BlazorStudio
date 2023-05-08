using System.Collections.Immutable;
using BlazorCommon.RazorLib.Dimensions;
using BlazorTextEditor.RazorLib.HelperComponents;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Editor;

public partial class EditorDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public ElementDimensions EditorElementDimensions { get; set; } = null!;
    
    private static readonly ImmutableArray<TextEditorHeaderButtonKind> TextEditorHeaderButtonKinds = 
        Enum
            .GetValues(typeof(TextEditorHeaderButtonKind))
            .Cast<TextEditorHeaderButtonKind>()
            .ToImmutableArray();
}