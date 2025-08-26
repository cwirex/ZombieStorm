# Wave System & Shop Implementation Plan

## Project Overview

This document outlines the complete implementation plan for adding a dynamic wave system and shop functionality to ZombieStorm. The wave system will replace the current static spawners with dynamic, progressively difficult waves. Players will earn money to purchase weapon upgrades and ammo between waves.

## Current System Analysis

### Existing Components

- **Spawners** (`Spawner.cs`): Static spawners with fixed `spawnCount=3` and `spawnInterval=0.6f`
- **Weapons** (`WeaponManager.cs`): 8 weapon types (PISTOL, UZI, SHOTGUN, M4, AWP, M249, RPG7, FLAMETHROWER)
- **Ammo System** (`Ammo.cs`): Magazine-based system with reload mechanics
- **Score System** (`ScoreManager.cs`): Tracks kills and maintains leaderboard
- **Game States** (`GameManager.cs`): MainMenu, Playing, Paused, GameOver

### Current Limitations

- Spawners auto-start and spawn independently
- Fixed ammo amounts per weapon type
- No dynamic difficulty progression
- No economy or upgrade system
- No wave progression or objectives

## Phase 1: Wave System Foundation ✅ COMPLETED

### 1.1 WaveManager System ✅

**Implementation**: Complete centralized wave management system

**Implemented Features**:

- Wave progression tracking with configurable wave data
- Dynamic enemy count monitoring and wave completion detection
- Spawner coordination with automatic activation/deactivation
- Global wave progression configuration system
- Integration with GameManager state transitions
- Wave completion rewards and bonuses

**Key Implementation Details**:

- `WaveManager.cs`: Central controller managing all wave states and progression
- Global wave progression configuration using ScriptableObject pattern
- Real-time enemy tracking with automatic wave completion detection
- Seamless integration with existing GameManager and spawner systems
- Event-driven architecture for clean separation of concerns

### 1.2 Enhanced Spawner System ✅

**Implementation**: Complete spawner refactoring for external wave control

**Changes Made**:

- Removed automatic spawning from `Spawner.cs` Start() method
- Added external control interface for wave-based spawning
- Implemented dynamic spawn parameter configuration per wave
- Added spawner state management (enabled/disabled, spawning/complete)
- Created wave-specific spawn timing and enemy type support

**New Spawner Capabilities**:

- External wave initiation and control
- Dynamic spawn parameters (count, interval, enemy types)
- Spawning completion callbacks to WaveManager
- Multi-spawner coordination support
- Wave-specific enemy composition handling

### 1.3 Wave Configuration System ✅

**Implementation**: Comprehensive wave definition and progression system

**Global Wave Progression Config**:

- Centralized wave progression configuration using ScriptableObject
- Dynamic difficulty scaling with configurable parameters
- Wave-specific enemy compositions and spawn patterns
- Reward calculation system for wave completion bonuses
- Extensible configuration for future wave types and mechanics

**Wave Progression Features**:

- Configurable base enemy counts and spawn intervals
- Dynamic difficulty multipliers per wave
- Multiple enemy type support with spawn distribution
- Wave completion reward calculations
- Easy designer-friendly wave configuration through Unity Inspector

## Phase 2: Economy System

### 2.1 CurrencyManager

**Purpose**: Separate money system from existing score system

**Core Features**:

- Track player money independently from `ScoreManager.cs`
- Award money for enemy kills (different rates than score)
- Wave completion bonuses
- Purchase transaction handling

**Money Sources**:

- Enemy kills: Base amount + type multiplier
- Wave completion: Escalating bonus per wave
- Perfect wave bonus: No damage taken
- Survival bonus: Time-based rewards

**Integration with ScoreManager**:

- Subscribe to same enemy death events
- Parallel money/score calculation
- Maintain separate leaderboards for money earned

### 2.2 Shop System Architecture

**ShopManager Responsibilities**:

