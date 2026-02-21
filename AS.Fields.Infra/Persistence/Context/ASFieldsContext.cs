using AS.Fields.Domain.Entities;
using AS.Fields.Infra.Persistence.Mappings;
using Microsoft.EntityFrameworkCore;

namespace AS.Fields.Infra.Persistence.Context
{
    public class ASFieldsContext(DbContextOptions<ASFieldsContext> options) : DbContext(options)
    {
        public DbSet<Field> Fields { get; set; }
        public DbSet<Property> Properties { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new FieldMapping());
            modelBuilder.ApplyConfiguration(new PropertyMapping());

            base.OnModelCreating(modelBuilder);
        }
    }
}
