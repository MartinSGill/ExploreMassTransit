FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MassTransitDocker.csproj", "MassTransitDocker/"]
RUN dotnet restore "MassTransitDocker/MassTransitDocker.csproj"

COPY . MassTransitDocker/.
RUN dotnet publish -c Release -o /app MassTransitDocker/MassTransitDocker.csproj 

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS publish
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "MassTransitDocker.dll"]