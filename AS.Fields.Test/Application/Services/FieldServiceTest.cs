using AS.Fields.Application.Exceptions;
using AS.Fields.Application.Publishers.Interfaces;
using AS.Fields.Application.Services;
using AS.Fields.Domain.DTO.Field;
using AS.Fields.Domain.DTO.Messaging.Sensor;
using AS.Fields.Domain.Entities;
using AS.Fields.Domain.Enums;
using AS.Fields.Domain.Interfaces.Repositories;
using AS.Fields.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using MockQueryable.Moq;

namespace AS.Fields.Test.Application.Services
{
    public class FieldServiceTest
    {
        #region Setup
        private readonly FieldService service;

        private readonly Mock<ILogger<FieldService>> loggerMock = new();
        private readonly Mock<IFieldRepository> fieldRepositoryMock = new();
        private readonly Mock<IPropertyRepository> propertyRepositoryMock = new();
        private readonly Mock<ISensorPublisher> sensorPublisherMock = new();

        public FieldServiceTest()
        {
            service = new FieldService(
                loggerMock.Object,
                fieldRepositoryMock.Object,
                propertyRepositoryMock.Object,
                sensorPublisherMock.Object
            );
        }
        #endregion


        #region GetFieldByIdAsync
        [Fact]
        public async Task GetFieldByIdAsync_DeveRetornarTalhao_QuandoTalhaoExistir()
        {
            // Arrange
            Guid fieldId = Guid.NewGuid();
            Field expectedField = new(fieldId) { PropertyId = Guid.NewGuid(), Description = "Descricao", Boundary = new Boundary(1, 2, 2, 3) };

            fieldRepositoryMock.Setup(repo => repo.GetById(fieldId))
                .ReturnsAsync(expectedField);

            // Act
            Field result = await service.GetFieldByIdAsync(fieldId);

            // Assert
            Assert.Equal(expectedField, result);
        }

        [Fact]
        public async Task GetFieldByIdAsync_DeveLancarNotFoundException_QuandoTalhaoNaoExistir()
        {
            // Arrange
            Guid fieldId = Guid.NewGuid();
            fieldRepositoryMock.Setup(repo => repo.GetById(fieldId))
                .ReturnsAsync((Field?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => service.GetFieldByIdAsync(fieldId));
        }
        #endregion

        #region CreateFieldAsync
        /*[Fact]
        public async Task CreateFieldAsync_DeveCriarTalhao_QuandoDadosValidos()
        {
            // Arrange
            Guid propertyId = Guid.NewGuid();
            CreateFieldDTO dto = new()
            {
                Description = "Descricao",
                Boundary = new Boundary(1, 2, 3, 4),
                Crop = CropType.Corn,
                PlantingDate = DateTime.UtcNow,
                Observations = "Observacoes"
            };
            Property property = new(propertyId) { Name = "Propriedade", Description = "Descricacao", FarmerId = Guid.NewGuid() };
            Field createdField = new(Guid.NewGuid()) { PropertyId = propertyId, Description = dto.Description, Boundary = dto.Boundary };

            propertyRepositoryMock.Setup(repo => repo.GetById(propertyId))
                .ReturnsAsync(property);
            fieldRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Field>()))
                .ReturnsAsync(createdField);

            var existingFields = new List<Field>
            {
                // Adicione fields que simulem sobreposição ou não
                new Field(Guid.NewGuid())
                {
                    PropertyId = propertyId,
                    Description = dto.Description,
                    Boundary = createdField.Boundary
                }
            };
            fieldRepositoryMock
                .Setup(repo => repo.QueryAsync(It.IsAny<Expression<Func<Field, bool>>>()))
                .Returns(existingFields.AsQueryable());

            // Act
            Field result = await service.CreateFieldAsync(propertyId, dto);

            // Assert
            Assert.Equal(createdField, result);
            sensorPublisherMock.Verify(publisher => publisher.CreateSensorAsync(It.Is<CreateSensorDTO>(dto => dto.FieldId == createdField.Id && dto.Name == "Sensor")), Times.Once);
        }*/

        #endregion
    }
}
