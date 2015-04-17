using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace calculator {
    public class CalculationException : Exception {
        public CalculationException() {
        }

        public CalculationException(string message)
            : base(message) {
        }

        public CalculationException(string message, Exception inner)
            : base(message, inner) {
        }
    }

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
        public String Open;
        public String Close;

        public Bracket(String Open, String Close) {
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
            Operations.Sort((x, y) => (y.Symbol.Length.CompareTo(x.Symbol.Length)));

            Brackets.Add(new Bracket("(", ")"));
            Brackets.Add(new Bracket("[", "]"));
        }

        public double Solve(String Expression) {
            String PostfixExpression = Postfix(Expression);
            Console.WriteLine(PostfixExpression);
            return 0;
        }

        private String Postfix(String Expression) {
            StringBuilder PostfixExpression = new StringBuilder();
            Stack<Operation<double>> OperationStack = new Stack<Operation<double>>();
            double ForTest;

            foreach (string Part in Seporate(Expression)) {
                if (double.TryParse(Part, out ForTest)) {
                    PostfixExpression.Append(Part);
                }
                else if (Brackets.Any(x => (x.Open == Part))) {
                    OperationStack.Push(new Operation<double>(Part, null, 0));
                }
                else if (Operations.Any(x => (x.Symbol == Part))) {
                    Operation<double> CurrentOperation = GetOperationBySymbol(Part);
                    while (OperationStack.Count > 0) {
                        if (OperationStack.Peek().Priority <= CurrentOperation.Priority) {
                            break;
                        }
                        PostfixExpression.Append(OperationStack.Pop().Symbol);
                    }
                    OperationStack.Push(CurrentOperation);
                }
                else if (Brackets.Any(x => (x.Close == Part))) {
                    Bracket CurrentBrackets = GetBracketByCloseSymbol(Part);
                    while (OperationStack.Count > 0) {
                        if (OperationStack.Peek().Symbol == CurrentBrackets.Open) {
                            break;
                        }
                        if (Brackets.Any(x => (x.Open == OperationStack.Peek().Symbol))) {
                            throw new CalculationException("Error in brackets");
                        }
                        PostfixExpression.Append(OperationStack.Pop().Symbol);
                    }
                    if (OperationStack.Count == 0) {
                        throw new CalculationException("Error in brackets");
                    }
                    OperationStack.Pop();
                }
            }
            while (OperationStack.Count > 0) {
                if (Brackets.Any(x => (x.Open == OperationStack.Peek().Symbol))) {
                    throw new CalculationException("Error in brackets");
                }
                PostfixExpression.Append(OperationStack.Pop().Symbol);
            }
            return PostfixExpression.ToString();
        }

        private Operation<double> GetOperationBySymbol(String Symbol) {
            for (int i = 0; i < Operations.Count; i++) {
                if (Operations[i].Symbol == Symbol) {
                    return Operations[i];
                }
            }
            throw new CalculationException("Something wrong!");
        }

        private Bracket GetBracketByCloseSymbol(String Symbol) {
            for (int i = 0; i < Brackets.Count; i++) {
                if (Brackets[i].Close == Symbol) {
                    return Brackets[i];
                }
            }
            throw new CalculationException("Something wrong!");
        }

        private IEnumerable<string> Seporate(String Expression) {
            Expression = Expression.Trim();
            int Position = 0;
            Regex PartRegex = CreatePartRegex();

            while (Position < Expression.Length) {
                Match RegexResult = PartRegex.Match(Expression.Substring(Position));
                if (!RegexResult.Success) {
                    throw new CalculationException("Illegal symbols in expression");
                }
                yield return RegexResult.Groups[1].Value;
                Position += RegexResult.Value.Length;
            }
        }

        private Regex CreatePartRegex() {
            String Pattern = @"^\s*(\d+(\,\d+)?";
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
            c.Solve(" 2,7 + [5*3+( 7- 2)] * 4 /5  ");
            Console.ReadKey();
        }
    }
}
