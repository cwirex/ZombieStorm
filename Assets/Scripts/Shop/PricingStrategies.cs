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
}