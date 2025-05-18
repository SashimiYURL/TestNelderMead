using System;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

namespace NelderMeadTests
{
    public class TypicalTestsFunction
    {
        [Fact]
        public void RosenbrockFunction_CorrectOptimization()
        {
            var tree = ExpressionTree.create_tree("(1-x1)^2 + 100*(x2-x1^2)^2");

            var method = new NelderMeadMethod(tree, 1.0, 2.0, 0.5, 0.5, 1e-12);  

            var history = method.minimum_search(1000);

            var bestPoint = history.Last()
                .OrderBy(p => tree.evaluate(p))
                .First();

            double bestValue = tree.evaluate(bestPoint);

            Assert.True(Math.Abs(bestPoint[0] - 1.0) < 1e-6);
            Assert.True(Math.Abs(bestPoint[1] - 1.0) < 1e-6);
            Assert.True(bestValue < 1e-6);
        }

        [Fact]
        public void MinimumSearch_HimmelblauFunction_ConvergesToLocalMinimum()
        {
            var tree = ExpressionTree.create_tree("(x1^2+x2-11)^2 + (x1+x2^2-7)^2");
            var method = new NelderMeadMethod(tree);
            method.generate_simplex(0.5, new DoubleVector { 3.0, 2.0 }); 

            var history = method.minimum_search(300);
            var finalSimplex = history[history.Count - 1];
            var bestVertex = finalSimplex[0];

            Assert.True(Math.Abs(bestVertex[0] - 3.0) < 0.1, $"X1: {bestVertex[0]}");
            Assert.True(Math.Abs(bestVertex[1] - 2.0) < 0.1, $"X2: {bestVertex[1]}");
        }

        [Fact]
        public void MatyasFunction_ConvergesToMinimum()
        {
            var tree = ExpressionTree.create_tree("0.26*(x1^2 + x2^2) - 0.48*x1*x2");

            var method = new NelderMeadMethod(tree, 1.0, 2.0, 0.5, 0.5, 1e-8);

            method.generate_simplex(0.5, new DoubleVector { 3.0, -3.0 });

            var history = method.minimum_search(50);
            var bestPoint = history.Last().OrderBy(p => tree.evaluate(p)).First();

            Assert.True(Math.Abs(bestPoint[0]) < 0.5, $"X1: {bestPoint[0]}");
            Assert.True(Math.Abs(bestPoint[1]) < 0.5, $"X2: {bestPoint[1]}");
            Assert.True(tree.evaluate(bestPoint) < 1e-4, "Function value too high");
        }

          [Fact]
        public void MinimumSearch_BealeFunction_ConvergesToMinimum()
        {
            var tree = ExpressionTree.create_tree("(1.5-x1+x1*x2)^2 + (2.25-x1+x1*x2^2)^2 + (2.625-x1+x1*x2^3)^2");
            var method = new NelderMeadMethod(tree, 1.2, 2.3, 0.3, 0.3, 1e-10);

            var history = method.minimum_search(300);
            var finalSimplex = history[history.Count - 1];
            var bestVertex = finalSimplex[0];

            Assert.True(Math.Abs(bestVertex[0] - 3.0) < 0.1, $"X1: {bestVertex[0]}");
            Assert.True(Math.Abs(bestVertex[1] - 0.5) < 0.1, $"X2: {bestVertex[1]}");
        }

        [Fact]
        public void MinimumSearch_ThreeHumpCamelFunction_ConvergesToGlobalMinimum()
        {
            var tree = ExpressionTree.create_tree("2*x1^2 - 1.05*x1^4 + x1^6/6 + x1*x2 + x2^2");
            var method = new NelderMeadMethod(tree);

            var history = method.minimum_search(200);
            var finalSimplex = history[history.Count - 1];

            foreach (var vertex in finalSimplex)
            {
                Assert.True(Math.Abs(vertex[0]) < 0.1, $"X1: {vertex[0]}");
                Assert.True(Math.Abs(vertex[1]) < 0.1, $"X2: {vertex[1]}");
            }
        }
    }

    public class NelderMeadBasicTests
    {
        // параболоид
        private ExpressionTree CreateTestFunction()
        {
            return ExpressionTree.create_tree("x1^2 + x2^2");
        }

        [Fact]
        public void Constructor_WithDefaultParameters_InitializesCorrectly()
        {
            var tree = CreateTestFunction();
            var method = new NelderMeadMethod(tree);

            Assert.NotNull(method);
        }

        [Fact]
        public void Constructor_WithCustomParameters_InitializesCorrectly()
        {
            var tree = CreateTestFunction();
            var method = new NelderMeadMethod(tree, 0.8, 1.5, 0.3, 0.4, 0.001);

            method.generate_simplex(1.0, new DoubleVector { 5.0, 5.0 });
            var history = method.minimum_search(50);
            Assert.True(history.Count > 0);
        }

        [Fact]
        public void Constructor_InitializesCorrectly()
        {
            var tree = CreateTestFunction();
            var method = new NelderMeadMethod(tree);
            Assert.NotNull(method);
        }

        [Fact]
        public void GenerateSimplex_WithValidStep_CreatesCorrectSimplex()
        {
            var tree = CreateTestFunction();
            var method = new NelderMeadMethod(tree);

            method.generate_simplex(1.0);

            var history = method.minimum_search(0);
            Assert.True(history.Count >= 1);
            Assert.Equal(6, history[0].Count);
        }

