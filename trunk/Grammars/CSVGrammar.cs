using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    
    public class CSVGrammar : Grammar
    {
        public static Rule NL = Opt((MatchChar('\r') + MatchChar('\n')) | MatchChar('\r'));
        public static Rule UnquotedChar = Node(ExceptCharSet(" \n,\""));
        public static Rule UnquotedField = Node(ZeroOrMore(UnquotedChar));
        public static Rule QuotedChar = Node(MatchString("\"\"") | MatchChar('\n') | MatchChar(',') | UnquotedChar);
        public static Rule QuotedField = Node(MatchChar('"') + ZeroOrMore(QuotedChar) + MatchChar('"'));
        public static Rule Field = Node(QuotedField | UnquotedField);
        public static Rule Record = Node(Opt(Field) + ZeroOrMore(MatchChar(',') | Field));
        public static Rule Records = Node(Opt(Record) + ZeroOrMore(MatchChar('\n') + Record)) + End;

        static CSVGrammar() { InitGrammar(typeof(CSVGrammar)); }
    }
}
