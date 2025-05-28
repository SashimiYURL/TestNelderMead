using System;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

namespace NelderMeadTests
{

    public class NelderMeadBasicTests : IDisposable
    {
        private List<IDisposable> _disposables = new List<IDisposable>();

        // Параболоид: f(x,y) = x1^2 + x2^2
        private ExpressionTree CreateTestFunction()
        {
            var tree = ExpressionTree.create_tree("x1^2 + x2^2");
            _disposables.Add(tree);
            return tree;
        }

        private IPoint CreatePoint(double x, double y)
        {
            var coords = new List<double> { x, y };
            var point = IPoint.create_point([.. coords], 2);
            _disposables.Add(point);
            return point;
        }

        private Simplex CreateSimplexFromPoints(params IPoint[] points)
        {
            var pointVector = new IPointVector();

            foreach (var point in points)
            {
                pointVector.Add(point);
            }

            var simplex = Simplex.create_simplex(pointVector);

            _disposables.Add(simplex);
            return simplex;
        }



        private Simplex CreateDefaultSimplex(double step, IPoint startPoint = null)
        {
            var simplex = startPoint != null
                ? Simplex.create_simplex(step, (uint)startPoint.dimensions(), startPoint)
                : Simplex.create_simplex(step, 2);
            _disposables.Add(simplex);
            return simplex;
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable?.Dispose();
            }
            _disposables.Clear();
        }

        [Fact]
        public void Constructor_WithDefaultParameters_InitializesCorrectly()
        {
            using var tree = CreateTestFunction();
            using var method = new NelderMeadMethod(tree);

            Assert.NotNull(method);
        }

        [Fact]
        public void Constructor_WithCustomParameters_InitializesCorrectly()
        {
            using var tree = CreateTestFunction();
            using var method = new NelderMeadMethod(tree, 0.8, 1.5, 0.3, 0.4, 0.001);

            var point1 = CreatePoint(0.0, 0.0);
            var point2 = CreatePoint(1.0, 0.0);
            var point3 = CreatePoint(0.0, 1.0);
            using var simplex = CreateSimplexFromPoints(point1, point2, point3);
            method.set_simplex(simplex);

            using var history = method.minimum_search(50);
            var historyData = history.get_vector_history();
            Assert.True(historyData.Count > 0);
        }

        [Fact]
        public void GenerateSimplex_WithValidStep_CreatesCorrectSimplex()
        {
            using var tree = CreateTestFunction();
            using var method = new NelderMeadMethod(tree);

            var startPoint = CreatePoint(0.0, 0.0);
            using var simplex = CreateDefaultSimplex(1.0, startPoint);
            method.set_simplex(simplex);

            using var history = method.minimum_search(0);
            var historyData = history.get_vector_history();
            Assert.True(historyData.Count >= 1);
            Assert.Equal(3, (int)historyData[0].vertex_count());
        }

        [Fact]
        public void SetSimplex_WithValidSimplex_WorksCorrectly()
        {
            using var tree = CreateTestFunction();
            using var method = new NelderMeadMethod(tree);

            var point1 = CreatePoint(0.0, 0.0);
            var point2 = CreatePoint(1.0, 0.0);
            var point3 = CreatePoint(0.0, 1.0);

            using var simplex = CreateSimplexFromPoints(point1, point2, point3);
            method.set_simplex(simplex);

            using var history = method.minimum_search(0);
            var historyData = history.get_vector_history();
            Assert.Equal(3, (int)historyData[0].vertex_count());
        }

        [Fact]
        public void MinimumSearch_ZeroSteps_ReturnsOnlyInitialSimplex()
        {
            using var tree = CreateTestFunction();
            using var method = new NelderMeadMethod(tree);

            var startPoint = CreatePoint(0.0, 0.0);
            using var simplex = CreateDefaultSimplex(1.0, startPoint);
            method.set_simplex(simplex);

            using var history = method.minimum_search(0);
            var historyData = history.get_vector_history();
            Assert.True(historyData.Count == 1 || historyData.Count == 2);
            Assert.Equal(3, (int)historyData[0].vertex_count());
        }

