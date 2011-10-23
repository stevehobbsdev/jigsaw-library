using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/aa140277(v=office.10).aspx
    /// http://msdn.microsoft.com/en-us/library/aa140301(v=office.10).aspx
    /// </summary>
    public class RTFGrammar : SharedGrammar
    {
        public static Rule OpenBrace = MatchChar('{');
        public static Rule CloseBrace = MatchChar('}');
        public static Rule Group(Rule r) { return OpenBrace + r + CloseBrace; }
        public static Rule Control(string s) { return Node((MatchChar('\\') + MatchString(s) + WS).SetName(s)); }
        //public static Rule CharSet = Control("ansi");
        //public static Rule FontTable = ile;
        //public static Rule Header = Control("rtf") + CharSet + Opt(Control("deff")) + FontTable + WS; 
        //public static Rule File = Group(WS + Header + WS); 
    }
}
