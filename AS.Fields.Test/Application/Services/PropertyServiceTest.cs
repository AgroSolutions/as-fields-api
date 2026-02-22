using AS.Fields.Application.Exceptions;
using AS.Fields.Application.Services;
using AS.Fields.Domain.DTO.Property;
using AS.Fields.Domain.Entities;
using AS.Fields.Domain.Interfaces.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using System.Linq.Expressions;

namespace AS.Fields.Test.Application.Services
{
    public class PropertyServiceTest
    {
        #region Setup
        private readonly PropertyService service;

        private readonly Mock<ILogger<PropertyService>> loggerMock = new();
        private readonly Mock<IPropertyRepository> propertyRepositoryMock = new();

        private Guid farmerIdDefault = Guid.NewGuid();
        private SavePropertyDTO savePropertyDTODefault = new()
        {
            Name = "Propriedade Teste",
            Description = "Descrição da propriedade"
        };

        public PropertyServiceTest()
        {
            SetupProperties([]);

            service = new PropertyService(
                loggerMock.Object,
                propertyRepositoryMock.Object
            );
        }

        private void SetupProperties(List<Property> properties)
        {
            propertyRepositoryMock
                .Setup(repo => repo.QueryAsync(It.IsAny<Expression<Func<Property, bool>>>()))
                .Returns(properties.BuildMock());
        }
        #endregion

        #region GetAllPropertiesAsync
        [Fact]
        public async Task GetAllPropertiesAsync_DeveRetornarListaDePropriedades_QuandoExistiremPropriedades()
        {
            // Arrange
            List<Property> expectedProperties = new()
            {
                new(Guid.NewGuid()) { FarmerId = farmerIdDefault, Name = "Propriedade 1", Description = "Descrição 1" },
                new(Guid.NewGuid()) { FarmerId = farmerIdDefault, Name = "Propriedade 2", Description = "Descrição 2" }
            };
            SetupProperties(expectedProperties);

            // Act
            List<Property> result = await service.GetAllPropertiesAsync(farmerIdDefault);

            // Assert
            Assert.Equal(expectedProperties.Count, result.Count);
            Assert.All(result, property => Assert.Contains(expectedProperties, expected => expected.Id == property.Id));
        }
        #endregion

        #region GetPropertyByIdAsync
        [Fact]
        public async Task GetPropertyByIdAsync_DeveRetornarPropriedade_QuandoPropriedadeExistir()
        {
            // Arrange
            Guid propertyId = Guid.NewGuid();
            Property expectedProperty = new(propertyId) { FarmerId = farmerIdDefault, Name = "Propriedade", Description = "Descrição" };

            propertyRepositoryMock.Setup(repo => repo.GetById(propertyId))
                .ReturnsAsync(expectedProperty);

            // Act
            Property result = await service.GetPropertyByIdAsync(farmerIdDefault, propertyId);

            // Assert
            Assert.Equal(expectedProperty, result);
        }

        [Fact]
        public async Task GetPropertyByIdAsync_DeveLancarNotFoundException_QuandoPropriedadeNaoExistir()
        {
            // Arrange
            Guid propertyId = Guid.NewGuid();
            propertyRepositoryMock.Setup(repo => repo.GetById(propertyId))
                .ReturnsAsync((Property?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => service.GetPropertyByIdAsync(farmerIdDefault, propertyId));
        }

        [Fact]
        public async Task GetPropertyByIdAsync_DeveLancarNotFoundException_QuandoPropriedadeNaoPertenceAoFazendeiro()
        {
            // Arrange
            Guid propertyId = Guid.NewGuid();
            Guid otherFarmerId = Guid.NewGuid();
            Property property = new(propertyId) { FarmerId = otherFarmerId, Name = "Propriedade", Description = "Descrição" };

            propertyRepositoryMock.Setup(repo => repo.GetById(propertyId))
                .ReturnsAsync(property);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => service.GetPropertyByIdAsync(farmerIdDefault, propertyId));
        }
        #endregion

        #region CreatePropertyAsync
        [Fact]
        public async Task CreatePropertyAsync_DeveCriarPropriedade_QuandoDadosValidos()
        {
            // Arrange
            Property createdProperty = new(Guid.NewGuid()) { FarmerId = farmerIdDefault, Name = savePropertyDTODefault.Name, Description = savePropertyDTODefault.Description };

            propertyRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Property>()))
                .ReturnsAsync(createdProperty);

            // Act
            Property result = await service.CreatePropertyAsync(farmerIdDefault, savePropertyDTODefault);

