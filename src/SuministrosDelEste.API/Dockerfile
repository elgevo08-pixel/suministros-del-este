FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/SuministrosDelEste.API/SuministrosDelEste.API.csproj", "src/SuministrosDelEste.API/"]
COPY ["src/SuministrosDelEste.Application/SuministrosDelEste.Application.csproj", "src/SuministrosDelEste.Application/"]
COPY ["src/SuministrosDelEste.Domain/SuministrosDelEste.Domain.csproj", "src/SuministrosDelEste.Domain/"]
COPY ["src/SuministrosDelEste.Infrastructure/SuministrosDelEste.Infrastructure.csproj", "src/SuministrosDelEste.Infrastructure/"]
RUN dotnet restore "src/SuministrosDelEste.API/SuministrosDelEste.API.csproj"
COPY . .
WORKDIR "/src/src/SuministrosDelEste.API"
RUN dotnet publish "SuministrosDelEste.API.csproj" -c Release -o /app/publish /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
RUN mkdir -p /app/data
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SuministrosDelEste.API.dll"]
