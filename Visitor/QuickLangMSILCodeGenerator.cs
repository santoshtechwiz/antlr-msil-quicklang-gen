
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

public class QuickLangMSILCodeGenerator : QuickLangBaseVisitor<object>
{

    // program       : 
    //               declaration*
    //               statement*
    //               ;

    //global variable
    StringBuilder sb = new StringBuilder();
    private List<string> _variables = new List<string>();
    private HashSet<string> localStack = new HashSet<string>();
    private string statemnt = "";
    private string declaration = "";
    public override object VisitProgram([NotNull] QuickLangParser.ProgramContext context)
    {
        var sb = new StringBuilder();
        sb.AppendLine(".assembly extern mscorlib { }");
        sb.AppendLine(".assembly hello { }");
        sb.AppendLine(".module hello.exe");
        sb.AppendLine(".method public static void main() cil managed");
        sb.AppendLine("{");
        sb.AppendLine(".entrypoint");
        sb.AppendLine(".maxstack 8");
        sb.AppendLine(".locals init (");

        //generate code for declaration and statement using visitor pattern
        foreach (var declaration in context.declaration())
        {
            statemnt = Visit(declaration)?.ToString();
        }
        foreach (var statement in context.statement())
        {
            declaration = Visit(statement)?.ToString();
        }
        int stackCount =0;
        //build local stack with proper index for each variable
        foreach (var item in localStack)
        {
            sb.AppendLine("[" + stackCount + "]  int32  " + item);
            sb.Append(", ");
            stackCount++;
        }
        //remove last comma
        sb.Remove(sb.Length - 2, 2);
     

        sb.AppendLine(")");
        sb.AppendLine(statemnt);
        sb.AppendLine(declaration);
        sb.AppendLine("ret");
        sb.AppendLine("}");
        return sb.ToString();
    }

    // declaration   : 
    //           INT NAME SEMICOLON 
    //           ;

    public override object VisitDeclaration([NotNull] QuickLangParser.DeclarationContext context)
    {

        _variables.Add(context.NAME().GetText());
        localStack.Add(context.NAME().GetText());

        sb.AppendLine("ldc.i4 0");
        sb.AppendLine("stloc " + context.NAME().GetText());
        return sb.ToString();
    }



    // statement      : 
    //            ifstmt 
    //          | printstmt 
    //          | assignstmt 
    //            ;


    public override object VisitStatement([NotNull] QuickLangParser.StatementContext context)
    {
        if (context.ifstmt() != null)
        {
            return Visit(context.ifstmt());
        }
        else if (context.printstmt() != null)
        {
            return Visit(context.printstmt());
        }
        else
        {
            return Visit(context.assignstmt());
        }
    }


    // printstmt      : 
    //            PRINT term SEMICOLON 

    //             ;

    public override object VisitPrintstmt([NotNull] QuickLangParser.PrintstmtContext context)
    {
        Visit(context.term());
        sb.AppendLine("call void [mscorlib]System.Console::WriteLine(int32)");

        return sb.ToString();
    }


    // identifier   : NAME  ;

    public override object VisitIdentifier([NotNull] QuickLangParser.IdentifierContext context)
    {
       //if variable is declared then load it
        if (_variables.Contains(context.NAME().GetText()))
        {
            sb.AppendLine("ldloc " + context.NAME().GetText());
        }
        //if it is new variable then declare it
        else
        {
            sb.AppendLine("ldc.i4 0");
            sb.AppendLine("stloc " + context.NAME().GetText());
        }
        return sb.ToString();


    }


    // integer      : INTEGER  ;


    public override object VisitInteger([NotNull] QuickLangParser.IntegerContext context)
    {
        return sb.AppendLine("ldc.i4 " + context.INTEGER().GetText());
    }

    // expression      : 
    //                 term
    //               | 
    //                 term PLUS term 
    //                 ;

    public override object VisitExpression([NotNull] QuickLangParser.ExpressionContext context)
    {
        //if expression is of type term then visit term

        if (context.term().Length == 1)
        {
            return Visit(context.term(0));
        }

        //if expression is of type term PLUS term then visit term PLUS term
        else
        {
            Visit(context.term(0));
            Visit(context.term(1));
            sb.AppendLine("add");
            return sb.ToString();
        }

    }

    // term          : 
    //           identifier
    //         | integer 
    //           ;

    public override object VisitTerm([NotNull] QuickLangParser.TermContext context)
    {
        //if variable is a local variable then load it from stack 

        if (localStack.Contains(context.GetText()))
        {
            return sb.AppendLine("ldloc " + context.GetText());
        }

        //if term is of type identifier then visit identifier
        if (context.identifier() != null)
        {
            return Visit(context.identifier());
        }
        //if term is of type integer then visit integer
        else
        {
            return Visit(context.integer());
        }
    }

    // assignstmt      : 
    //             NAME ASSIGN expression SEMICOLON 
    //             ;

    public override object VisitAssignstmt([NotNull] QuickLangParser.AssignstmtContext context)
    {
        //if identifier is not declared then throw exception
        if (!_variables.Contains(context.NAME().GetText()))
        {
            throw new Exception("Variable not declared");
        }
        //if it is declare then update the value from local variable
        else
        {
            Visit(context.expression());
            sb.AppendLine("stloc " + context.NAME().GetText());
            return sb.ToString();
        }

    }

    // ifstmt      : 
    //         IF LPAREN identifier EQUAL integer RPAREN
    //         statement*
    //         ENDIF
    //         ;

    int _labelCount = 0;
    public override object VisitIfstmt([NotNull] QuickLangParser.IfstmtContext context)
    {
        //output is in the form of IL code it shoud be 4 and 6
        var label1 = _labelCount++;
        var label2 = _labelCount++;
        //visit identifier and integer
        Visit(context.identifier());
        Visit(context.integer());
        //compare identifier and integer
        sb.AppendLine("beq L" + label1);
        //if identifier and integer are not equal then jump to label1
        sb.AppendLine("br L" + label2);
        sb.AppendLine("L" + label1 + ":");
        //visit statement
        foreach (var statement in context.statement())
        {
            Visit(statement);
        }
        sb.AppendLine("L" + label2 + ":");
        return sb.ToString();



    }
}