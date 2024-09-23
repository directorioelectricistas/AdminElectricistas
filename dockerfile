FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DirectorioElectricistas/DirectorioElectricistas.csproj", "DirectorioElectricistas/"]
RUN dotnet restore "DirectorioElectricistas/DirectorioElectricistas.csproj"
COPY . .
WORKDIR "/src/DirectorioElectricistas"
RUN dotnet build "DirectorioElectricistas.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DirectorioElectricistas.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DirectorioElectricistas.dll"]
