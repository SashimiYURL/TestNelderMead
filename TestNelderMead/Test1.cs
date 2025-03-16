using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Runtime.InteropServices;


namespace TestNelderMead
{
    [TestClass]
    public class NelderMeadTests
    {
        [DllImport("C:\\Users\\Admin\\source\\repos\\TestNelderMead\\TestNelderMead\\NelderMead.dll", CallingConvention = CallingConvention.StdCall)]
        static extern double NM_addition(double number_one, double number_two);
        [DllImport("C:\\Users\\Admin\\source\\repos\\TestNelderMead\\TestNelderMead\\NelderMead.dll", CallingConvention = CallingConvention.StdCall)]
        static extern double NM_subtraction(double number_one, double number_two);
        [DllImport("C:\\Users\\Admin\\source\\repos\\TestNelderMead\\TestNelderMead\\NelderMead.dll", CallingConvention = CallingConvention.StdCall)]
        static extern double NM_multiplication(double number_one, double number_two);
        [DllImport("C:\\Users\\Admin\\source\\repos\\TestNelderMead\\TestNelderMead\\NelderMead.dll", CallingConvention = CallingConvention.StdCall)]
        static extern double NM_division(double number_one, double number_two);

        [TestMethod]
        public void Test_NM_ReturnsSum()
        {
            double numberOne = 5.5;
            double numberTwo = 4.5;

            double result = NM_addition(numberOne, numberTwo);

            Assert.AreEqual(10.0, result);
        }
        [TestMethod]
        public void Test_NM_ReturnsDifference()
        {
            double numberOne = 10.0;
            double numberTwo = 4.5;

            double result = NM_subtraction(numberOne, numberTwo);

            Assert.AreEqual(5.5, result);
        }
        [TestMethod]
        public void Test_NM_ReturnsProduct()
        {
            double numberOne = 3.0;
            double numberTwo = 2.5;

            double result = NM_multiplication(numberOne, numberTwo);

            Assert.AreEqual(7.5, result);
        }
        [TestMethod]
        public void Test_NM_ReturnsQuotient()
        {
            double divisible = 10.0;
            double divisor = 2.0;

            double result = NM_division(divisible, divisor);

            Assert.AreEqual(5.0, result);
        }
        [TestMethod]
        public void Test_NM_ReturnsMaxValue()
        {
            double divisible = 10.0;
            double divisor = 0.0;

            double result = NM_division(divisible, divisor);

            Assert.AreEqual((double)ulong.MaxValue, result);
        }
    }
}
