# SupportChat - IT Support Chatbot System

Ein intelligentes IT-Support-System mit KI-gestütztem Chatbot für automatisierte Problembehebung und Ticket-Management.

---

## 📋 Inhaltsverzeichnis

- [Projektübersicht](#projektübersicht)
- [Architektur](#architektur)
- [Technologie-Stack](#technologie-stack)
- [Features](#features)
- [Projektstruktur](#projektstruktur)
- [Installation & Setup](#installation--setup)
- [API-Dokumentation](#api-dokumentation)
- [n8n Integration](#n8n-integration)
- [Frontend (Angular)](#frontend-angular)
- [Datenbank](#datenbank)
- [Entwicklung](#entwicklung)
- [Testing](#testing)
- [Deployment](#deployment)

---

## 🎯 Projektübersicht

SupportChat ist ein Full-Stack IT-Support-System, das KI-gestützte Chatbots zur automatisierten Problemlösung einsetzt. Das System kombiniert moderne Webtechnologien mit künstlicher Intelligenz, um Benutzern bei IT-Problemen zu helfen und gleichzeitig Support-Tickets zu verwalten.

### Hauptfunktionen

- **KI-Chatbot**: Intelligente Konversationen durch n8n AI Agent mit ChatGPT
- **Ticket-Management**: Automatische Erstellung und Verwaltung von Support-Tickets
- **Session-basierter Chat**: Kontextbezogene Gespräche mit vollständiger Historie
- **Multi-Language**: Unterstützung für Deutsch und Englisch
- **User Authentication**: Rollenbasierte Zugriffssteuerung (SysAdmin, AppAdmin, AppUser)
- **Responsive UI**: Modernes Angular Frontend mit Bootstrap 5

---

## 🏗️ Architektur

### System-Architektur

┌─────────────────┐ │  Angular App    │ ◄──────┐ │  (Frontend)     │        │ └────────┬────────┘        │ │ HTTPS            │ ▼                  │ ┌─────────────────┐        │ │   ASP.NET Core  │        │ │   Web API       │        │ └────────┬────────┘        │ │                  │ ▼                  │ ┌─────────────────┐        │ │  EF Core        │        │ │  (Data Access)  │        │ └────────┬────────┘        │ │                  │ ▼                  │ ┌─────────────────┐   Webhook │  PostgreSQL /   │        │ │  SQLite /       │        │ │  SQL Server     │        │ └─────────────────┘        │ │ ┌─────────────────┐        │ │   n8n Workflow  │ ───────┘ │   AI Agent      │ └────────┬────────┘ │ ▼ ┌─────────────────┐ │   ChatGPT API   │ └─────────────────┘

### Datenfluss: Chat-Interaktion

1.	User sendet Nachricht (Angular) ↓
2.	PUT /api/SupportTickets/{ticketId}/messages ↓
3.	Backend speichert User-Message in DB ↓
4.	Backend ruft n8n Webhook auf ↓
5.	n8n lädt Chat-Historie aus DB (session-basiert) ↓
6.	n8n AI Agent verarbeitet Anfrage (ChatGPT) ↓
7.	n8n gibt Bot-Antwort zurück ↓
8.	Backend speichert Bot-Message in DB ↓
9.	Backend erstellt ChatResponse mit Metadaten ↓
10.	Response zurück an Frontend (synchron!)



---

## 🛠️ Technologie-Stack

### Backend (.NET 8)

| Komponente | Technologie | Zweck |
|------------|-------------|-------|
| **Framework** | ASP.NET Core 8 | Web API |
| **ORM** | Entity Framework Core | Datenbankzugriff |
| **Database** | PostgreSQL / SQLite / SQL Server | Datenpersistenz |
| **JSON** | Newtonsoft.Json | Serialisierung |
| **Security** | Custom Authentication | Session-basierte Auth |

### Frontend (Angular 18)

| Komponente | Technologie | Zweck |
|------------|-------------|-------|
| **Framework** | Angular 18 | SPA Framework |
| **UI** | Bootstrap 5 | Responsive Design |
| **Icons** | Bootstrap Icons | UI Icons |
| **i18n** | ngx-translate | Mehrsprachigkeit |
| **HTTP** | HttpClient | API-Kommunikation |

### External Services

| Service | Zweck |
|---------|-------|
| **n8n** | Workflow Automation & AI Agent |
| **OpenAI ChatGPT** | KI-gestützte Antworten |
| **PostgreSQL** | n8n Chat Memory (Session-basiert) |

### Desktop Apps (Optional)

| Komponente | Technologie |
|------------|-------------|
| **MVVM App** | AvaloniaUI / WPF |

---

## ✨ Features

### 1. Intelligenter Chatbot

- **Kontextbezogene Konversationen**: Session-basierte Chat-Historie
- **KI-gestützte Antworten**: Integration mit ChatGPT via n8n
- **Multi-Turn Dialoge**: Der Bot merkt sich vorherige Nachrichten
- **Systeminfo-Erkennung**: Automatische Erfassung von Browser/OS-Daten

### 2. Ticket-Management

- **Automatische Ticket-Erstellung**: Bei erster Nachricht
- **Status-Tracking**: Resolved / Unresolved
- **Priority-Levels**: Low, Medium, High, Critical
- **Troubleshooting-Steps**: Dokumentation durchgeführter Schritte
- **Zeiterfassung**: Start- und End-Zeit pro Ticket

### 3. Chat-API (Ticket-basiert)

**Empfohlener Endpoint**: `PUT /api/SupportTickets/{ticketId}/messages`

- ✅ Synchrone Bot-Antwort im Response
- ✅ Automatisches Ticket-Management
- ✅ Session-basierte KI-Integration
- ✅ Vollständige Metadaten (Response-Zeit, Token-Counts)

### 4. Benutzer-Authentifizierung

- **Rollenbasierte Zugriffskontrolle**:
  - `SysAdmin`: Vollzugriff + User-Management
  - `AppAdmin`: Ticket-Verwaltung
  - `AppUser`: Eigene Tickets
- **Session-Token-basiert**: Sichere API-Calls
- **Auto-Logout**: Bei Timeout

### 5. Frontend-Features

- **Responsive Design**: Desktop & Mobile
- **Dark/Light Mode**: Theme-Wechsel
- **Multi-Language**: DE/EN mit ngx-translate
- **Real-time Chat UI**: Moderne Chat-Oberfläche
- **Dashboard**: Ticket-Übersicht & Statistiken

---

## 📁 Projektstruktur

SupportChat/ │ ├── SupportChat.WebApi/              # ASP.NET Core Web API │   ├── Controllers/                 # API Controllers │   │   ├── App/                     # Business Controllers │   │   │   ├── SupportTicketsController.Custom.cs │   │   │   ├── ChatMessagesController.Custom.cs │   │   │   └── TicketStepsController.cs │   │   ├── AccountsController.cs    # Authentication │   │   └── GenericEntityController.cs │   ├── Models/                      # DTOs │   │   └── App/ │   ├── Docs/                        # API-Dokumentation │   │   ├── API_DOCUMENTATION.md │   │   ├── N8N_WEBHOOK_PAYLOAD.md │   │   └── TICKET_CHAT_API.md │   └── appsettings.Development.json │ ├── SupportChat.Logic/               # Business Logic Layer │   ├── Entities/                    # Domain Entities │   │   └── App/ │   │       ├── SupportTicket.cs │   │       ├── ChatMessage.cs │   │       ├── ChatResponse.cs │   │       └── TicketStep.cs │   ├── DataContext/                 # EF Core DbContext │   │   ├── ProjectDbContext.cs │   │   ├── EntitySet.cs │   │   └── App/ │   │       ├── SupportTicketSet.cs │   │       └── ChatMessageSet.Webhook.cs │   ├── Contracts/                   # Interfaces │   │   ├── IContext.cs │   │   └── IEntitySet.cs │   └── Modules/                     # Shared Modules │       ├── Account/                 # Authentication Logic │       └── Security/ │ ├── SupportChat.Common/              # Shared Library │   ├── Contracts/                   # Shared Interfaces │   ├── Enums/                       # Enumerations │   │   ├── TicketStatus.cs │   │   └── TicketPriority.cs │   └── Modules/ │       ├── Configuration/ │       └── RestApi/ │ ├── SupportChat.AngularApp/          # Angular Frontend │   ├── src/ │   │   ├── app/ │   │   │   ├── pages/ │   │   │   │   ├── chatbot/         # Chat-UI │   │   │   │   ├── dashboard/       # Dashboard │   │   │   │   ├── auth/            # Login/Logout │   │   │   │   └── chat-history/    # Ticket-Historie │   │   │   ├── services/            # Angular Services │   │   │   │   ├── chat.service.ts │   │   │   │   ├── auth.service.ts │   │   │   │   └── support-ticket.service.ts │   │   │   ├── interceptor/         # HTTP Interceptor │   │   │   └── models/              # TypeScript Interfaces │   │   └── assets/ │   │       └── i18n/                # Übersetzungen (DE/EN) │   │           ├── de.json │   │           └── en.json │   └── package.json │ ├── SupportChat.MVVMApp/             # AvaloniaUI Desktop App (Optional) │   ├── Views/ │   ├── ViewModels/ │   └── Models/ │ ├── SupportChat.ConApp/              # Console App (Testing) │ └── TemplateTools.*/                 # Code-Generierung (nicht Teil des Hauptprojekts)


---

## 🚀 Installation & Setup

### Voraussetzungen

- **.NET 8 SDK** ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- **Node.js 18+** & **npm** ([Download](https://nodejs.org/))
- **PostgreSQL 14+** (oder SQLite/SQL Server)
- **n8n** ([Self-hosted](https://docs.n8n.io/hosting/) oder [Cloud](https://n8n.io/))
- **OpenAI API Key** ([OpenAI Platform](https://platform.openai.com/))

### 1. Repository klonen
git clone https://github.com/riosarah/SupportChat.git cd SupportChat


### 2. Backend Setup (.NET)

#### 2.1 Datenbank konfigurieren

Bearbeite `SupportChat.WebApi/appsettings.Development.json`:



#### 2.2 Datenbank erstellen
cd SupportChat.Logic dotnet ef migrations add InitialCreate --startup-project ../SupportChat.WebApi dotnet ef database update --startup-project ../SupportChat.WebApi


#### 2.3 Backend starten

cd ../SupportChat.WebApi dotnet run


API läuft auf: `https://localhost:7074`

### 3. Frontend Setup (Angular)

#### 3.1 Dependencies installieren
cd SupportChat.AngularApp npm install


#### 3.2 API-URL konfigurieren (falls nötig)

`src/app/services/support-ticket.service.ts`:
private readonly baseUrl = 'https://localhost:7074/api';


#### 3.3 Frontend starten
npm start


Frontend läuft auf: `http://localhost:4200`

### 4. n8n Setup

#### 4.1 n8n installieren & starten

Docker
docker run -it --rm -p 5678:5678 -v ~/.n8n:/home/node/.n8n n8nio/n8n
Oder via npm
npx n8n


#### 4.2 Workflow erstellen

1. **Webhook Node** hinzufügen:
   - HTTP Method: `POST`
   - Path: `Itsupport-hook`

2. **Postgres Chat Memory** konfigurieren:
   - Connection String: (gleiche DB wie Backend!)
   - Session ID Expression: `{{ $json.sessionId }}`

3. **AI Agent Node** hinzufügen:
   - Model: OpenAI Chat Model
   - Memory: Postgres Chat Memory
   - System Prompt: (IT Support Persona)

4. **Respond to Webhook** Node:

{ "message": "{{ $json.output }}" }


---

## 📖 API-Dokumentation

### Base URL
https://localhost:7074/api



### Authentication

Alle Endpoints (außer Login) benötigen einen `Authorization` Header:

Authorization: Bearer {sessionToken}


### Wichtigste Endpoints

#### 🔐 Authentication
POST /api/Accounts/login Content-Type: application/json
{ "email": "appuser@gmx.at", "password": "1234AppUser" }
Response: { "sessionToken": "abc-123...", "identityId": 1, "name": "App User", "email": "appuser@gmx.at" }


#### 💬 Ticket-basierter Chat (Empfohlen!)
PUT /api/SupportTickets/{ticketId}/messages Authorization: Bearer {token} Content-Type: application/json
{ "content": "Mein Drucker druckt nicht", "identityId": 1, "systemInfo": "Windows 11, Chrome", "priority": 1 }
Response: { "id": 1, "responseContent": "Hallo! Ich helfe...", "supportTicketId": 1, "userMessageId": 123, "botMessageId": 124, "sessionId": 1, "isSuccessful": true }


#### 🎫 Tickets abrufen
GET /api/SupportTickets Authorization: Bearer {token}
Response: [ { "id": 1, "problemDescription": "Drucker Problem", "status": 1, "priority": 2, "chatMessages": [...] } ]


### Vollständige API-Dokumentation

Siehe: [`SupportChat.WebApi/Docs/API_DOCUMENTATION.md`](SupportChat.WebApi/Docs/API_DOCUMENTATION.md)

---

## 🤖 n8n Integration

### Webhook-Payload Format

Das Backend sendet folgende Daten an n8n:

{ "action": "Added", "id": 123, "content": "Mein Drucker druckt nicht", "isUserMessage": true, "timestamp": "2024-01-15T10:35:00Z", "supportTicketId": 1, "sessionId": "ticket-1", "ticketProblemDescription": "Drucker Problem", "ticketStatus": "Unresolved", "ticketPriority": "Medium", "ticketIdentityId": 3, "sentAt": "2024-01-15T10:35:01Z" }



### Wichtig: Session-basierte Chat-Memory

Der n8n AI Agent lädt die **gesamte Chat-Historie** aus der PostgreSQL-Datenbank über die `sessionId`:

// n8n Code Node const sessionId = $json.sessionId;  // "ticket-1" const ticketId = parseInt(sessionId.replace('ticket-', ''));
// Postgres Memory lädt automatisch alle Nachrichten zu diesem Ticket!


**Vorteil**: Kein Overhead im Webhook, konsistente Datenquelle, skalierbar.

### Detaillierte n8n-Dokumentation

Siehe: [`SupportChat.WebApi/Docs/N8N_WEBHOOK_PAYLOAD.md`](SupportChat.WebApi/Docs/N8N_WEBHOOK_PAYLOAD.md)

---

## 🎨 Frontend (Angular)

### Komponenten-Struktur
app/ ├── pages/ │   ├── chatbot/                 # Chat-Interface │   │   ├── chatbot.component.ts │   │   └── chatbot.component.html │   ├── dashboard/               # Ticket-Übersicht │   ├── chat-history/            # Historische Tickets │   └── auth/ │       └── login/               # Login-Seite ├── services/ │   ├── chat.service.ts          # Chat-API-Calls │   ├── auth.service.ts          # Authentication │   └── support-ticket.service.ts └── interceptor/ └── http-token-interceptor.ts  # Auto-Token-Injection


### Service-Beispiel: Chat
@Injectable({ providedIn: 'root' }) export class ChatService { private readonly baseUrl = 'https://localhost:7074/api';
sendMessage( ticketId: number, content: string, identityId: number ): Observable<ChatResponse> { return this.http.put<ChatResponse>( ${this.baseUrl}/SupportTickets/${ticketId}/messages, { content, identityId } ); } }


### Verwendung im Component

export class ChatbotComponent { sendMessage(text: string) { this.chatService.sendMessage( this.currentTicketId, text, this.authService.getIdentityId() ).subscribe({ next: (response) => { this.messages.push({ content: response.responseContent, isUserMessage: false }); } }); } }



---

## 🗄️ Datenbank

### Unterstützte Datenbanken

- **PostgreSQL** (empfohlen für Production)
- **SQLite** (gut für Development)
- **SQL Server**

### Entity-Modell

#### Kernentitäten

SupportTicket ├── Id (PK) ├── ProblemDescription ├── SystemInfo ├── Status (enum: Resolved, Unresolved) ├── Priority (enum: Low, Medium, High, Critical) ├── Recommendation ├── StartTime ├── EndTime ├── IdentityId (FK) ├── ChatMessages (1:n) └── TicketSteps (1:n)
ChatMessage ├── Id (PK) ├── Content ├── IsUserMessage ├── Timestamp └── SupportTicketId (FK)
TicketStep ├── Id (PK) ├── StepNumber ├── Description ├── ExecutedAt ├── WasSuccessful └── SupportTicketId (FK)
ChatResponse (Metadaten) ├── Id (PK) ├── ResponseContent ├── ModelUsed ├── PromptTokens ├── CompletionTokens ├── TotalTokens ├── ResponseTimeMs ├── IsSuccessful ├── SupportTicketId (FK) ├── UserMessageId (FK) └── BotMessageId (FK)


### Migrations
Neue Migration erstellen
dotnet ef migrations add MigrationName --project SupportChat.Logic --startup-project SupportChat.WebApi
Datenbank aktualisieren
dotnet ef database update --project SupportChat.Logic --startup-project SupportChat.WebApi
Migration rückgängig machen
dotnet ef database update PreviousMigration --project SupportChat.Logic --startup-project SupportChat.WebApi



---

## 🧪 Testing

### Test-Accounts

| Rolle | Email | Passwort |
|-------|-------|----------|
| System Admin | sysadmin@gmx.at | 1234SysAdmin |
| App Admin | appadmin@gmx.at | 1234AppAdmin |
| App User | appuser@gmx.at | 1234AppUser |

### API Testing mit cURL
Login
curl -X POST https://localhost:7074/api/Accounts/login 
-H "Content-Type: application/json" 
-d '{ "email": "appuser@gmx.at", "password": "1234AppUser" }'
Chat-Nachricht senden
curl -X PUT https://localhost:7074/api/SupportTickets/1/messages 
-H "Authorization: Bearer YOUR_SESSION_TOKEN" 
-H "Content-Type: application/json" 
-d '{ "content": "Test Nachricht", "identityId": 1 }'



### Frontend Testing

cd SupportChat.AngularApp npm test              # Unit Tests npm run e2e          # End-to-End Tests


---

## 🚢 Deployment

### Backend (ASP.NET Core)

#### 1. Build für Production

cd SupportChat.WebApi dotnet publish -c Release -o ./publish


#### 2. Docker (Optional)

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base WORKDIR /app EXPOSE 80 EXPOSE 443
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build WORKDIR /src COPY . . RUN dotnet restore RUN dotnet build -c Release -o /app/build
FROM build AS publish RUN dotnet publish -c Release -o /app/publish
FROM base AS final WORKDIR /app COPY --from=publish /app/publish . ENTRYPOINT ["dotnet", "SupportChat.WebApi.dll"]

docker build -t supportchat-api . docker run -d -p 7074:80 --name supportchat-api supportchat-api

### Frontend (Angular)

#### 1. Build für Production

cd SupportChat.AngularApp npm run build --prod

Output: `dist/support-chat-angular-app/`

#### 2. Deploy zu Nginx
server { listen 80; server_name your-domain.com;
root /var/www/supportchat;
index index.html;

location / {
    try_files $uri $uri/ /index.html;
}

location /api/ {
    proxy_pass https://localhost:7074;
    proxy_http_version 1.1;
    proxy_set_header Upgrade $http_upgrade;
    proxy_set_header Connection 'upgrade';
    proxy_set_header Host $host;
    proxy_cache_bypass $http_upgrade;
}
}


---

## 🧑‍💻 Entwicklung

### Branch-Strategie

- `master`: Production-ready Code
- `develop`: Development Branch
- `feature/*`: Feature-Branches
- `bugfix/*`: Bugfix-Branches

### Code-Conventions

- **C#**: Microsoft Coding Conventions
- **TypeScript**: Angular Style Guide
- **Commits**: Conventional Commits (`feat:`, `fix:`, `docs:`)

### Debugging

#### Backend

cd SupportChat.WebApi dotnet run --launch-profile https


Debugger in Visual Studio / VS Code anhängen.

#### Frontend


Chrome DevTools öffnen (`F12`)

---

## 📝 Weitere Dokumentation

- [API-Dokumentation](SupportChat.WebApi/Docs/API_DOCUMENTATION.md)
- [n8n Webhook-Integration](SupportChat.WebApi/Docs/N8N_WEBHOOK_PAYLOAD.md)
- [Ticket-Chat-API](SupportChat.WebApi/Docs/TICKET_CHAT_API.md)

---

## 🤝 Kontakt & Support

- **Repository**: [https://github.com/riosarah/SupportChat](https://github.com/riosarah/SupportChat)
- **Issues**: GitHub Issues verwenden

---

## 📄 Lizenz

Dieses Projekt ist ein Demonstrationprojekt um die Einbindung von n8n 
für einen It Support Chat Assistenten zu demonstrieren. 