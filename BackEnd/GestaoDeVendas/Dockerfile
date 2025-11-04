# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["GestaoDeVendas.Api/GestaoDeVendas.Api.csproj", "GestaoDeVendas.Api/"]
COPY ["GestaoDeVendas.Application/GestaoDeVendas.Application.csproj", "GestaoDeVendas.Application/"]
COPY ["GestaoDeVendas.Domain/GestaoDeVendas.Domain.csproj", "GestaoDeVendas.Domain/"]
COPY ["GestaoDeVendas.Infrastructure/GestaoDeVendas.Infrastructure.csproj", "GestaoDeVendas.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "GestaoDeVendas.Api/GestaoDeVendas.Api.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/GestaoDeVendas.Api"
RUN dotnet build "GestaoDeVendas.Api.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "GestaoDeVendas.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Copy published files from publish stage
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

# Run the application
ENTRYPOINT ["dotnet", "GestaoDeVendas.Api.dll"]