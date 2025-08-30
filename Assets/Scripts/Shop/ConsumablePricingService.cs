using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Player;

namespace Assets.Scripts.Shop
{
    /// <summary>
    /// Service that manages dynamic pricing for consumable items
    /// Uses Strategy pattern for flexible pricing algorithms
    /// </summary>
    public class ConsumablePricingService : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private bool debugMode = false;
        
        // Strategy registry
        private readonly Dictionary<ConsumableType, IPricingStrategy> strategies = new();
        
        // Static instance for easy access
        public static ConsumablePricingService Instance { get; private set; }
        
        // Events for UI updates
        public System.Action<ConsumableType, int> OnPriceChanged;
        
        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeDefaultStrategies();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// Initialize default pricing strategies according to our design
        /// </summary>
        private void InitializeDefaultStrategies()
        {
            // Medkits: Exponential pricing to prevent stockpiling
            // Formula: $150 × (1.7)^(Number of Medkits Owned) - bulk of x3 available
            RegisterPricingStrategy(ConsumableType.Medkit, new ExponentialBulkPricingStrategy(150, 1.7f, 3, 400)); // Bulk: 3 for $400 (save $50)
            
            // TNT: Fixed pricing with bulk option
            RegisterPricingStrategy(ConsumableType.TNT, new BulkPricingStrategy(50, 10, 450)); // Bulk: 10 for $450 (save $50)
            
            if (debugMode)
            {
                Debug.Log("ConsumablePricingService initialized with default strategies");
            }
        }
        
        /// <summary>
        /// Registers a pricing strategy for a consumable type
        /// </summary>
        /// <param name="itemType">Type of consumable</param>
        /// <param name="strategy">Pricing strategy to use</param>
        public void RegisterPricingStrategy(ConsumableType itemType, IPricingStrategy strategy)
        {
            if (strategy == null)
            {
                Debug.LogError($"Cannot register null strategy for {itemType}");
                return;
            }
            
            strategies[itemType] = strategy;
            
            if (debugMode)
            {
                Debug.Log($"Registered pricing strategy for {itemType}: {strategy.GetType().Name}");
            }
        }
        
        /// <summary>
        /// Gets the current price for purchasing one more item of the specified type
        /// </summary>
        /// <param name="itemType">Type of consumable</param>
        /// <param name="currentQuantity">Current quantity owned</param>
        /// <returns>Price for purchasing one more item</returns>
        public int GetPrice(ConsumableType itemType, int currentQuantity)
        {
            if (!strategies.TryGetValue(itemType, out var strategy))
            {
                Debug.LogError($"No pricing strategy found for {itemType}");
                return 0;
            }
            
            int price = strategy.CalculatePrice(currentQuantity);
            
            if (debugMode)
            {
                Debug.Log($"{itemType} price with {currentQuantity} owned: ${price}");
            }
            
            return price;
        }
        
        /// <summary>
        /// Gets the price for purchasing multiple items
        /// </summary>
        /// <param name="itemType">Type of consumable</param>
        /// <param name="currentQuantity">Current quantity owned</param>
        /// <param name="quantityToPurchase">Quantity to purchase</param>
        /// <returns>Total price for the purchase</returns>
        public int GetBulkPrice(ConsumableType itemType, int currentQuantity, int quantityToPurchase)
        {
            if (!strategies.TryGetValue(itemType, out var strategy))
            {
                Debug.LogError($"No pricing strategy found for {itemType}");
                return 0;
            }
            
            int totalPrice = 0;
            
            // Calculate cumulative price for each item
            for (int i = 0; i < quantityToPurchase; i++)
            {
                totalPrice += strategy.CalculatePrice(currentQuantity + i);
            }
            
            if (debugMode)
            {
                Debug.Log($"{itemType} bulk price: {quantityToPurchase} items starting from {currentQuantity} owned = ${totalPrice}");
            }
            
            return totalPrice;
        }
        
