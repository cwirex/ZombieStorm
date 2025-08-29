---
### **Project "Wild" Shop System V2.1**

This document outlines the final, refined design for the in-game shop and upgrade systems. It integrates all recent decisions, including the simplified weapon progression, the balanced consumable mechanics, and the streamlined ammo philosophy, to create a cohesive and rewarding gameplay loop.
---

### **A Wildly Rewarding Shop System**

The core philosophy of this system is to simplify player choice and promote powerful, "almost overpowered" late-game builds. We have eliminated tedious resource management to focus 100% of the player's spending on permanent, exciting upgrades.

---

#### **Unified Weapon Level-Up System**

Forget about complex stat menus. Each weapon now has a single **Level Up** button. This progression system feels like a true RPG, where every level is a permanent, exciting investment.

- **How it Works:** Each time a level is purchased, the weapon receives a pre-determined stat boost. Early levels offer minor improvements, while later levels provide powerful, merged upgrades.
- **Cost Structure:** The cost is designed to make the first few levels a quick and cheap way to get a weapon up to speed, while later levels are a series of deliberate, high-impact investments.
  - **Base Weapon Cost:** 100%
  - **Total Upgrade Cost:** 300%
  - **Lvl 2-4:** Incremental costs, cumulative cost to reach level 4 is ~25% of the weapon's base price.
  - **Lvl 5-9:** Significant cost jumps, with each subsequent level being more expensive than the last. These levels provide powerful, merged upgrades.
  - **Lvl 10 (MAX):** A final, massive investment that unlocks a game-changing "Headhunter" ability.

**Example: M4 Rifle Cost and Upgrade Path**

- **Base Price:** $500
- **Total Upgrade Cost:** $1,500
- **Cumulative Cost to Level 4:** ~$125

| Level            | Cost to Reach | Cumulative Cost | Upgrade Received                                                                             |
| ---------------- | ------------- | --------------- | -------------------------------------------------------------------------------------------- |
| **1 (Purchase)** | $500          | $500            | Unlocks the weapon.                                                                          |
| 2                | $30           | $530            | +5% Damage                                                                                   |
| 3                | $45           | $575            | +5% Fire Rate                                                                                |
| 4                | $50           | $625            | +1 Magazine Capacity                                                                         |
| **5**            | **$150**      | **$775**        | **+15% Damage & +10% Fire Rate**                                                             |
| 6                | $175          | $950            | +10% Accuracy & -5% Recoil                                                                   |
| 7                | $200          | $1,150          | +15% Fire Rate & +2 Magazine Capacity                                                        |
| 8                | $225          | $1,375          | +20% Damage & +10% Accuracy                                                                  |
| 9                | $250          | $1,625          | +20% Fire Rate & -10% Recoil                                                                 |
| **10 (MAX)**     | **$375**      | **$2,000**      | **Headhunter Rounds: Bullets now have a 10% chance to be a headshot, dealing +200% damage.** |

---

#### **Consumables with a New Philosophy**

Consumables are designed to be a strategic choice, not a resource to be hoarded. A dynamic pricing system encourages players to think carefully about when to heal or use a tactical item.

- **Medkits: Dynamic Pricing:** The price of a Medkit increases dramatically based on how many you already own. This prevents easy stockpiling and makes buying one a significant strategic choice.

  - **Formula:** $\text{Medkit Cost} = \$200 \times (1.7)^{\text{Number of Medkits Owned}}$
  - **Cost to buy your 1st Medkit:** $200
  - **Cost to buy your 2nd Medkit:** $340
  - **Cost to buy your 3rd Medkit:** $578
  - **Cost to buy your 4th Medkit:** $983

- **TNTs: A Tactical Investment:** The price increase makes TNT a more tactical tool rather than something to be spammed carelessly.
  - **Single TNT:** $50
  - **TNT Pack (10x):** $480 (a small discount for buying in bulk)

---

#### **Ammunition: No More Micromanagement**

To streamline gameplay and keep the focus on permanent upgrades, we have completely removed ammo purchases from the shop.

- **Automatic Refills:** Players now start every wave with a full stock of ammunition for all owned weapons.
- **Focus on Upgrades:** This change focuses 100% of the player's spending on permanent, exciting upgrades. The only resource to manage during a wave is the ammo in your current magazine, which makes the **Magazine Capacity** stat a much more valuable and noticeable upgrade.
