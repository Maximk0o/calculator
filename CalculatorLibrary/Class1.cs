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

    public class Calculator {
        private List<Operation<double>> Operations;
        private List<Bracket> Brackets;

        private class Operand {
            public String Symbol;
            public uint Priority;

            public Operand(String Symbol, uint Priority) {
                this.Symbol = Symbol;
                this.Priority = Priority;
            }
        }

        private class Operation<T> : Operand {
            public Func<T, T, T> Action;

            public Operation(String Symbol, Func<T, T, T> Action, uint Priority)
                : base(Symbol, Priority) {
                this.Action = Action;
            }
        }

        private class Bracket : Operand {
            public String CloseSymbol;

            public Bracket(String Open, String Close)
                : base(Open, 0) {
                this.CloseSymbol = Close;
            }
        }

        public Calculator() {
            Operations = new List<Operation<double>>();
            Brackets = new List<Bracket>();

            AddOperation(new Operation<double>("+", (x, y) => (y + x), 10));
            AddOperation(new Operation<double>("-", (x, y) => (y - x), 10));
            AddOperation(new Operation<double>("*", (x, y) => (y * x), 20));
            AddOperation(new Operation<double>("/", (x, y) => (y / x), 20));

            AddBrackets(new Bracket("(", ")"));
        }

        private void AddOperation(Operation<double> NewOperation) {
            if (Operations.Any(x => (x.Symbol == NewOperation.Symbol))) {
                return;
            }
            Operations.Add(NewOperation);
            Operations.Sort((x, y) => (y.Symbol.Length.CompareTo(x.Symbol.Length)));
        }

        public void AddOperation(String Symbol, Func<double, double, double> Action, uint Priority) {
            AddOperation(new Operation<double>(Symbol, Action, Priority));
        }

        private void AddBrackets(Bracket NewBrackets) {
            if (Brackets.Any(x => ((x.Symbol == NewBrackets.Symbol) || (x.Symbol == NewBrackets.CloseSymbol) || (x.CloseSymbol == NewBrackets.Symbol) || (x.CloseSymbol == NewBrackets.CloseSymbol)))) {
                return;
            }
            Brackets.Add(NewBrackets);
        }

        public void AddBrackets(String Open, String Close) {
            AddBrackets(new Bracket(Open, Close));
        }

        public double Solve(String Expression) {
            String PostfixExpression = Postfix(Expression);
            Stack<double> NumbersStack = new Stack<double>();
            double Temp;
            foreach (String Part in Seporate(PostfixExpression)) {
                if (double.TryParse(Part, out Temp)) {
                    NumbersStack.Push(Temp);
                }
                else if (Operations.Any(x => (x.Symbol == Part))) {
                    if (NumbersStack.Count < 2) {
                        throw new CalculationException("Error in expression");
                    }
                    NumbersStack.Push(GetOperationBySymbol(Part).Action(NumbersStack.Pop(), NumbersStack.Pop()));
                }
            }

            if (NumbersStack.Count != 1) {
                throw new CalculationException("Error in expression");
            }
            return NumbersStack.Pop();
        }

        private String Postfix(String Expression) {
            Expression = Expression.Trim();
            if (Expression.Length == 0) {
                throw new CalculationException("Empty expression");
            }
            StringBuilder PostfixExpression = new StringBuilder();
            Stack<Operand> OperationStack = new Stack<Operand>();
            double ForTest;

            foreach (string Part in Seporate(Expression)) {
                if (double.TryParse(Part, out ForTest)) {
                    PostfixExpression.Append(" ");
                    PostfixExpression.Append(Part);
                }
                else if (Brackets.Any(x => (x.Symbol == Part))) {
                    OperationStack.Push(new Operand(Part, 0));
                }
                else if (Operations.Any(x => (x.Symbol == Part))) {
                    Operation<double> CurrentOperation = GetOperationBySymbol(Part);
                    while (OperationStack.Count > 0) {
                        if (OperationStack.Peek().Priority < CurrentOperation.Priority) {
                            break;
                        }
                        PostfixExpression.Append(" ");
                        PostfixExpression.Append(OperationStack.Pop().Symbol);
                    }
                    OperationStack.Push(CurrentOperation);
                }
                else if (Brackets.Any(x => (x.CloseSymbol == Part))) {
                    Bracket CurrentBrackets = GetBracketByCloseSymbol(Part);
                    while (OperationStack.Count > 0) {
                        if (OperationStack.Peek().Symbol == CurrentBrackets.Symbol) {
                            break;
                        }
                        if (Brackets.Any(x => (x.Symbol == OperationStack.Peek().Symbol))) {
                            throw new CalculationException("Error in brackets");
                        }
                        PostfixExpression.Append(" ");
                        PostfixExpression.Append(OperationStack.Pop().Symbol);
                    }
                    if (OperationStack.Count == 0) {
                        throw new CalculationException("Error in brackets");
                    }
                    OperationStack.Pop();
                }
            }
            while (OperationStack.Count > 0) {
                if (Brackets.Any(x => (x.Symbol == OperationStack.Peek().Symbol))) {
                    throw new CalculationException("Error in brackets");
                }
                PostfixExpression.Append(" ");
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
                if (Brackets[i].CloseSymbol == Symbol) {
                    return Brackets[i];
                }
            }
            throw new CalculationException("Something wrong!");
        }

        private IEnumerable<string> Seporate(String Expression) {
            int Position = 0;
            Regex PartRegex = CreatePartRegex();

            while (Position < Expression.Length) {
                Match RegexResult = PartRegex.Match(Expression.Substring(Position));
                if (!RegexResult.Success) {
                    throw new CalculationException("Illegal expression");
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
                Pattern += "|" + Regex.Escape(Brackets[i].Symbol.ToString()) + "|" + Regex.Escape(Brackets[i].CloseSymbol.ToString());
            }
            return new Regex(Pattern + ")");
        }
    }
}
