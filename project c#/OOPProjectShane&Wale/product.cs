using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace OOPProjectShane_Wale
{
    // Base Product class (abstract)
    public abstract class Product
    {
        // Properties
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int InStock { get; set; }




        // Constructor
        public Product(string code, string description, decimal price, int inStock)
        {
            Code = code;
            Description = description;
            Price = price;
            InStock = inStock;
        }

        // Virtual method to calculate discount (to be overridden)
        public virtual decimal CalculateDiscount(int quantity)
        {
            // Base discount logic: 5% for 10+ items
            if (quantity >= 10)
                return Price * quantity * 0.05m;
            return 0;
        }

        // Method to check if product is available
        public bool IsAvailable()
        {
            return InStock > 0;
        }

        // Method to decrease stock
        public bool DecreaseStock(int quantity)
        {
            if (quantity <= 0 || quantity > InStock)
                return false;

            InStock -= quantity;
            return true;
        }

        // Abstract method for display information (to be implemented by derived classes)
        public abstract string GetProductInfo();

        // Operator overloading: Addition (add two product prices)
        public static decimal operator +(Product p1, Product p2)
        {
            return p1.Price + p2.Price;
        }

        // Operator overloading: Greater than (compare prices)
        public static bool operator >(Product p1, Product p2)
        {
            return p1.Price > p2.Price;
        }

        // Operator overloading: Less than (compare prices)
        public static bool operator <(Product p1, Product p2)
        {
            return p1.Price < p2.Price;
        }
    }

    // AutoPart class - derived from Product
    public class AutoPart : Product
    {
        // Additional properties specific to AutoPart
        public string Manufacturer { get; set; }
        public string ModelCompatibility { get; set; }
        public int WarrantyMonths { get; set; }
        public bool IsOEM { get; set; }

        // Constructor
        public AutoPart(string code, string description, decimal price, int inStock,
                      string manufacturer, string modelCompatibility,
                      int warrantyMonths, bool isOEM)
            : base(code, description, price, inStock)
        {
            Manufacturer = manufacturer;
            ModelCompatibility = modelCompatibility;
            WarrantyMonths = warrantyMonths;
            IsOEM = isOEM;
        }

        // Override discount calculation
        public override decimal CalculateDiscount(int quantity)
        {
            // AutoPart specific discount: 8% for 10+ items and 15% for 20+ items
            if (quantity >= 20)
                return Price * quantity * 0.15m;
            if (quantity >= 10)
                return Price * quantity * 0.08m;
            return 0;
        }

        // Implement abstract method
        public override string GetProductInfo()
        {
            return $"{Code} - {Description} - ${Price} - {Manufacturer} - " +
                   $"Fits: {ModelCompatibility} - Warranty: {WarrantyMonths} months - " +
                   $"{(IsOEM ? "OEM" : "Aftermarket")} - In Stock: {InStock}";
        }
    }

    // ServiceItem class - derived from Product
    public class ServiceItem : Product
    {
        // Additional properties specific to ServiceItem
        public int EstimatedHours { get; set; }
        public string Complexity { get; set; } // "Basic", "Intermediate", "Advanced"
        public bool RequiresCertification { get; set; }

        // Constructor
        public ServiceItem(string code, string description, decimal price, int inStock,
                         int estimatedHours, string complexity, bool requiresCertification)
            : base(code, description, price, inStock)
        {
            EstimatedHours = estimatedHours;
            Complexity = complexity;
            RequiresCertification = requiresCertification;
        }

        // Override discount calculation
        public override decimal CalculateDiscount(int quantity)
        {
            // ServiceItem specific discount: 10% for multiple services
            if (quantity > 1)
                return Price * quantity * 0.10m;
            return 0;
        }

        // Implement abstract method
        public override string GetProductInfo()
        {
            return $"{Code} - {Description} - ${Price} - Est. Hours: {EstimatedHours} - " +
                   $"Complexity: {Complexity} - " +
                   $"{(RequiresCertification ? "Requires Certification" : "No Certification Required")} - " +
                   $"Available Slots: {InStock}";
        }
    }
}