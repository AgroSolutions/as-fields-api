using AS.Fields.Domain.DTO.Field;
using AS.Fields.Domain.Entities;

namespace AS.Fields.Domain.Interfaces.Services
{
    public interface IFieldService
    {
        Task<List<Field>> GetAllFields(Guid propertyId);
        Task<Field> GetFieldByIdAsync(Guid id);
        Task<Field> CreateFieldAsync(Guid propertyId, CreateFieldDTO dto);
        Task<bool> PartialUpdateFieldAsync(Guid id, PartialUpdateFieldDTO dto);
        Task<bool> DeleteFieldAsync(Guid id);
    }
}
