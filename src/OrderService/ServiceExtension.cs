using Microsoft.EntityFrameworkCore;
using OrderService.Clients;
using OrderService.Database;
using OrderService.Endpoints;
using OrderService.Exceptions.ExceptionHandlers;

namespace OrderService;

public static class ServiceExtension
{
    public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgresDb");
        services.AddDbContextPool<OrderDbContext>(options => { options.UseNpgsql(connectionString); });
    }

    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<Services.IOrderService, Services.OrderService>();
    }

    public static void ConfigureHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceUrls = configuration.GetSection("ServiceUrls");

        services.AddHttpClient<IProductServiceClient, ProductServiceClient>(c =>
            c.BaseAddress = new Uri(serviceUrls["ProductService"]!));

        services.AddHttpClient<IInventoryServiceClient, InventoryServiceClient>(c =>
            c.BaseAddress = new Uri(serviceUrls["InventoryService"]!));

        services.AddHttpClient<INotificationServiceClient, NotificationServiceClient>(c =>
            c.BaseAddress = new Uri(serviceUrls["NotificationService"]!));
    }

    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        app.MapOrderEndpoints();
        return app;
    }

    public static void ConfigureExceptionHandlers(this IServiceCollection services)
    {
        services.AddExceptionHandler<NotFoundExceptionHandler>();
        services.AddExceptionHandler<ConflictExceptionHandler>();
        services.AddExceptionHandler<BadRequestExceptionHandler>();
        services.AddProblemDetails();
    }
}
