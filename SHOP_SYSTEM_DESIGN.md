# Shop System Design V2.1 - "Wild" Rewarding System

## Core Philosophy

The new shop system prioritizes **permanent, exciting upgrades** over tedious resource management. Every purchase is meaningful, every level-up is rewarding, and late-game builds become "almost overpowered" in the best possible way.

### Key Changes from V1:
- **Simplified weapon progression**: Single "Level Up" button per weapon (10 levels max)
- **No ammo purchases**: Full ammo refill at start of each wave
- **Dynamic consumable pricing**: Prevents stockpiling, encourages strategic use
- **Ultimate abilities**: Each weapon gets a game-changing Level 10 ability

---

## Unified Weapon Level-Up System

Each weapon follows a **10-level progression path** with a single "Level Up" button. This creates an RPG-like experience where every level is a meaningful investment.

### Universal Cost Structure

Every weapon follows this precise formula based on its base price:

| Level | Cost (% of Base) | Type | Description |
|-------|------------------|------|-------------|
| **1** | 100% | Purchase | Unlocks the weapon |
| **2** | 5% | Minor Boost | Single stat improvement |
| **3** | 8% | Minor Boost | Single stat improvement |
| **4** | 12% | Minor Boost | Single stat improvement |
| **5** | **30%** | **Power Spike** | **Major combined upgrade** |
| **6** | 35% | Combined | Multi-stat improvements |
| **7** | 40% | Combined | Multi-stat improvements |
| **8** | 45% | Combined | Multi-stat improvements |
| **9** | 50% | Combined | Multi-stat improvements |
| **10** | **75%** | **Ultimate** | **Game-changing ability** |

- **Total Upgrade Cost**: 300% of weapon base price
- **Total Investment**: 400% of weapon base price
- **Quick Start**: Levels 2-4 cost only ~25% of base price combined

---

## Weapon Tiers & Pricing

### New Weapon Pricing Structure

| Tier | Weapon | Base Price | Role |
|------|--------|------------|------|
| **Starter** | PISTOL | $0* | Reliable backup weapon |
| **Tier 1** | UZI (SMG) | $300 | Close-quarters spray |
| **Tier 1** | SHOTGUN | $400 | Close-range burst damage |
| **Tier 2** | FLAMETHROWER | $700 | Area denial, crowd control |
| **Tier 2** | M4 RIFLE | $800 | Versatile workhorse |
| **Tier 3** | AWP SNIPER | $1,500 | Precision elimination |
| **Tier 3** | M249 (LMG) | $1,800 | Heavy sustained fire |
| **Tier 3** | RPG-7 | $2,000 | Explosive wave clearing |

*Pistol upgrades cost as if base price is $200

### Example: M4 Rifle Progression

**Base Price**: $800 | **Total Investment**: $3,200

| Level | Cost | Cumulative | Upgrade |
|-------|------|------------|---------|
| **1** | $800 | $800 | Base weapon unlocked |
| 2 | $40 | $840 | +5% Damage |
| 3 | $64 | $904 | +5% Fire Rate |
| 4 | $96 | $1,000 | +5 Magazine Capacity |
| **5** | **$240** | **$1,240** | **+15% Damage & +5% Fire Rate** |
| 6 | $280 | $1,520 | +10% Accuracy & -10% Recoil |
| 7 | $320 | $1,840 | +10% Fire Rate & +5 Magazine |
| 8 | $360 | $2,200 | +15% Damage & +10% Accuracy |
| 9 | $400 | $2,600 | +10% Fire Rate & -10% Recoil |
| **10** | **$600** | **$3,200** | **Headhunter Rounds: 10% crit chance (+200% damage)** |

---

## Ultimate Abilities (Level 10)

Each weapon's Level 10 upgrade provides a unique, gameplay-altering ability:

- **PISTOL**: **Akimbo** - Gain second pistol, double magazine & fire rate
- **UZI**: **Hollow-Point Fury** - Shots slow enemies by 30% for 1 second
- **SHOTGUN**: **Overwhelm Shells** - Fire +2 additional pellets per shot
- **M4 RIFLE**: **Headhunter Rounds** - 10% chance for critical hits (+200% damage)
- **FLAMETHROWER**: **Napalm Canister** - Flames leave burning ground pools
- **AWP SNIPER**: **Executioner's Mark** - Bonus damage = 25% of target's missing health
- **M249 LMG**: **Sustained Barrage** - +2% damage per second of continuous fire (max +50%)
- **RPG-7**: **Thermobaric Warhead** - +25% blast radius, stuns all enemies hit

---

## Consumables with Dynamic Pricing

