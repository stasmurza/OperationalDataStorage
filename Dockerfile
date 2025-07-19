# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OperationalDataStorage.Presentation/OperationalDataStorage.Presentation.csproj", "OperationalDataStorage.Presentation/"]
COPY ["OperationalDataStorage.Application.Models/OperationalDataStorage.Application.Models.csproj", "OperationalDataStorage.Application.Models/"]
COPY ["OperationalDataStorage.Application/OperationalDataStorage.Application.csproj", "OperationalDataStorage.Application/"]
COPY ["OperationalDataStorage.Domain/OperationalDataStorage.Domain.csproj", "OperationalDataStorage.Domain/"]
COPY ["OperationalDataStorage.Contracts/OperationalDataStorage.Contracts.csproj", "OperationalDataStorage.Contracts/"]
COPY ["OperationalDataStorage.Infrastructure.Persistance/OperationalDataStorage.Infrastructure.Persistence.csproj", "OperationalDataStorage.Infrastructure.Persistance/"]
COPY ["OperationalDataStorage.Infrastructure/OperationalDataStorage.Infrastructure.csproj", "OperationalDataStorage.Infrastructure/"]
COPY ["OperationalDataStorage.Infrastructure.Models/OperationalDataStorage.Infrastructure.Models.csproj", "OperationalDataStorage.Infrastructure.Models/"]
RUN dotnet restore "./OperationalDataStorage.Presentation/OperationalDataStorage.Presentation.csproj"
COPY . .
WORKDIR "/src/OperationalDataStorage.Presentation"
RUN dotnet build "./OperationalDataStorage.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OperationalDataStorage.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OperationalDataStorage.Presentation.dll"]