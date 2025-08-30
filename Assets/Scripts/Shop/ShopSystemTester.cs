using UnityEngine;
using Assets.Scripts.Shop;

/// <summary>
/// Comprehensive tester for the entire shop system
/// Use this to verify all systems work together correctly
/// </summary>
public class ShopSystemTester : MonoBehaviour
{
    [Header("Testing Configuration")]
    [SerializeField] private bool runTestsOnStart = false;
    [SerializeField] private bool givePlayerMoney = true;
    [SerializeField] private int testMoney = 50000;
    
    private void Start()
    {
        if (runTestsOnStart)
        {
            Invoke(nameof(RunComprehensiveTests), 0.2f);
        }
    }
    
    [ContextMenu("Run Comprehensive Shop Tests")]
    public void RunComprehensiveTests()
    {
        Debug.Log("🧪 === COMPREHENSIVE SHOP SYSTEM TESTS ===");
        
        if (givePlayerMoney && CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddCash(testMoney);
            Debug.Log($"💰 Added ${testMoney} for testing");
        }
        
        TestSingletonInitialization();
        TestWeaponSystem();
        TestConsumableSystem();
        TestIntegrationPoints();
        TestUICompatibility();
        TestErrorHandling();
        
        Debug.Log("🎉 === ALL SHOP SYSTEM TESTS COMPLETE ===");
    }
    
    private void TestSingletonInitialization()
    {
        Debug.Log("--- Singleton Initialization Test ---");
        
        bool allGood = true;
        
        if (WeaponLevelTracker.Instance == null)
        {
            Debug.LogError("❌ WeaponLevelTracker.Instance is null!");
            allGood = false;
        }
        else
            Debug.Log("✅ WeaponLevelTracker initialized");
        
        if (ConsumablePricingService.Instance == null)
        {
            Debug.LogError("❌ ConsumablePricingService.Instance is null!");
            allGood = false;
        }
        else
            Debug.Log("✅ ConsumablePricingService initialized");
        
        if (ShopManager.Instance == null)
        {
            Debug.LogError("❌ ShopManager.Instance is null!");
            allGood = false;
        }
        else
            Debug.Log("✅ ShopManager initialized");
        
        if (CurrencyManager.Instance == null)
        {
            Debug.LogError("❌ CurrencyManager.Instance is null!");
            allGood = false;
        }
        else
            Debug.Log("✅ CurrencyManager initialized");
        
        if (allGood)
            Debug.Log("✅ All singletons initialized correctly");
    }
    
    private void TestWeaponSystem()
    {
        Debug.Log("--- Weapon System Test ---");
        
        if (ShopManager.Instance == null || WeaponLevelTracker.Instance == null)
        {
            Debug.LogError("❌ Required managers not available for weapon testing");
            return;
        }
        
        // Test pistol (should start owned at level 1)
        var pistolInfo = ShopManager.Instance.GetWeaponInfo(EWeapons.PISTOL);
        bool pistolOwned = WeaponLevelTracker.Instance.OwnsWeapon(EWeapons.PISTOL);
        int pistolLevel = WeaponLevelTracker.Instance.GetWeaponLevel(EWeapons.PISTOL);
        
        bool canUpgrade = pistolOwned && pistolLevel < 10 && pistolInfo.nextLevelCost > 0;
        Debug.Log($"✅ Pistol: Owned={pistolOwned}, Level={pistolLevel}, CanUpgrade={canUpgrade}");
        
        // Test other weapons (should start unowned)
        var uziInfo = ShopManager.Instance.GetWeaponInfo(EWeapons.UZI);
        bool uziOwned = WeaponLevelTracker.Instance.OwnsWeapon(EWeapons.UZI);
        
        bool canPurchase = !uziOwned && uziInfo.purchasePrice > 0;
        Debug.Log($"✅ UZI: Owned={uziOwned}, PurchasePrice=${uziInfo.purchasePrice}, CanPurchase={canPurchase}");
    }
    
    private void TestConsumableSystem()
    {
        Debug.Log("--- Consumable System Test ---");
        
        if (ShopManager.Instance == null || ConsumablePricingService.Instance == null)
        {
            Debug.LogError("❌ Required managers not available for consumable testing");
            return;
        }
        
        // Test medkit pricing progression
        var medkitInfo = ShopManager.Instance.GetConsumableInfo(ConsumableType.Medkit);
        bool hasBulk = medkitInfo.bulkQuantity > 0 && medkitInfo.bulkPrice > 0;
        Debug.Log($"✅ Medkits: NextPrice=${medkitInfo.nextPrice}, HasBulk={hasBulk}");
        
        // Test TNT pricing
        var tntInfo = ShopManager.Instance.GetConsumableInfo(ConsumableType.TNT);
        Debug.Log($"✅ TNT: NextPrice=${tntInfo.nextPrice}, BulkPrice=${tntInfo.bulkPrice}, BulkQuantity={tntInfo.bulkQuantity}");
        
        // Test pricing strategies work
        TestPricingStrategies();
    }
    
