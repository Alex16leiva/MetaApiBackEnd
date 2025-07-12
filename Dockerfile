# Imagen base de ASP.NET para correr la app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Imagen para compilar el proyecto
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar todo el código fuente
COPY . .

# Cambiar al directorio donde está el proyecto
WORKDIR /src/WebServices

# Publicar el proyecto
RUN dotnet publish WebServices.csproj -c Release -o /app/publish

# Imagen final para correr la app publicada
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebServices.dll"]
