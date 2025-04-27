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
    public partial class LoginScreen: Form
    {
        private List<User> users;
        private const string DEFAULT_USER_FILE = "users.txt";
        public LoginScreen()
        {
            InitializeComponent();
        }

        private void LoginScreen_Load(object sender, EventArgs e)
        {
            // Attempt to load users when form loads
            LoadUsers();

            // Set password char
            txtPassword.PasswordChar = '*';
        }

      

            private void LoadUsers()
            {
            try
            {
                users = FileHelper.LoadUsers(DEFAULT_USER_FILE);
                // Show success message or update UI
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show($"Error loading user data: {ex.Message}\n\nDefault users will be created.",
                                "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Create default admin user when file is not found
                users = new List<User>
        {
            new Admin("admin", "admin", "Administrator", 1),
            new Customer("cathy", "cathy", "Cathy Customer", "123 Main St", "555-1234")
        };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error loading user data: {ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Create default admin user as fallback
                users = new List<User>
        {
            new Admin("admin", "admin", "Administrator", 1)
        };
            }
        }

           

           

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string userId = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.",
                               "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            User user = users.Find(u => u.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase));

            if (user == null || !user.ValidatePassword(password))
            {
                MessageBox.Show("Invalid username or password.",
                               "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Successful login
            if (user is Admin admin)
            {
                admin.RecordLogin();

                // Open admin view of OrderScreen
                OrderScreen orderScreen = new OrderScreen(admin);
                this.Hide();
                orderScreen.ShowDialog();
                this.Show();
            }
            else if (user is Customer customer)
            {
                // Open customer view of OrderScreen
                OrderScreen orderScreen = new OrderScreen(customer);
                this.Hide();
                orderScreen.ShowDialog();
                this.Show();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to exit?",
                                                   "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }
    }
    }
