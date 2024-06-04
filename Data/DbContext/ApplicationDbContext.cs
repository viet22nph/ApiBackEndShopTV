using Application.DAL.Models;
using Data.Mapping;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using System;
using System.Linq;
using System.Reflection;

namespace Data.Contexts
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            base.OnModelCreating(modelBuilder);
            RegisterEntityMapping(modelBuilder);
        }

        public void RegisterEntityMapping(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }
            var typeConfigurations = Assembly.GetExecutingAssembly().GetTypes().Where(type =>
                (type.BaseType?.IsGenericType ?? false) &&
                (type.BaseType.GetGenericTypeDefinition() == typeof(MappingEntityTypeConfiguration<>))
            );
            foreach (var item in typeConfigurations)
            {
                var configuration = (IMappingConfiguration)Activator.CreateInstance(item);
                configuration.ApplyConfiguration(modelBuilder);
            }
        }

        public virtual new DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }
    }
}
