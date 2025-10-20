💾 PlayPointPlaylist: Projektanleitung für Copilot
Dieses Projekt implementiert eine webbasierte, Event-gesteuerte Jukebox mit Warteschlangenfunktion.

🎯 Projektziele und Technologien
Projektname: PlayPointPlaylist

Technologie: ASP.NET Core 9 (Razor Pages oder MVC), Entity Framework Core (EF Core), SQLite (als initiale Datenbank), Bootstrap 5.

Musikquelle: YouTube Data API v3 und direkter YouTube-Link-Input.

Deployment: GitHub Actions für einfaches Continuous Deployment (CD) mit Docker.

🛠️ Backend-Infrastruktur
1. Datenbank (EF Core mit SQLite)
Verwende Entity Framework Core und konfiguriere SQLite als initialen Provider.

Erstelle die folgenden Entitäten:

Event:

Id (Primary Key)

Name (z.B. "Jugenddisco 20.10.2025")

UniqueId (String, generiertes, eindeutiges Kürzel für die öffentliche URL, z.B. xyz123)

IsActive (Boolean)

Playlist (Navigation Property zur Liste der QueueItems)

QueueItem (Warteschlangeneintrag):

Id (Primary Key)

EventId (Foreign Key zu Event)

YouTubeVideoId (String, die YouTube-ID, z.B. dQw4w9WgXcQ)

Title (String, Titel des Songs/Videos)

Artist (String, Künstler/Channel)

DurationSeconds (Integer)

RequestedBy (String, optionaler Name/Alias des Anfragenden)

AddedOn (DateTime, Zeitpunkt der Hinzufügung)

PlayOrder (Integer, wichtig für die Sortierung in der Warteliste)

IsPlayed (Boolean, um bereits gespielte Songs zu markieren)

2. Services
YouTubeApiService (Singleton):

Verwaltet den API-Schlüssel (aus appsettings.json).

Methode: SearchVideos(string query) – Ruft die YouTube Data API auf, um Videos zu suchen.

Methode: GetVideoDetails(string videoId) – Ruft Details für eine gegebene Video-ID ab (kostengünstiger als Suche).

QueueService (Scoped/Singleton):

Verwaltet die Logik zum Hinzufügen, Entfernen und Abrufen des nächsten Songs für ein spezifisches Event.

Methode: AddSongToPlaylist(string eventUid, string videoId, ...)

Methode: GetPlaylistForEvent(string eventUid)

💻 Routen und Benutzeroberfläche (Design: 90er-Stil/Bootstrap)
1. Startseite (/)
Inhalt: Nur ein einfacher Platzhalter (z.B. "Willkommen bei PlayPointPlaylist - Wählen Sie eine Disco aus") und ein Link zur Admin-Seite.

2. Admin-Seite (/admin)
Zugriffsschutz: Einfacher Passwort-Schutz, der gegen das hartkodierte AdminSettings:Password aus der appsettings.json prüft (Session- oder Cookie-basiert).

Funktionen:

Event-Verwaltung:

Liste aller bestehenden Events (Name, UniqueID, Status).

Button "Neues Event erstellen" (Formular für Name).

Buttons "Aktivieren/Deaktivieren" und "Löschen" pro Event.

Playlist-Übersicht:

Für jedes aktive Event: Ein Link zur Playlist-Ansicht und eine kurze Statistik (Anzahl Songs in Queue).

Ein Link zur Host-Steuerung (z.B. /admin/play/{uid}), um den aktuellen Song zu sehen und Songs zu überspringen (Skip-Button).

3. Öffentliche Playlist-Seite (/playlist/{uid})
Route: Muss die UniqueId des Events als Parameter nutzen.

Oben:

Suchleiste: Eingabe für Suchbegriffe.

Suchergebnisse: Dynamische Anzeige der Ergebnisse (Titel, Künstler/Channel, Dauer).

"Hinzufügen"-Button: Neben jedem Ergebnis, der den Song über den QueueService zur Warteliste hinzufügt.

Link-Input: Ein Textfeld, in das Benutzer einen direkten YouTube-Link (youtu.be/... oder youtube.com/watch?v=...) einfügen können. Das Backend muss die Video-ID parsen und Details über die API abrufen, bevor es den Song hinzufügt.

Unten:

Live-Playlist-Anzeige: Eine scrollbare, im 90er-Stil gehaltene Liste aller Songs in der Warteschlange (QueueItem).

Die Liste sollte den Title, Artist und die Position (PlayOrder) anzeigen.

Die Seite muss automatisch aktualisiert werden (z.B. per SignalR oder kurzem AJAX-Polling), wenn neue Songs hinzugefügt werden.

📦 Deployment und Betrieb
Dockerfile: Erstelle ein einfaches, mehrstufiges Dockerfile für die ASP.NET Core 9 Anwendung.

GitHub Actions: Füge eine einfache GitHub Action (.github/workflows/deploy.yml) hinzu, die bei einem Push auf main das Docker-Image baut und es auf einer Zielplattform (z.B. GitHub Container Registry oder ein einfaches SSH-Deployment) bereitstellt.

Cross-Plattform: Stelle sicher, dass das Projekt auf Linux (für Docker) lauffähig ist.