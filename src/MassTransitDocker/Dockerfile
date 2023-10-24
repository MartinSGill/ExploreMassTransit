FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src/

COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/MassTransitDocker/MassTransitDocker.csproj", "MassTransitDocker/"]
RUN dotnet restore "MassTransitDocker/MassTransitDocker.csproj"

COPY . MassTransitDocker/.
RUN dotnet publish -c Release -o /app MassTransitDocker/MassTransitDocker.csproj 

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS publish
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "MassTransitDocker.dll"]