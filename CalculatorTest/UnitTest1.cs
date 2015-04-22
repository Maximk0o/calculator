using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using calculator;

namespace CalculatorTest {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void BasicTest() {
            Calculator calc = new Calculator();
            calc.AddBrackets("[", "]");
            calc.AddOperation("**", (x, y) => (Math.Pow(x, y)), 30);

            Double result = calc.Solve(" ( 2,7 + [5*3+( 7- 2)] * [4 /(5  -4 +(9-2))])**2 \n\r\t");

            Assert.AreEqual(161.29, result);
        }

        [TestMethod]
        public void ProcedureTest() {
            Calculator calc = new Calculator();

            Double result = calc.Solve("2+2*2");

            Assert.AreEqual(6, result);
        }

        [TestMethod]
        [ExpectedException(typeof(CalculationException))]
        public void UndefinedOperatorTest() {
            Calculator calc = new Calculator();

            calc.Solve(" 2,7 & 2");
        }

        [TestMethod]
        public void IntricateBracketsTest() {
            Calculator calc = new Calculator();
            calc.AddBrackets("@--", "--@");
            calc.AddBrackets("((", "))");

            Double result = calc.Solve(" @--( 2,4 + ((4 - 5 - 4)) )--@ * 3");

            Assert.AreEqual(((2.4 + (4 - 5 - 4))) * 3, result);
        }

        [TestMethod]
        [ExpectedException(typeof(CalculationException))]
        public void ExistsBracketsTest() {
            Calculator calc = new Calculator();

            calc.AddBrackets("(", "%");
        }

        [TestMethod]
        [ExpectedException(typeof(CalculationException))]
        public void OperationSymbolBracketsTest() {
            Calculator calc = new Calculator();

            calc.AddBrackets(":", "+");
        }

        [TestMethod]
        [ExpectedException(typeof(CalculationException))]
        public void NumberBracketsTest() {
            Calculator calc = new Calculator();

            calc.AddBrackets(":", "@9,34--");
        }

        [TestMethod]
        [ExpectedException(typeof(CalculationException))]
        public void SameBracketsTest() {
            Calculator calc = new Calculator();

            calc.AddBrackets(":", ":");
        }

        [TestMethod]
        [ExpectedException(typeof(CalculationException))]
        public void EmptyBracketsTest() {
            Calculator calc = new Calculator();

            calc.AddBrackets("", ":");
        }

        [TestMethod]
        [ExpectedException(typeof(CalculationException))]
        public void WhitespaceBracketsTest() {
            Calculator calc = new Calculator();

            calc.AddBrackets("\n", ":");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullBracketsTest() {
            Calculator calc = new Calculator();

            calc.AddBrackets("^", null);
        }

        [TestMethod]
        [ExpectedException(typeof(CalculationException))]
        public void OverlapsBracketsTest() {
            Calculator calc = new Calculator();
            calc.AddBrackets("[", "]");

            calc.Solve(" ( 2,4 + 4 - [ 5 - 4) ]");
        }

        [TestMethod]
        [ExpectedException(typeof(CalculationException))]
        public void ExcessCloseBracketTest() {
            Calculator calc = new Calculator();
            calc.AddBrackets("[", "]");

            calc.Solve(" ( 2,4 + 4) - [ 5 - 4 ])");
        }

        [TestMethod]
        [ExpectedException(typeof(CalculationException))]
        public void ExcessOpenBracketTest() {
            Calculator calc = new Calculator();
            calc.AddBrackets("[", "]");

            calc.Solve(" (( 2,4 + 4) - [ 5 - 4 ]");
        }

        [TestMethod]
        [ExpectedException(typeof(CalculationException))]
        public void IllegalNumberFormatTest1() {
            Calculator calc = new Calculator();

            calc.Solve(" ( 2,4,7 + 4) ");
        }

        [TestMethod]
        [ExpectedException(typeof(CalculationException))]
        public void IllegalNumberFormatTest2() {
            Calculator calc = new Calculator();

            calc.Solve(" ( 2, + 4) ");
        }

        [TestMethod]
        [ExpectedException(typeof(CalculationException))]
        public void IllegalNumberFormatTest3() {
            Calculator calc = new Calculator();

            calc.Solve(" ( 2 + 4,,4) ");
        }

        [TestMethod]
        [ExpectedException(typeof(CalculationException))]
        public void IllegalNumberFormatTest4() {
            Calculator calc = new Calculator();

            calc.Solve(" ( 2 + ,4) ");
        }

        [TestMethod]
        [ExpectedException(typeof(CalculationException))]
        public void BigNumberTest() {
            Calculator calc = new Calculator();

            calc.Solve(" 10000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000 ");
        }

        [TestMethod]
        [ExpectedException(typeof(CalculationException))]
        public void EmptyExpressionTest() {
            Calculator calc = new Calculator();

            calc.Solve("   \n\t ");
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void NullExpressionTest() {
            Calculator calc = new Calculator();

            calc.Solve(null);
        }

        [TestMethod]
        [ExpectedException(typeof(CalculationException))]
        public void OperationLessTest() {
            Calculator calc = new Calculator();

            calc.Solve(" ( 2,4 + 4)7,5+4 ");
        }

        [TestMethod]
        [ExpectedException(typeof(CalculationException))]
        public void NumberLessTest() {
            Calculator calc = new Calculator();

            calc.Solve(" ( 2 + -4) ");
        }

        [TestMethod]
        [ExpectedException(typeof(CalculationException))]
        public void AddBracketOperationTest() {
            Calculator calc = new Calculator();

            calc.AddOperation(")", (x, y) => (x % y), 54);
        }

        [TestMethod]
        [ExpectedException(typeof(CalculationException))]
        public void AddEmptyOperationTest() {
            Calculator calc = new Calculator();

            calc.AddOperation("", (x, y) => (x % y), 54);
        }

        [TestMethod]
        [ExpectedException(typeof(CalculationException))]
        public void AddOperationWithWhitespaceTest() {
            Calculator calc = new Calculator();

            calc.AddOperation("% %", (x, y) => (x % y), 54);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNullSymbolOperationTest() {
            Calculator calc = new Calculator();

            calc.AddOperation(null, (x, y) => (x % y), 54);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void AddNullActionOperationTest() {
            Calculator calc = new Calculator();
            calc.AddOperation("$", null, 51);

            calc.Solve("2 $ 2");
        }
    }
}
