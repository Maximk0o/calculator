using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CalculatorLibrary
{
    /// <summary>
    /// Класс вычисления результата арифметических выражений
    /// </summary>
    public class Calculator
    {
        private readonly List<BinaryOperation<double>> _binaryOperations;
        private readonly List<UnaryOperation<double>> _unaryOperations;
        private readonly List<Bracket> _brackets;
        private const string NumberRegex = @"\d+(,\d+)?";

        private enum LexemeType
        {
            OpenBracket,
            CloseBracket,
            BinaryOperation,
            UnaryOperation,
            Number,
            Unknown
        };

        public Calculator()
        {
            _binaryOperations = new List<BinaryOperation<double>>();
            _unaryOperations = new List<UnaryOperation<double>>();
            _brackets = new List<Bracket>();

            AddOperation("+", (x, y) => (x + y), 10);
            AddOperation("-", (x, y) => (x - y), 10);
            AddOperation("*", (x, y) => (x * y), 20);
            AddOperation("/", (x, y) => (x / y), 20);

            AddOperation("-", x => (-x), 30);

            AddBrackets(new Bracket("(", ")"));
        }

        private void FatalError(string message)
        {
            throw new CalculationException.CalculationException(message);
        }

        private bool IsBinaryOperation(string lexeme)
        {
            return _binaryOperations.Any(x => (x.Symbol == lexeme));
        }

        private bool IsUnaryOperation(string lexeme)
        {
            return _unaryOperations.Any(x => (x.Symbol == lexeme));
        }

        private bool IsOpenBracket(string lexeme)
        {
            return _brackets.Any(x => (x.Symbol == lexeme));
        }

        private bool IsCloseBracket(string lexeme)
        {
            return _brackets.Any(x => (x.CloseSymbol == lexeme));
        }

        private bool IsNumber(string lexeme)
        {
            double forTest;
            return double.TryParse(lexeme, out forTest);
        }

        private LexemeType GetLexemeType(string lexeme)
        {
            if (IsNumber(lexeme))
                return LexemeType.Number;

            if (IsBinaryOperation(lexeme))
                return LexemeType.BinaryOperation;

            if (IsUnaryOperation(lexeme))
                return LexemeType.UnaryOperation;

            if (IsOpenBracket(lexeme))
                return LexemeType.OpenBracket;

            if (IsCloseBracket(lexeme))
                return LexemeType.CloseBracket;

            //Если лексема является числом, но не поместилась в double
            if (Regex.Match(lexeme, NumberRegex).Success)
                FatalError("Number too big");

            return LexemeType.Unknown;
        }

        private bool ValidateOperation(string operationSymbol)
        {
            if(string.IsNullOrWhiteSpace(operationSymbol))
                FatalError("Error while adding operations");

            bool isIncludeWhitespaces = Regex.Match(operationSymbol, @"\s+").Success;
            bool isBracket = IsOpenBracket(operationSymbol) || IsCloseBracket(operationSymbol);
            bool isNumber = Regex.Match(operationSymbol, NumberRegex).Success;

            if (isBracket || isNumber || isIncludeWhitespaces)
                FatalError("Error while adding operations");

            bool isExist = _binaryOperations.Any(x => (x.Symbol == operationSymbol));
            return !isExist;
        }

        private void AddOperation(BinaryOperation<double> newBinaryOperation) {
            if (!ValidateOperation(newBinaryOperation.Symbol))
                return;

            _binaryOperations.Add(newBinaryOperation);
        }

        private void AddOperation(UnaryOperation<double> newUnaryOperation) {
            if (!ValidateOperation(newUnaryOperation.Symbol))
                return;

            _unaryOperations.Add(newUnaryOperation);
        }

        /// <summary>
        /// Добавление новой бинарной алгебраической операции
        /// </summary>
        /// <param name="symbol">Символ операции</param>
        /// <param name="action">Функция, описывающая операцию</param>
        /// <param name="priority">Приоритет операции</param>
        public void AddOperation(string symbol, Func<double, double, double> action, uint priority)
        {
            AddOperation(new BinaryOperation<double>(symbol, action, priority));
        }

        /// <summary>
        /// Добавление новой унарной алгебраической операции
        /// </summary>
        /// <param name="symbol">Символ операции</param>
        /// <param name="action">Функция, описывающая операцию</param>
        /// <param name="priority">Приоритет операции</param>
        public void AddOperation(string symbol, Func<double, double> action, uint priority) {
            if (IsBinaryOperation(symbol))
                symbol = "UNARY" + symbol;

            AddOperation(new UnaryOperation<double>(symbol, action, priority));
        }

        private bool ValidateBrackets(string close, string open)
        {
            if(string.IsNullOrWhiteSpace(close) || string.IsNullOrWhiteSpace(open))
                FatalError("Error while adding brackets");

            bool isIncludeWhitespaces = Regex.Match(open, @"\s+").Success || Regex.Match(close, @"\s+").Success;
            bool isOverlap =
                _brackets.Any(
                    x =>
                        ((x.Symbol == open) || (x.Symbol == close) || (x.CloseSymbol == open) ||
                         (x.CloseSymbol == close)));
            bool isOperation = _binaryOperations.Any(x => ((x.Symbol == open) || (x.Symbol == close)));
            bool isEqual = (open == close);
            bool isNumber = Regex.Match(open, NumberRegex).Success || Regex.Match(close, NumberRegex).Success;

            if (isOverlap || isOperation || isEqual || isNumber || isIncludeWhitespaces)
                FatalError("Error while adding brackets");

            bool isExist = _brackets.Any(x => ((x.Symbol == open) && (x.CloseSymbol == close)));
            return !isExist;
        }

        private void AddBrackets(Bracket newBrackets)
        {
            if (!ValidateBrackets(newBrackets.Symbol, newBrackets.CloseSymbol))
                return;

            _brackets.Add(newBrackets);
        }

        /// <summary>
        /// Добавление нового вида скобок
        /// </summary>
        /// <param name="open">Символ открывающейся скобки</param>
        /// <param name="close">Символ закрывающейся скобки</param>
        public void AddBrackets(string open, string close)
        {
            AddBrackets(new Bracket(open, close));
        }

        /// <summary>
        /// Метод решения арифметического выражения
        /// </summary>
        /// <param name="expression">Строка - арифметическое выражение</param>
        /// <returns>Результат вычисления выражения</returns>
        public double Solve(string expression)
        {
            string postfixExpression = Postfix(expression);

            Stack<double> numbersStack = new Stack<double>();

            foreach (string part in postfixExpression.Split(' '))
            {
                switch (GetLexemeType(part))
                {
                    case LexemeType.Number:
                        numbersStack.Push(Convert.ToDouble(part));
                        break;

                    case LexemeType.BinaryOperation:
                        if (numbersStack.Count < 2)
                            FatalError("Error in expression");

                        double y = numbersStack.Pop();
                        double x = numbersStack.Pop();
                        numbersStack.Push(GetBinaryOperationBySymbol(part).Action(x, y));
                        break;

                    case LexemeType.UnaryOperation:
                        if (numbersStack.Count < 1)
                            FatalError("Error in expression");

                        numbersStack.Push(GetUnaryOperationBySymbol(part).Action(numbersStack.Pop()));
                        break;

                    default:
                        FatalError("Something wrong!");
                        break;
                }
            }

            //В стеке должен остаться только результат вычислений
            if (numbersStack.Count != 1)
                FatalError("Error in expression");

            return numbersStack.Pop();
        }

        /// <summary>
        /// Метод преобразования инфиксной записи в постфиксную
        /// </summary>
        /// <param name="expression">Выражение в инфиксной форме</param>
        /// <returns>Выражение в постфиксной форме</returns>
        private string Postfix(string expression)
        {
            expression = expression.Trim();

            if (expression.Length == 0)
            {
                FatalError("Empty expression");
            }

            StringBuilder postfixExpression = new StringBuilder();
            Stack<Operand> operationStack = new Stack<Operand>();

            foreach (string part in Seporate(expression))
            {

                switch (GetLexemeType(part))
                {
                    case LexemeType.Number:
                        postfixExpression.Append(" " + part);
                        break;

                    case LexemeType.OpenBracket:
                        operationStack.Push(new Operand(part, 0));
                        break;

                    case LexemeType.BinaryOperation:
                    case LexemeType.UnaryOperation:
                        Operand currentOperation = GetOperationBySymbol(part);

                        while (operationStack.Count != 0)
                        {
                            if (operationStack.Peek().Priority < currentOperation.Priority)
                                break;

                            postfixExpression.Append(" " + operationStack.Pop().Symbol);
                        }

                        operationStack.Push(currentOperation);
                        break;

                    case LexemeType.CloseBracket:
                        Bracket currentBrackets = GetBracketByCloseSymbol(part);

                        while (operationStack.Count != 0)
                        {
                            if (operationStack.Peek().Symbol == currentBrackets.Symbol)
                                break;

                            if (IsOpenBracket(operationStack.Peek().Symbol))
                                FatalError("Error in brackets");

                            postfixExpression.Append(" " + operationStack.Pop().Symbol);
                        }
                        if (operationStack.Count == 0)
                            FatalError("Error in brackets");

                        operationStack.Pop();
                        break;
                }

            }
            while (operationStack.Count > 0)
            {
                if (IsOpenBracket(operationStack.Peek().Symbol))
                    FatalError("Error in brackets");

                postfixExpression.Append(" " + operationStack.Pop().Symbol);
            }

            return postfixExpression.ToString().Trim();
        }

        private BinaryOperation<double> GetBinaryOperationBySymbol(string symbol) {
            int index = _binaryOperations.FindIndex(x => (x.Symbol == symbol));

            return _binaryOperations[index];
        }

        private UnaryOperation<double> GetUnaryOperationBySymbol(string symbol) {
            int index = _unaryOperations.FindIndex(x => (x.Symbol == symbol));

            return _unaryOperations[index];
        }

        private Operand GetOperationBySymbol(string symbol) {
            int index = _binaryOperations.FindIndex(x => (x.Symbol == symbol));

            if (index < 0)
            {
                index = _unaryOperations.FindIndex(x => (x.Symbol == symbol));
                return _unaryOperations[index];
            }

            return _binaryOperations[index];
        }

        private Bracket GetBracketByCloseSymbol(string symbol)
        {
            int index = _brackets.FindIndex(x => (x.CloseSymbol == symbol));

            return _brackets[index];
        }

        private IEnumerable<string> Seporate(string expression)
        {
            int position = 0;
            Regex partRegex = CreatePartRegex();
            string lastPart = null;

            while (position < expression.Length)
            {
                Match regexResult = partRegex.Match(expression.Substring(position));

                if (!regexResult.Success)
                    FatalError("Illegal expression");

                string currentPart = regexResult.Groups[1].Value;

                if (IsBinaryOperation(currentPart) && ((!IsNumber(lastPart) && !IsCloseBracket(lastPart)) ||
                    lastPart == null))
                {
                    currentPart = "UNARY" + currentPart;
                    if(!IsUnaryOperation(currentPart))
                        FatalError("Illegal expression");
                }

                yield return currentPart;

                lastPart = currentPart;
                position += regexResult.Value.Length;
            }
        }

        private Regex CreatePartRegex()
        {
            List<string> operands = new List<string>();
            operands.AddRange(_brackets.Select(x => x.Symbol));
            operands.AddRange(_brackets.Select(x => x.CloseSymbol));
            operands.AddRange(_binaryOperations.Select(x => x.Symbol));

            operands.Sort((x, y) => (y.Length.CompareTo(x.Length)));

            string pattern = operands.Aggregate(@"^\s*(" + NumberRegex, (current, t) => current + ("|" + Regex.Escape(t)));

            return new Regex(pattern + ")");
        }
    }
}
