
using Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Models.Models;

namespace Application.Api.Extentions
{
    public static class DatabaseExtentions
    {

        public static IServiceCollection AddDatabaseService(this IServiceCollection services, IConfiguration configuration )
        {


            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
            return services;
        }

    }
}
