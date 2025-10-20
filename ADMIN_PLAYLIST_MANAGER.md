# Admin Playlist-Verwaltung - Feature Update

## ? Implementierte Features

### 1. **Korrekte Song-Zählung**
- ? `GetAllEvents()` lädt jetzt automatisch die Playlist mit (`.Include(e => e.Playlist)`)
- ? Song-Count zeigt jetzt die korrekte Anzahl der Songs in der Queue
- ? Aktualisiert sich automatisch nach jeder Änderung

### 2. **Playlist-Manager Modal**
Neuer Button in der Admin-Tabelle: **"Playlist verwalten"** (Stern-Icon)

#### Features im Playlist-Manager:
- ? **Anzeige aller Songs** mit Details (Titel, Artist, Dauer, Requester)
- ? **Sortierung ändern**: Songs nach oben/unten verschieben
- ? **Songs löschen**: Einzelne Songs aus der Playlist entfernen
- ? **Gespielte Songs markiert**: Unterscheidung zwischen gespielt/nicht gespielt
- ? **Responsive Modal**: Funktioniert auf allen Geräten

### 3. **Neue Service-Methode**
```csharp
public async Task UpdatePlayOrder(int queueItemId, int newPlayOrder)
```
- Ermöglicht das Ändern der Reihenfolge von Songs
- Wird für Sortierung verwendet

## ?? Verwendung

1. **Admin-Bereich öffnen**: `/admin`
2. **Einloggen** mit Admin-Passwort
3. **Playlist-Manager öffnen**: Klick auf das Stern-Icon bei einem Event
4. **Songs verwalten**:
   - ?? **Nach oben**: Pfeil-nach-oben Button
   - ?? **Nach unten**: Pfeil-nach-unten Button
   - ??? **Löschen**: Papierkorb-Icon
5. **Modal schließen**: "Schließen" Button

## ?? UI-Details

### Playlist-Manager Modal
- **Header**: Event-Name + Stern-Icon
- **Body**: Liste aller Songs
- **Footer**: Schließen-Button

### Song-Item Layout
```
[??] [#1] [Song-Info] [Status] [???]
```

- **Sortier-Buttons**: Links (deaktiviert wenn nicht möglich)
- **Position-Badge**: Zeigt aktuelle PlayOrder
- **Song-Info**: Titel, Artist, Dauer, Requester
- **Status**: "Gespielt" Badge wenn `IsPlayed = true`
- **Löschen-Button**: Rechts

### Gespielte Songs
- Grauer Hintergrund (`bg-light`)
- Durchgestrichen (`text-decoration-line-through`)
- Graue Badge (`bg-secondary`)

## ?? Technische Implementierung

### Sortierung
```csharp
private async Task MoveSongUp(QueueItem item)
{
    // Finde vorherigen Song
    // Tausche PlayOrder
    // Speichere in DB
    // Aktualisiere Anzeige
}
```

### Datenkonsistenz
- Alle Änderungen werden sofort in der DB gespeichert
- Nach jeder Aktion werden Playlist UND Events neu geladen
- Garantiert korrekte Song-Counts

## ?? Mobile-Optimierung
- Modal ist scrollbar (`modal-dialog-scrollable`)
- Responsive Layout
- Touch-freundliche Buttons
- Kompakte Darstellung

## ?? Performance
- Lazy Loading: Playlist wird erst geladen wenn Modal öffnet
- Effiziente DB-Queries mit `Include()`
- Minimal nötige Updates

## ?? Testing-Checkliste

- [x] Song-Count korrekt angezeigt
- [x] Playlist-Manager öffnet
- [x] Songs nach oben verschieben
- [x] Songs nach unten verschieben
- [x] Songs löschen
- [x] Gespielte Songs erkennbar
- [x] Modal schließbar
- [x] Build erfolgreich
