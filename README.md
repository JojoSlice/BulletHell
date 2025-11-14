# Bullet Hell Space Shooter

Ett bullet hell-spel byggt med MonoGame och .NET 9.0. Projektet följer clean architecture-principer med separation av concerns och dependency injection.

## Beskrivning

Ett klassiskt bullet hell space shooter-spel där spelaren styr ett rymdskepp och skjuter projektiler. Projektet är byggt med modern C#-arkitektur och inkluderar en REST API för highscore-hantering.

## Funktioner

- Spelarstyrning med tangentbord
- Skjutmekanik med projektiler
- Dependency injection för testbarhet
- REST API för highscore-hantering
- SQLite-databas för persistence
- Enhetstest med xUnit
- Clean architecture med separerade lager

## Teknologier

- **.NET 9.0** - Ramverk
- **MonoGame 3.8** - Spelutvecklingsramverk
- **MonoGame.Extended** - Utökad funktionalitet för MonoGame
- **ASP.NET Core** - Web API
- **Entity Framework Core** - ORM
- **SQLite** - Databas
- **xUnit** - Testramverk

## Projektstruktur

```
BulletHellSpaceShooter/
├── BulletHell/              # Huvudspelprojekt
│   ├── Models/              # Spelmodeller (Player, Bullet)
│   ├── Interfaces/          # Abstraktioner (IInputProvider, ISpriteHelper)
│   ├── Inputs/              # Input-providers
│   ├── Helpers/             # Hjälpklasser
│   ├── Configurations/      # Konfigurationsfiler
│   └── Content/             # Spelresurser (sprites, texturer)
├── Api/                     # REST API för highscores
│   └── Models/              # API-modeller
├── Domain/                  # Domänentiteter
│   └── Entities/            # User, HighScore
├── Repository/              # Datalager
│   ├── Data/                # DbContext och databas
│   ├── Interfaces/          # Repository-interfaces
│   └── Repositories/        # Repository-implementationer
├── BulletHell.test/         # Speltester
└── Api.test/                # API-tester
```

## Installation

### Förutsättningar

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Linux, macOS eller Windows

### Steg

1. Klona repositoryt:
```bash
git clone <repository-url>
cd bullethell
```

2. Återställ NuGet-paket:
```bash
dotnet restore
```

3. Bygg projektet:
```bash
dotnet build
```

## Användning

### Köra spelet

```bash
cd BulletHell
dotnet run
```

### Kontroller

- **Piltangenter** - Rörelse
- **Mellanslag** - Skjut
- **Escape** - Avsluta spelet

### Köra API:et

```bash
cd Api
dotnet run
```

API:et kommer att vara tillgängligt på `https://localhost:5001` (eller den port som anges i launchSettings.json).

### Köra tester

Kör alla tester:
```bash
dotnet test
```

Kör specifika testprojekt:
```bash
dotnet test BulletHell.test/BulletHell.test.csproj
dotnet test Api.test/Api.test.csproj
```

## Arkitektur

Projektet använder dependency injection för att möjliggöra testbarhet och loose coupling:

- **IInputProvider** - Abstraktion för input (tangentbord, gamepad, etc.)
- **ISpriteHelper** - Abstraktion för sprite-hantering
- **IRepository** - Abstraktion för dataaccess

### Huvudkomponenter

#### Game1.cs
Huvudspelklassen som hanterar:
- Game loop (Update/Draw)
- Spelarhantering
- Bullet-hantering
- Kollisionsdetektering

#### Player.cs
Spelarklass med:
- Rörelse baserad på input
- Skjutmekanik
- Position och hastighet

#### Bullet.cs
Projektilklass med:
- Rörelselogik
- Livstid
- Ritning

## Konfiguration

Spelkonfiguration finns i `Configurations/`-mappen:

- **PlayerConfig.cs** - Spelarinställningar
- **BulletConfig.cs** - Projektilsinställningar
- **SpriteDefaults.cs** - Standardvärden för sprites

## API Endpoints

API:et exponerar följande endpoints (exempel):

- `GET /weatherforecast` - Exempel-endpoint (kommer att ersättas med highscore-endpoints)

## Databas

Projektet använder SQLite för lokal datalagring. Databasen innehåller:

- **Users** - Användarinformation
- **HighScores** - Highscore-poster

## Utveckling

### Lägga till nya features

1. Skapa modeller i `BulletHell/Models/`
2. Definiera interfaces i `BulletHell/Interfaces/`
3. Implementera logik i lämpliga klasser
4. Uppdatera `Game1.cs` för att integrera nya features
5. Lägg till tester i `BulletHell.test/`

### Kodstil

Projektet använder:
- Nullable reference types aktiverat
- .NET 9.0 conventions
- Dependency injection pattern
- Repository pattern för dataaccess

## Testing

Projektet använder xUnit för enhetstester. Tester finns i:

- `BulletHell.test/` - Speltester
  - `Player.test.cs` - Spelartester
  - `InputHandlerTests.cs` - Input-tester
- `Api.test/` - API-tester

## Bidra

1. Forka projektet
2. Skapa en feature branch (`git checkout -b feature/AmazingFeature`)
3. Committa dina ändringar (`git commit -m 'Add some AmazingFeature'`)
4. Pusha till branchen (`git push origin feature/AmazingFeature`)
5. Öppna en Pull Request

---

*Genererad med hjälp av Jojo's README Generator MCP Server*
