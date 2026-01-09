# IT-Support Chatbot Component

## Übersicht

Die Chatbot-Komponente bietet einen interaktiven IT-Support-Chat, der Benutzer bei technischen Problemen unterstützt. Die Komponente integriert sich mit dem Backend und erstellt automatisch Support-Tickets für jede Chat-Sitzung.

## Features

### ✨ Hauptfunktionen

- **Interaktiver Chat**: Echtzeit-Konversation mit dem IT-Support-Bot
- **Automatisches Ticketing**: Erstellt automatisch ein Support-Ticket für jede Session
- **Nachrichten-Persistierung**: Alle Chat-Nachrichten werden im Backend gespeichert
- **System-Info-Erfassung**: Sammelt automatisch Browser- und Systeminformationen
- **Status-Management**: Tickets können als gelöst markiert werden
- **Mehrsprachig**: Unterstützt Deutsch und Englisch

### 🎨 UI-Features

- **Welcome Screen**: Attraktive Startseite mit Feature-Übersicht
- **Chat-Interface**: Moderne Chat-Bubble-Darstellung
- **Typing-Indikator**: Zeigt an, wenn der Bot eine Antwort vorbereitet
- **Responsive Design**: Optimiert für Desktop und Mobile
- **Animationen**: Smooth Scrolling und visuelle Feedback-Effekte

## Verwendung

### Navigation

Die Chatbot-Komponente ist über zwei Wege erreichbar:

1. **Navbar**: Link "IT-Support Chat" in der Hauptnavigation
2. **Dashboard**: Karte "IT-Support Chat" auf dem Dashboard

### Chat starten

1. Klicken Sie auf "Chat starten" auf dem Welcome Screen
2. Der Bot begrüßt Sie automatisch
3. Geben Sie Ihr Problem im Textfeld ein
4. Drücken Sie Enter oder klicken Sie auf den Senden-Button

### Chat beenden

- **Als gelöst markieren**: Klicken Sie auf das Häkchen-Symbol (✓)
- **Chat beenden**: Klicken Sie auf das X-Symbol
- Beide Aktionen schließen das Ticket und speichern alle Nachrichten

## Technische Details

### Komponenten-Struktur

```
src/app/pages/chatbot/
├── chatbot.component.ts      # Hauptlogik
├── chatbot.component.html    # Template
├── chatbot.component.css     # Styling
└── chatbot.component.spec.ts # Tests
```

### Dependencies

- **Services**:
  - `SupportTicketService`: Erstellt und verwaltet Support-Tickets
  - `ChatMessageService`: Speichert Chat-Nachrichten
  - `MessageBoxServiceService`: Zeigt Bestätigungsdialoge
  - `AuthService`: Authentifizierung (via AuthGuard)

- **Models**:
  - `ISupportTicket`: Ticket-Datenmodell
  - `IChatMessage`: Nachrichten-Datenmodell
  - `TicketStatus`: Enum für Ticket-Status (Resolved/Unresolved)
  - `TicketPriority`: Enum für Priorität (Low/Medium/High/Critical)

### Workflow

1. **Chat-Start**:
   - Benutzer klickt "Chat starten"
   - Bot sendet Willkommensnachricht
   - Chat-Interface wird aktiviert

2. **Erste Nachricht**:
   - Benutzer sendet erste Nachricht
   - System erstellt neues Support-Ticket
   - Nachricht wird gespeichert
   - Bot generiert Antwort basierend auf Keywords

3. **Folge-Nachrichten**:
   - Jede Nachricht wird mit dem Ticket verknüpft
   - Bot-Antworten werden ebenfalls gespeichert
   - Conversation Flow wird aufrechterhalten

4. **Chat-Ende**:
   - Benutzer markiert Problem als gelöst oder beendet Chat
   - Ticket wird aktualisiert (Status: Resolved, EndTime gesetzt)
   - Chat-Session wird geschlossen

### AI-Integration

Aktuell verwendet die Komponente eine einfache keyword-basierte Logik für Bot-Antworten. Die Methode `generateAIResponse()` kann leicht durch eine echte AI-Integration ersetzt werden:

