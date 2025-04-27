using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOPProjectShane_Wale
{
    public partial class CheckOutOrder: Form
    {
        private Order currentOrder;
        public CheckOutOrder()
        {
            InitializeComponent();
        }

        // Constructor that accepts an Order
        public CheckOutOrder(Order order)
        {
            InitializeComponent();
            currentOrder = order;
        }

        private void CheckOutOrder_Load(object sender, EventArgs e)
        {

            if (currentOrder == null)
            {
                MessageBox.Show("No order information available.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            // Display order summary
            label5.Text = $"Order #{currentOrder.OrderId}";
            label4.Text = $"Thank you for your order, {currentOrder.Customer.Name}!";

            string parts = "";
            string services = "";

            foreach (OrderItem item in currentOrder.Items)
            {
                if (item.Product is AutoPart)
                {
                    parts += $"{item.Product.Description} x{item.Quantity}: ${item.Subtotal}\n";
                }
                else if (item.Product is ServiceItem)
                {
                    services += $"{item.Product.Description} x{item.Quantity}: ${item.Subtotal}\n";
                }
            }

            label3.Text = parts;
            label7.Text = services;

            this.Text = $"Order Confirmation - Total: ${currentOrder.TotalAmount}";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Your order has been placed successfully! Thank you for your business.",
                           "Order Confirmed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to cancel your order?",
                                                  "Cancel Order", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }
    }
    }