- Manage shop state and availability
- Handle purchase transactions
- Validate player funds
- Apply upgrades to weapons
- Save/load purchase history

**Shop Categories**:

1. **Weapon Upgrades**:

   - Damage multipliers
   - Fire rate improvements
   - Magazine capacity increases
   - Range/accuracy enhancements

2. **Ammo Purchases**:

   - Bulk ammo for all non-pistol weapons
   - Ammo capacity upgrades
   - Special ammunition types

3. **Player Enhancements** (Future):
   - Health upgrades
   - Movement speed
   - Reload speed bonuses

**Pricing Strategy**:

- Escalating costs per upgrade tier
- Weapon-specific pricing (pistol cheapest, RPG most expensive)
- Wave-based price inflation

## Phase 3: Enhanced Ammo System

### 3.1 Weapon Enhancement Framework

**Current System**: Fixed ammo amounts in `WeaponManager.cs:112-133`

**Enhancement Tiers**:

- Tier 0: Base weapon (current stats)
- Tier 1-5: Progressive improvements per category
- Each tier increases: damage, ammo capacity, magazine size

**Persistent Upgrades**:

- Maintain upgrade levels between waves
- Save upgrades between game sessions
- Visual indicators for upgrade levels

**Pistol Special Case**:

- Remove ammo limitations from `Ammo.cs`
- Infinite ammo for pistol (encourages weapon switching)
- Still allow pistol damage/fire rate upgrades

### 3.2 Wave Start Ammo Management

**Wave Transition Behavior**:

- Reset all magazines to full capacity
- Restore ammo pools based on purchased upgrades
- Apply any ammo capacity bonuses from shop purchases

**Ammo Calculation Formula**:

```
MaxAmmo = BaseAmmo * (1 + UpgradeTier * 0.5) + PurchasedBulkAmmo
MagazineCapacity = BaseMagazine * (1 + CapacityUpgrades * 0.25)
```

## Phase 4: Game Flow Integration

### 4.1 GameState Expansion

**New GameState**: Add `BetweenWaves` to `GameManager.cs:4-9`

**State Transitions**:

- Playing → BetweenWaves (wave completed)
- BetweenWaves → Playing (wave started)
- BetweenWaves → GameOver (player chooses to quit)

**BetweenWaves Behavior**:

- Pause game timer
- Enable shop interface
- Display wave completion rewards
- Show wave statistics (kills, accuracy, time)
- Countdown timer or manual start for next wave

### 4.2 UI System Integration

**New UI Components**:

- Wave counter and progress bar
- Money display (separate from score)
- Shop interface with upgrade trees
- Wave completion celebration screen

**UI Layout Modifications**:

- Top HUD: Wave number, money, score
- Shop Screen: Weapon grid, upgrade options, purchase buttons
- Between-wave overlay: Statistics, rewards, shop access

**Integration Points**:

- Extend `UIController.cs` for shop functionality
- Update `Ammo.UpdateUI()` for enhanced display
- Add wave progression indicators

## Phase 5: Detailed Wave Management System

### 5.1 Wave State Machine

**Wave States**:

- `Preparing`: Setting up spawners, loading enemy types
- `Active`: Spawning enemies and combat
- `Cleanup`: Waiting for remaining enemies to die
- `Complete`: All enemies eliminated, calculating rewards
- `Transition`: Moving to next wave or shop phase

### 5.2 Enemy Tracking System

**WaveManager Enemy Monitoring**:

- Subscribe to `Enemy.Die()` events
- Maintain active enemy counter
- Track enemy types killed per wave
- Monitor spawner completion status

**Wave Completion Logic**:

```csharp
public bool IsWaveComplete()
{
    return GetActiveEnemyCount() == 0 && AllSpawnersComplete();
}

private int GetActiveEnemyCount()
{
    return FindObjectsOfType<Enemy>().Length;
}
```

### 5.3 Dynamic Difficulty Scaling

**Per-Wave Scaling Factors**:

