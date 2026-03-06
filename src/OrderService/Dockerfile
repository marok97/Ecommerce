FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Ecommerce.Shared/Ecommerce.Shared.csproj", "Ecommerce.Shared/"]
COPY ["OrderService/OrderService.csproj", "OrderService/"]
RUN dotnet restore "OrderService/OrderService.csproj"

COPY Ecommerce.Shared/ Ecommerce.Shared/
COPY OrderService/ OrderService/
RUN dotnet publish "OrderService/OrderService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OrderService.dll"]
