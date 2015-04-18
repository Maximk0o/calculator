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
            Assert.AreEqual(161.29, calc.Solve(" ( 2,7 + [5*3+( 7- 2)] * [4 /(5  -4 +(9-2))])**2 \n\r\t"));
            Assert.AreEqual(11, calc.Solve("( 2 + 2*4 )/ ( 2 - 3/3) + 1"));
            Assert.AreEqual(6, calc.Solve("2+2*2"));
            Assert.AreEqual(5, calc.Solve("3 + 4/2"));
            Assert.AreEqual(double.PositiveInfinity, calc.Solve("[8+1/(1+(3-8)/5)]-1000"));
        }

        [TestMethod]
        public void UndefinedOperatorTest() {
            Calculator calc = new Calculator();

            try {
                calc.Solve(" ( 2,7 + 5*3+( 7- 2) * 4 /(5  -4 +(9-2)))&2");
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Illegal expression");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void UndefinedBracketsTest() {
            Calculator calc = new Calculator();

            try {
                calc.Solve(" ( 2,4 + 4) - [ 5 - 4]");
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Illegal expression");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void IllegalBracketsTest() {
            Calculator calc = new Calculator();

            calc.AddBrackets("[", "]");
            calc.AddBrackets("((", "))");
            calc.AddBrackets("(", ")");

            int ExceptionCount = 0;
            try {
                calc.AddBrackets("(", "+");
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Brackets owerlaps with exists");
                ExceptionCount++;
            }
            try {
                calc.Solve(" ( 2,4 + 4 - [ 5 - 4)]");
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Error in brackets");
                ExceptionCount++;
            }
            try {
                calc.Solve(" ( 2,4 + [4 - 5]] - 4)");
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Error in brackets");
                ExceptionCount++;
            }
            Assert.AreEqual(3, ExceptionCount);
        }

        [TestMethod]
        public void IllegalNumberFormatTest() {
            Calculator calc = new Calculator();

            int ExceptionCount = 0;
            try {
                calc.Solve(" ( 2,4,7 + 4) ");
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Illegal expression");
                ExceptionCount++;
            }
            try {
                calc.Solve(" ( 2, + 4) ");
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Illegal expression");
                ExceptionCount++;
            }
            Assert.AreEqual(2, ExceptionCount);
        }

        [TestMethod]
        public void ExpressionTest() {
            Calculator calc = new Calculator();

            int ExceptionCount = 0;
            try {
                calc.Solve(" ( 2,4 + 4)7,5+4 ");
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Error in expression");
                ExceptionCount++;
            }
            try {
                calc.Solve(" ( 2 + +4) ");
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Error in expression");
                ExceptionCount++;
            }
            Assert.AreEqual(2, ExceptionCount);
        }

        [TestMethod]
        public void EmptyExpressionTest() {
            Calculator calc = new Calculator();

            try {
                calc.Solve("    ");
            }
            catch (CalculationException e) {
                StringAssert.Contains(e.Message, "Empty expression");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }
    }
}
