# ============================================================
# Dockerfile — SuministrosDelEste API  |  .NET 8 + SQLite
# Multi-stage build: SDK → Publish → Runtime
# ============================================================

# Etapa 1: Restaurar dependencias (cache optimizado por capas)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/SuministrosDelEste.API/SuministrosDelEste.API.csproj",            "src/SuministrosDelEste.API/"]
COPY ["src/SuministrosDelEste.Application/SuministrosDelEste.Application.csproj", "src/SuministrosDelEste.Application/"]
COPY ["src/SuministrosDelEste.Domain/SuministrosDelEste.Domain.csproj",       "src/SuministrosDelEste.Domain/"]
COPY ["src/SuministrosDelEste.Infrastructure/SuministrosDelEste.Infrastructure.csproj", "src/SuministrosDelEste.Infrastructure/"]
RUN dotnet restore "src/SuministrosDelEste.API/SuministrosDelEste.API.csproj"

# Etapa 2: Build en Release
COPY . .
WORKDIR "/src/src/SuministrosDelEste.API"
RUN dotnet build "SuministrosDelEste.API.csproj" -c Release -o /app/build

# Etapa 3: Publicar artefacto optimizado
FROM build AS publish
RUN dotnet publish "SuministrosDelEste.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 4: Imagen de runtime mínima
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Directorio persistente para la BD SQLite (montar volumen en Railway)
RUN mkdir -p /app/data && chmod 777 /app/data
VOLUME ["/app/data"]

EXPOSE 80
EXPOSE 443

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "SuministrosDelEste.API.dll"]
