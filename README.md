# ZombieStorm 🧟‍♂️⚡

**A Unity 3D survival shooter game where you battle waves of zombies using an arsenal of weapons and tactical explosives.**

*This is my first Unity project - an experimental venture into game development, showcasing various game mechanics and systems.*

## 🎮 Game Overview

ZombieStorm is a top-down survival shooter featuring a sophisticated wave progression system where players face increasingly challenging encounters across four distinct phases. Between waves, players can shop for weapon upgrades and consumables using earned currency. The game combines strategic resource management, tactical positioning, and intense combat with a deep weapon upgrade system and dynamic battlefield control through smart gate mechanics.

## ✨ Key Features

### 🔫 Weapon System

- **8 Different Weapons**: Pistol, Uzi, Shotgun, M4, AWP, M249, RPG7, and Flamethrower
- **Weapon Switching**: Seamless weapon swapping with mouse wheel or number keys
- **Advanced Reload System**: Audio-visual feedback with UI animations during reloading
- **Upgrade Integration**: Weapons adapt stats from shop upgrades (damage, magazine capacity, extra magazines)
- **Ammo Management**: Dynamic magazine and reserve ammo based on upgrade levels
- **Ballistics**: Realistic bullet physics with knockback effects

### 🧟 Zombie Varieties

- **Normal Zombie**: Basic enemy with standard health and damage
- **Gigant Zombie**: Large, high-health enemy with increased damage
- **Gigant Bomber**: Explodes on death, causing area damage and chain reactions
- **Toxic Zombie**: Ranged attacker that shoots toxic projectiles
- **Phoenix Zombie**: Special resurrection mechanics
- **Suicider Zombie**: Kamikaze-style enemy that explodes on contact

### 🎯 Combat Mechanics

- **Mouse Aiming**: Smooth character rotation following mouse cursor
- **Knockback System**: Enemies react to weapon impact with physics-based knockback
- **Damage Types**: Different weapons deal varying damage amounts
- **Explosion Chaining**: TNT and bomber zombies can trigger chain explosions

### 🎒 Inventory & Items

- **Medkits**: Healing items for health recovery (3 per game start)
- **TNT Explosives**: Placeable explosives for area damage (15 per game start)
- **Smart Placement**: TNT can only be placed in clear areas

### 🤖 AI System

- **NavMesh Navigation**: Zombies use Unity's AI navigation system
- **Spawner System**: Multiple spawn points with staggered timing
- **Attack Patterns**: Different zombie types have unique attack behaviors

### 🎨 Technical Features

- **Universal Render Pipeline (URP)**: Modern Unity graphics pipeline
- **Cinemachine**: Professional camera system for smooth tracking
- **Input System**: Modern Unity Input System for responsive controls
- **Animation System**: Character and weapon animations
- **Particle Effects**: Visual effects for explosions and combat

## 🛠 Technical Stack

- **Unity Version**: 2022.3.4f1 (LTS)
- **Render Pipeline**: Universal Render Pipeline (URP) 14.0.8
- **Input System**: Unity Input System 1.6.1
- **Camera System**: Cinemachine 2.9.7
- **AI Navigation**: Unity AI Navigation 1.1.4
- **Platform**: Windows (Standalone)

## 📁 Project Structure

```
Assets/
├── Scripts/
│   ├── PlayerScripts/          # Player movement, health, input, inventory
│   ├── Enemy/                  # AI, zombie types, spawners
│   ├── Weapon/                 # Weapon system, ammo, bullets
│   └── UI/                     # User interface controllers
├── Prefabs/
│   ├── Guns/                   # Weapon prefabs
│   ├── Zombies/                # Enemy prefabs
│   └── Spawners/               # Spawn point prefabs
├── Scenes/
│   └── GameScene/              # Main game scene
├── Animations/                 # Player and weapon animations
└── Sprites/                    # UI icons and weapon sprites
```

## 🎮 Controls

- **WASD**: Movement
- **Mouse**: Aim and look direction
- **Left Click**: Shoot
- **R**: Reload weapon
- **1-8**: Select specific weapon
- **Mouse Wheel**: Cycle through weapons
- **Q**: Use medkit
- **E**: Place TNT explosive
- **Space**: Start next wave (in lobby mode)
- **Shop Interface**: Auto-opens after wave completion for upgrades and purchases

## 🚀 Game Mechanics

### Health System

- Player starts with full health
- Zombies deal damage on contact  
- Health can be restored using medkits (purchasable from shop)
- Visual health bar shows current status

### Ammo System

- Each weapon has magazine and reserve ammo capacity
- Manual reloading required with audio-visual feedback
- Ammo capacity scales with weapon upgrades
- Automatic ammo refill at wave start
- UI displays current ammo and total reserves

### Wave Mechanics

