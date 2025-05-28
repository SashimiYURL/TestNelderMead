using System;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

namespace PointTests
{
    public class PointBasicTests 
    {
        private IPoint CreateTestPoint(params double[] coords)
            => Point.create_point([.. coords], (uint)coords.Length);

        [Fact]
        public void CreatePoint_WithValidCoords_ReturnsPoint()
        {
            double[] coords = { 1.0, 2.0, 3.0 };

            var point = CreateTestPoint(coords);

            Assert.NotNull(point);
            Assert.Equal(3, (int)point.dimensions());
        }

        [Fact]
        public void CreatePoint_WithEmptyCoords_ThrowsException()
        {
            double[] emptyCoords = Array.Empty<double>();

            Assert.Throws<ApplicationException>(() => Point.create_point([.. emptyCoords], 0));
        }

        [Fact]
        public void Get_ReturnsCorrectCoordinate()
        {
            var point = CreateTestPoint(1.5, 2.5);

            Assert.Equal(1.5, point.get(0));
            Assert.Equal(2.5, point.get(1));
        }

        [Fact]
        public void Get_WithInvalidIndex_ThrowsException()
        {
            var point = CreateTestPoint(1.0, 2.0);

            Assert.Throws<ApplicationException>(() => point.get(2));
        }

        [Fact]
        public void Set_ModifiesCoordinate()
        {
            var point = CreateTestPoint(1.0, 2.0);

            point.set(3.0, 0); 

            Assert.Equal(3.0, point.get(0));
            Assert.Equal(2.0, point.get(1));
        }

        [Fact]
        public void Set_WithInvalidIndex_ThrowsException()
        {
            var point = CreateTestPoint(1.0, 2.0);

            Assert.Throws<ApplicationException>(() => point.set(3.0, 2));
        }

        [Fact]
        public void Dimensions_ReturnsCorrectValue()
        {
            var point2D = CreateTestPoint(1.0, 2.0);
            var point3D = CreateTestPoint(1.0, 2.0, 3.0);

            Assert.Equal(2, (int)point2D.dimensions());
            Assert.Equal(3, (int)point3D.dimensions());
        }

        [Fact]
        public void Clone_CreatesIndependentCopy()
        {
            var original = CreateTestPoint(1.0, 2.0);

            var clone = original.clone();
            clone.set(3.0, 0);

            Assert.Equal(3.0, clone.get(0));
            Assert.Equal(1.0, original.get(0)); 
        }

        [Fact]
        public void GetVectorPoint_ReturnsAllCoordinates()
        {
            var point = CreateTestPoint(1.1, 2.2, 3.3);
            var coords = new List<double>
            {
                point.get(0),
                point.get(1),
                point.get(2)
            };

            Assert.Equal(new List<double> { 1.1, 2.2, 3.3 }, coords);
        }

        [Fact]
        public void CreatePoint_WithMismatchedDimensions_ThrowsException()
        {
            double[] coords = { 1.0, 2.0 };

            Assert.Throws<ApplicationException>(() => Point.create_point([.. coords], 3));
        }
    }
}