# PlayPointPlaylist

A Blazor-based web application for managing YouTube playlists.

## Prerequisites

- [Docker](https://www.docker.com/get-started) (version 20.10 or higher)
- [Docker Compose](https://docs.docker.com/compose/install/) (version 1.29 or higher)
- YouTube Data API v3 Key ([Get one here](https://console.cloud.google.com/apis/credentials))

## Docker Setup

### 1. Clone the Repository

```bash
git clone <repository-url>
cd PlayPointPlaylist
```

### 2. Configure Environment Variables

Copy the example environment file and configure your settings:

```bash
cp .env.example .env
```

Edit the `.env` file and provide your credentials:

```
YOUTUBE_API_KEY=your_youtube_api_key_here
ADMIN_PASSWORD=your_secure_admin_password_here
```

**Important**: If your password contains special characters like `$`, `!`, `@`, or `` ` ``, wrap it in single quotes to prevent shell variable expansion:

```
ADMIN_PASSWORD='MyP@ssw0rd$123!'
```

### 3. Configure Docker Compose

Copy the example Docker Compose file:

```bash
cp docker-compose.example.yml docker-compose.yml
```

The default configuration exposes the following ports:
- **8080**: HTTP
- **8081**: HTTPS (if configured)

You can modify these ports in `docker-compose.yml` if needed.

### 4. Build and Run with Docker Compose

Build and start the application:

```bash
docker-compose up -d
```

This will:
- Build the Docker image for the application
- Create a persistent volume for the SQLite database at `./data`
- Start the container in detached mode

### 5. Access the Application

Once the container is running, access the application at:

- **HTTP**: http://localhost:8080

### Docker Commands

#### View logs
```bash
docker-compose logs -f
```

#### Stop the application
```bash
docker-compose down
```

#### Restart the application
```bash
docker-compose restart
```

#### Rebuild after code changes
```bash
docker-compose up -d --build
```

#### Remove everything (including volumes)
```bash
docker-compose down -v
```

### Alternative: Using Docker without Docker Compose

If you prefer to use Docker directly without Docker Compose:

#### Build the image
```bash
docker build -t playpointplaylist .
```

#### Run the container
```bash
docker run -d \
  --name playpointplaylist \
  -p 8080:8080 \
  -p 8081:8081 \
  -v $(pwd)/data:/app/data \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ASPNETCORE_URLS=http://+:8080 \
  -e ConnectionStrings__DefaultConnection="Data Source=/app/data/playlist.db" \
  -e YouTubeApi__ApiKey="your_youtube_api_key_here" \
  -e AdminSettings__Password="your_secure_admin_password_here" \
  playpointplaylist
```

## Data Persistence

The application uses SQLite as its database. The database file is stored in the `./data` directory, which is mounted as a volume in the Docker container. This ensures that your data persists even if the container is removed or recreated.

## Troubleshooting

### Container won't start
Check the logs for errors:
```bash
docker-compose logs
```

### Port already in use
If ports 8080 or 8081 are already in use, modify the port mappings in `docker-compose.yml`:
```yaml
ports:
  - 9090:8080  # Change 9090 to your preferred port
```

### Database permissions issues
Ensure the `./data` directory has the correct permissions:
```bash
mkdir -p data
chmod 755 data
```

### Can't connect to YouTube API
Verify your `YOUTUBE_API_KEY` in the `.env` file is correct and has the YouTube Data API v3 enabled in your Google Cloud Console.

### Wrong password / Login fails
If your admin password contains special characters (`$`, `!`, `@`, `` ` ``), make sure it's wrapped in single quotes in the `.env` file:
```
ADMIN_PASSWORD='F7l6mENBp$GV57WrJgrzy5U!rGE@'
```
Without quotes, the `$` character will be interpreted as a variable expansion by Docker Compose.

## Development

For local development without Docker, see the development setup instructions in the project documentation.

## Technology Stack

- **.NET 9**: Application framework
- **Blazor**: UI framework
- **SQLite**: Database
- **Docker**: Containerization
- **YouTube Data API v3**: Playlist management

## License

[Add your license information here]
