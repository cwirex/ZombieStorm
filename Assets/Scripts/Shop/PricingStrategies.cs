using UnityEngine;

namespace Assets.Scripts.Shop
{
    /// <summary>
    /// Exponential pricing strategy for preventing item stockpiling
    /// Implements the formula: Cost = basePrice * (multiplier^currentQuantity)
    /// </summary>
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
            if (currentQuantity < 0)
                return basePrice;
            
            return Mathf.RoundToInt(basePrice * Mathf.Pow(multiplier, currentQuantity));
        }
    }
    
    /// <summary>
    /// Fixed pricing strategy for items with constant costs
    /// </summary>
    public class FixedPricingStrategy : IPricingStrategy
    {
        private readonly int price;
        
        public FixedPricingStrategy(int price)
        {
            this.price = price;
        }
        
        public int CalculatePrice(int currentQuantity)
        {
            return price;
        }
    }
    
    /// <summary>
    /// Bulk pricing strategy offering discounts for larger quantities
    /// </summary>
    public class BulkPricingStrategy : IPricingStrategy
    {
        private readonly int singlePrice;
        private readonly int bulkQuantity;
        private readonly int bulkPrice;
        
        public BulkPricingStrategy(int singlePrice, int bulkQuantity, int bulkPrice)
        {
            this.singlePrice = singlePrice;
            this.bulkQuantity = bulkQuantity;
            this.bulkPrice = bulkPrice;
        }
        
        public int CalculatePrice(int currentQuantity)
        {
            // This strategy calculates price for purchasing additional items
            // Could be extended to offer bulk discounts based on current quantity
            return singlePrice;
        }
        
        public int GetBulkPrice()
        {
            return bulkPrice;
        }
        
        public int GetBulkQuantity()
        {
            return bulkQuantity;
        }
    }
    
    /// <summary>
    /// Exponential pricing strategy with bulk purchase option
    /// Combines exponential pricing for singles with bulk discount option
    /// </summary>
    public class ExponentialBulkPricingStrategy : IPricingStrategy
    {
        private readonly int basePrice;
        private readonly float multiplier;
        private readonly int bulkQuantity;
        private readonly int bulkPrice;
        
        public ExponentialBulkPricingStrategy(int basePrice, float multiplier, int bulkQuantity, int bulkPrice)
        {
            this.basePrice = basePrice;
            this.multiplier = multiplier;
            this.bulkQuantity = bulkQuantity;
            this.bulkPrice = bulkPrice;
        }
        
        public int CalculatePrice(int currentQuantity)
        {
            if (currentQuantity < 0)
                return basePrice;
            
            return Mathf.RoundToInt(basePrice * Mathf.Pow(multiplier, currentQuantity));
        }
        
        public int GetBulkPrice()
        {
            return bulkPrice;
        }
        
        public int GetBulkQuantity()
        {
            return bulkQuantity;
        }
    }
    
    /// <summary>
    /// Fixed pricing for medkits with bulk discount only when player has none
    /// Prices: $150, $250, $450 (max 3 medkits)
    /// Bulk: 3 for $750 (save $100) only when owning 0 medkits
    /// </summary>
    public class FixedMedkitPricingStrategy : IPricingStrategy
    {
        private readonly int[] fixedPrices = { 150, 250, 450 }; // Prices for 1st, 2nd, 3rd medkit
        private readonly int bulkQuantity = 3;
        private readonly int bulkPrice = 750; // $850 - $100 savings = $750
        private readonly int maxQuantity = 3;
        
        public int CalculatePrice(int currentQuantity)
        {
            if (currentQuantity < 0 || currentQuantity >= maxQuantity)
                return 0; // No more medkits can be purchased
            
            return fixedPrices[currentQuantity];
        }
        
        public int GetBulkPrice()
        {
            return bulkPrice;
        }
        
        public int GetBulkQuantity()
        {
            return bulkQuantity;
        }
        
        public int GetMaxQuantity()
        {
            return maxQuantity;
        }
        
        public bool CanPurchaseBulk(int currentQuantity)
        {
            return currentQuantity == 0; // Only allow bulk purchase when player has 0 medkits
        }
    }
    
    /// <summary>
    /// Fixed pricing for TNT with max limit and bulk discount
    /// Price: $50 each, max 30 TNT, bulk 10 for 50% savings
    /// </summary>
    public class TNTPricingStrategy : IPricingStrategy
    {
        private readonly int singlePrice = 50;
        private readonly int bulkQuantity = 10;
        private readonly int maxQuantity = 30;
        
        public int CalculatePrice(int currentQuantity)
        {
            if (currentQuantity >= maxQuantity)
                return 0; // Cannot purchase more TNT
                
            return singlePrice;
        }
        
        public int GetBulkPrice()
        {
            return 450; // Fixed bulk price of $450 for 10 TNT
        }
        
        public int GetBulkQuantity()
        {
            return bulkQuantity;
        }
        
        public int GetMaxQuantity()
        {
            return maxQuantity;
        }
        
        public bool CanPurchaseBulk(int currentQuantity)
        {
            return currentQuantity + bulkQuantity <= maxQuantity; // Can buy bulk if won't exceed max
        }
    }
}