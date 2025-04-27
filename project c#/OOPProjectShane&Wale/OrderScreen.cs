using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOPProjectShane_Wale
{
    public partial class OrderScreen: Form
    {
        private User currentUser;
        private List<Product> products;
        private List<OrderItem> cart = new List<OrderItem>();
        private const string DEFAULT_PRODUCTS_FILE = "products.txt";
        private const string ORDERS_FILE = "orders.txt";
        private static int nextOrderId = 1;

        // Constructor that accepts a User
        public OrderScreen(User user)
        {
            InitializeComponent();
            currentUser = user;
        }

        private void OrderScreen_Load(object sender, EventArgs e)
        {
            SetupUIForUserType();
            LoadProducts();

            lblLoggedInAs.Text = $"Logged in as: {currentUser.Name} ({currentUser.AccessLevel})";
            nudQuantity.Minimum = 1;
            nudQuantity.Value = 1;

            if (currentUser is Admin)
            {
                picCart.Visible = false;
            }
            else
            {
                LoadCartImage(); // Will set visibility to true if file exists
            }
        }


        private void LoadCartImage()
        {
            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cart.png");

            if (File.Exists(imagePath))
            {
                picCart.Image = Image.FromFile(imagePath);
                picCart.Visible = true;
            }
            else
            {
                picCart.Visible = false; // No image, hide it quietly
            }
        }


        private void SetupUIForUserType()
            {
            if (currentUser is Admin)
            {
                // Admin view - enable admin controls
                btnPlaceOrder.Text = "Add New Product";
                btnCancelOrder.Text = "Update Product";
                btnDeleteProduct.Visible = true;

                // Hide customer-only controls
                btnAddToCart.Visible = false;
                btnRemoveFromCart.Visible = false;
                picCart.Visible = false;
                lblCart.Visible = false;
            }
            else
            {
                // Customer view
                btnPlaceOrder.Text = "Place Order";
                btnCancelOrder.Text = "Cancel Order";
                btnDeleteProduct.Visible = false;

                // Show customer-only controls
                btnAddToCart.Visible = true;
                btnRemoveFromCart.Visible = true;
                picCart.Visible = true;
                lblCart.Visible = cart.Count > 0; // show count only if cart not empty
            }
        }

            private void LoadProducts()
            {
            try
            {
                string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string filePath = Path.Combine(projectDirectory, DEFAULT_PRODUCTS_FILE);

                // For debugging
                MessageBox.Show($"Looking for file at: {filePath}");

                products = FileHelper.LoadProducts(filePath);
                RefreshProductList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}\n\nWould you like to select a product file?",
                               "File Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (MessageBox.Show($"Error loading products: {ex.Message}\n\nWould you like to select a product file?",
                                   "File Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                {
                    OpenFileDialog dialog = new OpenFileDialog
                    {
                        Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                        Title = "Select Products File"
                    };

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            products = FileHelper.LoadProducts(dialog.FileName);
                            RefreshProductList();
                        }
                        catch (Exception loadEx)
                        {
                            MessageBox.Show($"Could not load products from the selected file.\nError: {loadEx.Message}",
                                          "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            products = CreateDefaultProducts();
                            RefreshProductList();
                        }
                    }
                    else
                    {
                        products = new List<Product>();
                    }
                }
                else
                {
                    products = new List<Product>();
                }
            }
        }


        private List<Product> CreateDefaultProducts()
        {
            var defaultProducts = new List<Product>();

            defaultProducts.Add(new AutoPart("BP001", "Oil Filter", 12.99m, 25, "Bosch", "Toyota Camry/Corolla", 12, true));
            defaultProducts.Add(new AutoPart("BP002", "Air Filter", 15.99m, 30, "K&N", "Honda Accord/Civic", 24, false));
            defaultProducts.Add(new ServiceItem("SV001", "Oil Change", 39.99m, 20, 1, "Basic", false));
            defaultProducts.Add(new ServiceItem("SV002", "Tire Rotation", 25.00m, 15, (int)0.5, "Basic", false));

            return defaultProducts;
        }


        private void RefreshProductList()
            {
                lstPreview.Items.Clear();

                if (cboType.SelectedIndex == 0 || cboType.SelectedIndex == -1) // Parts or no selection
                {
                    foreach (Product product in products)
                    {
                        if (product is AutoPart && product.InStock > 0)
                        {
                            lstPreview.Items.Add(product.GetProductInfo());
                        }
                    }
                }
                else if (cboType.SelectedIndex == 1) // Services
                {
                    foreach (Product product in products)
                    {
                        if (product is ServiceItem && product.InStock > 0)
                        {
                            lstPreview.Items.Add(product.GetProductInfo());
                        }
                    }
                }

                if (lstPreview.Items.Count == 0)
                {
                    lstPreview.Items.Add("No products available in this category");
                }
            }

           

           

            

            private void UpdateCartDisplay()
            {
            // Here you can update a separate ListBox to show cart items
            // or modify the existing lstPreview to show cart items
            // For simplicity, we'll just show a count in the title
            int itemCount = cart.Sum(item => item.Quantity); // Total quantity of items

            // Show count in form title
            this.Text = $"Orders - Cart Items: {itemCount}";

            // Show or hide cart count label
            if (itemCount > 0)
            {
                lblCart.Text = itemCount.ToString();
                lblCart.Visible = true;
            }
            else
            {
                lblCart.Visible = false;
            }
        }

            

            private void AddNewProduct()
            {
                // This would typically open a form to add a new product
                // For simplicity, we'll use input dialogs

                string productType = "";
                DialogResult typeResult = MessageBox.Show("Is this a part (Yes) or service (No)?",
                                                        "Product Type", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (typeResult == DialogResult.Yes)
                    productType = "AutoPart";
                else
                    productType = "ServiceItem";

            string code = "";
            using (Form inputForm = new Form())
            {
                inputForm.Width = 300;
                inputForm.Height = 150;
                inputForm.Text = "New Product";

                Label label = new Label() { Left = 20, Top = 20, Text = "Enter product code:" };
                TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 200 };
                Button confirmButton = new Button() { Text = "OK", Left = 20, Top = 80, DialogResult = DialogResult.OK };

                confirmButton.Click += (sender, e) => { inputForm.Close(); };

                inputForm.Controls.Add(label);
                inputForm.Controls.Add(textBox);
                inputForm.Controls.Add(confirmButton);
                inputForm.AcceptButton = confirmButton;

                if (inputForm.ShowDialog() == DialogResult.OK)
                {
                    code = textBox.Text;
                }
            }
            if (string.IsNullOrEmpty(code) || !ValidationHelper.IsValidProductCode(code))
                {
                    MessageBox.Show("Invalid product code. Product not added.",
                                   "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Check if code already exists
                if (products.Exists(p => p.Code == code))
                {
                    MessageBox.Show("A product with this code already exists.",
                                   "Duplicate Code", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string description = Microsoft.VisualBasic.Interaction.InputBox("Enter product description:", "New Product", "");
                if (string.IsNullOrEmpty(description))
                {
                    MessageBox.Show("Description is required. Product not added.",
                                   "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string priceStr = Microsoft.VisualBasic.Interaction.InputBox("Enter price:", "New Product", "");
                if (!decimal.TryParse(priceStr, out decimal price) || !ValidationHelper.IsValidPrice(price))
                {
                    MessageBox.Show("Invalid price. Product not added.",
                                   "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string stockStr = Microsoft.VisualBasic.Interaction.InputBox("Enter quantity in stock:", "New Product", "");
                if (!int.TryParse(stockStr, out int stock) || !ValidationHelper.IsValidStockQuantity(stock))
                {
                    MessageBox.Show("Invalid stock quantity. Product not added.",
                                   "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Product newProduct;

                if (productType == "AutoPart")
                {
                    string manufacturer = Microsoft.VisualBasic.Interaction.InputBox("Enter manufacturer:", "New Auto Part", "");
                    string modelCompatibility = Microsoft.VisualBasic.Interaction.InputBox("Enter compatible models:", "New Auto Part", "");

                    string warrantyStr = Microsoft.VisualBasic.Interaction.InputBox("Enter warranty months:", "New Auto Part", "12");
                    if (!int.TryParse(warrantyStr, out int warranty) || warranty < 0)
                        warranty = 12;

                    DialogResult oemResult = MessageBox.Show("Is this an OEM part?",
                                                          "OEM Status", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    bool isOEM = (oemResult == DialogResult.Yes);

                    newProduct = new AutoPart(code, description, price, stock,
                                            manufacturer, modelCompatibility, warranty, isOEM);
                }
                else // ServiceItem
                {
                    string hoursStr = Microsoft.VisualBasic.Interaction.InputBox("Enter estimated hours:", "New Service", "1");
                    if (!int.TryParse(hoursStr, out int hours) || hours <= 0)
                        hours = 1;

                    string complexity = Microsoft.VisualBasic.Interaction.InputBox("Enter complexity (Basic, Intermediate, Advanced):", "New Service", "Basic");
                    if (string.IsNullOrEmpty(complexity))
                        complexity = "Basic";

                    DialogResult certResult = MessageBox.Show("Does this service require certification?",
                                                           "Certification", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    bool requiresCert = (certResult == DialogResult.Yes);

                    newProduct = new ServiceItem(code, description, price, stock,
                                              hours, complexity, requiresCert);
                }

                products.Add(newProduct);

                try
                {
                    FileHelper.SaveProducts(products, DEFAULT_PRODUCTS_FILE);
                    MessageBox.Show("Product added successfully!", "Success",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshProductList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving product: {ex.Message}",
                                   "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void PlaceOrder()
            {
                if (cart.Count == 0)
                {
                    MessageBox.Show("Your cart is empty. Please add items before placing an order.",
                                   "Empty Cart", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                decimal total = 0;
                foreach (OrderItem item in cart)
                {
                    total += item.Subtotal;
                }

                DialogResult confirmResult = MessageBox.Show($"Total order amount: ${total}. Would you like to proceed?",
                                                          "Confirm Order", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmResult == DialogResult.Yes)
                {
                    // Create the order
                    Order order = new Order(nextOrderId++, (Customer)currentUser);

                    foreach (OrderItem item in cart)
                    {
                        order.AddItem(item.Product, item.Quantity);

                        // Decrease stock
                        item.Product.DecreaseStock(item.Quantity);
                    }

                    try
                    {
                        // Save the order
                        FileHelper.SaveOrder(order, ORDERS_FILE);

                        // Update product stock in file
                        FileHelper.SaveProducts(products, DEFAULT_PRODUCTS_FILE);

                        // Add to customer's order history
                        ((Customer)currentUser).AddOrderToHistory(order);

                        // Show checkout form
                        CheckOutOrder checkoutForm = new CheckOutOrder(order);
                        this.Hide();
                        checkoutForm.ShowDialog();
                        this.Show();

                        // Clear cart after successful order
                        cart.Clear();
                        UpdateCartDisplay();
                        RefreshProductList(); // Refresh to show updated stock
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error processing order: {ex.Message}",
                                       "Order Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

           
            

            private void UpdateProduct()
            {
                if (lstPreview.SelectedIndex == -1 || lstPreview.SelectedItem.ToString().StartsWith("No products"))
                {
                    MessageBox.Show("Please select a product to update.",
                                   "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string selectedProductInfo = lstPreview.SelectedItem.ToString();
                string productCode = selectedProductInfo.Split('-')[0].Trim();

                Product productToUpdate = products.Find(p => p.Code == productCode);
                if (productToUpdate == null)
                {
                    MessageBox.Show("Product not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // For simplicity, we'll just update the stock quantity
                string stockStr = Microsoft.VisualBasic.Interaction.InputBox(
                    $"Enter new stock quantity for {productToUpdate.Description}:",
                    "Update Stock", productToUpdate.InStock.ToString());

                if (!int.TryParse(stockStr, out int newStock) || newStock < 0)
                {
                    MessageBox.Show("Invalid stock quantity. Update canceled.",
                                   "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                productToUpdate.InStock = newStock;

                try
                {
                    FileHelper.SaveProducts(products, DEFAULT_PRODUCTS_FILE);
                    MessageBox.Show("Product updated successfully!",
                                   "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshProductList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving product update: {ex.Message}",
                                   "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

           

        private void cboType_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            RefreshProductList();
        }

        private void btnLogout_Click_1(object sender, EventArgs e)
        {
            if (cart.Count > 0)
            {
                DialogResult result = MessageBox.Show("You have items in your cart. Are you sure you want to logout?",
                                                   "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    return;
                }
            }

            this.Close();
        }

        private void btnCancelOrder_Click(object sender, EventArgs e)
        {
            if (currentUser is Admin)
            {
                // Admin functionality: Update existing product
                UpdateProduct();
            }
            else
            {
                // Customer functionality: Cancel current order
                if (cart.Count > 0)
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to cancel your order?",
                                                       "Confirm Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        cart.Clear();
                        UpdateCartDisplay();
                        MessageBox.Show("Order has been canceled.",
                                       "Order Canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("There is no active order to cancel.",
                                   "No Order", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnPlaceOrder_Click(object sender, EventArgs e)
        {
            if (currentUser is Admin)
            {
                // Admin functionality: Add new product
                AddNewProduct();
            }
            else
            {
                // Customer functionality: Place order
                PlaceOrder();
            }
        }

        private void btnRemoveFromCart_Click(object sender, EventArgs e)
        {
            if (lstPreview.SelectedIndex == -1 || lstPreview.SelectedItem.ToString().StartsWith("No products"))
            {
                MessageBox.Show("Please select a valid product.", "Selection Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedProductInfo = lstPreview.SelectedItem.ToString();
            string productCode = selectedProductInfo.Split('-')[0].Trim();

            OrderItem itemToRemove = cart.Find(item => item.Product.Code == productCode);
            if (itemToRemove == null)
            {
                MessageBox.Show("This product is not in your cart.", "Not Found",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            cart.Remove(itemToRemove);
            MessageBox.Show($"Removed {itemToRemove.Product.Description} from cart.",
                           "Removed from Cart", MessageBoxButtons.OK, MessageBoxIcon.Information);

            UpdateCartDisplay();
        }

        private void btnAddToCart_Click(object sender, EventArgs e)
        {
            if (lstPreview.SelectedIndex == -1 || lstPreview.SelectedItem.ToString().StartsWith("No products"))
                {
                    MessageBox.Show("Please select a valid product.", "Selection Error",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string selectedProductInfo = lstPreview.SelectedItem.ToString();
                string productCode = selectedProductInfo.Split('-')[0].Trim();

                Product selectedProduct = products.Find(p => p.Code == productCode);
                if (selectedProduct == null)
                {
                    MessageBox.Show("Product not found.", "Error",
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int quantity = (int)nudQuantity.Value;
                if (quantity > selectedProduct.InStock)
                {
                    MessageBox.Show($"Only {selectedProduct.InStock} items available in stock.",
                                   "Insufficient Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Check if product is already in cart
                OrderItem existingItem = cart.Find(item => item.Product.Code == selectedProduct.Code);
                if (existingItem != null)
                {
                    if (existingItem.Quantity + quantity > selectedProduct.InStock)
                    {
                        MessageBox.Show($"Cannot add more. Total would exceed available stock ({selectedProduct.InStock}).",
                                       "Stock Limit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    existingItem.Quantity += quantity;
                }
                else
                {
                    cart.Add(new OrderItem(selectedProduct, quantity));
                }

                MessageBox.Show($"Added {quantity} x {selectedProduct.Description} to cart.",
                               "Added to Cart", MessageBoxButtons.OK, MessageBoxIcon.Information);

                UpdateCartDisplay();
        }

        private void btnDeleteProduct_Click(object sender, EventArgs e)
        {
            if (lstPreview.SelectedIndex == -1 || lstPreview.SelectedItem.ToString().StartsWith("No products"))
            {
                MessageBox.Show("Please select a product to delete.",
                               "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedProductInfo = lstPreview.SelectedItem.ToString();
            string productCode = selectedProductInfo.Split('-')[0].Trim();

            Product productToDelete = products.Find(p => p.Code == productCode);
            if (productToDelete == null)
            {
                MessageBox.Show("Product not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult result = MessageBox.Show($"Are you sure you want to delete '{productToDelete.Description}'?",
                                                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                products.Remove(productToDelete);

                try
                {
                    FileHelper.SaveProducts(products, DEFAULT_PRODUCTS_FILE);
                    MessageBox.Show("Product deleted successfully!",
                                   "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshProductList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving product changes: {ex.Message}",
                                   "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void picCart_Click(object sender, EventArgs e)
        {
            if (cart.Count == 0)
            {
                MessageBox.Show("Your cart is empty.", "Cart", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            StringBuilder cartSummary = new StringBuilder();
            foreach (var item in cart)
            {
                cartSummary.AppendLine($"{item.Quantity} x {item.Product.Description} - ${item.Subtotal:F2}");
            }

            MessageBox.Show(cartSummary.ToString(), "Cart Summary", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void lblCart_Click(object sender, EventArgs e)
        {
            if (cart.Count == 0)
            {
                MessageBox.Show("Your cart is empty.", "Cart", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            StringBuilder cartSummary = new StringBuilder();
            foreach (var item in cart)
            {
                cartSummary.AppendLine($"{item.Quantity} x {item.Product.Description} - ${item.Subtotal:F2}");
            }

            MessageBox.Show(cartSummary.ToString(), "Cart Summary", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
    }
    
