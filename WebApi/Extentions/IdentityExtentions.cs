
using Data.Contexts;
using Microsoft.AspNetCore.Identity;
using Models.Models;

namespace Application.Api.Extentions
{
    public static class IdentityExtentions
    {
        public static IServiceCollection AddIdentityService(this IServiceCollection services)
        {

            services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedEmail  = false)
                .AddDefaultTokenProviders()
                .AddTokenProvider("MyApp", typeof(DataProtectorTokenProvider<ApplicationUser>))
                .AddEntityFrameworkStores<ApplicationDbContext>();


            return services;
        }


    }
}
