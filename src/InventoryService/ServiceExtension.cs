using InventoryService.Database;
using InventoryService.Endpoints;
using InventoryService.Exceptions.ExceptionHandlers;
using InventoryService.Services;
using Microsoft.EntityFrameworkCore;

namespace InventoryService;

public static class ServiceExtension
{
    public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgresDb");
        services.AddDbContextPool<InventoryDbContext>(options => { options.UseNpgsql(connectionString); });
    }

    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IInventoryService, Services.InventoryService>();
    }

    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        app.MapInventoryEndpoints();
        return app;
    }

    public static void ConfigureExceptionHandlers(this IServiceCollection services)
    {
        services.AddExceptionHandler<NotFoundExceptionHandler>();
        services.AddExceptionHandler<ConflictExceptionHandler>();
        services.AddProblemDetails();
    }
}