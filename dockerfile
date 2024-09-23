FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DirectorioElectricistas/DirectorioElectricistas.csproj", "DirectorioElectricistas/"]
RUN dotnet restore "DirectorioElectricistas/DirectorioElectricistas.csproj"
COPY . .
WORKDIR "/src/DirectorioElectricistas"
RUN dotnet build "DirectorioElectricistas.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "DirectorioElectricistas.csproj" -c Release -o /app/publish
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "DirectorioElectricistas.dll"]
