FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
ENV ASPNETCORE_ENVIRONMENT=Development
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Vendas.API/Vendas.API.csproj", "Vendas.API/"]
COPY ["Vendas.Core/Vendas.Core.csproj", "Vendas.Core/"]
COPY ["Vendas.Abstractions/Vendas.Abstractions.csproj", "Vendas.Abstractions/"]
COPY ["Vendas.Model/Vendas.Model.csproj", "Vendas.Model/"]
COPY ["Vendas.Business/Vendas.Business.csproj", "Vendas.Business/"]
COPY ["Vendas.Common/Vendas.Common.csproj", "Vendas.Common/"]
COPY ["Vendas.DB/Vendas.DB.csproj", "Vendas.DB/"]
COPY ["Vendas.DB.Migrations/Vendas.DB.Migrations.csproj", "Vendas.DB.Migrations/"]
RUN dotnet restore "./Vendas.API/Vendas.API.csproj"
COPY . .
WORKDIR "/src/Vendas.API"
RUN dotnet build "./Vendas.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Vendas.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Vendas.API.dll"]
