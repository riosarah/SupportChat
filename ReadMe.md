# SupportChat – IT Support Chatbot System

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
- [Testing](#testing)
- [Deployment](#deployment)

---

## 🎯 Projektübersicht

SupportChat ist ein Full-Stack IT-Support-System mit KI-gestütztem Chatbot zur automatisierten Problemlösung und Ticketverwaltung.

### Hauptfunktionen

- **KI-Chatbot** via n8n AI Agent & ChatGPT
- **Ticket-Management**
- **Session-basierter Chat**
- **Mehrsprachigkeit (DE/EN)**
- **Rollenbasierte Authentifizierung**
- **Modernes Angular UI**

---

## 🏗️ Architektur

### System-Architektur

```text
┌─────────────────┐
│  Angular App    │
│  (Frontend)     │
└────────┬────────┘
         │ HTTPS
         ▼
┌─────────────────┐
│ ASP.NET Core    │
│ Web API         │
└────────┬────────┘
         ▼
┌─────────────────┐
│ EF Core         │
│ Data Access     │
└────────┬────────┘
         ▼
┌─────────────────────────────┐
│ PostgreSQL / SQLite /       │
│ SQL Server                  │
└─────────────────────────────┘
         ▲
         │ Webhook
┌────────┴────────┐
│ n8n Workflow    │
│ AI Agent        │
└────────┬────────┘
         ▼
┌─────────────────┐
│ ChatGPT API     │
└─────────────────┘
```
Datenfluss: Chat-Interaktion
User sendet Nachricht (Angular)

PUT /api/SupportTickets/{ticketId}/messages

Backend speichert User-Message

Webhook-Aufruf an n8n

n8n lädt Chat-Historie (Session)

AI Agent verarbeitet Anfrage

Bot-Antwort an Backend

Speicherung der Bot-Message

Response an Frontend (synchron)

🛠️ Technologie-Stack
Backend (.NET 8)
Komponente	Technologie
Framework	ASP.NET Core 8
ORM	Entity Framework Core
Datenbank	PostgreSQL / SQLite / SQL Server
Security	Session-basierte Auth

Frontend (Angular 18)
Komponente	Technologie
Framework	Angular 18
UI	Bootstrap 5
i18n	ngx-translate

📁 Projektstruktur
``` text
Copy code
SupportChat/
├── SupportChat.WebApi/
│   ├── Controllers/
│   ├── Models/
│   ├── Docs/
│   └── appsettings.Development.json
│
├── SupportChat.Logic/
│   ├── Entities/
│   ├── DataContext/
│   ├── Contracts/
│   └── Modules/
│
├── SupportChat.Common/
│   ├── Enums/
│   └── Modules/
│
├── SupportChat.AngularApp/
│   └── src/app/
│
├── SupportChat.MVVMApp/
├── SupportChat.ConApp/
└── TemplateTools.*/

```
🚀 Installation & Setup
Voraussetzungen
.NET 8 SDK

Node.js 18+

PostgreSQL / SQLite

n8n

OpenAI API Key

Repository klonen
```
bash
Copy code
git clone https://github.com/riosarah/SupportChat.git
cd SupportChat
```
Backend starten
```
bash
Copy code
cd SupportChat.WebApi
dotnet run
API: https://localhost:7074
```
Frontend starten
```
bash
Copy code
cd SupportChat.AngularApp
npm install
npm start
Frontend: http://localhost:4200
```

📖 API-Dokumentation
Login
http
Copy code
POST /api/Accounts/login
Content-Type: application/json
``` json
Copy code
{
  "email": "appuser@gmx.at",
  "password": "1234AppUser"
}
Ticket-Chat (Empfohlen)
http
Copy code
PUT /api/SupportTickets/{ticketId}/messages
Authorization: Bearer {token}
json
Copy code
{
  "content": "Mein Drucker druckt nicht",
  "identityId": 1
}
```

🗄️ Datenbank
Kernentitäten
text
``` Copy code
SupportTicket
├── Id
├── ProblemDescription
├── Status
├── Priority
├── IdentityId
├── ChatMessages (1:n)
└── TicketSteps (1:n)

ChatMessage
├── Id
├── Content
├── IsUserMessage
└── SupportTicketId
```

🧪 Testing
Test-Accounts
```
Rolle	Email	Passwort
SysAdmin	sysadmin@gmx.at	1234SysAdmin
AppAdmin	appadmin@gmx.at	1234AppAdmin
AppUser	appuser@gmx.at	1234AppUser
```

🚢 Deployment
Backend Build
```
bash
Copy code
dotnet publish -c Release
```
Frontend Build
```
bash
Copy code
npm run build --prod
```

📄 Lizenz
Demonstrationsprojekt zur Integration von n8n als KI-gestützter IT-Support-Chatbot.
