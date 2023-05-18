using BlazorStudio.ClassLib.Parsing.C;
using BlazorStudio.ClassLib.Parsing.C.SyntaxTokens;

namespace BlazorStudio.Tests.Basics.SemanticParsing.C;

public class USER_STORY_HELLO_WORLD
{
    [Fact]
    public void Enact()
    {
        string sourceText = @"#include <stdio.h>

int main() {
   // printf() displays the string inside quotation
   printf(""Hello, World!"");

   return 0;
}".ReplaceLineEndings("\n");

        var lexer = new Lexer(sourceText);
        
        lexer.Lex();

        throw new NotImplementedException("TODO: Perform assertions");
    }
}