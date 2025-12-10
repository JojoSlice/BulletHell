# BulletHell Space Shooter

Ett klassiskt bullet hell space shooter-spel med en REST API backend för highscore-hantering. Projektet är byggt med MonoGame och ASP.NET Core enligt Clean Architecture-principer.

## Översikt

Detta repository innehåller två huvudkomponenter:

1. **BulletHell** - Ett action-packat space shooter-spel med bullet hell-gameplay, vågbaserade fiender, parallax scrolling och explosion-animationer
2. **Api** - En REST API för användarhantering och highscore-funktionalitet

Båda komponenterna kan köras oberoende av varandra. Spelet kan spelas offline utan API:n, men highscore-funktionalitet kräver att API:n körs.

## Snabbstart

### Köra spelet

```bash
cd BulletHell
dotnet run
```

Se [BulletHell/README.md](BulletHell/README.md) för detaljerad information om spelmekanik, kontroller och funktioner.

### Köra API:n

```bash
cd Api
dotnet run
```

Se [Api/README.md](Api/README.md) för API-dokumentation, endpoints och användning.

## Förutsättningar

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- OpenGL-kompatibelt grafikkort (för spelet)
- Linux, macOS eller Windows

## Projektstruktur

```
BulletHellSpaceShooter/
├── BulletHell/              # 🎮 MonoGame-baserat space shooter-spel
│   ├── Scenes/              # Spelscener (Menu, Battle, GameOver)
│   ├── Models/              # Spelentiteter (Player, Enemy, Bullet)
│   ├── Managers/            # Spelsystem (Collision, Enemy, Bullet)
│   ├── Graphics/            # Kamera och renderingssystem
│   ├── UI/                  # Användargränssnitt
│   ├── Content/             # Spelresurser (sprites, ljud, fonts)
│   └── README.md            # Detaljerad speldokumentation
│
├── Api/                     # 🌐 REST API för highscores och användare
│   ├── Endpoints/           # API-endpoints
│   │   ├── UserEndpoints.cs
│   │   └── HighScoreEndpoints.cs
│   └── README.md            # API-dokumentation och endpoints
│
├── Application/             # 💼 Affärslogik och services
│   ├── Services/            # Service-implementationer
│   ├── Interfaces/          # Service-interfaces
│   └── Mapping/             # Objektmappning
│
├── Domain/                  # 📦 Domänmodeller
│   └── Entities/            # User, HighScore
│
├── Repository/              # 💾 Dataåtkomstlager
│   ├── Data/                # DbContext
│   ├── Interfaces/          # Repository-interfaces
│   └── Repositories/        # Repository-implementationer
│
├── Contracts/               # 📋 API-kontrakt
│   ├── Requests/            # Request DTOs
│   └── Responses/           # Response DTOs
│
├── BulletHell.test/         # ✅ Speltester
└── Api.test/                # ✅ API-tester
```

## Teknologier

### Spel (BulletHell)
- **MonoGame 3.8** - Cross-platform 2D-spelramverk
- **.NET 9.0** - Modern C# runtime
- **BCrypt.Net** - Lösenordshashning
- **StyleCop** - Kodkvalitetsanalys

### Backend (Api)
- **ASP.NET Core 9.0** - Web API-ramverk med Minimal APIs
- **Entity Framework Core** - ORM
- **SQLite** - Lokal databas
- **OpenAPI/Swagger** - API-dokumentation

### Arkitektur
- **Clean Architecture** - Separation av concerns med tydliga lager
- **Dependency Injection** - För testbarhet och loose coupling
- **Repository Pattern** - Abstraktion för dataåtkomst
- **xUnit** - Testramverk

## Installation och byggning

### Klona och bygg hela projektet

```bash
# Klona repository
git clone <repository-url>
cd BulletHell

# Återställ dependencies
dotnet restore

# Bygg alla projekt
dotnet build

# Kör alla tester
dotnet test
```

### Byggkonfigurationer

**Debug** (standardläge med debuginformation):
```bash
dotnet build
```

**Release** (optimerad för produktion):
```bash
dotnet build -c Release
```

## Användning

### Spela spelet

```bash
cd BulletHell
dotnet run
```

**Kontroller:**
- Piltangenter / WASD - Rörelse
- Automatisk skjutning
- Escape - Pausa/Tillbaka