            // Assert
            Assert.Equal(createdProperty, result);
        }

        [Fact]
        public async Task CreatePropertyAsync_DeveLancarValidationException_QuandoDadosInvalidos()
        {
            // Arrange
            SavePropertyDTO invalidDTO = new()
            {
                Name = "", // Nome inválido
                Description = "Descrição válida"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => service.CreatePropertyAsync(farmerIdDefault, invalidDTO));
        }
        #endregion

        #region UpdatePropertyAsync
        [Fact]
        public async Task UpdatePropertyAsync_DeveAtualizarPropriedade_QuandoDadosValidos()
        {
            // Arrange
            Guid propertyId = Guid.NewGuid();
            Property existingProperty = new(propertyId) { FarmerId = farmerIdDefault, Name = "Nome Antigo", Description = "Descrição Antiga" };

            propertyRepositoryMock.Setup(repo => repo.GetById(propertyId))
                .ReturnsAsync(existingProperty);

            propertyRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Property>()))
                .ReturnsAsync(true);

            // Act
            bool result = await service.UpdatePropertyAsync(farmerIdDefault, propertyId, savePropertyDTODefault);

            // Assert
            Assert.True(result);
            propertyRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Property>()), Times.Once);
        }

        [Fact]
        public async Task UpdatePropertyAsync_DeveLancarNotFoundException_QuandoPropriedadeNaoExistir()
        {
            // Arrange
            Guid propertyId = Guid.NewGuid();
            propertyRepositoryMock.Setup(repo => repo.GetById(propertyId))
                .ReturnsAsync((Property?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => service.UpdatePropertyAsync(farmerIdDefault, propertyId, savePropertyDTODefault));
        }

        [Fact]
        public async Task UpdatePropertyAsync_DeveLancarNotFoundException_QuandoPropriedadeNaoPertenceAoFazendeiro()
        {
            // Arrange
            Guid propertyId = Guid.NewGuid();
            Guid otherFarmerId = Guid.NewGuid();
            Property property = new(propertyId) { FarmerId = otherFarmerId, Name = "Propriedade", Description = "Descrição" };

            propertyRepositoryMock.Setup(repo => repo.GetById(propertyId))
                .ReturnsAsync(property);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => service.UpdatePropertyAsync(farmerIdDefault, propertyId, savePropertyDTODefault));
        }

        [Fact]
        public async Task UpdatePropertyAsync_DeveLancarValidationException_QuandoDadosInvalidos()
        {
            // Arrange
            Guid propertyId = Guid.NewGuid();
            Property existingProperty = new(propertyId) { FarmerId = farmerIdDefault, Name = "Nome Antigo", Description = "Descrição Antiga" };

            propertyRepositoryMock.Setup(repo => repo.GetById(propertyId))
                .ReturnsAsync(existingProperty);

            SavePropertyDTO invalidDTO = new()
            {
                Name = "", // Nome inválido
                Description = "Descrição válida"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => service.UpdatePropertyAsync(farmerIdDefault, propertyId, invalidDTO));
        }
        #endregion

        #region DeletePropertyAsync
        [Fact]
        public async Task DeletePropertyAsync_DeveDeletarPropriedade_QuandoPropriedadeExistir()
        {
            // Arrange
            Guid propertyId = Guid.NewGuid();
            Property existingProperty = new(propertyId) { FarmerId = farmerIdDefault, Name = "Propriedade", Description = "Descrição" };

            propertyRepositoryMock.Setup(repo => repo.GetById(propertyId))
                .ReturnsAsync(existingProperty);

            propertyRepositoryMock.Setup(repo => repo.DeleteAsync(existingProperty))
                .ReturnsAsync(true);

            // Act
            bool result = await service.DeletePropertyAsync(farmerIdDefault, propertyId);

            // Assert
            Assert.True(result);
            propertyRepositoryMock.Verify(repo => repo.DeleteAsync(existingProperty), Times.Once);
        }

        [Fact]
        public async Task DeletePropertyAsync_DeveLancarNotFoundException_QuandoPropriedadeNaoExistir()
        {
            // Arrange
            Guid propertyId = Guid.NewGuid();
            propertyRepositoryMock.Setup(repo => repo.GetById(propertyId))
                .ReturnsAsync((Property?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => service.DeletePropertyAsync(farmerIdDefault, propertyId));
        }

        [Fact]
        public async Task DeletePropertyAsync_DeveLancarNotFoundException_QuandoPropriedadeNaoPertenceAoFazendeiro()
        {
            // Arrange
            Guid propertyId = Guid.NewGuid();
            Guid otherFarmerId = Guid.NewGuid();
            Property property = new(propertyId) { FarmerId = otherFarmerId, Name = "Propriedade", Description = "Descrição" };

            propertyRepositoryMock.Setup(repo => repo.GetById(propertyId))
                .ReturnsAsync(property);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => service.DeletePropertyAsync(farmerIdDefault, propertyId));
        }
        #endregion
    }
}
