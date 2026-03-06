using InventoryService;
using InventoryService.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureExceptionHandlers();
builder.Services.ConfigureServices();

var app = builder.Build();
app.UseExceptionHandler();
app.MapApiEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