### Medkits: Anti-Stockpiling System

**Formula**: Cost = $200 × (1.7)^(Number Owned)

| Medkit # | Cost | Total Spent |
|----------|------|-------------|
| 1st | $200 | $200 |
| 2nd | $340 | $540 |
| 3rd | $578 | $1,118 |
| 4th | $983 | $2,101 |

This exponential pricing makes each additional medkit a significant strategic decision.

### TNT: Tactical Investment

- **Single TNT**: $50
- **TNT Pack (10x)**: $480 (saves $20)

Higher pricing encourages tactical use rather than spam.

---

## No Ammunition Management

**Revolutionary Change**: Ammo purchases completely removed from shop.

### Benefits:
- **Automatic Refills**: Full ammo stock at start of each wave
- **100% Focus on Upgrades**: All spending goes to permanent improvements
- **Magazine Capacity More Valuable**: Only ammo resource is current magazine
- **Streamlined Gameplay**: No micromanagement between waves

---

## Technical Implementation Plan (SOLID & Clean Code)

### Core Interfaces (Open/Closed Principle):

```csharp
public interface IUpgradeable
{
    bool CanUpgrade();
    int GetUpgradeCost();
    void ApplyUpgrade();
    int GetCurrentLevel();
    int GetMaxLevel();
}

public interface IPricingStrategy
{
    int CalculatePrice(int currentQuantity);
}

public interface IWeaponUpgradeRepository
{
    IWeaponUpgradePath GetUpgradePath(EWeapons weaponType);
    void RegisterUpgradePath(EWeapons weaponType, IWeaponUpgradePath path);
}

public interface IWeaponUpgradePath
{
    IWeaponUpgrade GetUpgradeForLevel(int level);
    int GetMaxLevel();
    bool HasUltimateAbility();
    IUltimateAbility GetUltimateAbility();
}

public interface IWeaponUpgrade
{
    int GetCostPercentage();
    string GetDescription();
    void ApplyTo(IWeaponStats stats);
}

public interface IUltimateAbility
{
    string Name { get; }
    string Description { get; }
    void Activate(IWeapon weapon);
    void Deactivate(IWeapon weapon);
}
```

### Single Responsibility Classes:

```csharp
// Manages ONLY weapon level tracking
public class WeaponLevelTracker : MonoBehaviour
{
    private Dictionary<EWeapons, int> weaponLevels = new();
    
    public int GetWeaponLevel(EWeapons weapon) => weaponLevels.GetValueOrDefault(weapon, 1);
    public void SetWeaponLevel(EWeapons weapon, int level) => weaponLevels[weapon] = level;
    public bool CanLevelUp(EWeapons weapon, int maxLevel) => GetWeaponLevel(weapon) < maxLevel;
}

// Handles ONLY upgrade application
public class WeaponUpgradeService
{
    private readonly IWeaponUpgradeRepository repository;
    
    public WeaponUpgradeService(IWeaponUpgradeRepository repository)
    {
        this.repository = repository;
    }
    
    public void ApplyUpgrade(EWeapons weaponType, int level, IWeaponStats stats)
    {
        var path = repository.GetUpgradePath(weaponType);
        var upgrade = path.GetUpgradeForLevel(level);
        upgrade.ApplyTo(stats);
    }
}

// Calculates ONLY upgrade costs
public class WeaponUpgradeCostCalculator
{
    public int CalculateCost(EWeapons weaponType, int level, int basePrice)
    {
        return level switch
        {
            2 => (int)(basePrice * 0.05f),
            3 => (int)(basePrice * 0.08f),
            4 => (int)(basePrice * 0.12f),
            5 => (int)(basePrice * 0.30f),
            6 => (int)(basePrice * 0.35f),
            7 => (int)(basePrice * 0.40f),
            8 => (int)(basePrice * 0.45f),
            9 => (int)(basePrice * 0.50f),
            10 => (int)(basePrice * 0.75f),
            _ => 0
        };
    }
}
```

### Strategy Pattern for Dynamic Pricing:

```csharp
public class ExponentialPricingStrategy : IPricingStrategy
{
    private readonly int basePrice;
    private readonly float multiplier;
    
    public ExponentialPricingStrategy(int basePrice, float multiplier)
    {
        this.basePrice = basePrice;
        this.multiplier = multiplier;
    }
    
    public int CalculatePrice(int currentQuantity)
    {
        return (int)(basePrice * Mathf.Pow(multiplier, currentQuantity));
    }
}

public class ConsumablePricingService
{
    private readonly Dictionary<ItemType, IPricingStrategy> strategies = new();
    
    public void RegisterPricingStrategy(ItemType itemType, IPricingStrategy strategy)
    {
        strategies[itemType] = strategy;
    }
    
    public int GetPrice(ItemType itemType, int currentQuantity)
    {
        return strategies.TryGetValue(itemType, out var strategy) 
            ? strategy.CalculatePrice(currentQuantity)
            : 0;
    }
}
```

