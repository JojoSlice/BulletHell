# BulletHell API

REST API för BulletHell-spelet som hanterar användare och highscores.

## Teknisk Stack

- **Framework**: ASP.NET Core 9.0 med Minimal APIs
- **Databas**: SQLite med Entity Framework Core
- **Arkitektur**: Clean Architecture med separation mellan API, Application och Repository-lager
- **Kodkvalitet**: StyleCop Analyzers för kodstilenforcement
- **API Dokumentation**: OpenAPI/Swagger (endast i development)

## Projektstruktur

```
Api/
├── Endpoints/
│   ├── HighScoreEndpoints.cs    # Endpoints för highscore-hantering
│   └── UserEndpoints.cs          # Endpoints för användarhantering
├── Program.cs                    # Applikationens entry point
├── appsettings.json             # Konfiguration
└── Api.csproj                   # Projektfil
```

## Förutsättningar

- .NET 9.0 SDK
- SQLite (inkluderas automatiskt)

## Köra API:n

```bash
cd ~/dev/BulletHell/Api
dotnet run
```

API:n kommer att starta på:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`

### OpenAPI/Swagger

I development-läge kan du komma åt OpenAPI-specifikationen på:
```
https://localhost:5001/openapi/v1.json
```

## Databas

API:n använder SQLite med automatisk databasgenerering. Om databasen inte finns kommer den att skapas automatiskt vid första körningen via `EnsureCreated()`.

Databasfilen skapas på standardplatsen som definieras i `MyDbContext.GetDefaultDatabasePath()`.

## API Endpoints

Alla endpoints returnerar ett standardiserat svar-format:

```json
{
  "isSuccess": true,
  "data": { ... }
}
```

### User Endpoints

#### GET /api/users
Hämta alla användare.

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": 1,
      "userName": "player1"
    }
  ]
}
```

#### GET /api/users/{id}
Hämta en specifik användare via ID.

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "id": 1,
    "userName": "player1"
  }
}
```

#### POST /api/users
Skapa en ny användare.

**Request Body:**
```json
{
  "userName": "player1",
  "passwordHash": "hashedpassword123"
}
```

**Validering:**
- `userName`: Max 50 tecken

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "id": 1,
    "userName": "player1"
  }
}
```

#### PUT /api/users
Uppdatera en befintlig användare.

**Request Body:**
```json
{
  "id": 1,
  "userName": "player1_updated",
  "passwordHash": "newhashedpassword"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "id": 1,
    "userName": "player1_updated"
  }
}
```

#### DELETE /api/users/{id}
Ta bort en användare.

**Response:**
```json
{
  "isSuccess": true,
  "data": "User deleted successfully"
}
```

#### POST /api/users/login
Logga in en användare.

**Request Body:**
```json
{
  "userName": "player1",
  "password": "password123"
}
```

**Validering:**
- `userName`: Max 50 tecken

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "id": 1,
    "userName": "player1"
  }
}
```

### HighScore Endpoints

#### GET /api/highscores
Hämta alla highscores.

**Response:**
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": 1,
      "score": 10000,
      "userId": 1,
      "userName": "player1"
    }
  ]
}
```

#### GET /api/highscores/{id}
Hämta en specifik highscore via ID.

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "id": 1,
    "score": 10000,
    "userId": 1,
    "userName": "player1"
  }
}
```

#### POST /api/highscores
Skapa en ny highscore.

**Request Body:**
```json
{
  "score": 10000,
  "userId": 1
}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "id": 1,
    "score": 10000,
    "userId": 1,
    "userName": "player1"
  }
}
```

#### PUT /api/highscores
Uppdatera en befintlig highscore.

**Request Body:**
```json
{
  "id": 1,
  "score": 15000,
  "userId": 1
}
```

**Response:**
```json
{
  "isSuccess": true,
  "data": {
    "id": 1,
    "score": 15000,
    "userId": 1,
    "userName": "player1"
  }
}
```

#### DELETE /api/highscores/{id}
Ta bort en highscore.

**Response:**
```json
{
  "isSuccess": true,
  "data": "HighScore deleted successfully"
}
```

## Beroenden

API-projektet refererar till följande interna projekt:
- **Application**: Innehåller affärslogik och service-interfaces
- **Contracts**: Innehåller request/response-modeller och API-endpoint-definitioner
- **Repository**: Hanterar databasåtkomst

## HTTP Test File

En `Api.http` fil finns tillgänglig för att testa endpoints direkt från VS Code eller andra verktyg som stödjer HTTP-filer.

## Utveckling

### Kodstil

Projektet använder StyleCop Analyzers för att upprätthålla enhetlig kodstil. Vissa regler är avstängda via `NoWarn` i projektfilen.

### Testing

API:n exponerar en partial `Program`-klass för att möjliggöra integration testing med `WebApplicationFactory`.

## Säkerhet

- API:n använder HTTPS-redirection
- Lösenord hashas innan de lagras (hanteras av Application-lagret)
- Konfigurationen tillåter alla hosts (`"AllowedHosts": "*"`) - bör begränsas i produktion
