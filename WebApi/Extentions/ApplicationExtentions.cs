
using Application.DAL.Helper;
using Core.Interfaces;
using Core.Services;
using Data.Contexts;
using Data.Repos;
using Data.Repos.DiscountRepo;
using Data.Repos.OrderRepo;
using Data.Repos.ProductRepo;
using Data.SeedData;
using Data.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Models.ResponseModels;
using Models.Settings;
using Newtonsoft.Json;
using Services.AccountServices;
using Services.Concrete;
using Services.Interfaces;
using System.Text;
using WebApi.Helpers;
using WebApi.Udapters;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Application.Api.Extentions
{
    public static class ApplicationExtentions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.Configure<GoogleSetting>(configuration.GetSection("GoogleAuthSettings"));
          
            services.AddTransient<IEmailCoreService, EmailCoreService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddTransient<IDbContext, ApplicationDbContext>();
            services.AddScoped<IAccountServices, AccountServices>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddAutoMapper(typeof(MappingProfile));

            services.AddScoped<IUploadPhotoService, UploadFileService>();
            services.AddScoped<IUploadPhotoCoreService, UploadPhotoCoreService>();

            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ITransactionService, TransactionService>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IReviewService,  ReviewService>();
            services.AddScoped<RoleManager<IdentityRole>>();
            var jwt = new JWTSettings();
            configuration.GetSection(nameof(JWTSettings)).Bind(jwt); 
           
            services.AddScoped<IDiscountService, DiscountService>();
            services.AddScoped<DiscountStatusUpdater>();
            services.AddSingleton<IVnPayService, VnpayService>();
            services.AddScoped<IReportService, ReportService>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
             {
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuerSigningKey = true,
                     ValidateLifetime = true,
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                     ValidIssuer = jwt.Issuer,
                     ValidAudience = jwt.Audience
                 };
                 options.Events = new JwtBearerEvents()
                 {
                    
                     OnChallenge = context =>
                     {
                        
                             context.HandleResponse();
                             context.Response.StatusCode = 401;
                             context.Response.ContentType = "application/json";
                             var result = JsonConvert.SerializeObject(new BaseResponse<string>("You are not Authorized"));
                             return context.Response.WriteAsync(result);
                       
                     },
                     OnForbidden = context =>
                     {
                         context.Response.StatusCode = 403;
                         context.Response.ContentType = "application/json";
                         var result = JsonConvert.SerializeObject(new BaseResponse<string>("You are not authorized to access this resource"));
                         return context.Response.WriteAsync(result);
                     },
                 };
             });

            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "Bearer Authentication with JWT Token",
                    Type = SecuritySchemeType.Http
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                });
            });

            return services;
        }


    }
}