- **Lobby System**: Safe exploration and shopping between waves
- **Manual Wave Start**: Player controls when to begin next wave
- **Dynamic Spawning**: Wave areas change based on progression phase
- **Gate Control**: Walls/gates open/close strategically for combat flow
- **Escape Routes**: Higher waves provide tactical escape areas

### Economy System

- **Earnings**: Cash from enemy kills, wave bonuses, and boss bonuses
- **Spending**: Weapon upgrades, consumables, and new weapon purchases
- **Balance**: Dynamic pricing ensures meaningful economic decisions

## 🏗 Learning Objectives

This project serves as a comprehensive introduction to Unity game development, covering:

**Core Systems:**
- **Game Architecture**: Component-based design patterns with singleton managers
- **Physics Integration**: Rigidbody dynamics and collision detection
- **AI Programming**: NavMesh pathfinding and state management
- **Input Handling**: Modern input system implementation
- **UI Development**: Canvas-based user interface design
- **Animation Systems**: Animator controllers and state machines

**Advanced Patterns:**
- **Event-Driven Architecture**: Decoupled system communication
- **Service Layer Design**: Modular business logic separation
- **Data-Driven Configuration**: ScriptableObject-based game balancing
- **State Management**: Complex finite state machines for game flow
- **Economic Design**: Balanced progression and pricing systems
- **Asset Management**: Automated content generation and organization

### 🌊 Wave Progression System

- **Dynamic Wave Phases**: Four distinct progression phases with increasing complexity
  - **Learning Phase** (Waves 1-6): Center spawns → Single direction attacks  
  - **Combination Phase** (Waves 8-11): Multiple simultaneous directions
  - **Inverted Phase** (Waves 13-16): All directions except one (tactical positioning)
  - **Final Phase** (Wave 18+): All directions endless scaling
- **Boss Waves**: Special encounters at waves 7, 12, 17, then every 5 waves
- **Smart Gate System**: Dynamic wall/gate opening based on wave areas and escape routes
- **Player Teleportation**: Auto-teleport to escape areas for waves 5+

### 🛒 Shop & Upgrade System

- **Weapon Upgrades**: 10-level progression system for all weapons
- **Dynamic Pricing**: Cost scaling based on weapon tier and current level
- **Ultimate Abilities**: Special weapon effects unlocked at level 10
- **Consumable Shop**: TNT and medkits with dynamic pricing based on current inventory
- **Auto-Features**: Automatic shop opening after wave completion and ammo refill at wave start
- **Currency Integration**: Earn cash from enemy kills and wave completion bonuses

### 💰 Currency System

- **Multi-Source Income**: Cash from enemy kills, wave completion bonuses, and boss bonuses
- **Dynamic Bonuses**: Wave completion rewards scale with wave number and boss level
- **Economic Balance**: Integrated with shop pricing for strategic resource management

### 🎵 Audio System

- **Sound Events**: Centralized audio event system with weapon purchase feedback
- **Sound Manager**: Comprehensive audio management for game effects

### ⚙️ Enhanced Technical Systems

- **Modular Architecture**: Component-based shop system with service layers
- **Scriptable Object Configuration**: Data-driven wave progression and weapon upgrades
- **State Management**: Complex wave states (Preparing, Active, Cleanup, Complete, Transition, Lobby)
- **Event-Driven Design**: Extensive use of Unity Events for loose coupling

## 🎯 Advanced Features

Already implemented systems that enhance gameplay:

- **Adaptive Difficulty**: Wave progression automatically adjusts challenge level
- **Strategic Economics**: Dynamic pricing creates meaningful resource decisions  
- **Weapon Mastery**: Deep upgrade paths with ultimate abilities
- **Tactical Positioning**: Gate system creates dynamic battlefield control
- **Progress Persistence**: Wave completion tracking and currency accumulation

## 🎯 Current Development Status

**Fully Implemented Systems:**
✅ Complete wave progression with 4 phases and boss encounters  
✅ Comprehensive shop system with weapon upgrades and consumables  
✅ Dynamic economy with scaling rewards and strategic pricing  
✅ Advanced weapon system with upgrade integration and reload feedback  
✅ Smart battlefield control through gate/wall management  
✅ Audio system with event-driven sound effects  

**Recent Achievements:**
- Wave balancing and progression tuning for optimal challenge scaling
- Shop system redesign with modular architecture and pricing algorithms
- Weapon reload system enhancement with audio-visual feedback
- Boss wave mechanics with area-specific spawning strategies

## 🚀 Future Development Opportunities

- **Save System**: Persistent progress and high scores
- **Multiple Levels**: Different maps with unique challenges  
- **Multiplayer Support**: Cooperative or competitive modes
- **Mobile Platform**: Touch controls and UI adaptation
- **Achievement System**: Unlockable content and progression rewards

---

*What began as a first Unity project has evolved into a sophisticated survival shooter demonstrating advanced game development concepts. The codebase showcases professional patterns including event-driven architecture, service-oriented design, and data-driven configuration - making it an excellent foundation for continued learning and expansion.*
