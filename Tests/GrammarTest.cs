using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    public class GrammarTest
    {
        public static void Test(string s, Rule r)
        {
            try
            {
                Console.WriteLine("Using rule {0} to parse string {1}", r.Name, s);
                var nodes = r.Parse(s);
                if (nodes == null || nodes.Count != 1)
                    Console.WriteLine("Parsing failed!");
                else if (nodes[0].Text != s)
                    Console.WriteLine("Parsing partially succeeded");
                else
                    Console.WriteLine("Parsing suceeded");
                Console.WriteLine(nodes[0].Text);
            }
            catch (Exception e)
            {
                Console.WriteLine("Parsing failed with exception" + e.Message);
            }
        }
    }
}
