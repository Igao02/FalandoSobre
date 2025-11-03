# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FalandoSobre.Web/FalandoSobre.Web/FalandoSobre.Web.csproj", "FalandoSobre.Web/FalandoSobre.Web/"]
COPY ["FalandoSobre.Infrastructure/FalandoSobre.Infrastructure.csproj", "FalandoSobre.Infrastructure/"]
COPY ["FalandoSobre.Domain/FalandoSobre.Domain.csproj", "FalandoSobre.Domain/"]
COPY ["FalandoSobre.DomainCore/FalandoSobre.DomainCore.csproj", "FalandoSobre.DomainCore/"]
COPY ["FalandoSobreApplication/FalandoSobreApplication.csproj", "FalandoSobreApplication/"]
COPY ["FalandoSobre.SharedKernel/FalandoSobre.SharedKernel.csproj", "FalandoSobre.SharedKernel/"]
COPY ["FalandoSobre.Web/FalandoSobre.Web.Client/FalandoSobre.Web.Client.csproj", "FalandoSobre.Web/FalandoSobre.Web.Client/"]
RUN dotnet restore "./FalandoSobre.Web/FalandoSobre.Web/FalandoSobre.Web.csproj"
COPY . .
WORKDIR "/src/FalandoSobre.Web/FalandoSobre.Web"
RUN dotnet build "./FalandoSobre.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FalandoSobre.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FalandoSobre.Web.dll"]