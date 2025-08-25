# Map Layout System - NESWC Hub Design

## Overview

ZombieStorm now uses a **hub-and-spoke map layout** where the player starts at the **Center** and defends against zombies coming from the 4 surrounding areas (**North, East, South, West**). Walls separate each area, and the wave system controls which areas spawn zombies.

## Map Areas

### Area Layout

```
        [NORTH]
           |
   [WEST]--[CENTER]--[EAST]
           |
       [SOUTH]
```

### Area Definitions

| Area       | Code                 | Purpose                | Player Access     |
| ---------- | -------------------- | ---------------------- | ----------------- |
| **Center** | `SpawnerArea.Center` | Player spawn/safe zone | Always accessible |
| **North**  | `SpawnerArea.North`  | Spawn area             | Via North Wall    |
| **East**   | `SpawnerArea.East`   | Spawn area             | Via East Wall     |
| **South**  | `SpawnerArea.South`  | Spawn area             | Via South Wall    |
| **West**   | `SpawnerArea.West`   | Spawn area             | Via West Wall     |

## Wall System

### Wall Components

- **4 Walls Total**: NorthWall, EastWall, SouthWall, WestWall
- Each wall connects **Center** to one spawn area
- Walls can be **opened/closed** dynamically per wave
- Optional: Destructible walls with health system

### Wall Positions

- `WallPosition.NorthWall` - Between Center and North
- `WallPosition.EastWall` - Between Center and East
- `WallPosition.SouthWall` - Between Center and South
- `WallPosition.WestWall` - Between Center and West

### Wall States

- **Closed (Active)**: Blocks movement, zombies can't pass
- **Open (Inactive)**: Allows movement between areas
- **Destroyed**: Permanently open until repaired

## Unity Setup Instructions

### Step 1: Manager Setup (Foundation)

1. **Create Managers GameObject**:

   - Create empty GameObject named "Managers"
   - Position at (0, 0, 0)

2. **Add WaveManager**:

   - Add `WaveManager.cs` script to Managers GameObject
   - This will automatically find all spawners in scene
   - Leave spawner list empty (auto-populated)

3. **Add WallManager**:

   - Add `WallManager.cs` script to Managers GameObject
   - Check `Control Walls With Waves` ✓
   - Check `Open Walls For Active Spawners` ✓
   - Leave wall list empty (auto-populated)

4. **Add PlayerSpawnManager**:
   - Add `PlayerSpawnManager.cs` script to Managers GameObject
   - Set `Center Position` to (0, 0, 0) or your map center
   - Set `Spawn Radius` to 2.0
   - Check `Find Player Automatically` ✓
   - Check `Use Transform Position` ✓

### Step 2: Map Areas Setup

1. **Create Area Parent Objects**:

   ```
   Scene Hierarchy:
   └── Areas
       ├── CenterArea (position: 0, 0, 0)
       ├── NorthArea (position: 0, 0, 20)
       ├── EastArea (position: 20, 0, 0)
       ├── SouthArea (position: 0, 0, -20)
       └── WestArea (position: -20, 0, 0)
   ```

2. **Build Area Geometry**:
   - Add floor/ground objects to each area
   - Create visual boundaries (optional)
   - Ensure clear paths between areas
   - Center area should be easily defensible

### Step 3: Spawner Setup

1. **Create Spawners in Each Area**:

   ```
   Areas/NorthArea/
   └── Spawner_North_01 (add Spawner.cs)
   └── Spawner_North_02 (add Spawner.cs)

   Areas/EastArea/
   └── Spawner_East_01 (add Spawner.cs)

   Areas/SouthArea/
   └── Spawner_South_01 (add Spawner.cs)
   └── Spawner_South_02 (add Spawner.cs)

   Areas/WestArea/
   └── Spawner_West_01 (add Spawner.cs)
   ```

2. **Configure Each Spawner**:
   - **Spawner Area**: Set dropdown to match area (North/East/South/West)
   - **Default Enemy Prefab**: Assign your zombie prefab
   - **Default Spawn Count**: 3
   - **Default Spawn Interval**: 0.6
   - Position spawners at area edges facing toward center

### Step 4: Wall Setup

1. **Create Wall Objects**:

   ```
   Scene Hierarchy:
   └── Walls
       ├── Wall_North (between Center and North areas)
       ├── Wall_East (between Center and East areas)
       ├── Wall_South (between Center and South areas)
       └── Wall_West (between Center and West areas)
   ```

2. **Position Walls**:

   - **Wall_North**: Position at (0, 0, 10)
   - **Wall_East**: Position at (10, 0, 0)
   - **Wall_South**: Position at (0, 0, -10)
   - **Wall_West**: Position at (-10, 0, 0)

3. **Configure Wall Components**:

   - Add `Wall.cs` script to each wall
   - **Wall Position**: Set dropdown (NorthWall, EastWall, etc.)
   - **Max Health**: 100 (if using destructible walls)
   - **Starts Active**: ✓ (walls start closed)
   - **Is Destructible**: ✓ or ✗ based on preference

4. **Wall Script Configuration**:
   - Your WallDoor already has all required components ✓
   - In the Wall.cs script inspector, assign references:
     - `Wall Mesh`: Drag the WallDoor GameObject itself (it has the Mesh Renderer)
     - `Wall Collider`: Drag your Box Collider component
     - `Nav Mesh Obstacle`: Drag your NavMeshObstacle component
   - **Verify NavMeshObstacle settings**:
     - `Carve` should be ✓ enabled
     - `Move Threshold`: 0.1
     - `Time To Stationary`: 0.5
     - `Carve Only Stationary`: ✓ enabled

