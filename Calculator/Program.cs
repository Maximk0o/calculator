using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace calculator {
    class Program {
        static void Main(string[] args) {
            Calculator c = new Calculator();
            c.AddBrackets("[", "]");
            c.AddBrackets("[[", "]]");
            c.AddOperation("**", (x, y) => (Math.Pow(x, y)), 30);
            Console.WriteLine(c.Solve("8+1/[[(1+([3+3]-8)/5)-12]]"));

            c.AddBrackets("@--", "--@");
            c.AddBrackets("((", "))");
            c.AddBrackets("(", ")");

            Console.WriteLine(c.Solve(" @--( 2,4 + 4) - (( 5 - 4))--@ * 3"));

            c.Solve(" ( 2,4 + 4 - [ 5 - 4)]");
            Console.ReadKey();
        }
    }
}