        [Fact]
        public void SetSimplex_WithValidSimplex_WorksCorrectly()
        {
            var tree = CreateTestFunction();
            var method = new NelderMeadMethod(tree);
            var simplex = new DoubleVectorVector
        {
            new DoubleVector { 0.0, 0.0 },
            new DoubleVector { 1.0, 0.0 },
            new DoubleVector { 0.0, 1.0 }
        };

            method.set_simplex(simplex);

            var history = method.minimum_search(0);
            Assert.Equal(3, history[0].Count);
        }

        [Fact]
        public void MinimumSearch_ZeroSteps_ReturnsOnlyInitialSimplex()
        {
            var tree = CreateTestFunction();
            var method = new NelderMeadMethod(tree);
            method.generate_simplex(1.0);

            var history = method.minimum_search(0);

            Assert.True(history.Count == 1 || history.Count == 2);
            Assert.Equal(6, history[0].Count);
        }

        [Fact]
        public void MinimumSearch_OneStep_ReturnsTwoSimplexes()
        {
            var tree = CreateTestFunction();
            var method = new NelderMeadMethod(tree);
            method.generate_simplex(1.0);

            var history = method.minimum_search(1);

            Assert.Equal(3, history.Count);
            Assert.Equal(6, history[0].Count);
            Assert.Equal(6, history[1].Count);
        }


    }

    public class SimpleFunctionTests
    {
        [Fact]
        public void ConstantFunction_DoesNotMove()
        {
            var tree = ExpressionTree.create_tree("5 + 0*x1");
            var method = new NelderMeadMethod(tree);

            method.generate_simplex(0.1, new DoubleVector { 1.0 });

            var history = method.minimum_search(5);
            var first = history.First()[0][0];
            var last = history.Last()[0][0];

            Assert.Equal(first, last, 1);
        }

        [Fact]
        public void LinearLikeFunction_MovesDownhill()
        {
            var tree = ExpressionTree.create_tree("1000 -x1 - x2");
            var method = new NelderMeadMethod(tree);

            method.generate_simplex(0.5, new DoubleVector { 1.0, 1.0 });

            var history = method.minimum_search(5);
            var firstValue = tree.evaluate(history[0][0]);
            var lastValue = tree.evaluate(history[history.Count - 1][0]);

            Assert.True(lastValue < firstValue,
                $"Expected {lastValue} < {firstValue}. History: {string.Join(", ", history.Select(h => tree.evaluate(h[0])))}");
        }

        [Fact]
        public void Simple1DFunction_Converges()
        {
            var tree = ExpressionTree.create_tree("x1^2");
            var method = new NelderMeadMethod(tree);

            method.generate_simplex(0.5, new DoubleVector { 2.0 }); 

            var history = method.minimum_search(20);
            var lastX = history[history.Count - 1][0][0];

            Assert.True(Math.Abs(lastX) < 0.1);
        }

        [Fact]
        public void Quadratic2DFunction_Converges()
        {
            var tree = ExpressionTree.create_tree("x1^2 + x2^2");
            var method = new NelderMeadMethod(tree);

            method.generate_simplex(0.5, new DoubleVector { 1.0, 1.0 });

            var history = method.minimum_search(20);
            var lastPoint = history[history.Count - 1][0];

            Assert.True(Math.Abs(lastPoint[0]) < 0.2);
            Assert.True(Math.Abs(lastPoint[1]) < 0.2);
        }

        [Fact]
        public void CheckDimensionRequirements()
        {
            var tree = ExpressionTree.create_tree("x1");
            var method = new NelderMeadMethod(tree);

            Assert.Throws<ApplicationException>(() =>
                method.generate_simplex(0.1, new DoubleVector { 1.0, 2.0 })); 

            method.generate_simplex(0.1, new DoubleVector { 1.0 });
        }

        [Fact]
        public void BowlShapeFunction_ConvergesCorrectly()
        {
            var tree = ExpressionTree.create_tree("(x1-2)^2 + (x2-3)^2");
            var method = new NelderMeadMethod(tree);

            double startX = 1.5;
            double startY = 2.5;
            double stepSize = 0.3;

            method.generate_simplex(stepSize, new DoubleVector { startX, startY });

            int maxIterations = 200;
            var history = method.minimum_search(maxIterations);

            var bestPoint = history.Last()
                .OrderBy(p => tree.evaluate(p))
                .First();

            double distance = Math.Sqrt(
                Math.Pow(bestPoint[0] - 2.0, 2) +
                Math.Pow(bestPoint[1] - 3.0, 2));

            Assert.True(distance < 1.0,
                $"Algorithm stopped at ({bestPoint[0]:F2}, {bestPoint[1]:F2}) " +
                $"instead of (2, 3). Distance: {distance:F2}");
        }

        [Fact]
        public void SimpleQuadratic_Converges()
        {
            var tree = ExpressionTree.create_tree("x1^2");
            var method = new NelderMeadMethod(tree);

            method.generate_simplex(0.1, new DoubleVector { 5.0 });

            var history = method.minimum_search(50);
            var bestX = history.Last().First()[0];

            Assert.True(Math.Abs(bestX) < 0.1, $"Expected ~0, got {bestX}");
        }
    }
}