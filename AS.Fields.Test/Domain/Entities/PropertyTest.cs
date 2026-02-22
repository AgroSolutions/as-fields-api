using AS.Fields.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AS.Fields.Test.Domain.Entities
{
    public class PropertyTest
    {
        #region Construtor
        [Fact]
        public void Constructor_DeveIniciarAsPropriedades()
        {
            // Arrange
            var name = "Test Property";
            var description = "Test Description";
            var farmerId = Guid.NewGuid();

            // Act
            var property = new Property
            {
                Name = name,
                Description = description,
                FarmerId = farmerId
            };

            // Assert
            Assert.Equal(name, property.Name);
            Assert.Equal(description, property.Description);
            Assert.Equal(farmerId, property.FarmerId);
        }
        #endregion
    }
}
