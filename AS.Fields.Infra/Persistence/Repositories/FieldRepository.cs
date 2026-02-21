using AS.Fields.Domain.Entities;
using AS.Fields.Domain.Interfaces.Repositories;
using AS.Fields.Infra.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AS.Fields.Infra.Persistence.Repositories
{
    public class FieldRepository(ASFieldsContext context) : BaseRepository<Field>(context), IFieldRepository
    {
    }
}