### Step 5: Player Integration

1. **Ensure Player GameObject exists** in scene
2. **PlayerSpawnManager will auto-find** the Player component
3. **Test center spawning**:
   - Play scene
   - Player should spawn at center position
   - Check Console for spawn confirmation logs

### Step 6: Testing and Validation

1. **Check Manager Connections**:

   - Play scene and check Console logs
   - Should see: "Found X spawners in scene"
   - Should see: "WallManager found X walls in scene"
   - Should see: "Player spawned at center position"

2. **Verify Area Organization**:

   - Console should show spawner area assignments
   - Check: "Assigned spawner [name] to area [North/East/South/West]"

3. **Test Wave Progression**:
   - Wave 1 should only activate North spawners
   - North wall should open, others stay closed
   - Enemies should spawn from North area only

### Step 7: Debugging Setup

1. **Enable Gizmos** in Scene view:

   - Select PlayerSpawnManager → See center spawn visualization
   - Select Wall objects → See area connection lines
   - Select WaveManager → See spawner organization

2. **Console Monitoring**:
   - Watch for wave state changes
   - Monitor wall open/close events
   - Track spawner activation logs

### Common Setup Issues

- **Spawners not found**: Ensure Spawner.cs is attached and areas are set
- **Walls not responding**: Check WallManager is present and walls have Wall.cs
- **Player not centering**: Verify PlayerSpawnManager center position
- **Wrong areas spawning**: Double-check Spawner Area dropdown settings

### Scene Hierarchy Example

```
YourScene
├── Managers
│   ├── WaveManager
│   ├── WallManager
│   └── PlayerSpawnManager
├── Player (with Player.cs)
├── Areas
│   ├── CenterArea
│   ├── NorthArea
│   │   ├── Spawner_North_01
│   │   └── Spawner_North_02
│   ├── EastArea
│   │   └── Spawner_East_01
│   ├── SouthArea
│   │   ├── Spawner_South_01
│   │   └── Spawner_South_02
│   └── WestArea
│       └── Spawner_West_01
└── Walls
    ├── Wall_North
    ├── Wall_East
    ├── Wall_South
    └── Wall_West
```

## Wave Integration

### Area-Based Wave Patterns

The system includes predefined wave patterns:

| Wave         | Active Areas     | Description              |
| ------------ | ---------------- | ------------------------ |
| 1, 7, 13...  | North            | Single direction assault |
| 2, 8, 14...  | South            | Single direction assault |
| 3, 9, 15...  | North + South    | Two-front attack         |
| 4, 10, 16... | East + West      | Side attacks             |
| 5, 11, 17... | All sides (NESW) | Surrounded               |
| 6, 12, 18... | Center           | Boss/special waves       |

### Wall Behavior During Waves

- **Wave Start**: Only walls for active spawn areas open
- **Wave Active**: Walls remain in configured state
- **Wave Complete**: Optional wall closing between waves

## System Architecture

### Core Components

#### Wall.cs

- Individual wall behavior
- Health/destruction system
- Area linking
- Visual/collision control

#### WallManager.cs

- Coordinates all walls
- Wave integration
- Area access control
- Wall state management

#### PlayerSpawnManager.cs

- Center spawn positioning
- Spawn validation
- Player repositioning
- Integration with game systems

### Integration Points

#### WaveManager Integration

- WallManager subscribes to wave events
- Walls open/close based on active spawn areas
- Coordinated with spawner activation

#### GameManager Integration

- Player respawning on game restart
- Wall state management across game states

## Gameplay Flow

### Game Start

1. **Player spawns** at center position
2. **All walls closed** initially
3. **WaveManager starts** first wave
4. **Walls open** for active spawn areas only

### During Wave

1. **Zombies spawn** in active areas only
2. **Zombies move** through open walls toward center
3. **Player defends** from center position
4. **Walls block** inactive areas

### Wave Complete

1. **All enemies eliminated**
2. **Optional wall closing** between waves
3. **Next wave preparation**
4. **Wall reconfiguration** for new active areas

## Customization Options

### Wave Patterns

Modify `WallManager.GetActiveAreasForCurrentWave()` to create custom patterns:

- Single area waves
- Multi-directional attacks
- Progressive area opening
- Special event waves

### Wall Behavior

- **Destructible walls**: Enable health system
- **Repair mechanics**: Allow wall rebuilding
- **Timed walls**: Auto-close after delay
- **Progressive opening**: Gradual area access

### Spawn Distribution

- **Area-specific enemies**: Different zombie types per area
- **Weighted spawning**: More enemies from certain directions
- **Dynamic difficulty**: Area-based enemy scaling

## Debug Features

### Visual Debugging

- **Gizmos** show area connections and wall positions
- **Console logs** for wall state changes and area access
- **Inspector debugging** with real-time state display

### Runtime Testing

- **Context menu** commands to open/close walls
- **Manual area control** for testing
- **Spawn position validation** with visual feedback

## Performance Considerations

- **Efficient area queries** using dictionary lookups
- **Event-driven updates** rather than polling
- **Minimal GameObject activation/deactivation**
- **Cached wall references** for quick access

This system creates a strategic hub-defense gameplay where players must manage multiple directions of attack while being anchored to the center position, creating engaging tactical decisions about positioning and resource management.
