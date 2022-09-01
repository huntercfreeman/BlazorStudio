using System.Collections.Immutable;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.RoslynHelpers;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Store.TextEditorCase;
using BlazorStudio.ClassLib.TextEditor;
using BlazorStudio.ClassLib.TextEditor.Cursor;
using BlazorStudio.ClassLib.TextEditor.Enums;
using BlazorStudio.ClassLib.TextEditor.IndexWrappers;
using BlazorStudio.RazorLib.CustomEvents;
using BlazorStudio.RazorLib.CustomJavaScriptDtos;
using BlazorStudio.RazorLib.ShouldRender;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.JSInterop;

namespace BlazorStudio.RazorLib.TextEditorCase;

public partial class TextEditorDisplay : FluxorComponent
{
    [Inject]
    private IStateSelection<TextEditorStates, TextEditorBase> TextEditorStatesSelection { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    [Parameter, EditorRequired]
    public TextEditorKey TextEditorKey { get; set; } = null!;

    private Guid _textEditorGuid = Guid.NewGuid();
    private TextPartition? _textPartition;
    private SequenceKey _previousTextPartitionSequenceKey = SequenceKey.Empty();
    private TextEditorKey _previousTextEditorKey = TextEditorKey.Empty();
    private TextCursor _cursor = new();
    private TextEditorCursorDisplay? _textEditorCursorDisplay;
    private Virtualize<TextCharacterSpan>? _virtualize;
    private RelativeCoordinates? _mostRecentRelativeCoordinates;
    private int _temporaryHardCodeRowHeight = 39;
    private int _temporaryHardCodeCharacterWidth = 16;

    private string GetTextEditorElementId => $"bstudio_{_textEditorGuid}";
    
    protected override void OnInitialized()
    {
        TextEditorStatesSelection
            .Select(x =>
                x.TextEditors.SingleOrDefault(x => x.TextEditorKey == TextEditorKey));

        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_previousTextEditorKey != TextEditorStatesSelection.Value.TextEditorKey &&
            _virtualize is not null)
        {
            await _virtualize.RefreshDataAsync();

            _previousTextEditorKey = TextEditorStatesSelection.Value.TextEditorKey;

            await InvokeAsync(StateHasChanged);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private bool ShouldRenderFunc(ShouldRenderBoundary.IsFirstShouldRenderValue firstShouldRender)
    {
        var shouldRender = TextEditorStatesSelection.Value is not null &&
                           _textPartition is not null &&
                           _previousTextPartitionSequenceKey != _textPartition.SequenceKey;

        _previousTextPartitionSequenceKey = _textPartition?.SequenceKey ?? SequenceKey.Empty();

        return shouldRender;
    }

    /// <summary>
    /// For multi cursor I imagine one would foreach() loop
    /// </summary>
    private void HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (_textEditorCursorDisplay is null)
            return;

        if (KeyboardKeyFacts.IsMovementKey(keyboardEventArgs.Key))
        {
            _textEditorCursorDisplay.MoveCursor(keyboardEventArgs);
        }
        else
        {
            Dispatcher.Dispatch(new TextEditorEditAction(
                TextEditorKey,
                new[] { (new ImmutableTextCursor(_cursor), _cursor) }.ToImmutableArray(),
                keyboardEventArgs,
                CancellationToken.None));

            if (_virtualize is not null)
                _virtualize.RefreshDataAsync();

            StateHasChanged();
        }
    }
    
    private async Task HandleOnCustomClick(CustomOnClick customOnClick)
    {
        var localTextEditorState = TextEditorStatesSelection.Value;

        if (_textEditorCursorDisplay is not null)
        {
            await _textEditorCursorDisplay.FocusAsync();
        }

        _mostRecentRelativeCoordinates = await JsRuntime
            .InvokeAsync<RelativeCoordinates>("blazorStudio.getRelativeClickPosition",
                GetTextEditorElementId,
                customOnClick.ClientX,
                customOnClick.ClientY);

        var columnIndexDouble = _mostRecentRelativeCoordinates.RelativeX / _temporaryHardCodeCharacterWidth;
        var columnIndexRounded = Math.Round(columnIndexDouble, MidpointRounding.AwayFromZero);

        var rowIndex = new RowIndex((int)_mostRecentRelativeCoordinates.RelativeY / _temporaryHardCodeRowHeight);

        if (rowIndex.Value >= localTextEditorState.LineEndingPositions.Length &&
            localTextEditorState.LineEndingPositions.Length > 0)
        {
            rowIndex = new(localTextEditorState.LineEndingPositions.Length - 1);
        }
        else
        {
            rowIndex = new(0);
        }        
        
        var columnIndex = new ColumnIndex((int)columnIndexRounded);

        var rowLength = TextEditorBase
            .GetLengthOfRow(rowIndex, localTextEditorState.LineEndingPositions);

        if (columnIndex.Value >= rowLength &&
            rowLength != 0)
        {
            columnIndex = new(rowLength - 1);
        }
        else
        {
            columnIndex = new(0);
        }
        
        var cursorIndexCoordinates = (rowIndex, columnIndex);

        _cursor.PreferredColumnIndex = columnIndex;
        _cursor.IndexCoordinates = cursorIndexCoordinates;
        
        _previousTextPartitionSequenceKey = SequenceKey.Empty();
    }

    private async ValueTask<ItemsProviderResult<TextCharacterSpan>> LoadTextCharacterSpans(
        ItemsProviderRequest request)
    {
        var localTextEditorState = TextEditorStatesSelection.Value;

        var numTextCharacterSpans = Math
            .Min(
                request.Count,
                localTextEditorState.LineEndingPositions.Length - request.StartIndex);

        _textPartition = localTextEditorState.GetTextPartition(new RectangularCoordinates(
            TopLeftCorner: (new(request.StartIndex), new(0)),
            BottomRightCorner: (new(request.StartIndex + numTextCharacterSpans), new(10))));

        return new ItemsProviderResult<TextCharacterSpan>(_textPartition.TextSpanRows,
            localTextEditorState.LineEndingPositions.Length);
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
                        var identifierToken =
                            pd.ChildTokens().FirstOrDefault(x => x.Kind() == SyntaxKind.IdentifierToken);

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
}