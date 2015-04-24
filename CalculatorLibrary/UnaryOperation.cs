using System;

namespace CalculatorLibrary
{
    internal class UnaryOperation<T> : Operation<Func<T, T>> {
        public UnaryOperation(string symbol, Func<T, T> action, uint priority) : base(symbol, action, priority) { }
    }
}