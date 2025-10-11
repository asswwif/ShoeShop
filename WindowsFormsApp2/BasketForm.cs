using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class BasketForm : Form
    {
        private Customer _selectedCustomer;
        private CustomerService _customerService = new CustomerService();
        private List<Customer> _allCustomers; // Зберігаємо всіх клієнтів для пошуку

        public BasketForm()
        {
            InitializeComponent();

            // Ініціалізуємо вибраного клієнта як гостя
            _selectedCustomer = new Customer
            {
                CustomerId = 0,
                FirstName = "Гість",
                LastName = "",
                DiscountPercent = 0,
                PhoneNumber = ""
            };

            // Прив'язка обробників подій для DataGridView
            if (dataGridView1 != null)
            {
                dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
                dataGridView1.CellContentClick += dataGridView1_CellContentClick;
            }

            LoadClientsToComboBox();
            LoadBasketItems();
        }

        private void LoadClientsToComboBox()
        {
            try
            {
                _allCustomers = _customerService.GetAllCustomersWithDiscounts();

                // Додаємо варіант "Гість"
                _allCustomers.Insert(0, new Customer
                {
                    CustomerId = 0,
                    FirstName = "Гість",
                    LastName = "",
                    DiscountPercent = 0,
                    PhoneNumber = ""
                });

                if (comboBox1 != null)
                {
                    // Відключаємо обробник подій
                    comboBox1.SelectedIndexChanged -= comboBox1_SelectedIndexChanged;
                    comboBox1.TextChanged -= comboBox1_TextChanged;

                    // Налаштовуємо ComboBox
                    comboBox1.DataSource = _allCustomers;
                    comboBox1.DisplayMember = "DisplayText"; 
                    comboBox1.ValueMember = "CustomerId";

                    // Налаштування для пошуку
                    comboBox1.AutoCompleteMode = AutoCompleteMode.None; // Відключаємо стандартне автозавершення
                    comboBox1.DropDownStyle = ComboBoxStyle.DropDown; // Дозволяємо ввід тексту

                    // Встановлюємо "Гість" як вибраний за замовчуванням
                    comboBox1.SelectedIndex = 0;

                    // Підключаємо обробники подій
                    comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
                    comboBox1.TextChanged += comboBox1_TextChanged;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження клієнтів: {ex.Message}", "Помилка",
                                 MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1 == null || _allCustomers == null) return;

                string searchText = comboBox1.Text.ToLower();

                if (string.IsNullOrWhiteSpace(searchText))
                {
                    UpdateComboBoxData(_allCustomers);
                    return;
                }

                // Фільтруємо клієнтів за номером телефону, ім'ям або прізвищем
                var filteredCustomers = _allCustomers.Where(c =>
                    c.PhoneNumber.Contains(searchText) ||
                    c.FirstName.ToLower().Contains(searchText) ||
                    c.LastName.ToLower().Contains(searchText) ||
                    c.DisplayText.ToLower().Contains(searchText)
                ).ToList();

                UpdateComboBoxData(filteredCustomers);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка пошуку: {ex.Message}", "Помилка");
            }
        }

        private void UpdateComboBoxData(List<Customer> customers)
        {
            comboBox1.SelectedIndexChanged -= comboBox1_SelectedIndexChanged;
            comboBox1.TextChanged -= comboBox1_TextChanged; 

            string currentText = comboBox1.Text;

            comboBox1.DataSource = null;
            comboBox1.DataSource = customers;
            comboBox1.DisplayMember = "DisplayText";
            comboBox1.ValueMember = "CustomerId";

            comboBox1.Text = currentText;

            // Показуємо список (якщо є результати)
            if (customers.Any() && !string.IsNullOrWhiteSpace(currentText))
            {
                comboBox1.SelectionStart = currentText.Length;
                comboBox1.DroppedDown = true;
            }
            else
            {
                comboBox1.DroppedDown = false;
            }

            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            comboBox1.TextChanged += comboBox1_TextChanged; 
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedItem is Customer selectedCustomer)
                {
                    _selectedCustomer = selectedCustomer;
                    LoadBasketItems();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка вибору клієнта: {ex.Message}", "Помилка");
            }
        }

        // Логіка завантаження та відображення кошика

        private DataGridViewTextBoxColumn CreateColumn(string name, string header, bool isReadOnly, string dataPropertyName)
        {
            var col = new DataGridViewTextBoxColumn();
            col.Name = name;
            col.HeaderText = header;
            col.ReadOnly = isReadOnly;
            col.DataPropertyName = dataPropertyName;
            return col;
        }

        public void LoadBasketItems()
        {
            try
            {
                var items = BasketManager.GetItems();
                DataTable dt = new DataTable();

                dt.Columns.Add("ArticleNumber", typeof(string));
                dt.Columns.Add("Name", typeof(string));
                dt.Columns.Add("Color", typeof(string));
                dt.Columns.Add("Size", typeof(string));
                dt.Columns.Add("Quantity", typeof(int));
                dt.Columns.Add("Price", typeof(string));
                dt.Columns.Add("Total", typeof(string));
                dt.Columns.Add("ColorSizeId", typeof(int));

                foreach (var item in items)
                {
                    dt.Rows.Add(
                        item.ArticleNumber,
                        item.Name,
                        item.Color,
                        item.Size,
                        item.Quantity,
                        item.Price.ToString("N2", CultureInfo.CreateSpecificCulture("uk-UA")) + " ₴",
                        item.TotalPrice.ToString("N2", CultureInfo.CreateSpecificCulture("uk-UA")) + " ₴",
                        item.ColorSizeId
                    );
                }

                if (dataGridView1 != null)
                {
                    dataGridView1.Columns.Clear();
                    dataGridView1.AutoGenerateColumns = false;

                    dataGridView1.Columns.Add(CreateColumn("ArticleNumber", "Артикул", true, "ArticleNumber"));
                    dataGridView1.Columns.Add(CreateColumn("Name", "Назва", true, "Name"));
                    dataGridView1.Columns.Add(CreateColumn("Color", "Колір", true, "Color"));
                    dataGridView1.Columns.Add(CreateColumn("Size", "Розмір", true, "Size"));
                    dataGridView1.Columns.Add(CreateColumn("Quantity", "Кількість", false, "Quantity")); 
                    dataGridView1.Columns.Add(CreateColumn("Price", "Ціна", true, "Price"));
                    dataGridView1.Columns.Add(CreateColumn("Total", "Сума", true, "Total"));

                    var idCol = CreateColumn("ColorSizeId", "ID", true, "ColorSizeId");
                    idCol.Visible = false;
                    dataGridView1.Columns.Add(idCol);

                    // Додавання кнопки "Видалити"
                    if (dataGridView1.Columns["RemoveButton"] == null)
                    {
                        DataGridViewButtonColumn removeBtn = new DataGridViewButtonColumn();
                        removeBtn.HeaderText = "Дія";
                        removeBtn.Text = "X";
                        removeBtn.Name = "RemoveButton";
                        removeBtn.UseColumnTextForButtonValue = true;
                        dataGridView1.Columns.Add(removeBtn);
                    }

                    dataGridView1.DataSource = dt;
                    dataGridView1.AllowUserToAddRows = false;
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dataGridView1.ReadOnly = false;
                }

                // Оновлення загальної суми з урахуванням знижки клієнта
                decimal totalAmount = BasketManager.GetTotalAmount();
                decimal discountFactor = 1 - (decimal)_selectedCustomer.DiscountPercent / 100;
                decimal totalWithDiscount = totalAmount * discountFactor;

                Label totalLabel = this.Controls.Find("label1", true).FirstOrDefault() as Label;
                if (totalLabel != null)
                {
                    string displaySum = totalWithDiscount.ToString("N2", CultureInfo.CreateSpecificCulture("uk-UA")) + " ₴";

                    if (_selectedCustomer.DiscountPercent > 0)
                    {
                        totalLabel.Text = $"Загальна сума: {displaySum} (Знижка {_selectedCustomer.DiscountPercent}%)";
                    }
                    else
                    {
                        totalLabel.Text = $"Загальна сума: {displaySum}";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження кошика: {ex.Message}", "Помилка");
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView1.Columns.Contains("RemoveButton") &&
                dataGridView1.Columns[e.ColumnIndex].Name == "RemoveButton")
            {
                try
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                    if (row.Cells["ColorSizeId"].Value == null || row.Cells["ColorSizeId"].Value == DBNull.Value)
                    {
                        MessageBox.Show("Неможливо видалити: ідентифікатор товару не знайдено.", "Помилка");
                        return;
                    }

                    int colorSizeId = Convert.ToInt32(row.Cells["ColorSizeId"].Value);
                    string productName = row.Cells["Name"].Value?.ToString() ?? "Товар";
                    string productSize = row.Cells["Size"].Value?.ToString() ?? "";

                    DialogResult dialogResult = CustomConfirmDialog.Show($"Ви впевнені, що хочете видалити товар '{productName} ({productSize})' з кошика?","Підтвердження видалення");

                    if (dialogResult == DialogResult.Yes)
                    {
                        if (BasketManager.RemoveItem(colorSizeId))
                        {
                            LoadBasketItems();
                        }
                        else
                        {
                            MessageBox.Show("Помилка при видаленні товару.", "Помилка",
                                          MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Критична помилка при видаленні: {ex.Message}", "Помилка");
                    LoadBasketItems();
                }
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView1.Columns.Contains("Quantity") &&
                dataGridView1.Columns[e.ColumnIndex].Name == "Quantity")
            {
                // Тимчасово відключаємо обробник для уникнення рекурсії
                dataGridView1.CellValueChanged -= dataGridView1_CellValueChanged;

                try
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                    int colorSizeId = Convert.ToInt32(row.Cells["ColorSizeId"].Value);

                    if (!int.TryParse(row.Cells["Quantity"].Value?.ToString(), out int newQuantity))
                    {
                        MessageBox.Show("Будь ласка, введіть коректне числове значення для кількості.", "Помилка введення");
                        LoadBasketItems();
                        return;
                    }

                    int maxStock = GetMaxStockQuantity(colorSizeId);

                    if (newQuantity < 1)
                    {
                        BasketManager.UpdateQuantity(colorSizeId, 0);
                        MessageBox.Show("Кількість зменшено до нуля. Товар видалено з кошика.", "Видалення");
                    }
                    else if (newQuantity > maxStock)
                    {
                        MessageBox.Show($"На складі лише {maxStock} одиниць цього товару. Кількість буде встановлено на доступний максимум.", "Увага");
                        BasketManager.UpdateQuantity(colorSizeId, maxStock);
                    }
                    else
                    {
                        BasketManager.UpdateQuantity(colorSizeId, newQuantity);
                    }

                    LoadBasketItems();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка зміни кількості: {ex.Message}", "Помилка");
                    LoadBasketItems();
                }
                finally
                {
                    // Обов'язково вмикаємо обробник назад
                    dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
                }
            }
        }

        // Логіка БД(отримання актуального залишку товару)

        private int GetMaxStockQuantity(int colorSizeId)
        {
            int maxStock = 0;
            try
            {
                if (DbConection.ConnectionDB())
                {
                    string query = "SELECT stock_quantity FROM Color_Size WHERE color_size_id = @colorSizeId";

                    DbConection.msCommand.CommandText = query;
                    DbConection.msCommand.Parameters.Clear();
                    DbConection.msCommand.Parameters.AddWithValue("@colorSizeId", colorSizeId);

                    object result = DbConection.msCommand.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        maxStock = Convert.ToInt32(result);
                    }
                    DbConection.CloseDB();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка отримання залишку товару: {ex.Message}", "Помилка БД");
                maxStock = 0;
            }
            return maxStock;
        }

        private void button1_Click(object sender, EventArgs e) // Кнопка "Оформити продаж"
        {
            try
            {
                if (BasketManager.GetItems().Count == 0)
                {
                    MessageBox.Show("Ваш кошик порожній. Додайте товари перед оформленням.", "Помилка");
                    return;
                }

                SalesConfirmationForm salesForm = new SalesConfirmationForm(_selectedCustomer);
                DialogResult result = salesForm.ShowDialog();

                if (result == DialogResult.OK)
                {
                    this.Close();

                    if (this.Owner is SellerMainForm mainForm)
                    {
                        mainForm.Show();
                    }
                    else
                    {
                        SellerMainForm newMainForm = new SellerMainForm();
                        newMainForm.Show();
                    }
                }
                else
                {
                    LoadBasketItems();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при оформленні: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e) // Кнопка "Повернутися"
        {
            try
            {
                if (this.Owner is SellerMainForm mainForm)
                {
                    mainForm.Show();
                    this.Close();
                }
                else
                {
                    SellerMainForm newMainForm = new SellerMainForm();
                    newMainForm.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при поверненні: {ex.Message}", "Помилка");
                try
                {
                    SellerMainForm fallbackForm = new SellerMainForm();
                    fallbackForm.Show();
                    this.Close();
                }
                catch
                {
                    Application.Exit();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e) // Кнопка "Додати нового клієнта"
        {
            MessageBox.Show("Функціонал додавання клієнта ще не реалізовано.", "Інформація");
        }

        private void panel2_Paint(object sender, PaintEventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void close_Click(object sender, EventArgs e) { Application.Exit(); }
        private void label1_Click(object sender, EventArgs e) { }
        private void panel3_Paint(object sender, PaintEventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void pictureBox2_Click(object sender, EventArgs e) { }
    }
}