### Configuration-Driven Weapon Upgrades:

```csharp
[CreateAssetMenu(fileName = "WeaponUpgradePath", menuName = "Shop/Weapon Upgrade Path")]
public class WeaponUpgradePathSO : ScriptableObject, IWeaponUpgradePath
{
    [SerializeField] private WeaponUpgradeSO[] upgrades;
    [SerializeField] private UltimateAbilitySO ultimateAbility;
    
    public IWeaponUpgrade GetUpgradeForLevel(int level)
    {
        return level > 0 && level <= upgrades.Length ? upgrades[level - 1] : null;
    }
    
    public int GetMaxLevel() => upgrades.Length;
    public bool HasUltimateAbility() => ultimateAbility != null;
    public IUltimateAbility GetUltimateAbility() => ultimateAbility;
}

[CreateAssetMenu(fileName = "WeaponUpgrade", menuName = "Shop/Weapon Upgrade")]
public class WeaponUpgradeSO : ScriptableObject, IWeaponUpgrade
{
    [SerializeField] private int costPercentage;
    [SerializeField] private string description;
    [SerializeField] private StatModifier[] statModifiers;
    
    public int GetCostPercentage() => costPercentage;
    public string GetDescription() => description;
    
    public void ApplyTo(IWeaponStats stats)
    {
        foreach (var modifier in statModifiers)
        {
            modifier.Apply(stats);
        }
    }
}
```

### Clean Integration Points:

1. **WeaponManager.cs** - Uses `WeaponLevelTracker` and `WeaponUpgradeService`
2. **WeaponStats.cs** - Implements `IWeaponStats` interface
3. **ShopManager.cs** - Uses `WeaponUpgradeCostCalculator` and `ConsumablePricingService`
4. **PlayerInventory.cs** - Works with `ConsumablePricingService`
5. **UIController.cs** - Observes weapon level changes via events

### Benefits of This Design:

**Single Responsibility:**
- Each class has one reason to change
- Clear, focused responsibilities

**Open/Closed:**
- Easy to add new weapons via ScriptableObjects
- New pricing strategies without changing existing code
- New ultimate abilities as separate implementations

**Dependency Inversion:**
- High-level modules depend on abstractions
- Easy to test with mocks
- Flexible implementation swapping

**Interface Segregation:**
- Small, focused interfaces
- Clients depend only on what they need

**Liskov Substitution:**
- All implementations can be substituted
- Consistent behavior across types

---

## Implementation Tasks

### Phase 1: Level System Foundation
- [ ] Create WeaponLevelSystem class
- [ ] Define upgrade paths for all weapons
- [ ] Implement level-up cost calculations
- [ ] Add weapon level persistence

### Phase 2: Upgrade Application
- [ ] Modify WeaponStats to apply level bonuses
- [ ] Create ultimate ability system
- [ ] Test stat scaling and balance
- [ ] Add visual level indicators

### Phase 3: Dynamic Consumables
- [ ] Implement exponential medkit pricing
- [ ] Update TNT pricing system
- [ ] Remove ammo purchasing entirely
- [ ] Add auto-ammo refill system

### Phase 4: UI Integration
- [ ] Redesign shop interface for level system
- [ ] Add weapon progression visualization
- [ ] Create level-up confirmation dialogs
- [ ] Add ultimate ability previews

### Phase 5: Ultimate Abilities
- [ ] Implement each weapon's Level 10 ability
- [ ] Add special effects and animations
- [ ] Balance ultimate ability power levels
- [ ] Add audio/visual feedback

### Phase 6: Testing & Polish
- [ ] Balance weapon progression curves
- [ ] Test economy with new pricing
- [ ] Add shop animations and transitions
- [ ] Performance optimization
- [ ] Bug fixes and edge cases

---

## Economic Balance

### Cash Flow Analysis (Unchanged)
The existing currency system perfectly supports this new upgrade structure:

- **Early Waves (1-6)**: $150-900 per wave → Buy weapons, early levels
- **Mid Waves (7-12)**: $1000-2000 per wave → Power spikes (Level 5)  
- **Late Waves (13+)**: $2500+ per wave → Ultimate abilities (Level 10)

