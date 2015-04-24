using System;
using CalculatorLibrary;

namespace CalculatorProgram {
    class Program {
        static void Main(string[] args) {
            Calculator c = new Calculator();
            try
            {
                c.AddOperation("**", (x, y) => (Math.Pow(x, y)), 30);

                Console.WriteLine(c.Solve("--8+1/((1+((3+3)-8)/5)-12)"));
            }
            catch (CalculationException.CalculationException exception)
            {
                Console.WriteLine(exception.Message);
            }
            Console.ReadKey();
        }
    }
}