```typescript
private async getAIResponse(userMessage: string): Promise<void> {
  // TODO: Hier Backend-Call zum AI-Service
  // const response = await this.aiService.getResponse(userMessage, this.currentTicket);
  
  // Aktuell: Einfache Keyword-Erkennung
  const aiResponse = this.generateAIResponse(userMessage);
  // ...
}
```

### Keyword-Kategorien

Die Bot-Antworten basieren auf folgenden Kategorien:

- **Drucker-Probleme**: Keywords wie "drucke", "printer"
- **Netzwerk-Probleme**: Keywords wie "internet", "wlan", "wifi"
- **Passwort-Probleme**: Keywords wie "passwort", "password"
- **Performance-Probleme**: Keywords wie "langsam", "slow"
- **Standard-Antwort**: Bei unbekannten Problemen

## Internationalisierung

### Übersetzungs-Keys

Alle Texte sind übersetzt in `de.json` und `en.json`:

```json
"CHATBOT": {
  "WELCOME_TITLE": "...",
  "WELCOME_SUBTITLE": "...",
  "START_CHAT": "...",
  "ASSISTANT_NAME": "...",
  "MARK_RESOLVED": "...",
  "END_CHAT": "...",
  "INPUT_PLACEHOLDER": "...",
  "INPUT_HINT": "...",
  "FEATURE_FAST": "...",
  "FEATURE_SECURE": "...",
  "FEATURE_24_7": "...",
  "COMMON_TOPICS": "...",
  "TOPIC_PRINTER": "...",
  "TOPIC_NETWORK": "...",
  "TOPIC_PASSWORD": "...",
  "TOPIC_SOFTWARE": "...",
  "TOPIC_HARDWARE": "..."
}
```

## Styling

### Design-System

- **Farben**: Gradient von #667eea bis #764ba2
- **Komponenten**: Bootstrap 5 Klassen
- **Icons**: Bootstrap Icons
- **Schrift**: System-Schriftarten

### CSS-Klassen

- `.chatbot-container`: Haupt-Container
- `.welcome-screen`: Startbildschirm
- `.chat-screen`: Chat-Oberfläche
- `.message-bubble`: Nachrichten-Container
- `.typing-indicator`: "Bot tippt..." Anzeige

### Responsive Breakpoints

- Desktop: > 768px
- Mobile: ≤ 768px

## Testing

### Unit Tests

```bash
ng test --include='**/chatbot.component.spec.ts'
```

### Test-Coverage

- ✅ Component Creation
- ✅ Initial State
- ✅ Start Chat Functionality
- ⏳ Message Sending (TODO)
- ⏳ Ticket Creation (TODO)
- ⏳ Chat End Flow (TODO)

## Zukünftige Erweiterungen

### Geplante Features

1. **Echte AI-Integration**
   - Integration mit OpenAI/Azure OpenAI
   - Kontextbasierte Antworten
   - Lern-Algorithmen

2. **Erweiterte Features**
   - Datei-Upload (Screenshots)
   - Voice-to-Text
   - Chat-History anzeigen
   - Ticket-Verlauf einsehen

3. **Admin-Features**
   - Live-Monitoring von Chats
   - Manuelle Übernahme durch Support
   - Analytics und Statistiken

4. **Verbesserungen**
   - Offline-Modus mit Queue
   - Push-Benachrichtigungen
   - Markdown-Unterstützung in Nachrichten
   - Code-Snippet-Formatierung

## Entwickler-Notizen

### Wichtige Methoden

```typescript
// Chat starten
startNewChat(): void

// Nachricht senden
sendMessage(): Promise<void>

// Ticket erstellen
createTicket(problemDescription: string): Promise<void>

// Bot-Antwort generieren
generateAIResponse(userMessage: string): string

// Chat beenden
endChat(): void

// Als gelöst markieren
markAsResolved(): void
```

### Hooks

- `ngOnInit()`: System-Info sammeln
- `ngOnDestroy()`: Cleanup (aktuell leer)

### ViewChild References

- `chatContainer`: Für Auto-Scroll
- `messageInput`: Für Focus-Management

## Support

Bei Fragen oder Problemen:
- Siehe API_DOCUMENTATION.md für Backend-Endpoints
- Siehe FE_Copilot_Inst.md für Frontend-Standards
- GitHub Issues für Bug-Reports

## Version

- **Current**: 1.0.0
- **Last Updated**: Dezember 2025
- **Angular Version**: 17+
