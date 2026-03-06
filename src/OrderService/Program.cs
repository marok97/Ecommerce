using OrderService;
using OrderService.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureExceptionHandlers();
builder.Services.ConfigureServices();
builder.Services.ConfigureHttpClients(builder.Configuration);

var app = builder.Build();
app.UseExceptionHandler();
app.MapApiEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
