# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and project files
COPY *.sln .
COPY FuelManagementAPI/*.csproj ./FuelManagementAPI/

# Restore
RUN dotnet restore

# Copy all source files
COPY FuelManagementAPI/. ./FuelManagementAPI/
WORKDIR /app/FuelManagementAPI
RUN dotnet publish -c Release -o out

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/FuelManagementAPI/out .

EXPOSE 80
ENTRYPOINT ["dotnet", "FuelManagementAPI.dll"]
