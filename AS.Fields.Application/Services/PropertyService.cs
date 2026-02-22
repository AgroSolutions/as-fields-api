using AS.Fields.Application.Exceptions;
using AS.Fields.Application.Validators;
using AS.Fields.Domain.DTO.Property;
using AS.Fields.Domain.Entities;
using AS.Fields.Domain.Interfaces.Repositories;
using AS.Fields.Domain.Interfaces.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AS.Fields.Application.Services
{
    public class PropertyService(ILogger<PropertyService> logger, IPropertyRepository propertyRepository) : IPropertyService
    {
        public Task<List<Property>> GetAllPropertiesAsync(Guid farmerId) => propertyRepository.QueryAsync(p => p.FarmerId == farmerId).ToListAsync();

        public async Task<Property> GetPropertyByIdAsync(Guid farmerId, Guid propertyId)
        {
            Property property = await propertyRepository.GetById(propertyId)
                ?? throw new NotFoundException("Propriedade não encontrada");

            if (property.FarmerId != farmerId)
            {
                logger.LogWarning("Propriedade do usuario {OwnerId} não pertence ao usuario {UserId}", property.FarmerId, farmerId);
                throw new NotFoundException("Propriedade não encontrada");
            }

            return property;
        }

        public Task<Property> CreatePropertyAsync(Guid farmerId, SavePropertyDTO dto)
        {
            var result = new SavePropertyValidator().Validate(dto);
            if (!result.IsValid)
            {
                logger.LogWarning("Dados de criação de propriedade inválidos para o usuario {UserId}: {@Errors}", farmerId, result.Errors);
                throw new ValidationException(result.Errors);
            }

            return propertyRepository.AddAsync(new Property()
            {
                Description = dto.Description,
                Name = dto.Name,
                FarmerId = farmerId,
            });
        }

        public async Task<bool> UpdatePropertyAsync(Guid farmerId, Guid id, SavePropertyDTO dto)
        {
            Property property = await propertyRepository.GetById(id)
                ?? throw new NotFoundException("Propriedade não encontrada");

            if (property.FarmerId != farmerId)
            {
                logger.LogWarning("Propriedade do usuario {OwnerId} não pertence ao usuario {UserId}", property.FarmerId, farmerId);
                throw new NotFoundException("Propriedade não encontrada");
            }

            var result = new SavePropertyValidator().Validate(dto);
            if (!result.IsValid)
            {
                logger.LogWarning("Dados de atualização da propriedade inválidos para o usuario {UserId} e propriedade {PropertyId}: {@Errors}", farmerId, id, result.Errors);
                throw new ValidationException(result.Errors);
            }

            return await propertyRepository.UpdateAsync(new Property(id)
            {
                Description = dto.Description,
                Name = dto.Name,
                FarmerId = farmerId,
                CreatedAt = property.CreatedAt,
                Fields = property.Fields
            });
        }

        public async Task<bool> DeletePropertyAsync(Guid farmerId, Guid id)
        {
            Property property = await propertyRepository.GetById(id)
                ?? throw new NotFoundException("Propriedade não encontrada");

            if (property.FarmerId != farmerId)
            {
                logger.LogWarning("Propriedade do usuario {OwnerId} não pertence ao usuario {UserId}", property.FarmerId, farmerId);
                throw new NotFoundException("Propriedade não encontrada");
            }

            return await propertyRepository.DeleteAsync(property);
        }
    }
}
