using UnityEngine;

namespace Assets.Scripts.Shop
{
    /// <summary>
    /// Test script to verify the UpgradeDescriptionCalculator works correctly
    /// Can be removed after testing is complete
    /// </summary>
    public class UpgradeDescriptionCalculatorTest : MonoBehaviour
    {
        [ContextMenu("Test Calculator")]
        public void TestCalculator()
        {
            Debug.Log("🧪 Testing UpgradeDescriptionCalculator...");
            
            // Test 1: M4 Level 2 upgrade (+8% Damage, +1 Extra Magazine)
            var m4Level2Modifiers = new StatModifier[]
            {
                new StatModifier { statType = StatType.Damage, modifierType = ModifierType.Percentage, value = 8f, description = "+8% Damage" },
                new StatModifier { statType = StatType.ExtraMagazines, modifierType = ModifierType.Add, value = 1f, description = "+1 Extra Magazine" }
            };
            
            string m4Result = UpgradeDescriptionCalculator.GenerateSimplifiedDescription(m4Level2Modifiers, EWeapons.M4);
            Debug.Log($"M4 Level 2: Original '+8% Damage, +1 Extra Magazine' → Simplified: '{m4Result}'");
            
            // Test 2: UZI Level 3 upgrade (+4 Magazine Capacity)
            var uziLevel3Modifiers = new StatModifier[]
            {
                new StatModifier { statType = StatType.MagazineCapacity, modifierType = ModifierType.Add, value = 4f, description = "+4 Magazine" }
            };
            
            string uziResult = UpgradeDescriptionCalculator.GenerateSimplifiedDescription(uziLevel3Modifiers, EWeapons.UZI);
            Debug.Log($"UZI Level 3: Original '+4 Magazine Capacity' → Simplified: '{uziResult}'");
            
            // Test 3: Shotgun Level 4 upgrade (+1 Pellet per Shot, +1 Extra Magazine)
            var shotgunLevel4Modifiers = new StatModifier[]
            {
                new StatModifier { statType = StatType.PelletCount, modifierType = ModifierType.Add, value = 1f, description = "+1 Pellet" },
                new StatModifier { statType = StatType.ExtraMagazines, modifierType = ModifierType.Add, value = 1f, description = "+1 Extra Magazine" }
            };
            
            string shotgunResult = UpgradeDescriptionCalculator.GenerateSimplifiedDescription(shotgunLevel4Modifiers, EWeapons.SHOTGUN);
            Debug.Log($"Shotgun Level 4: Original '+1 Pellet per Shot, +1 Extra Magazine' → Simplified: '{shotgunResult}'");
            
            // Test 4: Mixed Power upgrade (Damage + Fire Rate)
            var mixedPowerModifiers = new StatModifier[]
            {
                new StatModifier { statType = StatType.Damage, modifierType = ModifierType.Percentage, value = 12f, description = "+12% Damage" },
                new StatModifier { statType = StatType.FireRate, modifierType = ModifierType.Percentage, value = 8f, description = "+8% Fire Rate" }
            };
            
            string mixedResult = UpgradeDescriptionCalculator.GenerateSimplifiedDescription(mixedPowerModifiers, EWeapons.M4);
            Debug.Log($"Mixed Power: Original '+12% Damage, +8% Fire Rate' → Simplified: '{mixedResult}'");
            
            Debug.Log("🧪 Calculator test complete!");
        }
        
        [ContextMenu("Test Multi-row Display")]
        public void TestMultiRowDisplay()
        {
            Debug.Log("🧪 Testing multi-row display...");
            
            // Test upgrade with 2+ different categories
            var multiUpgradeModifiers = new StatModifier[]
            {
                new StatModifier { statType = StatType.Damage, modifierType = ModifierType.Percentage, value = 15f, description = "+15% Damage" },
                new StatModifier { statType = StatType.MagazineCapacity, modifierType = ModifierType.Add, value = 4f, description = "+4 Magazine" },
                new StatModifier { statType = StatType.ExtraMagazines, modifierType = ModifierType.Add, value = 1f, description = "+1 Extra Magazine" }
            };
            
            string result = UpgradeDescriptionCalculator.GenerateSimplifiedDescription(multiUpgradeModifiers, EWeapons.M4);
            Debug.Log($"Multi-upgrade result: '{result}'");
            Debug.Log($"Contains newline: {result.Contains('\n')}");
            
            // Split by newline to show how it would appear in UI
            string[] lines = result.Split('\n');
            Debug.Log($"Line 1: '{lines[0]}'");
            if (lines.Length > 1)
                Debug.Log($"Line 2: '{lines[1]}'");
                
            Debug.Log("🧪 Multi-row test complete!");
        }
    }
}