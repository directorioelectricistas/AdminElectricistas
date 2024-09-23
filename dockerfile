# Etapa 1: Usar una imagen base de .NET Core SDK para compilar la aplicación
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copiar el archivo .csproj y restaurar las dependencias
COPY ["DirectorioElectricistas/DirectorioElectricistas.csproj", "DirectorioElectricistas/"]
RUN dotnet restore "DirectorioElectricistas/DirectorioElectricistas.csproj"

# Copiar el resto de los archivos del proyecto
COPY . .

# Compilar el proyecto
WORKDIR "/src/DirectorioElectricistas"
RUN dotnet build "DirectorioElectricistas.csproj" -c Release -o /app/build

# Publicar la aplicación
RUN dotnet publish "DirectorioElectricistas.csproj" -c Release -o /app/publish

# Etapa 2: Configurar una imagen más ligera para ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Exponer el puerto 80 para la aplicación
EXPOSE 80

# Iniciar la aplicación
ENTRYPOINT ["dotnet", "DirectorioElectricistas.dll"]
