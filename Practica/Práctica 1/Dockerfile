# Consulte https://aka.ms/customizecontainer para aprender a personalizar su contenedor de depuración y cómo Visual Studio usa este Dockerfile para compilar sus imágenes para una depuración más rápida.

# Esta fase se usa cuando se ejecuta desde VS en modo rápido (valor predeterminado para la configuración de depuración)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Esta fase se usa para compilar el proyecto de servicio
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Práctica 1.csproj", "."]
RUN dotnet restore "./Práctica 1.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./Práctica 1.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase se usa para publicar el proyecto de servicio que se copiará en la fase final.
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Práctica 1.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# --- INICIO DE LA CORRECCIÓN ---
# Nos cambiamos al usuario 'root' para poder instalar paquetes del sistema.
USER root
# Actualizamos la lista de paquetes de Linux e instalamos la librería que te hacía falta.
RUN apt-get update && apt-get install -y libgssapi-krb5-2
# Es una buena práctica de seguridad volver al usuario 'app' por defecto de la imagen.
USER app
# --- FIN DE LA CORRECCIÓN ---

# Esta fase se usa en producción o cuando se ejecuta desde VS en modo normal (valor predeterminado cuando no se usa la configuración de depuración)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Práctica 1.dll"]