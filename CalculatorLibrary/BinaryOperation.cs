using System;

namespace CalculatorLibrary
{
    internal class BinaryOperation<T> : Operation<Func<T, T, T>>
    {
        public BinaryOperation(string symbol, Func<T, T, T> action, uint priority) : base(symbol, action, priority) {}
    }
}