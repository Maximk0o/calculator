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
        private List<BinaryOperation<double>> BinaryOperations;
        private List<UnaryOperation<double>> UnaryOperations;
        private List<Bracket> Brackets;
        public bool SameBynaryAndUnarySymbols = false;

        private class Operand {
            public String Symbol;
            public uint Priority;

            public Operand(String Symbol, uint Priority) {
                this.Symbol = Symbol;
                this.Priority = Priority;
            }
        }

        private class BinaryOperation<T> : Operand {
            public Func<T, T, T> Action;

            public BinaryOperation(String Symbol, Func<T, T, T> Action, uint Priority)
                : base(Symbol, Priority) {
                this.Action = Action;
            }
        }

        private class UnaryOperation<T> : BinaryOperation<T> {
            public Func<T, T> Action;

            public UnaryOperation(String Symbol, Func<T, T> Action, uint Priority)
                : base(Symbol, null, Priority) {
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
            BinaryOperations = new List<BinaryOperation<double>>();
            UnaryOperations = new List<UnaryOperation<double>>();
            Brackets = new List<Bracket>();

            AddBinaryOperation(new BinaryOperation<double>("+", (x, y) => (x + y), 10));
            AddBinaryOperation(new BinaryOperation<double>("-", (x, y) => (x - y), 10));
            AddBinaryOperation(new BinaryOperation<double>("*", (x, y) => (x * y), 20));
            AddBinaryOperation(new BinaryOperation<double>("/", (x, y) => (x / y), 20));

            AddUnaryOperation(new UnaryOperation<double>("Abs", (x) => (Math.Abs(x)), 0));

            AddBrackets(new Bracket("(", ")"));
        }

        private void AddBinaryOperation(BinaryOperation<double> NewOperation) {
            if (UnaryOperations.Any(x => (x.Symbol == NewOperation.Symbol)) && !SameBynaryAndUnarySymbols) {
                throw new CalculationException("Binary and unary operations can't have same symbols");
            }
            else if (BinaryOperations.Any(x => (x.Symbol == NewOperation.Symbol))) {
                UnaryOperation<double> Temp = GetUnaryOperationBySymbol(NewOperation.Symbol);
                CreateSameOperations(NewOperation,
                                     Temp,
                                     out NewOperation,
                                     out Temp
                );
            }
            else if (BinaryOperations.Any(x => (x.Symbol == NewOperation.Symbol))) {
                return;
            }
            BinaryOperations.Add(NewOperation);
        }

        public void AddBinaryOperation(String Symbol, Func<double, double, double> Action, uint Priority) {
            AddBinaryOperation(new BinaryOperation<double>(Symbol, Action, Priority));
        }

        private void AddUnaryOperation(UnaryOperation<double> NewOperation) {
            if (UnaryOperations.Any(x => (x.Symbol == NewOperation.Symbol))) {
                return;
            }
            else if (BinaryOperations.Any(x => (x.Symbol == NewOperation.Symbol)) && !SameBynaryAndUnarySymbols) {
                throw new CalculationException("Binary and unary operations can't have same symbols");
            }
            else if (BinaryOperations.Any(x => (x.Symbol == NewOperation.Symbol))) {
                BinaryOperation<double> Temp;
                CreateSameOperations(GetOperationBySymbol(NewOperation.Symbol), 
                                     NewOperation,
                                     out Temp,
                                     out NewOperation
                );
            }

            UnaryOperations.Add(NewOperation);
        }

        public void AddUnaryOperation(String Symbol, Func<double, double> Action, uint Priority) {
            AddUnaryOperation(new UnaryOperation<double>(Symbol, Action, Priority));
        }

        private void CreateSameOperations(BinaryOperation<double> Binary, UnaryOperation<double> Unary,
                     out BinaryOperation<double> NewBinary, out UnaryOperation<double> NewUnary) {
            NewBinary = Binary;
            if (Binary.Symbol == null) {
                NewBinary.Symbol = Unary.Symbol;
            }
            NewUnary = Unary;
            NewUnary.Symbol = "UNARY" + Binary.Symbol;
        }

        private void AddBrackets(Bracket NewBrackets) {
            if (Brackets.Any(x => ((x.Symbol == NewBrackets.Symbol) && (x.CloseSymbol == NewBrackets.CloseSymbol)))) {
                return;
            }
            else if (Brackets.Any(x => ((x.Symbol == NewBrackets.Symbol) || (x.Symbol == NewBrackets.CloseSymbol) || 
                    (x.CloseSymbol == NewBrackets.Symbol) || (x.CloseSymbol == NewBrackets.CloseSymbol)))) {
                throw new CalculationException("Brackets owerlaps with exists");
            }
            Brackets.Add(NewBrackets);
        }

        public void AddBrackets(String Open, String Close) {
            AddBrackets(new Bracket(Open, Close));
        }

        public double Solve(String Expression) {
            String PostfixExpression = Postfix(Expression);
            bool OldSameBynaryAndUnarySymbols = SameBynaryAndUnarySymbols;
            SameBynaryAndUnarySymbols = false;
            Stack<double> NumbersStack = new Stack<double>();
            double Temp;
            foreach (String Part in Seporate(PostfixExpression)) {
                if (double.TryParse(Part, out Temp)) {
                    NumbersStack.Push(Temp);
                }
                else if (BinaryOperations.Any(x => (x.Symbol == Part))) {
                    if (NumbersStack.Count < 2) {
                        throw new CalculationException("Error in expression");
                    }
                    double y = NumbersStack.Pop();
                    double x = NumbersStack.Pop();
                    NumbersStack.Push(GetOperationBySymbol(Part).Action(x, y));
                }
                else if (UnaryOperations.Any(x => (x.Symbol == Part))) {
                    if (NumbersStack.Count < 1) {
                        throw new CalculationException("Error in expression");
                    }
                    NumbersStack.Push(GetUnaryOperationBySymbol(Part).Action(NumbersStack.Pop()));
                }
                else {
                    throw new CalculationException("Something wrong!");
                }
            }

            if (NumbersStack.Count != 1) {
                throw new CalculationException("Error in expression");
            }
            SameBynaryAndUnarySymbols = OldSameBynaryAndUnarySymbols;
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
                else if (BinaryOperations.Any(x => (x.Symbol == Part)) || UnaryOperations.Any(x => (x.Symbol == Part))) {
                    Operand CurrentOperand = GetOperationBySymbol(Part);
                    while (OperationStack.Count > 0) {
                        if (OperationStack.Peek().Priority < CurrentOperand.Priority) {
                            break;
                        }
                        PostfixExpression.Append(" ");
                        PostfixExpression.Append(OperationStack.Pop().Symbol);
                    }
                    OperationStack.Push(CurrentOperand);
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
                else {
                    throw new CalculationException("Something wrong!");
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

        private BinaryOperation<double> GetOperationBySymbol(String Symbol) {
            for (int i = 0; i < BinaryOperations.Count; i++) {
                if (BinaryOperations[i].Symbol == Symbol) {
                    return BinaryOperations[i];
                }
            }
            for (int i = 0; i < UnaryOperations.Count; i++) {
                if (UnaryOperations[i].Symbol == Symbol) {
                    return UnaryOperations[i];
                }
            }
            throw new CalculationException("Something wrong!");
        }

        private UnaryOperation<double> GetUnaryOperationBySymbol(String Symbol) {
            for (int i = 0; i < UnaryOperations.Count; i++) {
                if (UnaryOperations[i].Symbol == Symbol) {
                    return UnaryOperations[i];
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

            String LastPart = null;

            while (Position < Expression.Length) {
                Match RegexResult = PartRegex.Match(Expression.Substring(Position));
                if (!RegexResult.Success) {
                    throw new CalculationException("Illegal expression");
                }
                String CurrentPart = RegexResult.Groups[1].Value;
                if (SameBynaryAndUnarySymbols) { 
                    double Temp;
                    if (!Double.TryParse(CurrentPart, out Temp) && Brackets.All(x => (x.Symbol != CurrentPart))) {
                        if (LastPart == null) {
                            CurrentPart = "UNARY" + CurrentPart;
                        }
                        else if (!Double.TryParse(LastPart, out Temp) && Brackets.All(x => (x.CloseSymbol != LastPart))) {
                            CurrentPart = "UNARY" + CurrentPart;
                        } 
                    }
                }
                yield return CurrentPart;
                LastPart = CurrentPart;
                Position += RegexResult.Value.Length;
            }
        }

        private Regex CreatePartRegex() {
            String Pattern = @"^\s*(\d+(\,\d+)?";
            List<Operand> Operands = new List<Operand>();
            Operands.AddRange(BinaryOperations);
            Operands.AddRange(UnaryOperations);
            Operands.AddRange(Brackets);
            Operands.Sort((x, y) => (y.Symbol.Length.CompareTo(x.Symbol.Length)));
            for (int i = 0; i < Operands.Count; i++) {
                Pattern += "|" + Regex.Escape(Operands[i].Symbol);
            }
            for (int i = 0; i < Brackets.Count; i++) {
                Pattern += "|" + Regex.Escape(Brackets[i].CloseSymbol.ToString());
            }
            return new Regex(Pattern + ")");
        }
    }
}
