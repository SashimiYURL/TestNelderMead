using System;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

namespace TestForParser
{
    public class ParserNegativeTests
    {
        private void ExceptionEvaluteCatchingCheker(string expression, double[]? variables, string errorMessage)
        {
            var tree = ExpressionTree.create_tree(expression);

            var exception = Assert.Throws<ApplicationException>(() => tree.evaluate([.. variables]));
            Assert.Equal(errorMessage, exception.Message);
        }

        private void ExceptionCreateCatchingCheker(string expression, double[]? variables, string errorMessage)
        {

            var exception = Assert.Throws<ApplicationException>(() => ExpressionTree.create_tree(expression));
            Assert.Equal(errorMessage, exception.Message);
        }

        [Theory]
        [InlineData("x1/x2", new double[] { 10.0, 0.0 }, "division by zero.")]
        [InlineData("x1/(x2-x3)", new double[] { 10.0, 5.0, 5.0 }, "division by zero.")]
        [InlineData("1/0", new double[] {}, "division by zero.")]
        public void DivideByZeroException(string expression, double[] variables, string errorMessage)
            => ExceptionEvaluteCatchingCheker(expression, variables, errorMessage);

        [Theory]
        [InlineData("x1+6", new double[] { 50.0, 2.0 }, "The number of variables is incorrect")]
        [InlineData("x1+x2", new double[] { 50.0, 2.0, 3.5 }, "The number of variables is incorrect")]
        [InlineData("x1-x2", new double[] { 50.0}, "The number of variables is incorrect")]
        [InlineData("x1+x2-x3", new double[] {}, "The number of variables is incorrect")]
        public void IncorrectVariablesNumbers(string expression, double[] variables, string errorMessage)
            => ExceptionEvaluteCatchingCheker(expression, variables, errorMessage);

        [Theory]
        [InlineData("x y", null, "Invalid expression string")]
        [InlineData("2 3 +", null, "Invalid expression string")]
        [InlineData("x + * y", null, "Invalid expression string")]
        [InlineData("a / / b", null, "Invalid expression string")]
        [InlineData("(x + y", null, "Invalid expression string")]
        [InlineData("a * (b + c))", null, "Invalid expression string")]
        [InlineData("()", null, "Invalid expression string")]
        [InlineData("x + ()", null, "Invalid expression string")]
        public void SyntaxException(string expression, double[]? variables, string errorMessage)
            => ExceptionCreateCatchingCheker(expression, variables, errorMessage);

        [Theory]
        [InlineData(".", null, "Invalid expression string")]
        [InlineData("123.", null, "Invalid expression string")]
        [InlineData("1.2.3", null, "Invalid expression string")]
        [InlineData("1#2", null, "Invalid expression string")]
        [InlineData("a$ + b@", null, "Invalid expression string")]
        [InlineData("", null, "Invalid expression string")] 
        public void IncorrectNumbers(string expression, double[]? variables, string errorMessage)
            => ExceptionCreateCatchingCheker(expression, variables, errorMessage);

        [Theory]
        [InlineData("foo(x)", null, "Invalid expression string")] 
        [InlineData("bar(1, 2)", null, "Invalid expression string")]
        [InlineData("sine(x)", null, "Invalid expression string")] 
        [InlineData("sqrtt(y)", null, "Invalid expression string")]
        public void IncorrectNamesAndFunction(string expression, double[]? variables, string errorMessage)
            => ExceptionEvaluteCatchingCheker(expression, variables, errorMessage);

        [Theory]
        [InlineData("x1+x2", null, "Invalid expression string")]
        public void ArgumentNullException(string expression, double[]? variables, string errorMessage)
        {
            var tree = ExpressionTree.create_tree(expression);

            var exception = Assert.Throws<NullReferenceException>(() => tree.evaluate([.. variables]));
            Assert.Equal(errorMessage, exception.Message);
        }
    }
}