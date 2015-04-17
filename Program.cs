using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace calculator {
    public struct Operation<T> {
        public String Symbol;
        public Func<T, T, T> Action;
        public uint Priority;

        public Operation(String Symbol, Func<T, T, T> Action, uint Priority) {
            this.Symbol = Symbol;
            this.Action = Action;
            this.Priority = Priority;
        }
    }

    public struct Bracket {
        public char Open;
        public char Close;

        public Bracket(char Open, char Close) {
            this.Open = Open;
            this.Close = Close;         
        }
    }

    public class Calculator {
        private List<Operation<double>> Operations;
        private List<Bracket> Brackets;

        public Calculator() {
            Operations = new List<Operation<double>>();
            Brackets = new List<Bracket>();

            Operations.Add(new Operation<double>("+", (x, y) => (x + y), 10));
            Operations.Add(new Operation<double>("-", (x, y) => (x - y), 10));
            Operations.Add(new Operation<double>("*", (x, y) => (x * y), 20));
            Operations.Add(new Operation<double>("/", (x, y) => (x / y), 20));
            Operations.Add(new Operation<double>("**", (x, y) => (x + y), 20));
            Operations.Sort((x, y) => (y.Symbol.Length.CompareTo(x.Symbol.Length)));

            Brackets.Add(new Bracket('(', ')'));
        }

        public double Solve(String Expression) {
            String PostfixExpression = Postfix(Expression);
            return 0;
        }

        private String Postfix(String Expression) { 
            StringBuilder PostfixExpression = new StringBuilder();
            Stack<String> OperationStack = new Stack<string>();

            foreach (string Part in Seporate(Expression)) {
                Console.WriteLine(Part);
            }
            return "";
        }

        private IEnumerable<string> Seporate(String Expression) {
            int Position = 0;
            Regex PartRegex = CreatePartRegex();

            while (Position < Expression.Length) {
                Match RegexResult = PartRegex.Match(Expression.Substring(Position));
                if (!RegexResult.Success) {
                    throw new Exception("Error in expression format");
                }
                yield return RegexResult.Groups[1].Value;
                Position += RegexResult.Value.Length;
            }
        }

        private Regex CreatePartRegex() {
            String Pattern = @"^\s*(\d+(\.\d+)?";
            for (int i = 0; i < Operations.Count; i++) {
                Pattern += "|" + Regex.Escape(Operations[i].Symbol);
            }
            for (int i = 0; i < Brackets.Count; i++) {
                Pattern += "|" + Regex.Escape(Brackets[i].Open.ToString()) + "|" + Regex.Escape(Brackets[i].Close.ToString());
            }
            return new Regex(Pattern + ")");
        }
    }

    class Program {
        static void Main(string[] args) {
            Calculator c = new Calculator();
            c.Solve(" 2.7 + 5*3( 7- 2) ** 4 /5");
            Console.ReadKey();
        }
    }
}
