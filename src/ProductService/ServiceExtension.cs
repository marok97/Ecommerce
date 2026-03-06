using Microsoft.EntityFrameworkCore;
using ProductService.Database;
using ProductService.Endpoints;
using ProductService.Exceptions.ExceptionHandlers;

namespace ProductService;

public static class ServiceExtension
{
    public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgresDb");
        services.AddDbContextPool<ProductDbContext>(options => { options.UseNpgsql(connectionString); });
    }

    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<Services.IProductService, Services.ProductService>();
    }

    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        app.MapProductEndpoints();
        return app;
    }

    public static void ConfigureExceptionHandlers(this IServiceCollection services)
    {
        services.AddExceptionHandler<NotFoundExceptionHandler>();
        services.AddExceptionHandler<ConflictExceptionHandler>();
        services.AddProblemDetails();
    }
}
