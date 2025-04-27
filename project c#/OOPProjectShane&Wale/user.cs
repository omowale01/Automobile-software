using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOPProjectShane_Wale
{

    // Base User class
    public abstract class User
    {
        // Properties
        public string UserId { get; set; }
        public string Password { get; private set; }
        public string Name { get; set; }
        public string AccessLevel { get; protected set; }

        // Constructor
        public User(string userId, string password, string name)
        {
            UserId = userId;
            Password = password;
            Name = name;
        }

        // Method to validate password
        public bool ValidatePassword(string password)
        {
            return Password == password;
        }

        // Method to set/change password with validation
        public bool SetPassword(string newPassword)
        {
            // Password validation logic (you can customize this)
            if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 4)
            {
                return false;
            }

            Password = newPassword;
            return true;
        }

        // Abstract method to be implemented by derived classes
        public abstract bool CanOrderProducts();
    }

    // Customer class inherits from User
    public class Customer : User
    {
        // Additional properties for Customer
        public string CustomerAddress { get; set; }
        public string PhoneNumber { get; set; }
        public List<Order> OrderHistory { get; set; }

        // Constructor
        public Customer(string userId, string password, string name,
                      string address = "", string phone = "")
            : base(userId, password, name)
        {
            CustomerAddress = address;
            PhoneNumber = phone;
            AccessLevel = "Customer";
            OrderHistory = new List<Order>();
        }

        // Implementing abstract method
        public override bool CanOrderProducts()
        {
            return true; // Customers can order products
        }

        // Method to add an order to history
        public void AddOrderToHistory(Order order)
        {
            OrderHistory.Add(order);
        }
    }

    // Admin class inherits from User
    public sealed class Admin : User
    {
        // Additional properties specific to Admin
        public DateTime LastLoginTime { get; private set; }
        public int AdminLevel { get; set; }

        // Constructor
        public Admin(string userId, string password, string name, int adminLevel = 1)
            : base(userId, password, name)
        {
            AdminLevel = adminLevel;
            AccessLevel = "Admin";
            LastLoginTime = DateTime.Now;
        }

        // Implementing abstract method
        public override bool CanOrderProducts()
        {
            return false; // Admins don't order products
        }

        // Method to record login time
        public void RecordLogin()
        {
            LastLoginTime = DateTime.Now;
        }

        // Admin specific method to update product inventory
        public bool UpdateProductInventory(Product product, int newQuantity)
        {
            if (newQuantity < 0)
                return false;

            product.InStock = newQuantity;
            return true;
        }
    }

    // Simple Order class to store order information
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public Customer Customer { get; set; }
        public List<OrderItem> Items { get; set; }
        public decimal TotalAmount { get; set; }

        public Order(int orderId, Customer customer)
        {
            OrderId = orderId;
            OrderDate = DateTime.Now;
            Customer = customer;
            Items = new List<OrderItem>();
            TotalAmount = 0;
        }

        public void AddItem(Product product, int quantity)
        {
            Items.Add(new OrderItem(product, quantity));
            TotalAmount += product.Price * quantity;
        }
    }

    // Class to represent an individual order item
    public class OrderItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal => Product.Price * Quantity;

        public OrderItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }
    }
}
