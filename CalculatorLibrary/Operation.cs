namespace CalculatorLibrary
{
    internal class Operation<TFunc> : Operand {
        public readonly TFunc Action;

        public Operation(string symbol, TFunc action, uint priority)
            : base(symbol, priority) {
            Action = action;
        }
    }
}