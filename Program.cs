using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace calculator {
    private struct Operation<T> {
        String Symbol;
        Func<T, T, T> Action;
        uint Priority;

        public Operation(String Sym, Func<T, T, T> Act, uint Prior) {
            Symbol = Sym;
            Action = Act;
            Priority = Prior;
        }
    }

    public class Calculator {
        private List<Operation<double>> Operations;

        Calculator() {
            Operations = new List<Operation<double>>();

            Operations.Add(new Operation<double>("+", (x, y) => (x + y), 10));
            Operations.Add(new Operation<double>("-", (x, y) => (x - y), 10));
            Operations.Add(new Operation<double>("*", (x, y) => (x * y), 20));
            Operations.Add(new Operation<double>("/", (x, y) => (x / y), 20));
        }
    }

    class Program {
        static void Main(string[] args) {

        }
    }
}
