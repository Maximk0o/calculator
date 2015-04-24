namespace CalculatorLibrary
{
    internal class Operand
    {
        public string Symbol;
        public readonly uint Priority;

        public Operand(string symbol, uint priority) {
            Symbol = symbol;
            Priority = priority;
        } 
    }
}