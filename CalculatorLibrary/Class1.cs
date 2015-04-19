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

    /// <summary>
    /// Класс вычисления результата арифметических выражений
    /// </summary>
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

            AddOperation(new Operation<double>("+", (x, y) => (x + y), 10));
            AddOperation(new Operation<double>("-", (x, y) => (x - y), 10));
            AddOperation(new Operation<double>("*", (x, y) => (x * y), 20));
            AddOperation(new Operation<double>("/", (x, y) => (x / y), 20));

            AddBrackets(new Bracket("(", ")"));
        }

        private void AddOperation(Operation<double> NewOperation) {
            if (NewOperation.Symbol == String.Empty || NewOperation.Symbol == null || NewOperation.Action == null) {
                throw new CalculationException("Incorrect operation data");
            }
            else if (Regex.Match(NewOperation.Symbol, @"\s+").Success) {
                throw new CalculationException("Symbol can't include whitespace");
            }
            else if (Operations.Any(x => (x.Symbol == NewOperation.Symbol))) {
                return;
            }
            else if (Brackets.Any(x => ((x.Symbol == NewOperation.Symbol) || (x.CloseSymbol == NewOperation.Symbol)))) {
                throw new CalculationException("Operation symbol is bracket");
            }
            else if (Regex.Match(NewOperation.Symbol, @"(\d+(,\d+)?)|,").Success) {
                throw new CalculationException("Illegal operation format");
            }

            Operations.Add(NewOperation);
        }

        /// <summary>
        /// Добавление новой бинарной алгебраической операции
        /// </summary>
        /// <param name="Symbol">Символ операции</param>
        /// <param name="Action">Функция, описывающая операцию</param>
        /// <param name="Priority">Приоритет операции</param>
        public void AddOperation(String Symbol, Func<double, double, double> Action, uint Priority) {
            AddOperation(new Operation<double>(Symbol, Action, Priority));
        }

        private void AddBrackets(Bracket NewBrackets) {
            if (NewBrackets.Symbol == null || NewBrackets.CloseSymbol == null || NewBrackets.Symbol == String.Empty || NewBrackets.CloseSymbol == String.Empty) {
                throw new CalculationException("Incorrect brackets data");
            }
            else if (Regex.Match(NewBrackets.Symbol, @"\s+").Success || Regex.Match(NewBrackets.CloseSymbol, @"\s+").Success) {
                throw new CalculationException("Symbol can't include whitespace");
            }
            else if (Brackets.Any(x => ((x.Symbol == NewBrackets.Symbol) && (x.CloseSymbol == NewBrackets.CloseSymbol)))) {
                return;
            }
            else if (Brackets.Any(x => ((x.Symbol == NewBrackets.Symbol) || (x.Symbol == NewBrackets.CloseSymbol) ||
                    (x.CloseSymbol == NewBrackets.Symbol) || (x.CloseSymbol == NewBrackets.CloseSymbol)))) {
                throw new CalculationException("Brackets overlaps with exists");
            }
            else if (Operations.Any(x => ((x.Symbol == NewBrackets.Symbol) || (x.Symbol == NewBrackets.CloseSymbol)))) {
                throw new CalculationException("One or more symbols is exists operation");
            }
            else if (NewBrackets.Symbol == NewBrackets.CloseSymbol) {
                throw new CalculationException("Opend and closed brackets can't be the same");
            }
            else if (Regex.Match(NewBrackets.Symbol, @"(\d+(,\d+)?)|,").Success || Regex.Match(NewBrackets.CloseSymbol, @"(\d+(,\d+)?)|,").Success) {
                throw new CalculationException("Illegal bracket format");
            }

            Brackets.Add(NewBrackets);
        }

        /// <summary>
        /// Добавление нового вида скобок
        /// </summary>
        /// <param name="Open">Символ открывающейся скобки</param>
        /// <param name="Close">Символ закрывающейся скобки</param>
        public void AddBrackets(String Open, String Close) {
            AddBrackets(new Bracket(Open, Close));
        }

        /// <summary>
        /// Метод решения арифметического выражения
        /// </summary>
        /// <param name="Expression">Строка - арифметическое выражение</param>
        /// <returns>Результат вычисления выражения</returns>
        public double Solve(String Expression) {
            if (Expression == null) {
                throw new CalculationException("Error in expression");
            }

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

                    double y = NumbersStack.Pop();
                    double x = NumbersStack.Pop();
                    NumbersStack.Push(GetOperationBySymbol(Part).Action(x, y));

                    if (Double.IsNaN(NumbersStack.Peek())) {
                        throw new CalculationException("Result is too big");
                    }
                }
                else {
                    throw new CalculationException("Something wrong!");
                }
            }

            if (NumbersStack.Count != 1) {
                throw new CalculationException("Error in expression");
            }

            return NumbersStack.Pop();
        }

        /// <summary>
        /// Метод преобразования инфиксной записи в постфиксную
        /// </summary>
        /// <param name="Expression">Выражение в инфиксной форме</param>
        /// <returns>Выражение в постфиксной форме</returns>
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
                    Operand CurrentOperation = GetOperationBySymbol(Part);

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
                else if (Regex.Match(Part, @"^\d+(,\d+)?").Success) {
                    throw new CalculationException("Number too big");
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

            List<String> Operands = new List<String>();
            Operands.AddRange(Brackets.Select(x => x.Symbol));
            Operands.AddRange(Brackets.Select(x => x.CloseSymbol));
            Operands.AddRange(Operations.Select(x => x.Symbol));

            Operands.Sort((x, y) => (y.Length.CompareTo(x.Length)));

            for (int i = 0; i < Operands.Count; i++) {
                Pattern += "|" + Regex.Escape(Operands[i]);
            }

            return new Regex(Pattern + ")");
        }
    }
}
