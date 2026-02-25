FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files first for layer caching
COPY AlgreenMES.sln ./
COPY AlgreenMES.API/AlgreenMES.API.csproj AlgreenMES.API/
COPY src/BuildingBlocks/AlGreenMES.BuildingBlocks.Common/AlGreenMES.BuildingBlocks.Common.csproj src/BuildingBlocks/AlGreenMES.BuildingBlocks.Common/
COPY src/BuildingBlocks/AlGreenMES.BuildingBlocks.EventBus/AlGreenMES.BuildingBlocks.EventBus.csproj src/BuildingBlocks/AlGreenMES.BuildingBlocks.EventBus/
COPY src/Modules/Tenancy/AlGreenMES.Modules.Tenancy.Domain/AlGreenMES.Modules.Tenancy.Domain.csproj src/Modules/Tenancy/AlGreenMES.Modules.Tenancy.Domain/
COPY src/Modules/Tenancy/AlGreenMES.Modules.Tenancy.Application/AlGreenMES.Modules.Tenancy.Application.csproj src/Modules/Tenancy/AlGreenMES.Modules.Tenancy.Application/
COPY src/Modules/Tenancy/AlGreenMES.Modules.Tenancy.Infrastructure/AlGreenMES.Modules.Tenancy.Infrastructure.csproj src/Modules/Tenancy/AlGreenMES.Modules.Tenancy.Infrastructure/
COPY src/Modules/Tenancy/AlGreenMES.Modules.Tenancy.Api/AlGreenMES.Modules.Tenancy.Api.csproj src/Modules/Tenancy/AlGreenMES.Modules.Tenancy.Api/
COPY src/Modules/Identity/AlGreenMES.Modules.Identity.Domain/AlGreenMES.Modules.Identity.Domain.csproj src/Modules/Identity/AlGreenMES.Modules.Identity.Domain/
COPY src/Modules/Identity/AlGreenMES.Modules.Identity.Application/AlGreenMES.Modules.Identity.Application.csproj src/Modules/Identity/AlGreenMES.Modules.Identity.Application/
COPY src/Modules/Identity/AlGreenMES.Modules.Identity.Infrastructure/AlGreenMES.Modules.Identity.Infrastructure.csproj src/Modules/Identity/AlGreenMES.Modules.Identity.Infrastructure/
COPY src/Modules/Identity/AlGreenMES.Modules.Identity.Api/AlGreenMES.Modules.Identity.Api.csproj src/Modules/Identity/AlGreenMES.Modules.Identity.Api/
COPY src/Modules/Production/AlGreenMES.Modules.Production.Domain/AlGreenMES.Modules.Production.Domain.csproj src/Modules/Production/AlGreenMES.Modules.Production.Domain/
COPY src/Modules/Production/AlGreenMES.Modules.Production.Application/AlGreenMES.Modules.Production.Application.csproj src/Modules/Production/AlGreenMES.Modules.Production.Application/
COPY src/Modules/Production/AlGreenMES.Modules.Production.Infrastructure/AlGreenMES.Modules.Production.Infrastructure.csproj src/Modules/Production/AlGreenMES.Modules.Production.Infrastructure/
COPY src/Modules/Production/AlGreenMES.Modules.Production.Api/AlGreenMES.Modules.Production.Api.csproj src/Modules/Production/AlGreenMES.Modules.Production.Api/
COPY src/Modules/Orders/AlGreenMES.Modules.Orders.Domain/AlGreenMES.Modules.Orders.Domain.csproj src/Modules/Orders/AlGreenMES.Modules.Orders.Domain/
COPY src/Modules/Orders/AlGreenMES.Modules.Orders.Application/AlGreenMES.Modules.Orders.Application.csproj src/Modules/Orders/AlGreenMES.Modules.Orders.Application/
COPY src/Modules/Orders/AlGreenMES.Modules.Orders.Infrastructure/AlGreenMES.Modules.Orders.Infrastructure.csproj src/Modules/Orders/AlGreenMES.Modules.Orders.Infrastructure/
COPY src/Modules/Orders/AlGreenMES.Modules.Orders.Api/AlGreenMES.Modules.Orders.Api.csproj src/Modules/Orders/AlGreenMES.Modules.Orders.Api/

RUN dotnet restore

# Copy everything and build
COPY . .
RUN dotnet publish AlgreenMES.API/AlgreenMES.API.csproj -c Release -o /app/publish --no-restore

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

RUN mkdir -p /app/uploads

# Render uses PORT env var
ENV ASPNETCORE_URLS=http://+:${PORT:-10000}
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 10000

ENTRYPOINT ["dotnet", "AlgreenMES.API.dll"]
