using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OOPProjectShane_Wale
{
    // Static helper class for file operations
    public static class FileHelper
    {
        // Method to load users from a text file
        public static List<User> LoadUsers(string filePath)
        {
            List<User> users = new List<User>();

            try
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"User file not found: {filePath}");

                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    string[] parts = line.Split(',');
                    if (parts.Length < 4)
                        continue;

                    string userType = parts[0].Trim();
                    string userId = parts[1].Trim();
                    string password = parts[2].Trim();
                    string name = parts[3].Trim();

                    if (userType.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                    {
                        int adminLevel = parts.Length > 4 ? int.Parse(parts[4]) : 1;
                        users.Add(new Admin(userId, password, name, adminLevel));
                    }
                    else if (userType.Equals("Customer", StringComparison.OrdinalIgnoreCase))
                    {
                        string address = parts.Length > 4 ? parts[4] : "";
                        string phone = parts.Length > 5 ? parts[5] : "";
                        users.Add(new Customer(userId, password, name, address, phone));
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"Error loading users: {ex.Message}");
                throw; // Re-throw to be handled by the calling code
            }

            return users;
        }

        // Method to load products from a text file
        public static List<Product> LoadProducts(string filePath)
        {
            List<Product> products = new List<Product>();

            try
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"Product file not found: {filePath}");

                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    try
                    {
                        string[] parts = line.Split(',');
                        if (parts.Length < 5)
                            continue;

                        string productType = parts[0].Trim();
                        string code = parts[1].Trim();
                        string description = parts[2].Trim();
                        decimal price = decimal.Parse(parts[3].Trim());
                        int inStock = int.Parse(parts[4].Trim());

                        if (productType.Equals("AutoPart", StringComparison.OrdinalIgnoreCase))
                        {
                            if (parts.Length < 9) continue;

                            string manufacturer = parts[5].Trim();
                            string modelCompatibility = parts[6].Trim();
                            int warrantyMonths = int.Parse(parts[7].Trim());
                            bool isOEM = bool.Parse(parts[8].Trim());

                            products.Add(new AutoPart(code, description, price, inStock,
                                                    manufacturer, modelCompatibility,
                                                    warrantyMonths, isOEM));
                        }
                        else if (productType.Equals("ServiceItem", StringComparison.OrdinalIgnoreCase))
                        {
                            if (parts.Length < 8) continue;
                           
                            int estimatedHours = int.Parse(parts[5].Trim());
                            string complexity = parts[6].Trim();
                            bool requiresCertification = bool.Parse(parts[7].Trim());

                            products.Add(new ServiceItem(code, description, price, inStock, estimatedHours,  complexity, requiresCertification));
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the error but continue processing other lines
                        Console.WriteLine($"Error parsing line: {line}. Error: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading products: {ex.Message}", ex);
            }

            return products;
        }

        // Method to save products back to a file
        public static void SaveProducts(List<Product> products, string filePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (Product product in products)
                    {
                        if (product is AutoPart autoPart)
                        {
                            writer.WriteLine($"AutoPart,{autoPart.Code},{autoPart.Description}," +
                                           $"{autoPart.Price},{autoPart.InStock}," +
                                           $"{autoPart.Manufacturer},{autoPart.ModelCompatibility}," +
                                           $"{autoPart.WarrantyMonths},{autoPart.IsOEM}");
                        }
                        else if (product is ServiceItem serviceItem)
                        {
                            writer.WriteLine($"ServiceItem,{serviceItem.Code},{serviceItem.Description}," +
                                           $"{serviceItem.Price},{serviceItem.InStock}," +
                                           $"{serviceItem.EstimatedHours},{serviceItem.Complexity}," +
                                           $"{serviceItem.RequiresCertification}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving products: {ex.Message}");
                throw;
            }
        }

        // Method to save orders to a file
        public static void SaveOrder(Order order, string filePath)
        {
            try
            {
                bool fileExists = File.Exists(filePath);

                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    // Write header if file is new
                    if (!fileExists)
                    {
                        writer.WriteLine("OrderID,Customer,Date,TotalAmount,Items");
                    }

                    // Format items as a semicolon-separated list
                    string items = string.Join("; ", order.Items.Select(item =>
                        $"{item.Product.Code}({item.Quantity})"));

                    writer.WriteLine($"{order.OrderId},{order.Customer.Name}," +
                                   $"{order.OrderDate.ToString("yyyy-MM-dd HH:mm")}," +
                                   $"{order.TotalAmount},{items}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving order: {ex.Message}");
                throw;
            }
        }
    }

    // Static helper class for validation
    public static class ValidationHelper
    {
        // Method to validate product code format
        public static bool IsValidProductCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false;

            // Product code should be alphanumeric and at least 3 characters
            return code.Length >= 3 && code.All(char.IsLetterOrDigit);
        }

        // Method to validate price
        public static bool IsValidPrice(decimal price)
        {
            return price > 0 && price < 10000; // Assuming max price is $10,000
        }

        // Method to validate stock quantity
        public static bool IsValidStockQuantity(int quantity)
        {
            return quantity >= 0; // Stock can't be negative
        }

        // Method to validate user ID
        public static bool IsValidUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return false;

            // User ID should be between 3 and 20 characters, alphanumeric
            return userId.Length >= 3 && userId.Length <= 20 &&
                   userId.All(c => char.IsLetterOrDigit(c) || c == '_');
        }

        // Method to validate password strength
        public static bool IsStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                return false;

            // Check for at least one uppercase, one lowercase, and one digit
            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);

            return hasUpper && hasLower && hasDigit;
        }
    }
}
