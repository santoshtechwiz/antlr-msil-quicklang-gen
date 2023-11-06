using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace parser;
class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllText(@".\input\input.txt");

        var inputStream = new AntlrInputStream(input);
        var lexer = new QuickLangLexer(inputStream);
        var commonTokenStream = new CommonTokenStream(lexer);
        var parser = new QuickLangParser(commonTokenStream);
        var v = new QuickLangMSILCodeGenerator();
        var result = v.Visit(parser.program());
        Console.WriteLine(result);

        File.WriteAllText(@"c:\temp\hello.il", result.ToString());
    }
}

