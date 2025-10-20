# Voting Feature - Implementation Guide

## ? Implementierte Features

### 1. **Up/Down Voting System**
- ? Benutzer können Songs upvoten ??
- ? Benutzer können Songs downvoten ??
- ? Vote-Counts werden live angezeigt
- ? **Session-basiertes Tracking** - Ein Vote pro Song/Session
- ? Nur nicht-gespielte Songs können gevotet werden

### 2. **Startseite bereinigt**
- ? Admin-Link entfernt
- ? Cleanes, minimalistisches Design
- ? Nur Info-Text für Event-Links

## ?? Voting UI

### Playlist-Ansicht (`/playlist/{uid}`)

Jeder nicht-gespielte Song zeigt:
```
[Song Info]  [?? 5] [?? 2]
```

- **Grüner Button**: Upvote (??)
- **Roter Button**: Downvote (??)
- **Badge**: Zeigt Vote-Count
- **Disabled**: Nach dem Voten (Session-Lock)
- **Highlighted**: Button färbt sich nach Vote

### Admin-Ansicht
Im Playlist-Manager werden Votes angezeigt:
```
Song Title
Artist • 3:45 • ?? 5 | ?? 2
```

## ?? Session-Tracking

### Wie funktioniert es?

1. **Vote abgeben**:
   - User klickt auf ?? oder ??
   - Vote wird in DB gespeichert
   - SongId wird in Session-Dictionary gespeichert

2. **Vote-Status prüfen**:
   - Dictionary: `{songId: isUpVote}`
   - `HasVoted(songId, isUpVote)` prüft Status
   - Button wird disabled wenn bereits gevotet

3. **Session-Persistenz**:
   - Votes bleiben während der Browser-Session
   - Bei Reload: SessionStorage (optional)
   - Verhindert mehrfaches Voten

## ?? Datenbank-Schema

### QueueItem - Neue Felder:
```csharp
public int UpVotes { get; set; } = 0;
public int DownVotes { get; set; } = 0;
```

### Migration:
```sql
ALTER TABLE QueueItems ADD COLUMN UpVotes INTEGER NOT NULL DEFAULT 0;
ALTER TABLE QueueItems ADD COLUMN DownVotes INTEGER NOT NULL DEFAULT 0;
```

## ?? Services

### QueueService - Neue Methoden:

```csharp
public async Task<bool> UpVote(int queueItemId)
{
    // Erhöht UpVotes um 1
    // Nur für nicht-gespielte Songs
    // Returns true bei Erfolg
}

public async Task<bool> DownVote(int queueItemId)
{
    // Erhöht DownVotes um 1
    // Nur für nicht-gespielte Songs
    // Returns true bei Erfolg
}
```

## ?? UI-Details

### Button-States:

1. **Nicht gevotet**:
   - `btn-outline-success` (Grün umrandet)
   - `btn-outline-danger` (Rot umrandet)
   - Clickable

2. **Upvote abgegeben**:
   - `btn-success` (Grün ausgefüllt)
   - Disabled
   - Badge zeigt Count

3. **Downvote abgegeben**:
   - `btn-danger` (Rot ausgefüllt)
   - Disabled
   - Badge zeigt Count

### Icons:
- ?? `bi-hand-thumbs-up-fill`
- ?? `bi-hand-thumbs-down-fill`

## ?? Setup

### Für bestehende Datenbank:
```bash
# Option 1: SQL-Script ausführen
sqlite3 playlist.db < database_migration_voting.sql

# Option 2: Datenbank neu erstellen
rm playlist.db
dotnet run
```

### Für neue Installation:
```bash
dotnet run
# DB wird automatisch mit Voting-Feldern erstellt
```

## ?? Verwendung

### Als User (Playlist-Seite):
1. Öffne Event-Playlist: `/playlist/{uid}`
2. Songs in Liste finden
3. Klick auf ?? für Upvote
4. Klick auf ?? für Downvote
5. Button wird disabled nach Vote
6. Count aktualisiert sich automatisch

### Als Admin:
1. Playlist-Manager öffnen
2. Votes werden bei jedem Song angezeigt
3. Format: ?? 5 | ?? 2

## ?? Sicherheit

### Session-basiert:
- **Pro**: Einfach, keine Anmeldung nötig
- **Con**: Neues Browser-Fenster = neue Session

### Verhinderung von Mehrfach-Votes:
1. **Client-Side**: Dictionary im Component
2. **Session**: Persistenz über SessionStorage (optional)
3. **DB**: Votes werden akkumuliert

### Limitierungen:
- ? Kein Vote-Rückzug implementiert
- ? Keine IP-basierte Limitierung
- ? Einfach und funktional für Events

## ?? Mögliche Erweiterungen

### Future Features:
- [ ] Vote-Rückzug (Un-vote)
- [ ] Vote-Wechsel (Up?Down oder Down?Up)
- [ ] Cookie-basiertes Tracking (länger als Session)
- [ ] IP-basierte Rate-Limiting
- [ ] Admin-Ansicht mit Vote-Historie
- [ ] Automatische Sortierung nach Votes
- [ ] Vote-Trends (?? Trending)

## ?? Troubleshooting

### Votes funktionieren nicht:
1. Build erfolgreich? `dotnet build`
2. Datenbank aktualisiert? Check SQL-Script
3. Browser-Cache leeren

### Vote-Count zeigt 0:
1. Playlist neu laden
2. Timer läuft? (5 Sek Refresh)
3. DB-Check: `SELECT UpVotes, DownVotes FROM QueueItems;`

### Button bleibt enabled:
1. Session-Storage implementieren (JS Interop)
2. Dictionary wird nicht gespeichert
3. Page-Reload löscht in-memory State

## ?? Code-Referenzen

### Files geändert:
- `Models/QueueItem.cs` - Voting-Felder
- `Services/QueueService.cs` - Vote-Methoden
- `Components/Pages/Playlist.razor` - UI + Logic
- `Components/Pages/Admin.razor` - Vote-Anzeige
- `Components/Pages/Home.razor` - Admin-Link entfernt

### Database:
- `database_migration_voting.sql` - Migration-Script
