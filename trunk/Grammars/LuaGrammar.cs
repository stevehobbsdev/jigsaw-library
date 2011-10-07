using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    /// <summary>
    /// TODO: finish. Many rules are out of order, and the grammar has to be tested.
    /// http://www.lua.org/manual/5.1/manual.html#8
    /// </summary>
    class LuaGrammar : SharedGrammar
    {
        public static Rule Nil = Node(Keyword("Nil"));
        public static Rule True = Node(Keyword("True"));       
        public static Rule False = Node(Keyword("False"));       
        public static Rule Number = Node(Integer | Float);
        public static Rule String = JsonGrammar.String;

        public static Rule Name = Node(Identifier);
        public static Rule NameList = Node(Name + ZeroOrMore(Comma + Name));
	    public static Rule UnaryOp = Node(MatchStringSet("- not #"));
	    public static Rule BinaryOp = Node(MatchStringSet(".. < <= > >= == ~= and or + - * / ^ %"));
        public static Rule Ellipsis = StringToken("...");
        public static Rule ParanExpr = Node(Parenthesize(Expr));
	    public static Rule Args = Node(Parenthesize(Opt(ExprList)) | Table | String); 
	    public static Rule FunCall = Node(PrefixExpr + Opt(CharToken(':') + Name));
        public static Rule PrefixExpr = Node(Var | FunCall | ParanExpr);
        public static Rule Params = Node((NameList + Opt(Comma + Ellipsis)) | Ellipsis);
        public static Rule FuncBody = Node(Parenthesize(Opt(Params)) + Block + WS + Keyword("end"));	    
	
	    public static Rule FieldSep = Comma | Eos;
        public static Rule IndexedField = Node(CharToken('[') + Expr + WS + CharToken(']') + Eq + Expr);
        public static Rule NamedField = Node(Name + CharToken('=') + Expr);
        public static Rule SimpleField = Node(Expr);
	    public static Rule Field = Node(IndexedField | NamedField | SimpleField);
	    public static Rule FieldList = Field + ZeroOrMore(FieldSep + Field) + Opt(FieldSep);
        public static Rule Table = Node(CharToken('{') + Opt(FieldList) + WS + CharToken('}'));

        public static Rule Block = Node(ZeroOrMore(Statement + Opt(Eos)) + Opt(LastStat + Opt(Eos))); 
        
        public static Rule DoStatement = Node(Keyword("do") + Block + Keyword("end"));    
        public static Rule AssignmentStatement = Node(VarList + Eq + ExprList);
        public static Rule FunCallStatement = Node(FunCall);
        public static Rule WhileStatement = Node(Keyword("while") + Expr + WS + DoStatement);
        public static Rule RepeatStatement = Node(Keyword("repeat") + Block + Keyword("until") + Expr);
        public static Rule ForArgs = Node(Name + WS + Eq + Expr + WS + Comma + Expr + WS + Opt(Comma + Expr));
        public static Rule ForStatement = Node(Keyword("for") + ForArgs + DoStatement);
        public static Rule ForInStatement = Node(Keyword("for") + NameList + Keyword("in") + ExprList + DoStatement);
        public static Rule FunDefStatement = Node(Keyword("function") + FuncName + WS + FuncBody);
        public static Rule ElseStatement = Node(Keyword("else") + Block);
        public static Rule ElseIfStatement = Node(Keyword("elseif") + Expr + WS + Keyword("then") + Block);
        public static Rule IfStatement = Node(Keyword("if") + Expr + Keyword("then") + Block + ZeroOrMore(ElseIfStatement) + Opt(ElseStatement) + Keyword("end"));
        public static Rule LocalFunDefStatement = Node(Keyword("local") + Keyword("function") + Name + WS + FuncBody);
        public static Rule VarDeclStatement = Node(Keyword("local") + NameList + Opt(Eq + ExprList));
        public static Rule Statement = Node(AssignmentStatement | DoStatement | WhileStatement | RepeatStatement | IfStatement | ForInStatement | ForStatement | FunDefStatement | LocalFunDefStatement | VarDeclStatement + WS);
    
        public static Rule LastStat = Node((StringToken("return") + Opt(ExprList)) | StringToken("break"));
	    public static Rule FuncName = Node(Name + WS + ZeroOrMore(CharToken('.') + Name + WS) + Opt(CharToken(':') + Name));
	    public static Rule Var = Node(Name | (PrefixExpr + CharToken('[') + Exp + CharToken(']')) | (PrefixExpr + CharToken('.') + Name));
	    public static Rule VarList = Node(Var + ZeroOrMore(Comma + Var));
        public static Rule UnaryExpr = Node(UnaryOp + WS + Expr);
        public static Rule BinaryExpr = Node(Expr + WS + BinaryOp + WS + Expr);
        public static Rule FunExpr = Node(Keyword("function") + FuncBody);
    	public static Rule Expr = Node(Nil | False | True | Number | String | MatchString("...") | FunExpr | PrefixExpr | Table | BinaryExpr | UnaryExpr);
	    public static Rule ExprList = Node(Exp + ZeroOrMore(Comma + Expr));
    }
}
