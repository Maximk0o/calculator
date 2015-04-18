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
            c.AddBinaryOperation("**", (x, y) => (Math.Pow(x, y)), 30);
            c.SameBynaryAndUnarySymbols = true;
            c.AddUnaryOperation("-", x => (-x), 30);
            Console.WriteLine(c.Solve("-8+1/(1+-(3-8)/5)"));
            Console.ReadKey();
        }
    }
}