        /// <summary>
        /// Gets price breakdown for the next several purchases
        /// </summary>
        /// <param name="itemType">Type of consumable</param>
        /// <param name="currentQuantity">Current quantity owned</param>
        /// <param name="maxItems">Maximum items to show breakdown for</param>
        /// <returns>Dictionary of quantity -> price for next purchases</returns>
        public Dictionary<int, int> GetPriceBreakdown(ConsumableType itemType, int currentQuantity, int maxItems = 5)
        {
            var breakdown = new Dictionary<int, int>();
            
            if (!strategies.TryGetValue(itemType, out var strategy))
            {
                Debug.LogError($"No pricing strategy found for {itemType}");
                return breakdown;
            }
            
            for (int i = 0; i < maxItems; i++)
            {
                int itemNumber = currentQuantity + i + 1; // Next item to purchase
                int price = strategy.CalculatePrice(currentQuantity + i);
                breakdown[itemNumber] = price;
            }
            
            return breakdown;
        }
        
        /// <summary>
        /// Attempts to purchase an item using the currency manager
        /// </summary>
        /// <param name="itemType">Type of consumable to purchase</param>
        /// <param name="currentQuantity">Current quantity owned</param>
        /// <param name="quantity">Quantity to purchase (default: 1)</param>
        /// <returns>True if purchase was successful</returns>
        public bool TryPurchaseItem(ConsumableType itemType, int currentQuantity, int quantity = 1)
        {
            if (CurrencyManager.Instance == null)
            {
                Debug.LogError("CurrencyManager not found!");
                return false;
            }
            
            int totalCost = quantity == 1 
                ? GetPrice(itemType, currentQuantity)
                : GetBulkPrice(itemType, currentQuantity, quantity);
            
            bool success = CurrencyManager.Instance.SpendCash(totalCost);
            
            if (success)
            {
                OnPriceChanged?.Invoke(itemType, currentQuantity + quantity);
                
                if (debugMode)
                {
                    Debug.Log($"Successfully purchased {quantity}x {itemType} for ${totalCost}");
                }
            }
            else if (debugMode)
            {
                Debug.Log($"Failed to purchase {quantity}x {itemType} - insufficient funds (need ${totalCost})");
            }
            
            return success;
        }
        
        /// <summary>
        /// Gets the recommended bulk purchase option for TNT
        /// </summary>
        /// <returns>Bulk purchase info (quantity, total price, savings)</returns>
        public (int quantity, int totalPrice, int savings) GetTNTBulkOption()
        {
            const int bulkQuantity = 10;
            const int bulkPrice = 480; // $50 * 10 - $20 savings
            const int savings = 20;
            
            return (bulkQuantity, bulkPrice, savings);
        }
        
        /// <summary>
        /// Converts Item type to ConsumableType for pricing
        /// </summary>
        /// <param name="item">Item to convert</param>
        /// <returns>Corresponding ConsumableType or null if not supported</returns>
        public static ConsumableType? GetConsumableType(Item item)
        {
            return item switch
            {
                Medkit => ConsumableType.Medkit,
                TNT => ConsumableType.TNT,
                _ => null
            };
        }
        
        /// <summary>
        /// For debugging - logs price breakdown for an item type
        /// </summary>
        /// <param name="itemType">Item type to analyze</param>
        /// <param name="currentQuantity">Current quantity owned</param>
        public void LogPriceBreakdown(ConsumableType itemType, int currentQuantity)
        {
            Debug.Log($"=== {itemType} Price Breakdown (Current: {currentQuantity}) ===");
            
            var breakdown = GetPriceBreakdown(itemType, currentQuantity, 5);
            foreach (var kvp in breakdown)
            {
                Debug.Log($"{GetOrdinal(kvp.Key)} {itemType}: ${kvp.Value}");
            }
        }
        
        private string GetOrdinal(int number)
        {
            return number switch
            {
                1 => "1st",
                2 => "2nd", 
                3 => "3rd",
                _ => $"{number}th"
            };
        }
        
        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}