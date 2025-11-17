# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY *.csproj .
RUN dotnet restore

# Copy everything else
COPY . .

# Build the app
# dotnet publish -> package the application for deployment
# -c Release -> optimize the code for speed and size by using the production-ready settings
# -o /app/publish -> place the final, deployable files into a specific directory
RUN dotnet publish -c Release -o /app/publish


# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy build output from previous stage
COPY --from=build /app/publish .

# The API inside the container is configured to listen on port 8080
# External access still requires a Port Mapping
EXPOSE 8080

# Run the API
ENTRYPOINT ["dotnet", "Restaurant-API.dll"]