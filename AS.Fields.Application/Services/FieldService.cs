using AS.Fields.Application.Exceptions;
using AS.Fields.Application.Observability;
using AS.Fields.Application.Publishers.Interfaces;
using AS.Fields.Application.Validators;
using AS.Fields.Domain.DTO.Field;
using AS.Fields.Domain.DTO.Messaging.Field;
using AS.Fields.Domain.DTO.Messaging.Sensor;
using AS.Fields.Domain.Entities;
using AS.Fields.Domain.Enums;
using AS.Fields.Domain.Interfaces.Repositories;
using AS.Fields.Domain.Interfaces.Services;
using AS.Fields.Domain.ValueObjects;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AS.Fields.Application.Services
{
    public class FieldService(
        ILogger<FieldService> logger,
        IFieldRepository fieldRepository,
        IPropertyRepository propertyRepository,
        ISensorPublisher sensorPublisher,
        IFieldTelemetry telemetry
    ) : IFieldService
    {
        public Task<List<Field>> GetAllFieldsAsync(Guid propertyId) => fieldRepository.QueryAsync(f => f.PropertyId == propertyId).ToListAsync();

        public async Task<Field> GetFieldByIdAsync(Guid id)
        {
            Field field = await fieldRepository.GetById(id)
                ?? throw new NotFoundException("Talhão não encontrado");

            return field;
        }

        public async Task<Field> CreateFieldAsync(Guid propertyId, CreateFieldDTO dto)
        {
            Property property = await propertyRepository.GetById(propertyId)
                ?? throw new NotFoundException("Propriedade não encontrada");

            var result = new CreateFieldValidator().Validate(dto);
            if (!result.IsValid)
            {
                logger.LogWarning("Dados de criação do talhão inválidos para a propriedade {PropertyId}: {@Errors}", propertyId, result.Errors);
                throw new ValidationException(result.Errors);
            }

            if (await ExisteBoundarySobrepostoAsync(dto.Boundary))
                throw new BusinessErrorDetailsException("Já existe um talhão nessas coordenadas.");

            Field field = new()
            {
                Description = dto.Description,
                Boundary = dto.Boundary,
                Observations = dto.Observations,
                PropertyId = propertyId
            };
            field.ChangeCrop(dto.Crop, dto.PlantingDate);
            field.ChangeStatus(FieldStatus.Unknown);

            Field newField = await fieldRepository.AddAsync(field)
                ?? throw new Exception("Field não foi criado");

            await sensorPublisher.CreateSensorAsync(new CreateSensorDTO()
            {
                FieldId = newField.Id,
                Name = "Sensor"
            });

            telemetry.FieldCreated(propertyId, property.Name);
            return newField;
        }

        public async Task<bool> PartialUpdateFieldAsync(Guid id, PartialUpdateFieldDTO dto)
        {
            Field field = await fieldRepository.GetById(id)
                ?? throw new NotFoundException("Talhão não encontrado");

            if (dto.Boundary is not null)
            {
                if (await ExisteBoundarySobrepostoAsync(dto.Boundary, id))
                    throw new BusinessErrorDetailsException("Já existe um talhão nessas coordenadas.");
            }

            var result = new PartialUpdateFieldValidator().Validate(dto);
            if (!result.IsValid)
            {
                logger.LogWarning("Dados de atualização parcial do talhão inválidos para o talhão {FieldId}: {@Errors}", id, result.Errors);
                throw new ValidationException(result.Errors);
            }

            field.Description = dto.Description ?? field.Description;
            field.Boundary = dto.Boundary ?? field.Boundary;
            field.Observations = dto.Observations ?? field.Observations;

            if (dto.Crop.HasValue)
                field.ChangeCrop(dto.Crop.Value, dto.PlantingDate.GetValueOrDefault());

            return await fieldRepository.UpdateAsync(field);
        }

        public async Task<bool> DeleteFieldAsync(Guid id)
        {
            Field field = await fieldRepository.GetById(id)
                ?? throw new NotFoundException("Talhão não encontrado");

            Property property = await propertyRepository.GetById(field.PropertyId)
                ?? throw new NotFoundException("Propriedade não encontrada");

            bool deleted = await fieldRepository.DeleteAsync(field);

            if (deleted)
                telemetry.FieldDeleted(field.PropertyId, property.Name);
            return deleted;
        }

        private async Task<bool> ExisteBoundarySobrepostoAsync(Boundary novoBoundary, Guid? fieldId = null)
        {
            return await fieldRepository.QueryAsync(x => fieldId == null || x.Id != fieldId)
                .AsNoTracking()
                .Where(f =>
                    f.Boundary.LatMin <= novoBoundary.LatMax &&
                    f.Boundary.LatMax >= novoBoundary.LatMin &&
                    f.Boundary.LongMin <= novoBoundary.LongMax &&
                    f.Boundary.LongMax >= novoBoundary.LongMin
                ).AnyAsync();
        }

        public async Task<bool> UpdateStatus(UpdateFieldStatusDTO dto)
        {

            Field field = await fieldRepository.GetById(dto.FieldId)
                ?? throw new NotFoundException("Talhão não encontrado");
            field.ChangeStatus(dto.Status);

            Property property = await propertyRepository.GetById(field.PropertyId)
                ?? throw new NotFoundException("Propriedade não encontrada");

            bool atualizado = await fieldRepository.UpdateAsync(field);

            if (atualizado)
            {
                telemetry.FieldStatusChanged(field.PropertyId, property.Name,
                    field.Id, dto.Status);
            }

            return atualizado;
        }
    }
}
