using AS.Fields.Domain.DTO.Field;
using AS.Fields.Domain.DTO.Messaging.Field;
using AS.Fields.Domain.Entities;

namespace AS.Fields.Domain.Interfaces.Services
{
    public interface IFieldService
    {
        Task<List<Field>> GetAllFieldsAsync(Guid propertyId);
        Task<Field> GetFieldByIdAsync(Guid id);
        Task<Field> CreateFieldAsync(Guid propertyId, CreateFieldDTO dto);
        Task<bool> PartialUpdateFieldAsync(Guid id, PartialUpdateFieldDTO dto);
        Task<bool> DeleteFieldAsync(Guid id);
        Task<bool> UpdateStatus(UpdateFieldStatusDTO dto);
    }
}
