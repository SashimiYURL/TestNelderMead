using System;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

namespace TestForParser
{
    public class TestParser
    {
        [Theory]
        [InlineData("x1+x2", new double[] { 1, 2 }, 3.0)]
        [InlineData("x1 - x2", new double[] { 5.0, 8.0 }, -3.0)]
        [InlineData("x1 * x2*x3", new double[] { 5.0, 2.0, 3.0 }, 30.0)]
        [InlineData("x1/x2", new double[] { 10.0, 5.0 }, 2.0)]
        [InlineData("x1^x2", new double[] { 2.0, 3.0 }, 8.0)]
        public void BasicArithmeticOperation(string expression, double[] variables, double result)
            => CheckExpression(expression, variables, result);


        [Theory]
        [InlineData("x1+3", new double[] { 1 }, 4.0)]
        [InlineData("10 + x1", new double[] { 12 }, 22.0)]
        [InlineData("3*x1 + 13*x2 - 10", new double[] { 5.0, 2.0 }, 31.0)]
        [InlineData("-5 + x1", new double[] { 2.0 }, -3.0)]
        [InlineData("2 + 3*5", new double[] { }, 17.0)]
        public void ExpressionWithConstant(string expression, double[] variables, double result)
            => CheckExpression(expression, variables, result);


        [Theory]
        [InlineData("x1 + x2 -x3", new double[] { 5.0, 10.0, 8.0 }, 7.0)]
        public void ThreeOperand(string expression, double[] variables, double result)
            => CheckExpression(expression, variables, result);


        [Theory]
        [InlineData("x1 + x2 * x3", new double[] { 2, 3, 4 }, 14.0)]
        [InlineData("x1 - x2 / x3", new double[] { 8, 4, 2 }, 6.0)]
        [InlineData("x1 * x2 ^ x3", new double[] { 2, 3, 2 }, 18.0)]
        [InlineData("x1 + x2 ^ x3", new double[] { 4, 2, 2 }, 8.0)]
        public void BasicPriorityOperations(string expression, double[] variables, double result)
            => CheckExpression(expression, variables, result);


        [Theory]
        [InlineData("(x1 + x2) * x3", new double[] { 5, 3, 4 }, 32.0)]
        [InlineData("(x1 * (x2 + x3))/x4", new double[] { 3, 2, 4, 6 }, 3.0)]
        [InlineData("(x1 + x2) * (x3 - x4)", new double[] { 9, 5, 2, 4 }, -28.0)]
        public void ExpressionWithBrackets(string expression, double[] variables, double result)
            => CheckExpression(expression, variables, result);


        [Theory]
        [InlineData("x1^x2^x3", new double[] { 2, 2, 2 }, 16.0)]
        [InlineData("x1 - x2 - x3", new double[] { 1, 2, 3 }, -4.0)]
        public void AssociativityOperations(string expression, double[] variables, double result)
            => CheckExpression(expression, variables, result);


        [Theory]
        [InlineData("(-x1)^2", new double[] { 3.0 }, 9.0)]
        [InlineData("-x1^2", new double[] { 3.0 }, -9.0)]
        [InlineData("-x1", new double[] { 5.0 }, -5.0)]
        [InlineData("-(x1 + x2)", new double[] { 5.0, 3.0 }, -8.0)]
        public void UnaryOperations(string expression, double[] variables, double result)
            => CheckExpression(expression, variables, result);


        [Theory]
        [InlineData("sin(x1)", new double[] { 0 }, 0.0)]
        [InlineData("cos(x1)", new double[] { 0 }, 1.0)]
        [InlineData("abs(x1)", new double[] { -4 }, 4.0)]
        [InlineData("sqrt(x1)", new double[] { 9.0 }, 3.0)]
        //[InlineData("exp(x1)", new double[] {0}, 1.0)]
        public void MathFunctions(string expression, double[] variables, double result)
            => CheckExpression(expression, variables, result);


        [Theory]
        [InlineData("x1+x2", new double[] { 1.2, 4.35 }, 5.55)]
        [InlineData("(x1+x2)/x3", new double[] { 3, 5, 32 }, 0.25)]
        [InlineData("x1/x2", new double[] { 1, 2 }, 0.5)]
        [InlineData("0.5*x1 + 2.2*x2 + 10.5", new double[] { 1.5, 3.4 }, 18.73)]
        [InlineData("x1 + x2/x3", new double[] { 1.5, 3, 4 }, 2.25)]
        public void FloatingNumbers(string expression, double[] variables, double result)
            => CheckExpression(expression, variables, result);

        private void CheckExpression(string expression, double[] variables, double result)
        {
            var tree = ExpressionTree.create_tree(expression);
            double actual = tree.evaluate([.. variables]);
            Assert.Equal(result, actual);
        }
    }

}




