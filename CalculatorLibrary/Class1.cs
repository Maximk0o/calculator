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
        private const String NumberRegex = @"\d+(,\d+)?";

        private enum LexemeType { OpenBracket, CloseBracket, Operation, Number, Unknown };

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

        private void FatalError(String Message) {
            throw new CalculationException(Message);
        }

        private bool IsOperation(String Lexeme) {
            return Operations.Any(x => (x.Symbol == Lexeme));
        }

        private bool IsOpenBracket(String Lexeme) {
            return Brackets.Any(x => (x.Symbol == Lexeme));
        }

        private bool IsCloseBracket(String Lexeme) {
            return Brackets.Any(x => (x.CloseSymbol == Lexeme));
        }

        private LexemeType GetLexemeType(String Lexeme) {
            double ForTest;

            if (Double.TryParse(Lexeme, out ForTest)) 
                return LexemeType.Number;

            if (IsOperation(Lexeme)) 
                return LexemeType.Operation;

            if (IsOpenBracket(Lexeme)) 
                return LexemeType.OpenBracket;

            if (IsCloseBracket(Lexeme)) 
                return LexemeType.CloseBracket;

            //Если лексема является числом, но не поместилась в double
            if (Regex.Match(Lexeme, NumberRegex).Success)
                FatalError("Number too big");

            return LexemeType.Unknown;
        }

        private bool ValidateOperation(String OperationSymbol) {
            bool IsBlank = String.IsNullOrWhiteSpace(OperationSymbol);
            bool IsIncludeWhitespaces = Regex.Match(OperationSymbol, @"\s+").Success;
            bool IsBracket = IsOpenBracket(OperationSymbol) || IsCloseBracket(OperationSymbol);
            bool IsNumber = Regex.Match(OperationSymbol, NumberRegex).Success;

            if (IsBlank || IsBracket || IsNumber || IsIncludeWhitespaces) 
                FatalError("Error while adding operations");

            bool IsExist = Operations.Any(x => (x.Symbol == OperationSymbol));
            return !IsExist;
        }

        private void AddOperation(Operation<double> NewOperation) {
            if (!ValidateOperation(NewOperation.Symbol)) 
                return;

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

        private bool ValidateBrackets(String Close, String Open) {
            bool IsBlank = String.IsNullOrWhiteSpace(Close) || String.IsNullOrWhiteSpace(Open);
            bool IsIncludeWhitespaces = Regex.Match(Open, @"\s+").Success || Regex.Match(Close, @"\s+").Success;
            bool IsOverlap = Brackets.Any(x => ((x.Symbol == Open) || (x.Symbol == Close) || (x.CloseSymbol == Open) || (x.CloseSymbol == Close)));
            bool IsOperation = Operations.Any(x => ((x.Symbol == Open) || (x.Symbol == Close)));
            bool IsEqual = (Open == Close);
            bool IsNumber = Regex.Match(Open, NumberRegex).Success || Regex.Match(Close, NumberRegex).Success;

            if (IsBlank || IsOverlap || IsOperation || IsEqual || IsNumber || IsIncludeWhitespaces) 
                FatalError("Error while adding brackets");

            bool IsExist = Brackets.Any(x => ((x.Symbol == Open) && (x.CloseSymbol == Close)));
            return !IsExist;
        }

        private void AddBrackets(Bracket NewBrackets) {
            if (!ValidateBrackets(NewBrackets.Symbol, NewBrackets.CloseSymbol)) 
                return;

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
            String PostfixExpression = Postfix(Expression);

            Stack<double> NumbersStack = new Stack<double>();

            foreach (String Part in Seporate(PostfixExpression)) {
                switch (GetLexemeType(Part)) {
                    case LexemeType.Number:
                        NumbersStack.Push(Convert.ToDouble(Part));
                        break;

                    case LexemeType.Operation:
                        if (NumbersStack.Count < 2)
                            FatalError("Error in expression");

                        double y = NumbersStack.Pop();
                        double x = NumbersStack.Pop();
                        NumbersStack.Push(GetOperationBySymbol(Part).Action(x, y));
                        break;

                    default:
                        FatalError("Something wrong!");
                        break;
                }
            }

            //В стеке должен остаться только результат вычислений
            if (NumbersStack.Count != 1) 
                FatalError("Error in expression");

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
                FatalError("Empty expression");
            }

            StringBuilder PostfixExpression = new StringBuilder();
            Stack<Operand> OperationStack = new Stack<Operand>();

            foreach (string Part in Seporate(Expression)) {

                switch (GetLexemeType(Part)) {
                    case LexemeType.Number:
                        PostfixExpression.Append(" " + Part);
                        break;

                    case LexemeType.OpenBracket:
                        OperationStack.Push(new Operand(Part, 0));
                        break;

                    case LexemeType.Operation:
                        Operand CurrentOperation = GetOperationBySymbol(Part);

                        while (OperationStack.Count != 0) {
                            if (OperationStack.Peek().Priority < CurrentOperation.Priority)
                                break;

                            PostfixExpression.Append(" " + OperationStack.Pop().Symbol);
                        }

                        OperationStack.Push(CurrentOperation);
                        break;

                    case LexemeType.CloseBracket:
                        Bracket CurrentBrackets = GetBracketByCloseSymbol(Part);

                        while (OperationStack.Count != 0) {
                            if(OperationStack.Peek().Symbol == CurrentBrackets.Symbol)
                                break;

                            if (IsOpenBracket(OperationStack.Peek().Symbol))
                                FatalError("Error in brackets");

                            PostfixExpression.Append(" " + OperationStack.Pop().Symbol);
                        }
                        if (OperationStack.Count == 0)
                            FatalError("Error in brackets");

                        OperationStack.Pop();
                        break;
                }
                
            }
            while (OperationStack.Count > 0) {
                if (IsOpenBracket(OperationStack.Peek().Symbol))
                    FatalError("Error in brackets");

                PostfixExpression.Append(" " + OperationStack.Pop().Symbol);
            }

            return PostfixExpression.ToString();
        }

        private Operation<double> GetOperationBySymbol(String Symbol) {
            int Index = Operations.FindIndex(x => (x.Symbol == Symbol));

            return Operations[Index];
        }

        private Bracket GetBracketByCloseSymbol(String Symbol) {
            int Index = Brackets.FindIndex(x => (x.CloseSymbol == Symbol));

            return Brackets[Index];
        }

        private IEnumerable<string> Seporate(String Expression) {
            int Position = 0;
            Regex PartRegex = CreatePartRegex();

            while (Position < Expression.Length) {
                Match RegexResult = PartRegex.Match(Expression.Substring(Position));

                if (!RegexResult.Success) 
                    FatalError("Illegal expression");

                yield return RegexResult.Groups[1].Value;

                Position += RegexResult.Value.Length;
            }
        }

        private Regex CreatePartRegex() {
            String Pattern = @"^\s*(" + NumberRegex;

            List<String> Operands = new List<String>();
            Operands.AddRange(Brackets.Select(x => x.Symbol));
            Operands.AddRange(Brackets.Select(x => x.CloseSymbol));
            Operands.AddRange(Operations.Select(x => x.Symbol));

            Operands.Sort((x, y) => (y.Length.CompareTo(x.Length)));

            for (int i = 0; i < Operands.Count; i++) 
                Pattern += "|" + Regex.Escape(Operands[i]);

            return new Regex(Pattern + ")");
        }
    }
}
