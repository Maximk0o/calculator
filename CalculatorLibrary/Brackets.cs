namespace CalculatorLibrary
{
    internal class Bracket : Operand
    {
        public readonly string CloseSymbol;

        public Bracket(string open, string close)
            : base(open, 0)
        {
            CloseSymbol = close;
        }
    }
}