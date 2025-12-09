# BulletHell - Space Shooter

A classic vertical scrolling space shooter built with MonoGame, featuring bullet hell gameplay, multi-layer parallax scrolling, and smooth animations.

## Features

- **Player-Controlled Spaceship**: Smooth 8-directional movement with automatic shooting
- **Wave-Based Enemy System**: Enemies spawn from the top and attack with bullets
- **Bullet Hell Combat**: Dodge incoming projectiles while destroying enemies
- **Health & Lives System**: 100 HP with 3 lives, respawn on death
- **Advanced Visual Effects**:
  - Multi-layer parallax scrolling backgrounds
  - Smooth camera system with 1.5x zoom
  - Explosion animations
  - Dynamic turn animations for the player ship
- **Score Tracking**: Point-based progression with persistent high scores
- **Optional Online Features**: User accounts and leaderboard (offline mode available)

## Screenshots

*Add gameplay screenshots here to showcase your game's visual effects and action*

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download) or Runtime
- OpenGL-capable graphics card
- Supported platforms: Windows, Linux, macOS

## Installation & Running

### Option A: Run Pre-built Release

1. Download the latest release from the releases page
2. Extract the archive
3. Run the `BulletHell` executable

### Option B: Build from Source

```bash
# Clone the repository
git clone <repository-url>
cd BulletHell

# Restore dependencies
dotnet restore

# Build and run
dotnet run --project BulletHell/BulletHell.csproj
```

Or simply:

```bash
cd BulletHell
dotnet run
```

## How to Play

**Objective**: Survive waves of enemies and achieve the highest score possible.

**Gameplay**:
- Control your spaceship and navigate through enemy formations
- Your ship automatically shoots when enemies are present
- Dodge incoming enemy bullets and avoid collisions
- Destroy enemies to earn points
- Collect power-ups and manage your health wisely

**Progression**: Score points by destroying enemies. Try to beat your high score!

**Game Over**: The game ends when you lose all three lives.

## Controls

| Action | Keyboard | Gamepad |
|--------|----------|---------|
| Move | Arrow Keys / WASD | Left Stick |
| Shoot | Automatic | Automatic |
| Menu Navigation | Arrow Keys + Enter | D-Pad + A |
| Pause/Back | Escape | Start/B |

## Game Mechanics

### Player
- **Health**: 100 HP per life
- **Lives**: 3 lives total
- **Movement Speed**: Smooth and responsive
- **Weapon**: Automatic firing with cooldown
- **Knockback**: Gets pushed back when hit by enemies

### Enemies
- **Spawn Location**: Top of the screen
- **Health**: 20 HP per enemy
- **Attack Pattern**: Shoot bullets at the player
- **Score Value**: 1 point per enemy destroyed

### Combat
- **Player Bullets**: 10 damage per hit
- **Enemy Bullets**: 10 damage per hit
- **Collision Damage**: 25 damage when hitting enemies directly
- **Visual Feedback**: Explosion animations on enemy destruction

### Visual Systems
- **Camera**: Smooth following camera with 1.5x zoom for better visibility
- **Parallax Backgrounds**: Four layers of scrolling space backgrounds at different speeds
- **Effects**: Dash trails, turn animations, and explosion sprite sheets

## Building from Source

### Prerequisites Installation

1. **Install .NET 9.0 SDK**: Download from [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/9.0)
2. **Verify Installation**:
   ```bash
   dotnet --version
   ```

### Build Configurations

**Debug Build** (default):
```bash
dotnet build
```

**Release Build** (optimized):
```bash
dotnet build -c Release
```

### Using IDEs

**Visual Studio**:
1. Open `BulletHellSpaceShooter.sln`
2. Set `BulletHell` as the startup project
3. Press F5 to build and run

**JetBrains Rider**:
1. Open `BulletHellSpaceShooter.sln`
2. Select `BulletHell` configuration
3. Click Run or press Shift+F10

### Content Pipeline

The game uses MonoGame's Content Pipeline to process assets:
- Content files are defined in `Content/Content.mgcb`
- Assets are automatically built during compilation
- Processed content is output to `Content/bin/DesktopGL/`

## Project Structure

```
BulletHell/
├── BulletHell/           # Main game project
│   ├── Content/          # Game assets (sprites, audio, fonts)
│   ├── Scenes/           # Game states (Menu, Battle, GameOver)
│   ├── Models/           # Game entities (Player, Enemy, Bullet)
│   ├── Managers/         # Game systems (Collision, Enemy, Bullet)
│   ├── UI/               # User interface components
│   ├── Graphics/         # Camera and rendering systems
│   ├── Configurations/   # Game balance and settings
│   └── Services/         # External services and utilities
└── BulletHell.test/      # Unit tests
```

## Technologies

- **MonoGame 3.8** - Cross-platform 2D game framework
- **.NET 9.0** - Modern C# runtime
- **BCrypt.Net** - Password hashing for user authentication
- **StyleCop** - Code quality analysis

## Development

### Running Tests

```bash
dotnet test
```

### Code Style

This project uses StyleCop for code analysis and enforces consistent coding standards.

### Solution Structure

- **Main Solution**: `BulletHellSpaceShooter.sln`
- **Game Project**: `BulletHell/BulletHell.csproj`
- **Test Project**: `BulletHell.test/BulletHell.test.csproj`

## Contributing

Contributions are welcome! Please follow these guidelines:
- Follow the existing code style (StyleCop enforced)
- Write unit tests for new features
- Update documentation as needed

## License

*Add your license information here*

## Credits

- Built with [MonoGame](https://www.monogame.net/)
- Game developed by [Your Name/Team]

## Troubleshooting

### Audio Issues
If you experience audio problems, the game will gracefully degrade and continue without sound.

### Graphics Issues
Ensure your system has OpenGL support. Update your graphics drivers if needed.

### .NET Version
Make sure you have .NET 9.0 installed. Check with:
```bash
dotnet --version
```

---

**Enjoy playing BulletHell!** Try to beat the high score and climb the leaderboard.
