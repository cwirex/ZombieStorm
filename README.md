# ZombieStorm 🧟‍♂️⚡

**A Unity 3D survival shooter game where you battle waves of zombies using an arsenal of weapons and tactical explosives.**

*This is my first Unity project - an experimental venture into game development, showcasing various game mechanics and systems.*

## 🎮 Game Overview

ZombieStorm is a top-down survival shooter where players face increasingly challenging waves of diverse zombie types. Armed with multiple weapons and explosives, players must survive as long as possible while managing resources and positioning strategically.

## ✨ Key Features

### 🔫 Weapon System
- **8 Different Weapons**: Pistol, Uzi, Shotgun, M4, AWP, M249, RPG7, and Flamethrower
- **Weapon Switching**: Seamless weapon swapping with mouse wheel or number keys
- **Ammo Management**: Limited ammunition with reload mechanics
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

## 🚀 Game Mechanics

### Health System
- Player starts with full health
- Zombies deal damage on contact
- Health can be restored using medkits
- Visual health bar shows current status

### Ammo System
- Each weapon has limited ammo capacity
- Manual reloading required
- Different weapons have different ammo counts
- UI displays current ammo and total reserves

### Spawning System
- Multiple spawn points around the map
- Staggered spawning prevents clustering
- Different spawners for different zombie types
- Configurable spawn counts and intervals

## 🏗 Learning Objectives

This project serves as a comprehensive introduction to Unity game development, covering:

- **Game Architecture**: Component-based design patterns
- **Physics Integration**: Rigidbody dynamics and collision detection
- **AI Programming**: NavMesh pathfinding and state management
- **Input Handling**: Modern input system implementation
- **UI Development**: Canvas-based user interface design
- **Animation Systems**: Animator controllers and state machines
- **Asset Management**: Prefab workflows and scene organization
- **Performance Considerations**: Object pooling and efficient rendering

## 🎯 Future Enhancements

Potential areas for expansion and learning:
- Wave-based progression system
- Power-ups and weapon upgrades
- Multiple levels/maps
- Save system and high scores
- Audio system with sound effects and music
- Multiplayer capabilities
- Mobile platform support

---

*This project represents a foundational exploration of Unity's capabilities and game development principles. As a first Unity project, it demonstrates the implementation of core game systems while providing a solid base for future learning and iteration.*
