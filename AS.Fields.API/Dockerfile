# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AS.Fields.API/AS.Fields.API.csproj", "AS.Fields.API/"]
COPY ["AS.Fields.Application/AS.Fields.Application.csproj", "AS.Fields.Application/"]
COPY ["AS.Fields.Domain/AS.Fields.Domain.csproj", "AS.Fields.Domain/"]
COPY ["AS.Fields.Infra/AS.Fields.Infra.csproj", "AS.Fields.Infra/"]
RUN dotnet restore "./AS.Fields.API/AS.Fields.API.csproj"
COPY . .
WORKDIR "/src/AS.Fields.API"
RUN dotnet build "./AS.Fields.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AS.Fields.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AS.Fields.API.dll"]