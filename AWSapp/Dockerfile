# ==================================================
# BUILD STAGE
# ==================================================

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy project file first
# This allows Docker to cache the restore layer.

COPY ["AWSapp.csproj", "."]

RUN dotnet restore "AWSapp.csproj"

# Copy source code

COPY . .

# Publish application

RUN dotnet publish "AWSapp.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore


# ==================================================
# RUNTIME STAGE
# ==================================================

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

# Copy published application

COPY --from=build /app/publish .

# ECS/ALB will communicate with the application
# on port 8080.

EXPOSE 8080

# Tell ASP.NET Core to listen on all interfaces
# inside the container.

ENV ASPNETCORE_URLS=http://+:8080

# Start application

ENTRYPOINT ["dotnet", "AWSapp.dll"]