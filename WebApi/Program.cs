using Application.Api.Extentions;
using Caching;
using Data.Contexts;
using Data.SeedData;
using Models.Settings;
using WebApi.Middlewares;
using WebApi.Udapters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<SeedData>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//
builder.Services.AddDistributedMemoryCache();
builder.Services.AddIdentityService();
builder.Services.AddDatabaseService(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddRedis(builder.Configuration);
builder.Services.AddHostedService<DiscountStatusUpdater>();
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";




builder.Services.AddAuthorization(options =>
{
    //options.AddPolicy("CanEditProduct", policy =>
    //    policy.RequireClaim("canEditProduct", "true"));
    //options.AddPolicy("CanRemoveProduct", policy =>
    //    policy.RequireClaim("canRemoveProduct", "true"));
    //options.AddPolicy("CanCreateProduct", policy =>
    //    policy.RequireClaim("canCreateProduct", "true"));

    //options.AddPolicy("CanEditCategory", policy =>
    //policy.RequireClaim("canEditCategory"));

    foreach (var permission in Permissions.GetAllPermissions())
    {
        options.AddPolicy(permission, policy => policy.RequireClaim(permission, "true"));
    }
});
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();

app.UseCors(MyAllowSpecificOrigins).UseForwardedHeaders();
app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseMiddleware<AuthenticationErrorHandlingMiddleware>();
app.UseMiddleware<VisitTrackingMiddleware>();
//Seed
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;



try
{
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
    var seed = services.GetService<SeedData>();
    if (seed != null)
    {
        await seed.SeedDataAsync();

    }
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration");
}

app.Run();
