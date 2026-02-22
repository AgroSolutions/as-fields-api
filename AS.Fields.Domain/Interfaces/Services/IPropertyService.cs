using AS.Fields.Domain.DTO.Property;
using AS.Fields.Domain.Entities;

namespace AS.Fields.Domain.Interfaces.Services
{
    public interface IPropertyService
    {
        Task<List<Property>> GetAllPropertiesAsync(Guid farmerId);
        Task<Property> GetPropertyByIdAsync(Guid farmerId, Guid propertyId);
        Task<Property> CreatePropertyAsync(Guid farmerId, SavePropertyDTO dto);
        Task<bool> UpdatePropertyAsync(Guid farmerId, Guid id, SavePropertyDTO dto);
        Task<bool> DeletePropertyAsync(Guid farmerId, Guid id);
    }
}
