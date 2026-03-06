FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Ecommerce.Shared/Ecommerce.Shared.csproj", "Ecommerce.Shared/"]
COPY ["ProductService/ProductService.csproj", "ProductService/"]
RUN dotnet restore "ProductService/ProductService.csproj"

COPY Ecommerce.Shared/ Ecommerce.Shared/
COPY ProductService/ ProductService/
RUN dotnet publish "ProductService/ProductService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ProductService.dll"]
