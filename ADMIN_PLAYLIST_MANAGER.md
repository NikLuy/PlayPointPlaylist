# Admin Playlist-Verwaltung - Feature Update

## ? Implementierte Features

### 1. **Korrekte Song-Z�hlung**
- ? `GetAllEvents()` l�dt jetzt automatisch die Playlist mit (`.Include(e => e.Playlist)`)
- ? Song-Count zeigt jetzt die korrekte Anzahl der Songs in der Queue
- ? Aktualisiert sich automatisch nach jeder �nderung

### 2. **Playlist-Manager Modal**
Neuer Button in der Admin-Tabelle: **"Playlist verwalten"** (Stern-Icon)

#### Features im Playlist-Manager:
- ? **Anzeige aller Songs** mit Details (Titel, Artist, Dauer, Requester)
- ? **Sortierung �ndern**: Songs nach oben/unten verschieben
- ? **Songs l�schen**: Einzelne Songs aus der Playlist entfernen
- ? **Gespielte Songs markiert**: Unterscheidung zwischen gespielt/nicht gespielt
- ? **Responsive Modal**: Funktioniert auf allen Ger�ten

### 3. **Neue Service-Methode**
```csharp
public async Task UpdatePlayOrder(int queueItemId, int newPlayOrder)
```
- Erm�glicht das �ndern der Reihenfolge von Songs
- Wird f�r Sortierung verwendet

## ?? Verwendung

1. **Admin-Bereich �ffnen**: `/admin`
2. **Einloggen** mit Admin-Passwort
3. **Playlist-Manager �ffnen**: Klick auf das Stern-Icon bei einem Event
4. **Songs verwalten**:
   - ?? **Nach oben**: Pfeil-nach-oben Button
   - ?? **Nach unten**: Pfeil-nach-unten Button
   - ??? **L�schen**: Papierkorb-Icon
5. **Modal schlie�en**: "Schlie�en" Button

## ?? UI-Details

### Playlist-Manager Modal
- **Header**: Event-Name + Stern-Icon
- **Body**: Liste aller Songs
- **Footer**: Schlie�en-Button

### Song-Item Layout
```
[??] [#1] [Song-Info] [Status] [???]
```

- **Sortier-Buttons**: Links (deaktiviert wenn nicht m�glich)
- **Position-Badge**: Zeigt aktuelle PlayOrder
- **Song-Info**: Titel, Artist, Dauer, Requester
- **Status**: "Gespielt" Badge wenn `IsPlayed = true`
- **L�schen-Button**: Rechts

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
- Alle �nderungen werden sofort in der DB gespeichert
- Nach jeder Aktion werden Playlist UND Events neu geladen
- Garantiert korrekte Song-Counts

## ?? Mobile-Optimierung
- Modal ist scrollbar (`modal-dialog-scrollable`)
- Responsive Layout
- Touch-freundliche Buttons
- Kompakte Darstellung

## ?? Performance
- Lazy Loading: Playlist wird erst geladen wenn Modal �ffnet
- Effiziente DB-Queries mit `Include()`
- Minimal n�tige Updates

## ?? Testing-Checkliste

- [x] Song-Count korrekt angezeigt
- [x] Playlist-Manager �ffnet
- [x] Songs nach oben verschieben
- [x] Songs nach unten verschieben
- [x] Songs l�schen
- [x] Gespielte Songs erkennbar
- [x] Modal schlie�bar
- [x] Build erfolgreich
