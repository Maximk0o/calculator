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
        public void UndefinedOperatorTest() {
            Calculator calc = new Calculator();

            try {
                calc.Solve(" 2,7 & 2");
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Illegal expression");
                return;
            }
            Assert.Fail("No exception was thrown.");
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
        public void ExistsBracketsTest() {
            Calculator calc = new Calculator();

            try {
                calc.AddBrackets("(", "%");
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Brackets overlaps with exists");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void OperationSymbolBracketsTest() {
            Calculator calc = new Calculator();

            try {
                calc.AddBrackets(":", "+");
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "One or more symbols is exists operation");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void NumberBracketsTest() {
            Calculator calc = new Calculator();

            try {
                calc.AddBrackets(":", "@9,34--");
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Illegal bracket format");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void CommaBracketsTest() {
            Calculator calc = new Calculator();

            try {
                calc.AddBrackets("=,@", "--");
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Illegal bracket format");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void SameBracketsTest() {
            Calculator calc = new Calculator();

            try {
                calc.AddBrackets(":", ":");
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Opend and closed brackets can't be the same");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void EmptyBracketsTest() {
            Calculator calc = new Calculator();

            try {
                calc.AddBrackets("", ":");
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Incorrect brackets data");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void WhitespaceBracketsTest() {
            Calculator calc = new Calculator();

            try {
                calc.AddBrackets("\n", ":");
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Symbol can't include whitespace");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void NullBracketsTest() {
            Calculator calc = new Calculator();

            try {
                calc.AddBrackets("^", null);
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Incorrect brackets data");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void OverlapsBracketsTest() {
            Calculator calc = new Calculator();
            calc.AddBrackets("[", "]");

            try {
                calc.Solve(" ( 2,4 + 4 - [ 5 - 4) ]");
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Error in brackets");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void ExcessCloseBracketTest() {
            Calculator calc = new Calculator();
            calc.AddBrackets("[", "]");

            try {
                calc.Solve(" ( 2,4 + 4) - [ 5 - 4 ])");
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Error in brackets");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void ExcessOpenBracketTest() {
            Calculator calc = new Calculator();
            calc.AddBrackets("[", "]");

            try {
                calc.Solve(" (( 2,4 + 4) - [ 5 - 4 ]");
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Error in brackets");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void IllegalNumberFormatTest1() {
            Calculator calc = new Calculator();

            try {
                calc.Solve(" ( 2,4,7 + 4) ");
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Illegal expression");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void IllegalNumberFormatTest2() {
            Calculator calc = new Calculator();

            try {
                calc.Solve(" ( 2, + 4) ");
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Illegal expression");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void IllegalNumberFormatTest3() {
            Calculator calc = new Calculator();

            try {
                calc.Solve(" ( 2 + 4,,4) ");
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Illegal expression");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void IllegalNumberFormatTest4() {
            Calculator calc = new Calculator();

            try {
                calc.Solve(" ( 2 + ,4) ");
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Illegal expression");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void BigNumberTest() {
            Calculator calc = new Calculator();

            try {
                calc.Solve(" 10000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000 ");
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Number too big");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void EmptyExpressionTest() {
            Calculator calc = new Calculator();

            try {
                calc.Solve("   \n\t ");
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Empty expression");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void NullExpressionTest() {
            Calculator calc = new Calculator();

            try {
                calc.Solve(null);
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Error in expression");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void OperationLessTest() {
            Calculator calc = new Calculator();

            try {
                calc.Solve(" ( 2,4 + 4)7,5+4 ");
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Error in expression");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void NumberLessTest() {
            Calculator calc = new Calculator();

            try {
                calc.Solve(" ( 2 + -4) ");
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Error in expression");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void AddBracketOperationTest() {
            Calculator calc = new Calculator();

            try {
                calc.AddOperation(")", (x, y) => (x % y), 54);
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Operation symbol is bracket");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void AddEmptyBracketTest() {
            Calculator calc = new Calculator();

            try {
                calc.AddOperation("", (x, y) => (x % y), 54);
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Incorrect operation data");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void AddBracketWithWhitespaceTest() {
            Calculator calc = new Calculator();

            try {
                calc.AddOperation("% %", (x, y) => (x % y), 54);
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Symbol can't include whitespace");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void AddNullSymbolBracketTest() {
            Calculator calc = new Calculator();

            try {
                calc.AddOperation(null, (x, y) => (x % y), 54);
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Incorrect operation data");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void AddNullActionBracketTest() {
            Calculator calc = new Calculator();

            try {
                calc.AddOperation("", null, 51);
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Incorrect operation data");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }
    }
}
