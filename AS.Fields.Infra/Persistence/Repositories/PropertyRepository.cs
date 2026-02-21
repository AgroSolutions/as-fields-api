using AS.Fields.Domain.Entities;
using AS.Fields.Domain.Interfaces.Repositories;
using AS.Fields.Infra.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AS.Fields.Infra.Persistence.Repositories
{
    public class PropertyRepository(ASFieldsContext context) : BaseRepository<Property>(context), IPropertyRepository
    {
    }
}
