using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    class CodeProjectArticleSnippets
    {
        public static void YouStoleMyPen()
        {
            var word = Grammar.Node(Grammar.Pattern(@"\w+")).SetName("word");
            var ws = Grammar. Pattern(@"\s+");
            var eos = Grammar.CharSet("!.?");
            var sentence = Grammar.Node(Grammar.ZeroOrMore(word | ws) + eos).SetName("sentence");
            var sentences = Grammar.OneOrMore(sentence + Grammar.Opt(ws));
            var nodes = sentences.Parse("Hey! You stole my pen. Hey you stole my pen!");
            foreach (var n in nodes)
            {
                Console.WriteLine(n);
                foreach (var n2 in n.Nodes)
                    Console.WriteLine("  " + n2);
            } 
        }

        public class SentenceGrammar : Grammar
        {
            public static Rule word = Node(Pattern(@"\w+"));
            public static Rule ws = Pattern(@"\s+");
            public static Rule eos = CharSet("!.?");
            public static Rule sentence = Node(ZeroOrMore(word | ws) + eos);
            public static Rule sentences = OneOrMore(sentence + Opt(ws));

            static SentenceGrammar()
            {
                Grammar.InitGrammar(typeof(SentenceGrammar));
            }
        }

        public static void YouStoleMyPen2()
        {
            var nodes = SentenceGrammar.sentences.Parse("Hey! You stole my pen. Hey you stole my pen!");
            
            foreach (var n in nodes)
            {
                Console.WriteLine(n);
                foreach (var n2 in n.Nodes)
                    Console.WriteLine("  " + n2);
            }

            Console.WriteLine("The grammar is:");
            Grammar.OutputGrammar(typeof(SentenceGrammar));
        }

        public static void Tests()
        {
            YouStoleMyPen();
            YouStoleMyPen2();

            Console.WriteLine("JSON Grammar");
            Grammar.OutputGrammar(typeof(JsonGrammar));
        }
    }
}