        [Fact]
        public void MinimumSearch_OneStep_ReturnsTwoSimplexes()
        {
            using var tree = CreateTestFunction();
            using var method = new NelderMeadMethod(tree);

            var startPoint = CreatePoint(0.0, 0.0);
            using var simplex = CreateDefaultSimplex(1.0, startPoint);
            method.set_simplex(simplex);

            using var history = method.minimum_search(1);
            var historyData = history.get_vector_history();
            Assert.True(historyData.Count >= 2);
            Assert.Equal(3, (int)historyData[0].vertex_count());
            Assert.Equal(3, (int)historyData[1].vertex_count());
        }
    }

    public class TypicalTestsFunction
    {
        private IPoint CreatePoint(double x, double y)
        {
            var coords = new List<double> { x, y };
            var point = IPoint.create_point([.. coords], 2);
            return point;
        }

        private Simplex CreateDefaultSimplex(double step, IPoint startPoint = null)
        {
            var simplex = startPoint != null
                ? Simplex.create_simplex(step, (uint)startPoint.dimensions(), startPoint)
                : Simplex.create_simplex(step, 2);
            return simplex;
        }

        [Fact]
        public void RosenbrockFunction_CorrectOptimization()
        {
            var tree = ExpressionTree.create_tree("(1-x1)^2 + 100*(x2-x1^2)^2");
            var method = new NelderMeadMethod(tree, 1.0, 2.0, 0.5, 0.5, 1e-12);

            var history = method.minimum_search(1000); // SimplexHistory
            var simplexes = history.get_vector_history(); // SimplexVector

            var lastSimplex = simplexes[simplexes.Count - 1];

            // Получаем вершины симплекса
            var vertices = Enumerable.Range(0, (int)lastSimplex.vertex_count())
                                     .Select(i => lastSimplex.get_vertex((uint)i))
                                     .ToList();

            var bestPoint = vertices.OrderBy(p => tree.evaluate(p)).First();

            double bestValue = tree.evaluate(bestPoint);

            Assert.True(Math.Abs(bestPoint.get(0) - 1.0) < 1e-6);
            Assert.True(Math.Abs(bestPoint.get(0) - 1.0) < 1e-6);
            Assert.True(bestValue < 1e-6);
        }

        [Fact]
        public void MinimumSearch_HimmelblauFunction_ConvergesToLocalMinimum()
        {
            var tree = ExpressionTree.create_tree("(x1^2+x2-11)^2 + (x1+x2^2-7)^2");
            var method = new NelderMeadMethod(tree);

            var startPoint = CreatePoint(3.0, 2.0);
            using var simplex = CreateDefaultSimplex(0.5, startPoint);
            method.set_simplex(simplex);

            var history = method.minimum_search(300);
            var simplexes = history.get_vector_history();

            var finalSimplex = simplexes[simplexes.Count - 1];

            var bestVertex = finalSimplex.get_vertex(0);

            Assert.True(Math.Abs(bestVertex.get(0) - 3.0) < 0.1);
            Assert.True(Math.Abs(bestVertex.get(1) - 2.0) < 0.1);
        }

        [Fact]
        public void MatyasFunction_ConvergesToMinimum()
        {
            var tree = ExpressionTree.create_tree("0.26*(x1^2 + x2^2) - 0.48*x1*x2");
            var method = new NelderMeadMethod(tree, 1.0, 2.0, 0.5, 0.5, 1e-8);

            var startPoint = CreatePoint(3.0, -3.0);
            using var simplex = CreateDefaultSimplex(0.5, startPoint);
            method.set_simplex(simplex);

            var history = method.minimum_search(50);
            var simplexes = history.get_vector_history();

            var lastSimplex = simplexes[simplexes.Count - 1];

            var vertices = Enumerable.Range(0, (int)lastSimplex.vertex_count())
                                     .Select(i => lastSimplex.get_vertex((uint)i))
                                     .ToList();

            var bestPoint = vertices.OrderBy(p => tree.evaluate(p)).First();

            Assert.True(Math.Abs(bestPoint.get(0)) < 0.5);
            Assert.True(Math.Abs(bestPoint.get(1)) < 0.5);
            Assert.True(tree.evaluate(bestPoint) < 1e-4, "Function value too high");
        }

        [Fact]
        public void MinimumSearch_BealeFunction_ConvergesToMinimum()
        {
            var tree = ExpressionTree.create_tree("(1.5-x1+x1*x2)^2 + (2.25-x1+x1*x2^2)^2 + (2.625-x1+x1*x2^3)^2");
            var method = new NelderMeadMethod(tree, 1.2, 2.3, 0.3, 0.3, 1e-10);

            var history = method.minimum_search(300);
            var simplexes = history.get_vector_history();

            var finalSimplex = simplexes[simplexes.Count - 1];
            var bestVertex = finalSimplex.get_vertex(0);

            Assert.True(Math.Abs(bestVertex.get(0) - 3.0) < 0.1);
            Assert.True(Math.Abs(bestVertex.get(1) - 0.5) < 0.1);
        }

        [Fact]
        public void MinimumSearch_ThreeHumpCamelFunction_ConvergesToGlobalMinimum()
        {
            var tree = ExpressionTree.create_tree("2*x1^2 - 1.05*x1^4 + x1^6/6 + x1*x2 + x2^2");
            var method = new NelderMeadMethod(tree);

            var history = method.minimum_search(200);
            var simplexes = history.get_vector_history();

            var finalSimplex = simplexes[simplexes.Count - 1];

            var vertices = Enumerable.Range(0, (int)finalSimplex.vertex_count())
                                     .Select(i => finalSimplex.get_vertex((uint)i))
                                     .ToList();

            foreach (var vertex in vertices)
            {
                Assert.True(Math.Abs(vertex.get(0)) < 0.1);
                Assert.True(Math.Abs(vertex.get(1)) < 0.1);
            }
        }
    }


    // 
    public class SimpleFunctionTests
    {
        private IPoint CreatePoint(params double[] coords)
        {
            var point = IPoint.create_point([.. coords], (uint)coords.Count());
            return point;
        }

        private Simplex CreateDefaultSimplex(double step, IPoint startPoint = null)
        {
            var simplex = startPoint != null
                ? Simplex.create_simplex(step, (uint)startPoint.dimensions(), startPoint)
                : Simplex.create_simplex(step, 2);
            return simplex;
        }

        [Fact]
        public void ConstantFunction_DoesNotMove()
        {
            var tree = ExpressionTree.create_tree("5 + 0*x1");
            var method = new NelderMeadMethod(tree);

            var startPoint = CreatePoint(1.0);
            using var simplex = CreateDefaultSimplex(0.1, startPoint);
            method.set_simplex(simplex);

            var history = method.minimum_search(5);
            var simplexes = history.get_vector_history();

            var firstSimplex = simplexes[0];
            var lastSimplex = simplexes[simplexes.Count - 1];

            var firstPoint = firstSimplex.get_vertex(0);
            var lastPoint = lastSimplex.get_vertex(0);

            Assert.Equal(firstPoint.get(0), lastPoint.get(0), 1);
        }

        [Fact]
        public void LinearLikeFunction_MovesDownhill()
        {
            var tree = ExpressionTree.create_tree("1000 -x1 - x2");
            var method = new NelderMeadMethod(tree);

            var startPoint = CreatePoint(1.0, 1.0);
            using var simplex = CreateDefaultSimplex(0.5, startPoint);
            method.set_simplex(simplex);

            var history = method.minimum_search(5);
            var simplexes = history.get_vector_history();

            var firstSimplex = simplexes[0];
            var lastSimplex = simplexes[simplexes.Count - 1];

            var firstPoint = firstSimplex.get_vertex(0);
            var lastPoint = lastSimplex.get_vertex(0);

            var firstValue = tree.evaluate(firstPoint);
            var lastValue = tree.evaluate(lastPoint);

            Assert.True(lastValue < firstValue);
        }

        [Fact]
        public void Simple1DFunction_Converges()
        {
            var tree = ExpressionTree.create_tree("x1^2");
            var method = new NelderMeadMethod(tree);

            var startPoint = CreatePoint(2.0);
            using var simplex = CreateDefaultSimplex(0.5, startPoint);
            method.set_simplex(simplex);

            var history = method.minimum_search(20);
            var simplexes = history.get_vector_history();

            var lastSimplex = simplexes[simplexes.Count - 1];
            var lastPoint = lastSimplex.get_vertex(0);

            Assert.True(Math.Abs(lastPoint.get(0)) < 0.1);
        }

        [Fact]
        public void Quadratic2DFunction_Converges()
        {
            var tree = ExpressionTree.create_tree("x1^2 + x2^2");
            var method = new NelderMeadMethod(tree);

            var startPoint = CreatePoint(1.0, 1.0);
            using var simplex = CreateDefaultSimplex(0.5, startPoint);
            method.set_simplex(simplex);

            var history = method.minimum_search(20);
            var simplexes = history.get_vector_history();

            var lastSimplex = simplexes[simplexes.Count - 1];
            var lastPoint = lastSimplex.get_vertex(0);

            Assert.True(Math.Abs(lastPoint.get(0)) < 0.2);
            Assert.True(Math.Abs(lastPoint.get(1)) < 0.2);
        }

        [Fact]
        public void BowlShapeFunction_ConvergesCorrectly()
        {
            var tree = ExpressionTree.create_tree("(x1-2)^2 + (x2-3)^2");
            var method = new NelderMeadMethod(tree);

            var startPoint = CreatePoint(1.5, 2.5);
            using var simplex = CreateDefaultSimplex(0.3, startPoint);
            method.set_simplex(simplex);

            int maxIterations = 200;
            var history = method.minimum_search(maxIterations);
            var simplexes = history.get_vector_history();

            var lastSimplex = simplexes[simplexes.Count - 1];

            var vertices = Enumerable.Range(0, (int)lastSimplex.vertex_count())
                                     .Select(i => lastSimplex.get_vertex((uint)i))
                                     .ToList();

            var bestPoint = vertices.OrderBy(p => tree.evaluate(p)).First();

            double distance = Math.Sqrt(
                Math.Pow(bestPoint.get(0) - 2.0, 2) +
                Math.Pow(bestPoint.get(1) - 3.0, 2));

            Assert.True(distance < 1.0);
        }

        [Fact]
        public void SimpleQuadratic_Converges()
        {
            var tree = ExpressionTree.create_tree("x1^2");
            var method = new NelderMeadMethod(tree);

            var startPoint = CreatePoint(5.0);
            using var simplex = CreateDefaultSimplex(0.1, startPoint);
            method.set_simplex(simplex);

            var history = method.minimum_search(50);
            var simplexes = history.get_vector_history();

            var lastSimplex = simplexes[simplexes.Count - 1];
            var bestPoint = lastSimplex.get_vertex(0);

            Assert.True(Math.Abs(bestPoint.get(0)) < 0.1);
        }
    }
}