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
        private bool _isDeleting = false;

        public BasketForm()
        {
            InitializeComponent();
            InitializeCustomer();
            SetupPhoneTextBox();
            SetupDataGridView();
            SetupButton3();
            LoadBasketItems();
            UpdateCustomerLabel();
            this.MinimumSize = new Size(900, 500);
            this.MaximumSize = new Size(1800, 960);

            BasketManager.BasketChanged += OnBasketChanged;
        }

        private void InitializeCustomer()
        {
            _selectedCustomer = new Customer
            {
                CustomerId = 0,
                FirstName = "Гість",
                LastName = "",
                DiscountPercent = 0,
                PhoneNumber = ""
            };
        }

        private void SetupPhoneTextBox()
        {
            if (textBox1 != null)
            {
                textBox1.BackColor = Color.White;
                textBox1.KeyPress += textBox1_KeyPress;
            }
        }

        private void SetupButton3()
        {
            Button searchButton = this.Controls.Find("button3", true).FirstOrDefault() as Button;
            if (searchButton != null)
            {
                searchButton.Visible = true;
                searchButton.Text = "+";
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back && e.KeyChar != '+')
            {
                e.Handled = true;
            }
        }

        private void UpdateCustomerLabel()
        {
            Label customerLabel = this.Controls.Find("label5", true).FirstOrDefault() as Label;
            if (customerLabel != null)
            {
                if (_selectedCustomer.CustomerId == 0)
                {
                    customerLabel.Text = "Клієнт: Гість";
                    customerLabel.ForeColor = Color.Black;
                }
                else
                {
                    string customerInfo = $"Клієнт: {_selectedCustomer.FirstName} {_selectedCustomer.LastName}";
                    if (_selectedCustomer.DiscountPercent > 0)
                    {
                        customerInfo += $" (Знижка: {_selectedCustomer.DiscountPercent}%)";
                    }
                    customerLabel.Text = customerInfo;
                    customerLabel.ForeColor = Color.DarkGreen;
                }
            }
        }

        private void SetGuestCustomer()
        {
            _selectedCustomer = new Customer
            {
                CustomerId = 0,
                FirstName = "Гість",
                LastName = "",
                DiscountPercent = 0,
                PhoneNumber = ""
            };

            LoadBasketItems();
            UpdateCustomerLabel();
        }

        private Customer FindCustomerByPhone(string normalizedPhone)
        {
            Customer customer = null;

            try
            {
                if (DbConection.ConnectionDB())
                {
                    string query = @"SELECT 
                                        c.customer_id,
                                        c.first_name,
                                        c.last_name,
                                        c.phone_number,
                                        c.birth_date,
                                        COALESCE(dc.discount_percent, 0) as discount_percent
                                    FROM customer c
                                    LEFT JOIN discount_card dc ON c.customer_id = dc.customer_id
                                    WHERE REPLACE(REPLACE(REPLACE(c.phone_number, ' ', ''), '-', ''), '+', '') LIKE @phone
                                    LIMIT 1";

                    DbConection.msCommand.CommandText = query;
                    DbConection.msCommand.Parameters.Clear();
                    DbConection.msCommand.Parameters.AddWithValue("@phone", $"%{normalizedPhone}%");

                    using (var reader = DbConection.msCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            DateTime birthDate = DateTime.MinValue;
                            object birthDateValue = reader["birth_date"];
                            if (birthDateValue != null && birthDateValue != DBNull.Value)
                            {
                                birthDate = Convert.ToDateTime(birthDateValue);
                            }

                            customer = new Customer
                            {
                                CustomerId = reader.GetInt32("customer_id"),
                                FirstName = reader["first_name"]?.ToString() ?? "",
                                LastName = reader["last_name"]?.ToString() ?? "",
                                PhoneNumber = reader["phone_number"]?.ToString() ?? "",
                                BirthDate = birthDate,
                                DiscountPercent = reader.GetInt32("discount_percent")
                            };
                        }
                    }

                    DbConection.CloseDB();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка пошуку в БД: {ex.Message}", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                DbConection.CloseDB();
            }

            return customer;
        }

        private void buttonHistory_Click_1(object sender, EventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ValidateAndApplyCustomerDiscount();
        }

        private void ValidateAndApplyCustomerDiscount()
        {
            try
            {
                string phoneInput = textBox1.Text.Trim();

                if (string.IsNullOrWhiteSpace(phoneInput))
                {
                    MessageBox.Show("Будь ласка, введіть номер телефону.", "Увага",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textBox1.Focus();
                    return;
                }

                if (!InfoValidator.ValidatePhone(phoneInput, out string normalizedPhone, out string errorMessage))
                {
                    MessageBox.Show(errorMessage, "Помилка валідації",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox1.BackColor = Color.LightCoral;
                    textBox1.Focus();
                    return;
                }

                // Пошук клієнта за номером телефону
                string digitsOnly = new string(phoneInput.Where(char.IsDigit).ToArray());
                Customer foundCustomer = FindCustomerByPhone(digitsOnly);

                if (foundCustomer != null)
                {
                    // Клієнта знайдено - застосовуємо знижку
                    _selectedCustomer = foundCustomer;
                    textBox1.BackColor = Color.LightGreen;
                    LoadBasketItems(); // Оновлюємо кошик з новою знижкою
                    UpdateCustomerLabel(); 

                    string message = $"Клієнта знайдено!\n\n{foundCustomer.FirstName} {foundCustomer.LastName}\n" +
                                   $"Телефон: {InfoValidator.FormatPhoneForDisplay(foundCustomer.PhoneNumber)}";

                    if (foundCustomer.DiscountPercent > 0)
                    {
                        message += $"\n\n✓ Знижка {foundCustomer.DiscountPercent}% застосована до продажу!";
                    }
                    else
                    {
                        message += "\n\nЗнижка відсутня.";
                    }

                    MessageBox.Show(message, "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    textBox1.BackColor = Color.LightCoral;

                    MessageBox.Show("Клієнта з таким номером телефону не знайдено.\n\nПродовження як гість.",
                        "Клієнт не знайдений",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    SetGuestCustomer();
                    textBox1.BackColor = Color.White;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка перевірки клієнта: {ex.Message}", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetGuestCustomer();
            }
        }

        private void SetupDataGridView()
        {
            if (dataGridView1 != null)
            {
                dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
                dataGridView1.CellContentClick += dataGridView1_CellContentClick;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView1.MultiSelect = false;
            }
        }

        private void OnBasketChanged(object sender, EventArgs e)
        {
            if (!_isDeleting)
            {
                LoadBasketItems();
            }
        }

        private DataGridViewTextBoxColumn CreateColumn(string name, string header, bool isReadOnly, string dataPropertyName)
        {
            return new DataGridViewTextBoxColumn
            {
                Name = name,
                HeaderText = header,
                ReadOnly = isReadOnly,
                DataPropertyName = dataPropertyName
            };
        }

        public void LoadBasketItems()
        {
            try
            {
                if (dataGridView1 != null)
                {
                    dataGridView1.CellValueChanged -= dataGridView1_CellValueChanged;
                    dataGridView1.CellContentClick -= dataGridView1_CellContentClick;
                    dataGridView1.ClearSelection();
                }

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

                var culture = CultureInfo.CreateSpecificCulture("uk-UA");

                foreach (var item in items)
                {
                    dt.Rows.Add(
                        item.ArticleNumber ?? "",
                        item.Name ?? "",
                        item.Color ?? "",
                        item.Size ?? "",
                        item.Quantity,
                        item.Price.ToString("N2", culture) + " ₴",
                        item.TotalPrice.ToString("N2", culture) + " ₴",
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

                    DataGridViewButtonColumn removeBtn = new DataGridViewButtonColumn
                    {
                        HeaderText = "Дія",
                        Text = "X",
                        Name = "RemoveButton",
                        UseColumnTextForButtonValue = true,
                        Width = 60
                    };
                    dataGridView1.Columns.Add(removeBtn);

                    dataGridView1.DataSource = dt;
                    dataGridView1.AllowUserToAddRows = false;
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dataGridView1.ReadOnly = false;

                    dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
                    dataGridView1.CellContentClick += dataGridView1_CellContentClick;

                    dataGridView1.ClearSelection();
                    if (dataGridView1.Rows.Count > 0)
                    {
                        dataGridView1.CurrentCell = null;
                    }
                }

                UpdateTotalLabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження кошика: {ex.Message}",
                    "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateTotalLabel()
        {
            Label totalLabel = this.Controls.Find("label1", true).FirstOrDefault() as Label;
            if (totalLabel != null)
            {
                var culture = CultureInfo.CreateSpecificCulture("uk-UA");
                decimal totalAmount = BasketManager.GetTotalAmount();
                decimal finalAmount = totalAmount;

                if (_selectedCustomer.DiscountPercent > 0)
                {
                    decimal discountAmount = totalAmount * (decimal)_selectedCustomer.DiscountPercent / 100m;
                    finalAmount = totalAmount - discountAmount;

                    totalLabel.Text = $"Сума: {totalAmount.ToString("N2", culture)} ₴  |  " +
                                     $"Знижка ({_selectedCustomer.DiscountPercent}%): -{discountAmount.ToString("N2", culture)} ₴  |  " +
                                     $"До сплати: {finalAmount.ToString("N2", culture)} ₴";
                    totalLabel.ForeColor = Color.Green;
                }
                else
                {
                    totalLabel.Text = $"Загальна сума: {finalAmount.ToString("N2", culture)} ₴";
                    totalLabel.ForeColor = Color.Black;
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (_isDeleting || e.RowIndex < 0 || e.ColumnIndex < 0) return;

            try
            {
                if (!dataGridView1.Columns.Contains("RemoveButton") ||
                    dataGridView1.Columns[e.ColumnIndex].Name != "RemoveButton") return;

                if (e.RowIndex >= dataGridView1.Rows.Count) return;

                _isDeleting = true;

                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                if (row.Cells["ColorSizeId"].Value == null || row.Cells["ColorSizeId"].Value == DBNull.Value)
                {
                    MessageBox.Show("Неможливо видалити: ідентифікатор товару не знайдено.", "Помилка");
                    _isDeleting = false;
                    return;
                }

                int colorSizeId = Convert.ToInt32(row.Cells["ColorSizeId"].Value);
                string productName = row.Cells["Name"].Value?.ToString() ?? "Товар";
                string productSize = row.Cells["Size"].Value?.ToString() ?? "";

                DialogResult dialogResult = CustomConfirmDialog.Show(
                    $"Видалити '{productName}' (розмір: {productSize}) з кошика?",
                    "Підтвердження видалення");

                if (dialogResult == DialogResult.Yes)
                {
                    dataGridView1.CellContentClick -= dataGridView1_CellContentClick;
                    dataGridView1.CellValueChanged -= dataGridView1_CellValueChanged;

                    try
                    {
                        if (BasketManager.RemoveItem(colorSizeId))
                        {
                            this.BeginInvoke(new Action(() =>
                            {
                                try
                                {
                                    LoadBasketItems();
                                }
                                finally
                                {
                                    _isDeleting = false;
                                }
                            }));
                        }
                        else
                        {
                            MessageBox.Show("Помилка при видаленні товару.", "Помилка",
                                          MessageBoxButtons.OK, MessageBoxIcon.Error);
                            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
                            dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
                            _isDeleting = false;
                        }
                    }
                    catch
                    {
                        dataGridView1.CellContentClick += dataGridView1_CellContentClick;
                        dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
                        _isDeleting = false;
                        throw;
                    }
                }
                else
                {
                    _isDeleting = false;
                }
            }
            catch (Exception ex)
            {
                _isDeleting = false;
                MessageBox.Show($"Помилка при видаленні: {ex.Message}",
                    "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try { LoadBasketItems(); } catch { }
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dataGridView1.Columns.Contains("Quantity") &&
                e.ColumnIndex >= 0 &&
                e.ColumnIndex < dataGridView1.Columns.Count &&
                dataGridView1.Columns[e.ColumnIndex].Name == "Quantity")
            {
                dataGridView1.CellValueChanged -= dataGridView1_CellValueChanged;

                try
                {
                    if (e.RowIndex >= dataGridView1.Rows.Count) return;

                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                    if (row.Cells["ColorSizeId"].Value == null || row.Cells["ColorSizeId"].Value == DBNull.Value)
                    {
                        MessageBox.Show("Помилка: не вдалося визначити товар.", "Помилка");
                        LoadBasketItems();
                        return;
                    }

                    int colorSizeId = Convert.ToInt32(row.Cells["ColorSizeId"].Value);

                    if (!int.TryParse(row.Cells["Quantity"].Value?.ToString(), out int newQuantity))
                    {
                        MessageBox.Show("Введіть коректне числове значення.", "Помилка");
                        LoadBasketItems();
                        return;
                    }

                    int maxStock = GetMaxStockQuantity(colorSizeId);

                    if (newQuantity < 1)
                    {
                        DialogResult result = CustomConfirmDialog.Show(
                            "Кількість менше 1. Видалити товар з кошика?", "Підтвердження");

                        if (result == DialogResult.Yes)
                        {
                            BasketManager.UpdateQuantity(colorSizeId, 0);
                        }
                        LoadBasketItems();
                        return;
                    }

                    if (newQuantity > maxStock)
                    {
                        MessageBox.Show($"На складі лише {maxStock} од.\nВстановлено максимум.", "Увага");
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
                    dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
                }
            }
        }

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
                MessageBox.Show($"Помилка отримання залишку: {ex.Message}", "Помилка");
                DbConection.CloseDB();
            }
            return maxStock;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (BasketManager.GetItems().Count == 0)
                {
                    MessageBox.Show("Кошик порожній. Додайте товари.", "Помилка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        new SellerMainForm().Show();
                    }
                }
                else
                {
                    LoadBasketItems();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка оформлення: {ex.Message}", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.Owner is SellerMainForm mainForm)
                {
                    mainForm.Show();
                }
                else
                {
                    new SellerMainForm().Show();
                }
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка");
                Application.Exit();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            BasketManager.BasketChanged -= OnBasketChanged;
            base.OnFormClosing(e);
        }

        private void panel2_Paint(object sender, PaintEventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }
        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void close_Click(object sender, EventArgs e) { Application.Exit(); }
        private void label1_Click(object sender, EventArgs e) { }
        private void panel3_Paint(object sender, PaintEventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void pictureBox2_Click(object sender, EventArgs e) { }
    }
}