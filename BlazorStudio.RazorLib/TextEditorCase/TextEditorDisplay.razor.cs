using BlazorStudio.ClassLib.RoslynHelpers;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Store.TextEditorCase;
using BlazorStudio.ClassLib.TextEditor;
using BlazorStudio.ClassLib.TextEditor.Cursor;
using BlazorStudio.ClassLib.TextEditor.Enums;
using BlazorStudio.ClassLib.TextEditor.IndexWrappers;
using BlazorStudio.RazorLib.ShouldRender;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace BlazorStudio.RazorLib.TextEditorCase;

public partial class TextEditorDisplay : FluxorComponent
{
    [Inject]
    private IStateSelection<TextEditorStates, TextEditorBase> TextEditorStatesSelection { get; set; } = null!;

    [Parameter, EditorRequired]
    public TextEditorKey TextEditorKey { get; set; } = null!;

    private bool _colorFlip;
    private TextPartition? _textPartition;
    private SequenceKey _previousTextPartitionSequenceKey = SequenceKey.Empty();
    private TextCursor _cursor = new();
    private TextEditorCursorDisplay? _textEditorCursorDisplay;
    
    private string BackgroundColor => GetBackgroundColor();

    protected override void OnInitialized()
    {
        TextEditorStatesSelection
            .Select(x => x.TextEditorMap[TextEditorKey]);
        
        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // I don't want the IMMUTABLE state changing due to Blazor using a MUTABLE reference.
            var localTextEditorStates = TextEditorStatesSelection.Value;

            _textPartition = localTextEditorStates.GetTextPartition(new RectangularCoordinates(
                TopLeftCorner: (new(0), new(0)),
                BottomRightCorner: (new(Int32.MaxValue), new(10))));
            
            await InvokeAsync(StateHasChanged);
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }

    private string GetBackgroundColor()
    {
        var localColorFlip = _colorFlip;
        _colorFlip = !_colorFlip;

        return localColorFlip
            ? "var(--bstudio_primary-background-color)"
            : "var(--bstudio_secondary-background-color)";
    }
    
    private bool ShouldRenderFunc(ShouldRenderBoundary.IsFirstShouldRenderValue firstShouldRender)
    {
        var shouldRender = _textPartition is not null &&
               _previousTextPartitionSequenceKey != _textPartition.SequenceKey;

        _previousTextPartitionSequenceKey = _textPartition?.SequenceKey ?? SequenceKey.Empty();

        return shouldRender;
    }

    private async Task ApplyRoslynSyntaxHighlightingAsyncOnClick()
    {
        // I don't want the IMMUTABLE state changing due to Blazor using a MUTABLE reference.
        var localTextEditorState = TextEditorStatesSelection.Value;

        var text = localTextEditorState.GetAllText();

        var generalSyntaxCollector = new GeneralSyntaxCollector();

        var syntaxTree = CSharpSyntaxTree.ParseText(text);

        var syntaxNodeRoot = await syntaxTree.GetRootAsync();

        generalSyntaxCollector.Visit(syntaxNodeRoot);

        ApplyDecorations(localTextEditorState, generalSyntaxCollector);
        
        _previousTextPartitionSequenceKey = SequenceKey.Empty();
    }

    private void ApplyDecorations(TextEditorBase localTextEditorStates, GeneralSyntaxCollector generalSyntaxCollector)
    {
        // Type decorations
        {
            // Property Type
            localTextEditorStates.ApplyDecorationRange(
                DecorationKind.Type,
                generalSyntaxCollector.PropertyDeclarations
                    .Select(pds => pds.Type.Span));

            // Class Declaration
            localTextEditorStates.ApplyDecorationRange(
                DecorationKind.Type,
                generalSyntaxCollector.ClassDeclarations
                    .Select(cd => cd.Identifier.Span));

            // Method return Type
            localTextEditorStates.ApplyDecorationRange(
                DecorationKind.Type,
                generalSyntaxCollector.MethodDeclarations
                    .Select(md =>
                    {
                        var retType = md
                            .ChildNodes()
                            .FirstOrDefault(x => x.Kind() == SyntaxKind.IdentifierName);

                        return retType?.Span ?? default;
                    }));
            
            // Parameter declaration Type
            localTextEditorStates.ApplyDecorationRange(
                DecorationKind.Type,
                generalSyntaxCollector.ParameterDeclarations
                    .Select(pd =>
                    {
                        var identifierNameNode = pd.ChildNodes()
                            .FirstOrDefault(x => x.Kind() == SyntaxKind.IdentifierName);

                        if (identifierNameNode is null)
                        {
                            return TextSpan.FromBounds(0, 0);
                        }
                
                        return identifierNameNode.Span;
                    }));
        }

        // Method decorations
        {
            // Method declaration identifier
            localTextEditorStates.ApplyDecorationRange(
                DecorationKind.Method,
                generalSyntaxCollector.MethodDeclarations
                    .Select(md => md.Identifier.Span));
            
            // InvocationExpression
            localTextEditorStates.ApplyDecorationRange(
                DecorationKind.Method,
                generalSyntaxCollector.InvocationExpressions
                    .Select(md =>
                    {
                        var childNodes = md.Expression.ChildNodes();

                        var lastNode = childNodes.LastOrDefault();

                        return lastNode?.Span ?? TextSpan.FromBounds(0, 0);
                    }));
        }

        // Local variable decorations
        {
            // Parameter declaration identifier
            localTextEditorStates.ApplyDecorationRange(
                DecorationKind.AltFlagOne | DecorationKind.Variable,
                generalSyntaxCollector.ParameterDeclarations
                    .Select(pd =>
                    {
                        var identifierToken = pd.ChildTokens().FirstOrDefault(x => x.Kind() == SyntaxKind.IdentifierToken);
                
                        return identifierToken.Span;
                    }));
            
            // Argument declaration identifier
            localTextEditorStates.ApplyDecorationRange(
                DecorationKind.AltFlagOne | DecorationKind.Variable,
                generalSyntaxCollector.ArgumentDeclarations
                    .Select(ad => ad.Span));
        }

        // String literal
        {
            // String literal
            localTextEditorStates.ApplyDecorationRange(
                DecorationKind.AltFlagOne | DecorationKind.Constant,
                generalSyntaxCollector.StringLiteralExpressions
                    .Select(sl => sl.Span));
        }

        // Keywords
        {
            localTextEditorStates.ApplyDecorationRange(
                DecorationKind.Keyword,
                generalSyntaxCollector.Keywords
                    .Select(keyword => keyword.Span));
        }

        // Comments
        {
            localTextEditorStates.ApplyDecorationRange(
                DecorationKind.AltFlagTwo | DecorationKind.Method,
                generalSyntaxCollector.TriviaComments
                    .Select(tc => tc.Span));
            
            localTextEditorStates.ApplyDecorationRange(
                DecorationKind.AltFlagTwo | DecorationKind.Method,
                generalSyntaxCollector.XmlComments
                    .Select(xml => xml.Span));
        }
    }

    private void HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (_textEditorCursorDisplay is not null)
        {
            _textEditorCursorDisplay.MoveCursor(keyboardEventArgs);
        }
    }
    
    private async Task HandleOnClick(MouseEventArgs mouseEventArgs)
    {
        if (_textEditorCursorDisplay is not null)
        {
            await _textEditorCursorDisplay.FocusAsync();
        }
    }
}