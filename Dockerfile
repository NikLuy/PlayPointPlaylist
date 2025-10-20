# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["PlayPointPlaylist/PlayPointPlaylist.csproj", "PlayPointPlaylist/"]
RUN dotnet restore "PlayPointPlaylist/PlayPointPlaylist.csproj"

# Copy everything else and build
COPY PlayPointPlaylist/ PlayPointPlaylist/
WORKDIR "/src/PlayPointPlaylist"
RUN dotnet build "PlayPointPlaylist.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "PlayPointPlaylist.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Create data directory for SQLite database
RUN mkdir -p /app/data

EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .

# Update connection string to use volume-mounted database
ENV ASPNETCORE_URLS=http://+:8080;https://+:8081
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "PlayPointPlaylist.dll"]