För fullständig spelguide, se [BulletHell/README.md](BulletHell/README.md)

### Använda API:n

```bash
cd Api
dotnet run
```

API:n kommer att vara tillgänglig på `https://localhost:5001`

**Huvudendpoints:**
- `/api/users` - Användarhantering
- `/api/highscores` - Highscore-hantering
- `/openapi/v1.json` - OpenAPI-specifikation (endast development)

För fullständig API-dokumentation, se [Api/README.md](Api/README.md)

## Testing

Kör alla tester:
```bash
dotnet test
```

Kör specifika testprojekt:
```bash
dotnet test BulletHell.test/BulletHell.test.csproj
dotnet test Api.test/Api.test.csproj
```

Med kodtäckning:
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Arkitektur och designprinciper

### Clean Architecture

Projektet följer Clean Architecture med tydlig separation mellan lager:

1. **Presentation Layer** (BulletHell, Api) - UI och API-endpoints
2. **Application Layer** (Application) - Affärslogik och use cases
3. **Domain Layer** (Domain, Contracts) - Domänmodeller och kontrakt
4. **Infrastructure Layer** (Repository) - Dataåtkomst och externa tjänster

### Dependency Flow

```
BulletHell ──┐
             ├──> Application ──> Domain
Api ─────────┘                      ↑
                                    │
Repository ─────────────────────────┘
```

### Designmönster

- **Dependency Injection** - All beroende hantering
- **Repository Pattern** - Abstraktion för datalagring
- **Service Layer Pattern** - Affärslogik isolerad från presentation
- **Scene Pattern** - Spelstatus-hantering (Menu, Battle, GameOver)

## Databas

Projektet använder SQLite för lokal datalagring. Databasen skapas automatiskt vid första körningen av API:n.

**Tabeller:**
- **Users** - Användarinformation med hashade lösenord
- **HighScores** - Poängrekord kopplade till användare

Databasfilen lagras enligt `MyDbContext.GetDefaultDatabasePath()`.

## Utveckling

### Kodstil

- **StyleCop Analyzers** - Kodkvalitet enforced vid build
- **Nullable Reference Types** - Aktiverat i alla projekt
- **.editorconfig** - Enhetlig formattering
- **C# 13** / **.NET 9.0** - Moderna språkfunktioner

### Lägga till funktionalitet

**För spelet:**
1. Se utvecklingsguiden i [BulletHell/README.md](BulletHell/README.md)

**För API:n:**
1. Se utvecklingsguiden i [Api/README.md](Api/README.md)

### IDE-stöd

**Visual Studio:**
- Öppna `BulletHellSpaceShooter.sln`
- Sätt önskat startprojekt (BulletHell eller Api)

**JetBrains Rider:**
- Öppna `BulletHellSpaceShooter.sln`
- Full IntelliSense och debugging-stöd

**Visual Studio Code:**
- Installera C# Dev Kit
- Använd `dotnet run` för att starta projekt

## Bidra

1. Forka projektet
2. Skapa en feature branch (`git checkout -b feature/AmazingFeature`)
3. Följ befintlig kodstil (StyleCop enforced)
4. Skriv tester för ny funktionalitet
5. Committa dina ändringar (`git commit -m 'Add some AmazingFeature'`)
6. Pusha till branchen (`git push origin feature/AmazingFeature`)
7. Öppna en Pull Request

## Felsökning

### Spelet startar inte
- Kontrollera att .NET 9.0 SDK är installerat: `dotnet --version`
- Verifiera att OpenGL-drivrutiner är uppdaterade
- Kör `dotnet clean` följt av `dotnet build`

### API:n svarar inte
- Kontrollera att rätt port används (standard: 5001 för HTTPS)
- Verifiera att SQLite-databas kan skapas i målmappen
- Granska loggar för felmeddelanden

### Tester misslyckas
- Kör `dotnet restore` för att säkerställa alla dependencies
- Rensa tidigare builds: `dotnet clean`
- Kontrollera att inga andra instanser av API:n körs (för API-tester)

## License

*Lägg till licensinformation här*

---

**För detaljerad dokumentation:**
- 🎮 Spel: [BulletHell/README.md](BulletHell/README.md)
- 🌐 API: [Api/README.md](Api/README.md)
