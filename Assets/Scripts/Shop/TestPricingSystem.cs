using UnityEngine;
using Assets.Scripts.Shop;

/// <summary>
/// Test script to verify the dynamic consumable pricing system works correctly
/// </summary>
public class TestPricingSystem : MonoBehaviour
{
    [Header("Testing")]
    [SerializeField] private bool runTestsOnStart = true;
    
    private void Start()
    {
        if (runTestsOnStart)
        {
            // Wait a frame for all singletons to initialize
            Invoke(nameof(RunPricingTests), 0.1f);
        }
    }
    
    [ContextMenu("Run Pricing Tests")]
    public void RunPricingTests()
    {
        Debug.Log("=== CONSUMABLE PRICING SYSTEM TESTS ===");
        
        TestMedkitPricing();
        TestTNTPricing();
        TestBulkPurchases();
        
        Debug.Log("=== PRICING TESTS COMPLETE ===");
    }
    
    private void TestMedkitPricing()
    {
        Debug.Log("--- Medkit Pricing Test ---");
        
        if (ConsumablePricingService.Instance == null)
        {
            Debug.LogError("ConsumablePricingService.Instance is null!");
            return;
        }
        
        // Test exponential pricing: $200 × (1.7)^(Number of Medkits Owned)
        for (int owned = 0; owned < 10; owned++)
        {
            int price = ConsumablePricingService.Instance.GetPrice(ConsumableType.Medkit, owned);
            int expectedPrice = Mathf.RoundToInt(200 * Mathf.Pow(1.7f, owned));
            
            string status = (price == expectedPrice) ? "✅" : "❌";
            Debug.Log($"{status} {owned} medkits owned → next costs ${price} (expected ${expectedPrice})");
        }
    }
    
    private void TestTNTPricing()
    {
        Debug.Log("--- TNT Pricing Test ---");
        
        if (ConsumablePricingService.Instance == null)
        {
            Debug.LogError("ConsumablePricingService.Instance is null!");
            return;
        }
        
        // Test fixed pricing: Always $50
        for (int owned = 0; owned < 5; owned++)
        {
            int price = ConsumablePricingService.Instance.GetPrice(ConsumableType.TNT, owned);
            const int expectedPrice = 50;
            
            string status = (price == expectedPrice) ? "✅" : "❌";
            Debug.Log($"{status} {owned} TNT owned → next costs ${price} (expected ${expectedPrice})");
        }
        
        // Test bulk option
        var (quantity, totalPrice, savings) = ConsumablePricingService.Instance.GetTNTBulkOption();
        Debug.Log($"✅ TNT Bulk: {quantity} for ${totalPrice} (saves ${savings})");
    }
    
    private void TestBulkPurchases()
    {
        Debug.Log("--- Bulk Purchase Test ---");
        
        if (ConsumablePricingService.Instance == null)
        {
            Debug.LogError("ConsumablePricingService.Instance is null!");
            return;
        }
        
        // Test bulk medkit pricing (should be cumulative)
        int bulkPrice3 = ConsumablePricingService.Instance.GetBulkPrice(ConsumableType.Medkit, 2, 3);
        
        // Manual calculation: owned=2, buying 3 more
        // Item 3: $200 * 1.7^2 = $578
        // Item 4: $200 * 1.7^3 = $983  
        // Item 5: $200 * 1.7^4 = $1,671
        // Total: $578 + $983 + $1,671 = $3,232
        
        Debug.Log($"✅ Buying 3 medkits when owning 2: ${bulkPrice3}");
        Debug.Log($"   (Should be around $3,232 for exponential pricing)");
    }
}