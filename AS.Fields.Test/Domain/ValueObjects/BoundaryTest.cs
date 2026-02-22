using AS.Fields.Domain.ValueObjects;

namespace AS.Fields.Test.Domain.ValueObjects
{
    public class BoundaryTest
    {
        #region Construtor
        [Fact]
        public void Constructor_DeveIniciarAsPropriedades()
        {
            // Arrange
            var latMin = 1;
            var latMax = 2;
            var longMin = 3;
            var longMax = 4;

            // Act
            var boundary = new Boundary(latMin, latMax, longMin, longMax);

            // Assert
            Assert.Equal(latMin, boundary.LatMin);
            Assert.Equal(latMax, boundary.LatMax);
            Assert.Equal(longMin, boundary.LongMin);
            Assert.Equal(longMax, boundary.LongMax);
        }
        #endregion
    }
}
