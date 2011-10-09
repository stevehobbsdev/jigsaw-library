using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Diggins.Jigsaw
{
    public class JavaScriptEvaluator
    {
        public static dynamic RunScript(string s)
        {
            return new JavaScriptEvaluator().Eval(s, JavaScriptGrammar.Script);
        }

        public static dynamic EvalExpression(string s)
        {
            return new JavaScriptEvaluator().Eval(s, JavaScriptGrammar.Expr);
        }

        public class JSTransformer : TreeTransformer
        {
            public JSTransformer(Node n)
            {
                TransformNodes(n);

                Debug.Assert(!n.Descendants.Any(x => x.Label == "PostfixExpr"));
            }

            Node ToAssignment(Node left, string op, Node right)
            {
                var assignOp = new Node("AssignOp", "=");                
                var binOp = new Node("Binaryop", op);
                var binExpr = new Node("BinaryExpr", left, binOp, right);
                return new Node("AssignExpr", left, assignOp, binExpr);
            }

            protected override Node Transform(Node n)
            {
                switch (n.Label)
                {
                    case "PostfixExpr":
                        {
                            Debug.Assert(n.Count != 0);
                            if (n.Count == 1)
                                return n.Nodes[0];
                            var last = n.Nodes.Last();
                            switch (last.Label)
                            {
                                case "Field": return LeftGroup(n, "FieldExpr");
                                case "Index": return LeftGroup(n, "IndexExpr");
                                case "ArgList": return LeftGroup(n, "CallExpr");
                                default: throw new Exception("Unexpected node in postfix-expression " + last.Label);
                            }
                        }
                    case "AssignExpr":
                        {
                            if (n.Count == 1)
                                return n[0];
                            switch (n[1].Text)
                            {
                                case "=": return n;
                                case "+=": return ToAssignment(n[0], "+", n[2]);
                                case "-=": return ToAssignment(n[0], "-", n[2]);
                                case "*=": return ToAssignment(n[0], "*", n[2]);
                                case "/=": return ToAssignment(n[0], "/", n[2]);
                                case "%=": return ToAssignment(n[0], "%", n[2]);
                                case "|=": return ToAssignment(n[0], "|", n[2]);
                                case "&=": return ToAssignment(n[0], "&", n[2]);
                                case "^=": return ToAssignment(n[0], "^", n[2]);
                                case "||=": return ToAssignment(n[0], "||", n[2]);
                                case "&&=": return ToAssignment(n[0], "&&", n[2]);
                                case ">>=": return ToAssignment(n[0], ">>", n[2]);
                                case "<<=": return ToAssignment(n[0], "<<", n[2]);
                                default:
                                    throw new Exception("Unexpected assignment operator " + n[1].Text);
                            }
                        }
                }
                return n;
            }
        }

        public class JSFunction
        {
            public static int FuncCount = 0;
            public Node node;
            public Context capture;
            public Node parms;
            public string name = String.Format("_anonymous_{0}", FuncCount++);
            public Node body;
            
            public JSFunction(Context c, Node n) { 
                capture = c;  
                node = n;
                if (n.Count == 3)
                {
                    name = n[0].Text;
                    parms = n[1];
                    body = n[2];
                }
                else
                {
                    parms = n[0];
                    body = n[1];
                }
            }

            public dynamic Eval(JavaScriptEvaluator e, params dynamic[] args)
            {
                var originalContext = e.context;
                var originalReturningState = e.isReturning;
                dynamic result = null;

                try
                {
                    e.context = capture;
                    e.isReturning = false;
                    int i = 0;
                    foreach (var p in parms.Nodes)
                        e.AddContext(p.Text, args[i++]);                    
                    e.Eval(body);
                    result = e.result;
                }
                finally
                {
                    e.result = null;
                    e.context = originalContext;
                    e.isReturning = originalReturningState;
                }
                return result;
            }
        }

        Context context = new Context();
        bool isReturning = false;
        dynamic result = null;

        public dynamic AddContext(string name, dynamic x)
        {
            context = context.AddContext(name, x);
            return x;
        }

        public dynamic AddContextOrCreate(string name, dynamic value)
        {
            var c = context.FindContextOrDefault(name);
            return c != null ? c.Value = value : AddContext(name, value);
        }


        public dynamic Eval(string s, Rule r)
        {
            var nodes = r.Parse(s);
            var root = nodes[0];
            new JSTransformer(root);
            return Eval(root);
        }

        public dynamic EvalScoped(Func<dynamic> f)
        {
            var c = context;
            var r = f();
            context = c;
            return r;
        }

        public dynamic EvalNodes(IEnumerable<Node> nodes)
        {
            dynamic result = null;
            foreach (var n in nodes)
                result = Eval(n);
            return result;
        }

        public dynamic Eval(Node n)
        {
            if (isReturning)
                return result;

            switch (n.Label)
            {
                case "Return":
                    result = n.Count == 1 ? Eval(n[0]) : null;
                    isReturning = true;
                    return result;
                case "Script":
                    return EvalScoped(() => EvalNodes(n.Nodes));
                case "Statement":
                    return Eval(n[0]);
                case "NamedFunc":
                    return AddContext(n[0].Text, new JSFunction(context, n));
                case "AnonFunc":
                    return new JSFunction(context, n);
                case "While":
                    while (Eval(n[0]) ?? false)
                        Eval(n[1]);
                    return null;
                case "Block":
                    return EvalScoped(() => EvalNodes(n.Nodes));
                case "If":
                    if (Eval(n[0]) ?? false)
                        return Eval(n[1]);
                    else if (n.Count > 2)
                        return Eval(n[2]);
                    return null;
                case "VarDecl":
                    return (n.Count > 1) 
                        ? AddContext(n[0].Text, Eval(n[1]))
                        : AddContext(n[0].Text, null);
                case "For":
                    return EvalScoped(() =>
                    {
                        Eval(n[0]);
                        while (Eval(n[1]) ?? false)
                        {
                            Eval(n[3]);
                            Eval(n[2]);
                        }
                        return null;
                    });
                case "Expr":
                    if (n.Count > 1)
                        return Eval(n[0])
                            ? Eval(n[1])
                            : Eval(n[2]);
                    else
                        return Eval(n[0]);
                case "BinaryExpr":
                    if (n.Count > 1)
                        return Primitives.Eval(n[1].Text, Eval(n[0]), Eval(n[2]));
                    else
                        return Eval(n[0]);
                case "PrefixExpr":
                    switch (n[0].Text)
                    {
                        case "!":
                            return !Eval(n[1]);
                        case "~":
                            return ~Eval(n[1]);
                        case "-":
                            return -Eval(n[1]);
                        default:
                            throw new Exception("Unrecognized prefix operator " + n[0].Text);
                    }
                case "FieldExpr":
                    {
                        var jso = (JsonObject)Eval(n[0]);
                        return jso[n[1][0].Text];
                    }
                case "IndexExpr":
                    return Eval(n[0])[Eval(n[1])];
                case "CallExpr":
                    {
                        var args = n[1].Nodes.Select(Eval).ToArray();
                        var func = (JSFunction)Eval(n[0]);
                        return func.Eval(this, args);
                    }
                case "AssignExpr":
                    {
                        if (n.Count == 1)
                            return Eval(n[0]);
                        var lnode = n[0];
                        var rnode = n[2];
                        var rvalue = Eval(rnode);
                        switch (lnode.Label)
                        {
                            case "FieldExpr":
                                {
                                    Debug.Assert(lnode.Count == 2);
                                    var obj = Eval(lnode[0]);
                                    var name = lnode[1].Text;
                                    return obj[name] = rvalue;
                                }
                            case "IndexExpr":
                                {
                                    Debug.Assert(lnode.Count == 2);
                                    var obj = Eval(lnode[0]);
                                    var index = Eval(lnode[1]);
                                    return obj[index] = rvalue;
                                }
                            case "Identifier":
                                {
                                    var name = lnode.Text;
                                    return AddContextOrCreate(name, rvalue);
                                }
                            default: throw new Exception("Invalid lvalue " + n[0].Label);
                        }
                    }
                case "LeafExpr":
                    return Eval(n[0]);
                case "ParenExpr":
                    return Eval(n[0]);
                case "New":
                    return EvalScoped(() =>
                    {
                        AddContext("this", new JsonObject());
                        return Eval(n[0]);
                    });
                case "Identifier":
                    return context[n.Text];
                case "Literal":
                    return Eval(n[0]);
                case "Number":
                    return Eval(n[0]);
                case "Object":
                    {
                        var r = new JsonObject();
                        foreach (var pair in n.Nodes)
                        {
                            var name = pair[0].Text;
                            var value = Eval(pair[1]);
                            r[name] = value;
                        }
                        return r;
                    }
                case "Array":
                    return n.Nodes.Select(Eval).ToList();
                case "Integer":
                    return Int32.Parse(n.Text);
                case "Float":
                    return Double.Parse(n.Text);
                case "String":
                    return n.Text.Substring(1, n.Text.Length - 2);
                case "True":
                    return true;
                case "False":
                    return false;
                case "Null":
                    return JsonObject.Null;
                default:
                    throw new Exception("Unrecognized node type " + n.Label);
            }
        }
    }
}
