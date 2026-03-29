# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY HaulingDemoApp/HaulingDemoApp.csproj HaulingDemoApp/
RUN dotnet restore HaulingDemoApp/HaulingDemoApp.csproj

# Copy everything else and build
COPY HaulingDemoApp/ HaulingDemoApp/
RUN dotnet publish HaulingDemoApp/HaulingDemoApp.csproj -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Set port
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "HaulingDemoApp.dll"]