- Enemy health: `BaseHealth * (1 + wave * 0.15)`
- Enemy damage: `BaseDamage * (1 + wave * 0.1)`
- Spawn rate: `BaseInterval * (1 - wave * 0.05)` (faster spawning)
- Enemy count: `BaseCount + (wave * 2)`

**Special Wave Events**:

- Boss waves (every 5th wave): Single powerful enemy
- Horde waves (every 7th wave): 2x normal enemy count
- Mixed waves: Multiple enemy types with different spawn timing

### 5.4 Spawner Coordination

**Multi-Spawner Management**:

- Activate spawners in sequence or simultaneously
- Coordinate spawn timing to avoid overwhelming player
- Balance enemy distribution across multiple spawn points
- Handle spawner-specific enemy type assignments

**Spawner Communication Protocol**:

```csharp
interface ISpawnerController
{
    void StartWave(WaveConfig config);
    void StopSpawning();
    bool IsSpawningComplete();
    int GetRemainingEnemies();
    event Action<Enemy> OnEnemySpawned;
    event Action OnSpawningComplete;
}
```

## Phase 6: Balancing & Polish

### 6.1 Economic Balance

**Money Earning Rates**:

- Early waves: High money-per-enemy ratio for quick upgrades
- Mid waves: Balanced earning for steady progression
- Late waves: Lower ratio but higher volume for endgame purchases

**Upgrade Cost Progression**:

- Linear increase for basic upgrades
- Exponential increase for high-tier enhancements
- Weapon-specific cost multipliers based on power level

### 6.2 Gameplay Progression Curves

**Difficulty Curve Goals**:

- Waves 1-3: Tutorial/learning phase
- Waves 4-10: Core gameplay loop establishment
- Waves 11-20: Serious challenge requiring upgrades
- Waves 20+: Expert gameplay with all systems utilized

**Player Power vs Enemy Power**:

- Design upgrade effectiveness to match enemy scaling
- Ensure shop purchases feel meaningful and impactful
- Maintain tension without making waves impossible

### 6.3 Save System Enhancement

**Persistent Data**:

- Highest wave reached
- Total money earned across all games
- Weapon upgrade levels (if permanent)
- Achievement/milestone tracking

**Session Data**:

- Current wave progress
- Money earned this session
- Temporary upgrades/purchases

## Implementation Priority Order

### Phase A: Foundation (Wave Detection)

1. Implement basic WaveManager
2. Modify Spawner for external control
3. Add enemy death event subscription
4. Create wave completion detection

### Phase B: Economy Core

1. Implement CurrencyManager
2. Add money earning from kills
3. Create basic shop UI framework
4. Implement purchase transaction system

### Phase C: Shop Integration

1. Build shop interface
2. Implement weapon upgrade system
3. Add ammo purchase functionality
4. Create upgrade persistence

### Phase D: Game Flow

1. Add BetweenWaves game state
2. Integrate shop into game flow
3. Implement wave transition system
4. Add UI enhancements

### Phase E: Polish & Balance

1. Tune difficulty curves
2. Balance economy rates
3. Add visual/audio polish
4. Implement save system enhancements

## Technical Dependencies

**Required Files to Modify**:

- `Spawner.cs`: Remove auto-start, add external control
- `GameManager.cs`: Add BetweenWaves state
- `Enemy.cs`: Ensure death events fire properly
- `WeaponManager.cs`: Integrate upgrade system
- `Ammo.cs`: Handle infinite pistol ammo
- `UIController.cs`: Add shop and wave UI

**New Files to Create**:

- `WaveManager.cs`: Core wave control system
- `WaveConfig.cs`: ScriptableObject for wave definitions
- `CurrencyManager.cs`: Money and economy system
- `ShopManager.cs`: Shop functionality and upgrades
- `WeaponUpgradeData.cs`: Upgrade definitions and persistence

This comprehensive plan provides a complete roadmap for transforming ZombieStorm from a static spawning game into a dynamic, progression-based wave survival experience with meaningful player choices and economic strategy.
