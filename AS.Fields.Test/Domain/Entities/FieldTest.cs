using AS.Fields.Domain.Entities;
using AS.Fields.Domain.Enums;
using AS.Fields.Domain.ValueObjects;

namespace AS.Fields.Test.Domain.Entities
{
    public class FieldTest
    {
        #region Construtor
        [Fact]
        public void Constructor_DeveIniciarAsPropriedades()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var boundary = new Boundary(1, 2, 3, 4);
            var description = "Test Field";
            var status = FieldStatus.Normal;
            var observations = "No observations";

            // Act
            var field = new Field
            {
                PropertyId = propertyId,
                Boundary = boundary,
                Description = description,
                Observations = observations
            };
            field.ChangeStatus(status);

            // Assert
            Assert.Equal(propertyId, field.PropertyId);
            Assert.Equal(boundary, field.Boundary);
            Assert.Equal(description, field.Description);
            Assert.Equal(status, field.Status);
            Assert.Equal(observations, field.Observations);
        }

        [Fact]
        public void Constructor_DeveGerarNovoId_QuandoNaoForInformado()
        {
            // Act
            var field = new Field()
            {
                Boundary = new Boundary(1, 2, 3, 4),
                Description = "Test Field",
                PropertyId = Guid.NewGuid(),
            };

            // Assert
            Assert.NotEqual(Guid.Empty, field.Id);
        }
        #endregion

        #region ChangeCrop
        [Fact]
        public void ChangeCrop_DeveAtualizarOCultivoEDataDePlantio()
        {
            // Arrange
            var field = new Field()
            {
                Boundary = new Boundary(1, 2, 3, 4),
                Description = "Test Field",
                PropertyId = Guid.NewGuid(),
            };
            var newCrop = CropType.Corn;
            var plantingDate = new DateTime(2024, 5, 1);

            // Act
            field.ChangeCrop(newCrop, plantingDate);

            // Assert
            Assert.Equal(newCrop, field.Crop);
            Assert.Equal(plantingDate, field.PlantingDate);
        }
        #endregion
    }
}
