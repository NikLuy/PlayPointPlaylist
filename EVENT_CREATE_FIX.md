# Event erstellen - Debugging Guide

## ? Problem
Der "Event erstellen" Button funktioniert nicht.

## ? Fehlerbehebungen implementiert

### 1. **Besseres Error-Handling**
```csharp
private async Task CreateNewEvent()
{
    try
    {
        // Validierung
        if (string.IsNullOrWhiteSpace(newEventModel.Name))
        {
            errorMessage = "Bitte gib einen Event-Namen ein!";
            return;
        }
        
        // Event erstellen
        var createdEvent = await EventService.CreateEvent(newEventModel.Name);
        
        // Erfolgsmeldung
        successMessage = $"Event '{createdEvent.Name}' erstellt! Code: {createdEvent.UniqueId}";
        
        // Input leeren & Liste neu laden
        newEventModel.Name = string.Empty;
        await LoadEvents();
    }
    catch (Exception ex)
    {
        errorMessage = $"Fehler: {ex.Message}";
    }
}
```

### 2. **Success & Error Messages**
- ? Grüne Success-Message nach Erstellung
- ? Rote Error-Message bei Fehler
- ? Dismissible Alerts (mit X zum Schließen)

### 3. **Validierung**
- ? Leeres Input wird abgefangen
- ? Fehlermeldung wird angezeigt

## ?? Debugging-Schritte

### **Schritt 1: Browser-Konsole prüfen**
```
1. F12 drücken (Developer Tools)
2. Console-Tab öffnen
3. Login ? Event erstellen versuchen
4. Fehler in Console?
```

### **Schritt 2: Network-Tab prüfen**
```
1. Network-Tab öffnen
2. "Erstellen" Button klicken
3. Gibt es einen Request?
4. Status Code prüfen (200 = OK, 500 = Server Error)
```

### **Schritt 3: Datenbank prüfen**
```sh
# SQLite DB öffnen
sqlite3 playlist.db

# Events anzeigen
SELECT * FROM Events;

# Sollte Events anzeigen wenn Erstellung funktioniert
```

### **Schritt 4: Logs prüfen**
```sh
# App mit Logging starten
dotnet run --urls="https://localhost:5001"

# Output beobachten beim Event-Erstellen
```

## ?? Test-Schritte

1. **App starten:**
   ```sh
   cd PlayPointPlaylist
   dotnet run
   ```

2. **Admin öffnen:**
   - Browser: `https://localhost:5001/admin`

3. **Login:**
   - Passwort: `admin123`

4. **Event erstellen:**
   - Input: "Test Event"
   - Button "Erstellen" klicken

5. **Erwartetes Verhalten:**
   - ? Grüne Success-Message erscheint
   - ? Event in Tabelle erscheint
   - ? Input-Feld wird geleert

## ?? Mögliche Probleme

### **Problem 1: Datenbank existiert nicht**
```sh
# Lösung: DB neu erstellen
rm playlist.db
dotnet run
```

### **Problem 2: Migration fehlt (VoteRecords)**
```sh
# Lösung: Migration ausführen
sqlite3 playlist.db < database_migration_voting.sql

# ODER: DB neu erstellen
rm playlist.db
dotnet run
```

### **Problem 3: EventService nicht registriert**
```csharp
// Program.cs prüfen:
builder.Services.AddScoped<EventService>(); // Muss vorhanden sein
```

### **Problem 4: Blazor Server Render-Mode**
```razor
// Admin.razor prüfen:
@rendermode @(new InteractiveServerRenderMode(prerender: false))
```

## ?? Quick Fix Checklist

- [x] Error-Handling verbessert
- [x] Success-Message hinzugefügt
- [x] Validierung implementiert
- [x] Try-Catch Block
- [x] Build erfolgreich
- [ ] Datenbank existiert und ist migriert
- [ ] App läuft ohne Fehler
- [ ] Browser-Konsole zeigt keine Fehler

## ?? Test nach Fix

### **Manueller Test:**
```
1. rm playlist.db (DB löschen)
2. dotnet run
3. Browser: https://localhost:5001/admin
4. Login: admin123
5. Event-Name eingeben
6. "Erstellen" klicken
7. Sollte funktionieren!
```

### **Was sollte passieren:**
```
? Grüne Success-Box erscheint:
   "Event 'Test Event' erfolgreich erstellt! URL-Code: abc123"
   
? Event erscheint in Tabelle:
   Name: Test Event
   URL-Code: abc123
   Status: Aktiv
   Songs: 0
   
? Input-Feld ist leer
```

## ?? Häufige Fehler

### **Fehler 1: "Events are null"**
```
Ursache: LoadEvents() wurde nicht aufgerufen
Lösung: await LoadEvents() nach Login
```

### **Fehler 2: "Cannot create event"**
```
Ursache: DB-Constraint Violation
Lösung: 
1. sqlite3 playlist.db
2. .schema Events
3. Prüfe UniqueId Constraint
4. Evtl. DB neu erstellen
```

### **Fehler 3: "Button does nothing"**
```
Ursache: Render-Mode Problem
Lösung: @rendermode mit prerender: false
```

### **Fehler 4: "Disposed DbContext"**
```
Ursache: DbContextFactory nicht korrekt verwendet
Lösung: await using var context = await _factory.CreateDbContextAsync()
```

## ?? Debug-Output hinzufügen (Optional)

```csharp
private async Task CreateNewEvent()
{
    Console.WriteLine($"CreateNewEvent called with: '{newEventModel.Name}'");
    
    try
    {
        if (string.IsNullOrWhiteSpace(newEventModel.Name))
        {
            Console.WriteLine("Validation failed: Empty name");
            errorMessage = "Bitte gib einen Event-Namen ein!";
            return;
        }
        
        Console.WriteLine("Calling EventService.CreateEvent...");
        var createdEvent = await EventService.CreateEvent(newEventModel.Name);
        Console.WriteLine($"Event created: {createdEvent.Id} - {createdEvent.UniqueId}");
        
        successMessage = $"Event '{createdEvent.Name}' erfolgreich erstellt! URL-Code: {createdEvent.UniqueId}";
        newEventModel.Name = string.Empty;
        
        Console.WriteLine("Loading events...");
        await LoadEvents();
        Console.WriteLine($"Events loaded: {events.Count}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR: {ex.Message}");
        Console.WriteLine($"Stack: {ex.StackTrace}");
        errorMessage = $"Fehler beim Erstellen: {ex.Message}";
    }
}
```

## ? Zusammenfassung

**Was wurde gefixt:**
1. ? Error-Handling mit Try-Catch
2. ? Success & Error Messages
3. ? Input-Validierung
4. ? Besseres User-Feedback

**Nächste Schritte:**
1. DB löschen und neu erstellen: `rm playlist.db`
2. App starten: `dotnet run`
3. Testen: Admin ? Login ? Event erstellen

**Falls es immer noch nicht funktioniert:**
- Browser-Konsole prüfen (F12)
- Terminal-Output beim Klicken beobachten
- DB mit `sqlite3 playlist.db` öffnen und Events prüfen
