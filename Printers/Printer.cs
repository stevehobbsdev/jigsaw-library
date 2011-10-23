using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    public abstract class Printer
    {
        StringBuilder sb = new StringBuilder();
        int indent = 0;

        public abstract Printer Print(Node n);

        public string LineBeginning
        {
            get { return new String(' ', indent * 2); }
        }

        public Printer PrintLine(string s = "")
        {
            sb.AppendLine(s).Append(LineBeginning);
            return this;
        }

        public Printer Print(string s)
        {
            sb.Append(s);
            return this;
        }

        public Printer PrintLine(Node n)
        {
            Print(n);
            sb.AppendLine();
            sb.Append(LineBeginning);
            return this;
        }

        public Printer Print(IEnumerable<Node> nodes, string sep = "")
        {
            bool first = true;
            foreach (var n in nodes)
            {
                if (!first)
                    Print(sep);
                first = false;
                Print(n);
            }
            return this;
        }

        public Printer Print(IEnumerable<string> strings, string sep = "")
        {
            bool first = true;
            foreach (var s in strings)
            {
                if (!first)
                    Print(sep);
                first = false;
                Print(s);
            }
            return this;
        }

        public Printer PrintLines(IEnumerable<Node> nodes)
        {
            foreach (var n in nodes)
                PrintLine(n);
            return this;
        }

        public Printer PrintLines(IEnumerable<string> strings)
        {
            foreach (var s in strings)
                PrintLine(s);
            return this;
        }

        public Printer Indent(string s = "")
        {
            ++indent;
            return PrintLine(s);
        }

        public Printer Unindent(string s = "")
        {
            if (--indent < 0) throw new Exception("Cannot have a negative indentation");
            return PrintLine(s);
        }

        public Printer Indent(Func<Printer> f)
        {
            Indent();
            f();
            return Unindent();
        }

        public StringBuilder GetStringBuilder()
        {
            return sb;
        }
    }
}
