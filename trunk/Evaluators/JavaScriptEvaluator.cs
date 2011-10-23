using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Diggins.Jigsaw
{
    public class JavaScriptEvaluator : Evaluator
    {
        #region fields
        bool isReturning = false;
        dynamic result = null;
        #endregion fields

        public JavaScriptEvaluator()
        {
            // This is where you could add all sorts of primitive objects and functions. Or don't. Fine.
            AddBinding("alert", new JSPrimitive(args => { Console.WriteLine(args[0]); }));
        }

        #region static functions
        public static dynamic RunScript(string s)
        {
            return new JavaScriptEvaluator().Eval(s, JavaScriptGrammar.Script);
        }

        public static dynamic EvalExpression(string s)
        {
            return new JavaScriptEvaluator().Eval(s, JavaScriptGrammar.Expr);
        }
        #endregion

        #region helper classes
        /// <summary>
        /// Represents a JavaScript function. Note that a function is also an object.
        /// </summary>
        public class JSFunction : JsonObject
        {
            public static int FuncCount = 0;
            public Node node;
            public VarBindings capture;
            public Node parms;
            public string name = String.Format("_anonymous_{0}", FuncCount++);
            public Node body;

            public JSFunction()
            { }

            public JSFunction(VarBindings c, Node n) { 
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

            public virtual dynamic Apply(JavaScriptEvaluator e, dynamic self, params dynamic[] args)
            {
                var originalContext = e.env;
                var originalReturningState = e.isReturning;
                dynamic result = null;

                try
                {
                    e.env = capture.AddBinding("this", self);                    
                    e.isReturning = false;
                    int i = 0;
                    foreach (var p in parms.Nodes)
                        e.AddBinding(p.Text, args[i++]);                    
                    e.Eval(body);
                    result = e.result;
                }
                finally
                {
                    e.result = null;
                    e.env = originalContext;
                    e.isReturning = originalReturningState;
                }
                return result;
            }

            public override string ToString()
            {
                return node.ToString();
            }
        }

        /// <summary>
        /// Represents a built-in function. 
        /// </summary>
        public class JSPrimitive : JSFunction
        {
            Func<dynamic, dynamic[], dynamic> func;

            public JSPrimitive(Func<dynamic, dynamic[], dynamic> func)
            {
                this.func = func;
            }

            public JSPrimitive(Action<dynamic, dynamic[]> action)
            {
                this.func = (self, args) => { action(self, args); return null; };
            }

            public JSPrimitive(Action<dynamic[]> action)
            {
                this.func = (self, args) => { action(args); return null; };
            }

            public JSPrimitive(Func<dynamic[], dynamic> function)
            {
                this.func = (self, args) => function(args); 
            }

            public override dynamic Apply(JavaScriptEvaluator e, dynamic self, params dynamic[] args)
            {
                return func(self, args);
            }
        }
        #endregion

        public dynamic Eval(string s, Rule r)
        {
            var nodes = r.Parse(s);
            var root = JavaScriptTransformer.Transform(nodes[0]);
            return Eval(root);
        }

        public dynamic EvalNodes(IEnumerable<Node> nodes)
        {
            dynamic result = null;
            foreach (var n in nodes)
            {
                result = Eval(n);
                if (isReturning)
                    return result;
            }
            return result;
        }

        /// <summary>
        /// This evaluates the nodes of a JavaScript parse tree. The tree is assumed to 
        /// have first been transformed using the JavaScriptTransformer
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public dynamic Eval(Node n)
        {
            Debug.Assert(!isReturning);

            switch (n.Label)
            {
                case "Return":
                    // Evaluate the return value expression if present, or return null. 
                    result = n.Count == 1 ? Eval(n[0]) : null;
                    // Set the "isReturning" flag. 
                    isReturning = true;
                    return result;
                case "Script":
                    // Evaluate all statements in the script in order 
                    return EvalScoped(() => EvalNodes(n.Nodes));
                case "AnonFunc":
                    // Creates an unnamed function
                    return new JSFunction(env, n);
                case "Block":
                    // Execute a sequence of instructions
                    return EvalScoped(() => EvalNodes(n.Nodes));
                case "If":
                    // Check if the condition is false (or NULL)
                    if (Eval(n[0]) ?? false)
                        // Execute first statement
                        return Eval(n[1]);
                    else if (n.Count > 2)
                        // Execute else statement if the condition is false, and it exists 
                        return Eval(n[2]);
                    else
                        // By default reutrn the result 
                        return null;
                case "VarDecl":
                    // Variable declaration
                    // It may or may not be initialized
                    return AddBinding(n[0].Text, n.Count > 1 ? Eval(n[1]) : null);
                case "Empty":
                    // An empty statement means we do nothing
                    return null;
                case "ExprStatement":
                    return Eval(n[0]);
                case "For":
                    // For loop (could also have been a transformed while loop)
                    return EvalScoped(() =>
                    {
                        dynamic r = null;

                        // Typically this is the initialization statement. 
                        // Because it is scoped with the entire for statement
                        // we wrapped this all in a call to "EvalScoped".
                        Eval(n[0]);
                        
                        // We exit prematurely if a return statement was encountered. 
                        // We also exit the loop when the loop invariant is false or null
                        while (!isReturning && (Eval(n[1]) ?? false))
                        {
                            // Evaluate the body of the loop
                            Eval(n[3]);

                            // Evaluate the loop control statement 
                            r = Eval(n[2]);
                        }
                        
                        // We always return the "result" variable from a statement
                        // in case we are exiting the function 
                        return r;
                    });
                case "BinaryExpr":
                    // Evaluat a binary operation 
                    {
                        var op = n[1].Text;
                        var left = Eval(n[0]);
                        var right = Eval(n[2]);
                        return Primitives.Eval(op, left, right);
                    }
                case "PrefixExpr":
                    // Evaluates a prefixed unary operation
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
                    // Retrieves a field from an object
                    {
                        var obj = Eval(n[0]);
                        var field = n[1][0].Text;
                        return obj[field];
                    }
                case "IndexExpr":
                    // Retrieves an indexed field or property
                    {
                        var index = Eval(n[1]);
                        var array = Eval(n[0]);
                        return array[index];
                    }
                case "CallExpr":
                    // A function call that is not a method
                    {
                        var func = Eval(n[0]);
                        var args = n[1].Nodes.Select(Eval).ToArray();
                        return func.Apply(this, null, args);
                    }
                case "MethodCallExpr":
                    // Method invocation
                    {
                        var obj = Eval(n[0]);
                        var func = Eval(n[1]);
                        var args = n[2].Nodes.Select(Eval).ToArray();
                        return func.Apply(this, obj, args);
                    }
                case "NewExpr":
                    // A new expression. 
                    {
                        var func = Eval(n[0]);
                        var args = n[1].Nodes.Select(Eval).ToArray();
                        return func.Apply(this, new JsonObject(), args);
                    }
                case "AssignExpr":
                    // Assigns a value to a variable, creating it if necesseary
                    {
                        var lnode = n[0];
                        var rnode = n[2];
                        var rvalue = Eval(rnode);
                        switch (lnode.Label)
                        {
                            // Assignment to a field                            
                            case "FieldExpr":
                                {
                                    var obj = Eval(lnode[0]);
                                    var name = lnode[1].Text;
                                    return obj[name] = rvalue;
                                }
                            // Assignment to an index operation
                            case "IndexExpr":
                                {
                                    var obj = Eval(lnode[0]);
                                    var index = Eval(lnode[1]);
                                    return obj[index] = rvalue;
                                }
                            // Assignment to an identifier
                            case "Identifier":
                                {
                                    var name = lnode.Text;
                                    // See: https://developer.mozilla.org/en/JavaScript/Reference/Statements/Var
                                    // where unreference variables are created as new global variables
                                    return RebindOrCreateGlobalBinding(name, rvalue);
                                }
                            default: 
                                throw new Exception("Invalid lvalue " + n[0].Label);
                        }
                    }
                case "Identifier":
                    // Look-up an identifier in the environment 
                    return env[n.Text];
                case "Object":
                    // An object literal value
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
                    // An array literal value
                    return n.Nodes.Select(Eval).ToArray();
                case "Integer":
                    // An integer literal value
                    return Int32.Parse(n.Text);
                case "Float":
                    // A floating point literal value 
                    return Double.Parse(n.Text);
                case "String":
                    // A string literal value
                    return n.Text.Substring(1, n.Text.Length - 2);
                case "True":
                    // A true value
                    return true;
                case "False":
                    // A false value
                    return false;
                case "Null":
                    // A null value.
                    return null;
                default:
                    throw new Exception("Unrecognized node type " + n.Label);
            }
        }
    }
}
