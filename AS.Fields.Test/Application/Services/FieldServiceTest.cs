using AS.Fields.Application.Exceptions;
using AS.Fields.Application.Observability;
using AS.Fields.Application.Publishers.Interfaces;
using AS.Fields.Application.Services;
using AS.Fields.Domain.DTO.Field;
using AS.Fields.Domain.DTO.Messaging.Field;
using AS.Fields.Domain.DTO.Messaging.Sensor;
using AS.Fields.Domain.Entities;
using AS.Fields.Domain.Enums;
using AS.Fields.Domain.Interfaces.Repositories;
using AS.Fields.Domain.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using System.Linq.Expressions;

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
        private readonly Mock<IFieldTelemetry> telemetryMock = new();

        private Guid propertyIdDefault = Guid.NewGuid();
        private CreateFieldDTO createFieldDTODefault = new()
        {
            Description = "Descricao",
            Boundary = new Boundary(1, 2, 3, 4),
            Crop = CropType.Corn,
            PlantingDate = DateTime.UtcNow,
            Observations = "Observacoes"
        };

        public FieldServiceTest()
        {
            SetupFields([]);

            service = new FieldService(
                loggerMock.Object,
                fieldRepositoryMock.Object,
                propertyRepositoryMock.Object,
                sensorPublisherMock.Object,
                telemetryMock.Object
            );
        }

        private void SetupFields(List<Field> fields)
        {
            fieldRepositoryMock
                .Setup(repo => repo.QueryAsync(It.IsAny<Expression<Func<Field, bool>>>()))
                .Returns(fields.BuildMock());
        }
        #endregion

        #region GetAllFieldsAsync
        [Fact]
        public async Task GetAllFieldsAsync_DeveRetornarListaDeTalhoes_QuandoExistiremTalhoes()
        {
            // Arrange
            List<Field> expectedFields = new()
            {
                new(Guid.NewGuid()) { PropertyId = propertyIdDefault, Description = "Descricao 1", Boundary = new Boundary(1, 2, 2, 3) },
                new(Guid.NewGuid()) { PropertyId = propertyIdDefault, Description = "Descricao 2", Boundary = new Boundary(2, 3, 3, 4) }
            };
            SetupFields(expectedFields);

            // Act
            List<Field> result = await service.GetAllFieldsAsync(propertyIdDefault);

            // Assert
            Assert.Equal(expectedFields.Count, result.Count);
            Assert.All(result, field => Assert.Contains(expectedFields, expected => expected.Id == field.Id));
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
        [Fact]
        public async Task CreateFieldAsync_DeveCriarTalhao_QuandoDadosValidos()
        {
            // Arrange
            Property property = new(propertyIdDefault) { Name = "Propriedade", Description = "Descricacao", FarmerId = Guid.NewGuid() };
            Field createdField = new(Guid.NewGuid()) { PropertyId = propertyIdDefault, Description = createFieldDTODefault.Description, Boundary = createFieldDTODefault.Boundary };

            propertyRepositoryMock.Setup(repo => repo.GetById(propertyIdDefault))
                .ReturnsAsync(property);

            fieldRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Field>()))
                .ReturnsAsync(createdField);

            // Act
            Field result = await service.CreateFieldAsync(propertyIdDefault, createFieldDTODefault);

            // Assert
            Assert.Equal(createdField, result);
        }

        [Fact]
        public async Task CreateFieldAsync_DeveCriarSensor_QuandoTalhaoCriado()
        {
            // Arrange
            Property property = new(propertyIdDefault) { Name = "Propriedade", Description = "Descricacao", FarmerId = Guid.NewGuid() };
            Field createdField = new(Guid.NewGuid()) { PropertyId = propertyIdDefault, Description = createFieldDTODefault.Description, Boundary = createFieldDTODefault.Boundary };
            propertyRepositoryMock.Setup(repo => repo.GetById(propertyIdDefault))
                .ReturnsAsync(property);
            fieldRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Field>()))
                .ReturnsAsync(createdField);

            // Act
            await service.CreateFieldAsync(propertyIdDefault, createFieldDTODefault);

            // Assert
            sensorPublisherMock.Verify(publisher => publisher.CreateSensorAsync(It.Is<CreateSensorDTO>(dto => dto.FieldId == createdField.Id && dto.Name == "Sensor")), Times.Once);
        }

        [Fact]
        public async Task CreateFieldAsync_DeveLancarNotFoundException_QuandoPropriedadeNaoExistir()
        {
            // Arrange
            propertyRepositoryMock.Setup(repo => repo.GetById(propertyIdDefault))
                .ReturnsAsync((Property?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => service.CreateFieldAsync(propertyIdDefault, createFieldDTODefault));
        }

        [Fact]
        public async Task CreateFieldAsync_DeveLancarValidationException_QuandoDadosInvalidos()
        {
            // Arrange
            Property property = new(propertyIdDefault) { Name = "Propriedade", Description = "Descricacao", FarmerId = Guid.NewGuid() };
            propertyRepositoryMock.Setup(repo => repo.GetById(propertyIdDefault))
                .ReturnsAsync(property);
            CreateFieldDTO invalidDTO = new()
            {
                Description = "", // Descrição inválida
                Boundary = new Boundary(1, 2, 2, 3), // Boundary válido
                Crop = CropType.Corn,
                PlantingDate = DateTime.UtcNow,
                Observations = "Observacoes"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => service.CreateFieldAsync(propertyIdDefault, invalidDTO));
        }

        [Fact]
        public async Task CreateFieldAsync_DeveLancarBusinessErrorDetailsException_QuandoBoundarySobreposto()
        {
            // Arrange
            Property property = new(propertyIdDefault) { Name = "Propriedade", Description = "Descricacao", FarmerId = Guid.NewGuid() };
            Field existingField = new(Guid.NewGuid()) { PropertyId = propertyIdDefault, Description = "Existing Field", Boundary = new Boundary(1, 2, 2, 3) };
            propertyRepositoryMock.Setup(repo => repo.GetById(propertyIdDefault))
                .ReturnsAsync(property);
            SetupFields([existingField]);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessErrorDetailsException>(() => service.CreateFieldAsync(propertyIdDefault, createFieldDTODefault));
        }
        #endregion

        #region PartialUpdateFieldAsync
        [Fact]
        public async Task PartialUpdateFieldAsync_DeveAtualizarTalhao_QuandoDadosValidos()
        {
            // Arrange
            Guid fieldId = Guid.NewGuid();
            Field existingField = new(fieldId) { PropertyId = propertyIdDefault, Description = "Descricao", Boundary = new Boundary(1, 2, 2, 3) };
            PartialUpdateFieldDTO updateDTO = new() { Description = "Nova Descricao" };

            fieldRepositoryMock.Setup(repo => repo.GetById(fieldId))
                .ReturnsAsync(existingField);

            Field? updatedField = null;
            fieldRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Field>()))
                .Callback<Field, CancellationToken>((field, ct) => updatedField = field)
                .ReturnsAsync(true);

            // Act
            bool result = await service.PartialUpdateFieldAsync(fieldId, updateDTO);

            // Assert
            Assert.True(result);
            Assert.Equal(updateDTO.Description, updatedField?.Description);
        }

        [Fact]
        public async Task PartialUpdateFieldAsync_DeveLancarNotFoundException_QuandoTalhaoNaoExistir()
        {
            // Arrange
            Guid fieldId = Guid.NewGuid();
            fieldRepositoryMock.Setup(repo => repo.GetById(fieldId))
                .ReturnsAsync((Field?)null);
            PartialUpdateFieldDTO updateDTO = new() { Description = "Nova Descricao" };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => service.PartialUpdateFieldAsync(fieldId, updateDTO));
        }
        #endregion

        #region DeleteFieldAsync
        [Fact]
        public async Task DeleteFieldAsync_DeveDeletarTalhao_QuandoTalhaoExistir()
        {
            // Arrange
            Guid fieldId = Guid.NewGuid();
            Field existingField = new(fieldId) { PropertyId = propertyIdDefault, Description = "Descricao", Boundary = new Boundary(1, 2, 2, 3) };
            fieldRepositoryMock.Setup(repo => repo.GetById(fieldId))
                .ReturnsAsync(existingField);
            fieldRepositoryMock.Setup(repo => repo.DeleteAsync(existingField))
                .ReturnsAsync(true);
            propertyRepositoryMock.Setup(repo => repo.GetById(existingField.PropertyId))
                .ReturnsAsync(new Property()
                {
                    Name = "Propriedade",
                    FarmerId = Guid.NewGuid(),
                    Description = "Description",
                });

            // Act
            bool result = await service.DeleteFieldAsync(fieldId);

            // Assert
            Assert.True(result);
            fieldRepositoryMock.Verify(repo => repo.DeleteAsync(existingField), Times.Once);
        }

        [Fact]
        public async Task DeleteFieldAsync_DeveLancarNotFoundException_QuandoTalhaoNaoExistir()
        {
            // Arrange
            Guid fieldId = Guid.NewGuid();
            fieldRepositoryMock.Setup(repo => repo.GetById(fieldId))
                .ReturnsAsync((Field?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteFieldAsync(fieldId));
        }
        #endregion

        #region UpdateStatus
        [Fact]
        public async Task UpdateStatus_DeveAtualizarStatus_QuandoDadosValidos()
        {
            // Arrange
            Guid fieldId = Guid.NewGuid();
            Field existingField = new(fieldId) { PropertyId = propertyIdDefault, Description = "Descricao", Boundary = new Boundary(1, 2, 2, 3) };
            existingField.ChangeStatus(FieldStatus.Unknown);


            UpdateFieldStatusDTO updateDTO = new() { FieldId = fieldId, Status = FieldStatus.DroughtAlert };

            fieldRepositoryMock.Setup(repo => repo.GetById(fieldId))
                .ReturnsAsync(existingField);

            Field? updatedField = null;
            fieldRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Field>()))
                .Callback<Field, CancellationToken>((field, ct) => updatedField = field)
                .ReturnsAsync(true);

            // Act
            bool result = await service.UpdateStatus(updateDTO);

            // Assert
            Assert.True(result);
            Assert.Equal(updateDTO.Status, updatedField?.Status);
        }

        [Fact]
        public async Task UpdateStatus_DeveLancarNotFoundException_QuandoTalhaoNaoExistir()
        {
            // Arrange
            Guid fieldId = Guid.NewGuid();
            fieldRepositoryMock.Setup(repo => repo.GetById(fieldId))
                .ReturnsAsync((Field?)null);
            UpdateFieldStatusDTO updateDTO = new() { FieldId = fieldId, Status = FieldStatus.DroughtAlert };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateStatus(updateDTO));
        }
        #endregion
    }
}