### Strategic Considerations:
- **Early Game**: Focus on acquiring weapons, cheap level-ups
- **Mid Game**: Invest in Level 5 power spikes for key weapons
- **Late Game**: Push favorite weapons to Level 10 ultimates
- **Weapon vs Upgrade**: Sometimes buying new weapon > upgrading old one

This system creates meaningful choices while ensuring steady, satisfying progression throughout the entire game.

---

## Code Compatibility Analysis

### ✅ Excellent Compatibility - No Changes Needed

**CurrencyManager.cs** - Perfect alignment:
- Already implements `SpendCash(int amount)` method
- Event system `OnCashChanged` for UI updates
- Singleton pattern for easy access
- No modifications required

**WeaponStats.cs** - Ready for extension:
- Simple class with public properties
- Easy to extend with `IWeaponStats` interface
- Current structure: `Damage`, `Range`, `FireRate`, `BulletSpeed`, `MagazineCapacity`
- No changes to existing code needed

**WeaponManager.cs** - Good foundation:
- Already tracks weapons via `EWeapons` enum
- Has weapon instantiation and selection logic
- Uses `WeaponStatsRepository` for stat lookup
- Can be extended with weapon level tracking without breaking existing functionality

**PlayerInventory.cs** - Compatible structure:
- Uses generic `Item` base class with `Amount` property
- Already handles `Medkit` and `TNT` classes
- `TryGetItem<T>()` method can work with our pricing system
- Easy to extend with dynamic pricing without changing core logic

**UIController.cs** - Shop trigger ready:
- Line 184: `if (Input.GetKeyDown(KeyCode.B)) { OpenShop(); }`
- Method already exists: `private void OpenShop()` (currently placeholder)
- Just needs implementation, no existing code to modify

### 🔧 Recommended Adapters (Non-Invasive Extensions)

**WeaponStatsAdapter** - Bridge pattern to add upgrade capabilities:
```csharp
public class WeaponStatsAdapter : IWeaponStats
{
    private WeaponStats baseStats;
    private List<IWeaponUpgrade> appliedUpgrades;
    
    public WeaponStatsAdapter(WeaponStats original) 
    {
        baseStats = original;
        appliedUpgrades = new List<IWeaponUpgrade>();
    }
    
    public float Damage => CalculateFinalStat(baseStats.Damage, StatType.Damage);
    // ... other properties with upgrade calculations
}
```

**WeaponManagerExtension** - Composition over inheritance:
```csharp
public class WeaponManagerExtension : MonoBehaviour
{
    private WeaponManager originalManager; // Composition, not inheritance
    private WeaponLevelTracker levelTracker;
    private WeaponUpgradeService upgradeService;
    
    void Start() 
    {
        originalManager = FindObjectOfType<WeaponManager>();
        // Extend functionality without modifying original
    }
}
```

**InventoryPricingExtension** - Decorator pattern:
```csharp
public class InventoryPricingExtension : MonoBehaviour
{
    private PlayerInventory originalInventory;
    private ConsumablePricingService pricingService;
    
    public bool TryPurchaseItem<T>(int quantity) where T : Item
    {
        int cost = pricingService.GetPrice(typeof(T), GetCurrentAmount<T>());
        if (CurrencyManager.Instance.SpendCash(cost))
        {
            // Use original inventory's AddItem method
            return true;
        }
        return false;
    }
}
```

### 📋 Implementation Notes

**Preserve Existing Functionality:**
- All current weapon switching, stats, and inventory mechanics work unchanged
- New features are additive, not replacements
- Backward compatibility maintained

**Integration Strategy:**
1. **Phase 1**: Create new shop system classes alongside existing code
2. **Phase 2**: Use adapter patterns to bridge old and new systems  
3. **Phase 3**: Extend `UIController.OpenShop()` method to use new system
4. **Phase 4**: Add level tracking as extension to `WeaponManager`

**ScriptableObject Configuration:**
- New weapon upgrade paths won't interfere with existing `WeaponStatsRepository`
- Can be loaded separately and applied as modifiers
- Easy to balance and modify without code changes

**Event System Integration:**
- Leverage existing `CurrencyManager.OnCashChanged` for UI updates
- Add new events for weapon upgrades without breaking existing subscribers
- UI updates through established patterns

### 🎯 Conclusion

The current codebase is **excellently designed** for extension. The new shop system can be implemented with:
- **Zero breaking changes** to existing functionality
- **Minimal coupling** through well-designed interfaces
- **Clean separation** of concerns using adapter/decorator patterns
- **Configuration-driven** approach using ScriptableObjects

The existing code quality is high, follows good practices, and provides perfect extension points. No major refactoring required - just clean additions that respect the current architecture.
