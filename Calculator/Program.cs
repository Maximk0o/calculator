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
            c.AddOperation("**", (x, y) => (Math.Pow(y, x)), 30);
            Console.WriteLine(c.Solve(" 2,7 + ([5*3+( 7- 2)] * [4 /(5  -4 +(9-2))])**2"));
            Console.ReadKey();
        }
    }
}
