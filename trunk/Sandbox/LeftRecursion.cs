using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw.Sandbox
{
    class LeftRecursion
    {
        /*
        public class DecomposeSeqRule : Rule
        {
            public Rule Iteration { get { return Children[0]; } }
            public Rule RightMost { get { return Children[1]; } }
            public DecomposeSeqRule(Rule iteration, Rule rightMost)
                : base(iteration, rightMost)
            { }

            public bool Decompose(Node node)
            {
                if (node.Count < 2 || (node[node.Count - 1].Label != RightMost.Name))
                    return false;

                var n = new Node(Name);
                return true;
            }
        }

        public class LeftRecursionRule : Rule
        {
            public Rule baseRule;
            public Rule iterativeRule;
            public Rule rightHandRule;

            public LeftRecursionRule(Rule baseRule, params LeftRecursionOptionRule[] choices)
            {
                rightHandRule = new ChoiceRule(choices.Select(x => x.Right).ToArray());
                iterativeRule = new SeqRule(baseRule, new ZeroOrMoreRule(rightHandRule));
            }

            public override string Definition
            {
                get { return "???"; }
            }

            protected override bool InternalMatch(ParserState state)
            {
            }

            public static Node Decompose(List<Node> nodes)
        {
            if (nodes.Count < 2)
                return nodes[0];

            List<Node> leftGroup = nodes.GetRange(0, nodes.Count - 1);
            Node rightNode = nodes[nodes.Count - 1];
          


            Decompose(leftGroup);
        }
        }
        /*
        public static Rule IncExpr      = DecomposeSeq(PostfixBase, Inc);
        public static Rule DecExpr      = DecomposeSeq(PostfixBase, Dec);
        public static Rule FieldExpr    = DecomposeSeq(PostfixBase, Field);
        public static Rule IndexExpr    = DecomposeSeq(PostfixBase, Index);        
        public static Rule CallExpr     = DecomposeSeq(PostfixBase, ArgList);
        public static Rule PostfixExpr  = DecomposeChoice(UnaryExpr, IncExpr, DecExpr, FieldExpr, IndexExpr, CallExpr);
         */

        /*
        public static Rule PostIncExpr = LeftRecursiveChoice(() => PostfixExpr, Inc);
        public static Rule PostDecExpr = LeftRecursiveChoice(() => PostfixExpr, Dec);
        public static Rule FieldExpr = LeftRecursiveChoice(() => PostfixExpr, Field);
        public static Rule IndexExpr = LeftRecursiveChoice(() => PostfixExpr, Index);        
        public static Rule CallExpr = LeftRecursiveChoice(() => PostfixExpr, ArgList);
        public static Rule PostfixExpr = LeftRecursion(UnaryExpr, PostIncExpr, PostDecExpr, FieldExpr, IndexExpr, CallExpr);
         */

        /*
        public static Rule PostfixBaseExpr = Recursive(() => PostfixExpr);
        public static Rule PostIncExpr = PostfixBaseExpr + Inc;
        public static Rule PostDecExpr = PostfixBaseExpr + Dec;
        public static Rule FieldExpr = PostfixBaseExpr + Field;
        public static Rule IndexExpr = PostfixBaseExpr + Index;        
        public static Rule CallExpr = PostfixBaseExpr + ArgList;
        */ 

        //public static Rule PostfixExpr = Node(UnaryExpr | PostIncExpr | PostDecExpr | FieldExpr | IndexExpr | CallExpr);

        //public static Rule PostfixExpr = LeftRecursiveChoice(UnaryExpr, PostIncExpr, PostDecExpr, FieldExpr, IndexExpr, CallExpr);

        //public static Rule PostfixExpr = IterationToRecursion(UnaryExpr, ZeroOrMore(PostfixOp), (PostIncExpr | PostDecExpr | FieldExpr | IndexExpr | CallExpr));

        //public static Rule PostfixExpr = Node(UnaryExpr | PostIncExpr | PostDecExpr | FieldExpr | IndexExpr | CallExpr);

        /*
        public static Rule PostfixBaseExpr = Node(UnaryExpr + WS + Postfixes);        
        public static Rule IncExpr = DecomposableSeq(PostfixBaseExpr, Inc);
        public static Rule DecExpr = DecomposableSeq(PostfixBaseExpr, Dec);
        public static Rule FieldExpr = DecomposableSeq(PostfixBaseExpr, Field);
        public static Rule IndexExpr = DecomposableSeq(PostfixBaseExpr, Index);        
        public static Rule CallExpr = DecomposableSeq(PostfixBaseExpr, ArgList);
        public static Rule PostfixExpr = DecompsableChoice(IncExpr | DecExpr | FieldExpr | IndexExpr | CallExpr);
         */

        /*
        public static Rule PostfixExpr = LeftGroup(UnaryExpr + ZeroOrMore(PostfixOp), FieldExpr, IndexExpr, CallExpr);
        public static Rule FieldExpr = DecomposeSeq(PostfixExpr, Field);
        public static Rule IndexExpr = DecomposeSeq(PostfixExpr, Index);        
        public static Rule CallExpr = DecomposeSeq(PostfixExpr, ArgList);
         */

        /*
        public static Rule FieldExpr    = LeftRecursiveChoice(Field);
        public static Rule IndexExpr    = LeftRecursiveChoice(Index);        
        public static Rule CallExpr     = LeftRecursiveChoice(ArgList);
        public static Rule PostfixExpr  = LeftRecursiveNode(UnaryExpr, FieldExpr | IndexExpr | CallExpr);
         */

        /*
        public static Rule FieldExpr    = LeftRecursiveChoice(Field);
        public static Rule IndexExpr    = LeftRecursiveChoice(Index);        
        public static Rule CallExpr     = LeftRecursiveChoice(ArgList);
        public static Rule PostfixExpr  = LeftRecursiveNode(UnaryExpr, FieldExpr, IndexExpr, CallExpr);
        */

        /*
        public static Rule PostfixExpr  = LeftRecursiveNode(UnaryExpr, 
            new Dictionary<Rule, string>() { { Field, "FieldExpr" }, { Index, "IndexExpr" }, { ArgList, "CallExpr" } });

        public static Rule PostfixExpr  = LeftRecursiveNode(UnaryExpr)
            .AddOption(Field, "FieldExpr")
            .AddOption(Index, "IndexExpr")
            .AddOption(ArgList, "FunCall");
         */

    }
}