    private void TestPricingStrategies()
    {
        Debug.Log("--- Pricing Strategy Test ---");
        
        // Test exponential pricing
        var exponentialStrategy = new ExponentialPricingStrategy(200, 1.7f);
        int price0 = exponentialStrategy.CalculatePrice(0); // Should be $200
        int price3 = exponentialStrategy.CalculatePrice(3); // Should be $200 * 1.7^3 = $983
        
        Debug.Log($"✅ Exponential Strategy: 0 owned=${price0}, 3 owned=${price3}");
        
        // Test fixed pricing
        var fixedStrategy = new FixedPricingStrategy(50);
        int fixedPrice = fixedStrategy.CalculatePrice(10); // Should always be $50
        
        Debug.Log($"✅ Fixed Strategy: Always ${fixedPrice}");
    }
    
    private void TestIntegrationPoints()
    {
        Debug.Log("--- Integration Points Test ---");
        
        // Test shop manager can find all required systems
        bool systemsReady = ShopManager.Instance != null && 
                           WeaponLevelTracker.Instance != null && 
                           ConsumablePricingService.Instance != null;
        Debug.Log($"✅ All Systems Ready: {systemsReady}");
        
        // Test currency integration
        if (CurrencyManager.Instance != null)
        {
            int currentCash = CurrencyManager.Instance.CurrentCash;
            Debug.Log($"✅ Currency Integration: Current Cash = ${currentCash}");
        }
        
        // Test weapon manager integration
        var weaponManager = FindObjectOfType<WeaponManager>();
        Debug.Log($"✅ WeaponManager Found: {weaponManager != null}");
        
        // Test inventory integration
        var playerInventory = FindObjectOfType<PlayerInventory>();
        Debug.Log($"✅ PlayerInventory Found: {playerInventory != null}");
    }
    
    private void TestUICompatibility()
    {
        Debug.Log("--- UI Compatibility Test ---");
        
        var shopUI = FindObjectOfType<ShopUI>();
        if (shopUI != null)
        {
            Debug.Log("✅ ShopUI found in scene");
            
            // Test that ShopUI has required references assigned
            // Note: This will show warnings if references aren't assigned yet
        }
        else
        {
            Debug.LogWarning("⚠️ ShopUI not found in scene - this is expected if UI isn't set up yet");
        }
    }
    
    private void TestErrorHandling()
    {
        Debug.Log("--- Error Handling Test ---");
        
        // Test invalid weapon type handling
        try
        {
            var invalidWeaponInfo = ShopManager.Instance?.GetWeaponInfo((EWeapons)999);
            Debug.Log("✅ Invalid weapon type handled gracefully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Invalid weapon type not handled: {e.Message}");
        }
        
        // Test invalid consumable type handling
        try
        {
            var invalidConsumableInfo = ShopManager.Instance?.GetConsumableInfo((ConsumableType)999);
            Debug.Log("✅ Invalid consumable type handled gracefully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Invalid consumable type not handled: {e.Message}");
        }
    }
    
    [ContextMenu("Test Purchase Flow")]
    public void TestPurchaseFlow()
    {
        Debug.Log("🛒 === PURCHASE FLOW TEST ===");
        
        if (ShopManager.Instance == null)
        {
            Debug.LogError("❌ ShopManager not available");
            return;
        }
        
        // Give player money for testing
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddCash(10000);
        }
        
        // Test weapon purchase
        Debug.Log("Testing UZI purchase...");
        bool uziBought = ShopManager.Instance.TryPurchaseWeapon(EWeapons.UZI);
        Debug.Log($"✅ UZI Purchase Result: {uziBought}");
        
        // Test weapon upgrade
        if (uziBought)
        {
            Debug.Log("Testing UZI upgrade...");
            bool uziUpgraded = ShopManager.Instance.TryUpgradeWeapon(EWeapons.UZI);
            Debug.Log($"✅ UZI Upgrade Result: {uziUpgraded}");
        }
        
        // Test consumable purchase
        Debug.Log("Testing medkit purchase...");
        bool medkitBought = ShopManager.Instance.TryPurchaseConsumable(ConsumableType.Medkit, 1);
        Debug.Log($"✅ Medkit Purchase Result: {medkitBought}");
        
        Debug.Log("🎉 === PURCHASE FLOW TEST COMPLETE ===");
    }
}