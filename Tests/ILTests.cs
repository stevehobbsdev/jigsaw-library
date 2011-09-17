using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection.Emit;

namespace Diggins.Jigsaw.Tests
{
    public class ILTests
    {
        public static void Test(string s)
        {
            var dm = ILEvaluator.Eval(s);
            var r = dm.Invoke(null, new object[] { });
            Console.WriteLine(r);
            //OpCodes.Du

        }      

        public static void Tests()
        {
            Test("Int32 f() { ldc_i4_1; ret; }");
            Test("Int32 f() { ldc_i4_2; ldc_i4_3; add; ret; }");
            Test("Int32 f() { ldc_i4 Int32 42; ret; }");
            Test("Int32 f() { ldc_i4 Int32 42; ldc_i4_1; sub; ret; }");
            Test("Int32 f() { var Int32 x; ldc_i4_1; stloc var x; ldloc var x; ret; }");
            Test(
@"Int32 sum10() { 
    var Int32 x; 
    ldc_i4 Int32 10; 
    stloc var x; 
    var Int32 sum;
    ldc_i4 Int32 0;
    stloc var sum;
begin:
    ldloc var x;
    brfalse label end;
    ldloc var x;
    ldloc var sum;
    add;
    stloc var sum;
    ldloc var x;
    ldc_i4_1;
    sub;
    stloc var x;
end:
    ldloc var sum;
    ret;
}");
        }
    }
}
