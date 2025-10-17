using System;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class DiscountCardForm : Form
    {
        private Customer customer;
        private DiscountCardService discountCardService;

        public DiscountCardForm(Customer selectedCustomer)
        {
            InitializeComponent();
            customer = selectedCustomer;
            discountCardService = new DiscountCardService();

            InitializeForm();
        }

        private void InitializeForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = false; 
            this.StartPosition = FormStartPosition.CenterParent;

            // Відображаємо інформацію про клієнта
            if (customer != null)
            {
                Label customerLabel = this.Controls.Find("label2", true).FirstOrDefault() as Label;
                if (customerLabel != null)
                {
                    customerLabel.Text = $"У клієнта {customer.FirstName} {customer.LastName} немає дисконтної картки.";
                }
            }

            ComboBox discountComboBox = this.Controls.Find("comboBox1", true).FirstOrDefault() as ComboBox;
            if (discountComboBox != null)
            {
                discountComboBox.Items.Clear();
                discountComboBox.Items.AddRange(new object[] { 5, 10, 15, 20, 25, 30 });
                discountComboBox.SelectedIndex = 0; // За замовчуванням 5%
                discountComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            }
        }

        private void label1_Click(object sender, EventArgs e) { }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            Label confirmLabel = this.Controls.Find("label3", true).FirstOrDefault() as Label;

            if (comboBox != null && comboBox.SelectedItem != null && confirmLabel != null)
            {
                int selectedDiscount = (int)comboBox.SelectedItem;
                confirmLabel.Text = $"Знижка: {selectedDiscount}%";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (customer == null)
            {
                MessageBox.Show("Помилка: клієнт не вибраний!", "Помилка",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ComboBox discountComboBox = this.Controls.Find("comboBox1", true).FirstOrDefault() as ComboBox;
            if (discountComboBox == null || discountComboBox.SelectedItem == null)
            {
                MessageBox.Show("Оберіть відсоток знижки!", "Помилка",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateForm()) return;

            int discountPercent = (int)discountComboBox.SelectedItem;

            try
            {
                if (discountCardService.AddDiscountCard(customer.CustomerId, discountPercent))
                {
                    MessageBox.Show($"Дисконтну картку зі знижкою {discountPercent}% успішно створено для клієнта {customer.FirstName} {customer.LastName}!",
                                    "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Помилка при створенні дисконтної картки! (Перевірте, чи не існує вона вже)", "Помилка",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void close_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void label2_Click(object sender, EventArgs e) { }

        private void label3_Click(object sender, EventArgs e) { }

        private bool ValidateForm()
        {
            ComboBox discountComboBox = this.Controls.Find("comboBox1", true).FirstOrDefault() as ComboBox;

            if (discountComboBox == null || discountComboBox.SelectedItem == null)
            {
                return false;
            }

            int discount = (int)discountComboBox.SelectedItem;
            if (discount < 1 || discount > 50)
            {
                MessageBox.Show("Знижка повинна бути від 1% до 50%!", "Помилка валідації",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void DiscountCardForm_Load(object sender, EventArgs e)
        {

        }
    }
}