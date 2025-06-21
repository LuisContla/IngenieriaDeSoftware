# --- Etapa 1: Compilación (Build) ---
# Usamos la imagen del SDK de .NET 9 para compilar.
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# Copiamos primero el archivo del proyecto para optimizar el caché de Docker
COPY ["Práctica 1.csproj", "."]
RUN dotnet restore "Práctica 1.csproj"

# Copiamos el resto del código y publicamos la aplicación
COPY . .
RUN dotnet publish "Práctica 1.csproj" -c Release -o /app/publish


# --- Etapa 2: Ejecución (Final) ---
# Empezamos desde la imagen de runtime de ASP.NET 9.0, que es ligera.
FROM mcr.microsoft.com/dotnet/aspnet:9.0

# --- AQUÍ VA LA CORRECCIÓN, EN LA ETAPA FINAL ---
# Nos cambiamos a usuario 'root' para poder instalar paquetes.
USER root
# Actualizamos e instalamos la librería que falta.
RUN apt-get update && apt-get install -y libgssapi-krb5-2
# Volvemos al usuario 'app' por seguridad (buena práctica).
USER app
# --- FIN DE LA CORRECCIÓN ---

# Establecemos el directorio de trabajo
WORKDIR /app

# Copiamos la aplicación ya compilada desde la etapa de 'build'.
COPY --from=build /app/publish .

# Definimos el punto de entrada para ejecutar la aplicación cuando el contenedor inicie.
ENTRYPOINT ["dotnet", "Práctica 1.dll"]
