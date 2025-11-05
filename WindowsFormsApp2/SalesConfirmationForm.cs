using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Globalization;

namespace WindowsFormsApp2
{
    public partial class SalesConfirmationForm : Form
    {
        private Customer _customer;
        private decimal _totalAmount;
        private decimal _discountAmount;
        private decimal _finalAmount;
        private List<BasketItem> _items;

        private PrintDocument printDocument1;
        private PrintDialog printDialog1;

        // Поле для зберігання ID продажу (номера чека)
        private long _currentSaleId;

        public SalesConfirmationForm(Customer selectedCustomer)
        {
            InitializeComponent();
            this.MinimumSize = new Size(900, 500);
            this.MaximumSize = new Size(1800, 960);

            // Якщо клієнт не обраний, створюємо об'єкт "Без Клієнта"
            _customer = selectedCustomer ?? new Customer { CustomerId = 0, FirstName = "Без", LastName = "Клієнта", DiscountPercent = 0 };

            _items = BasketManager.GetItems() ?? new List<BasketItem>();
            _totalAmount = BasketManager.GetTotalAmount();
            _discountAmount = 0m;

            if (_customer.CustomerId != 0)
            {
                _discountAmount = _totalAmount * (decimal)_customer.DiscountPercent / 100m;
            }
            _finalAmount = _totalAmount - _discountAmount;

            InitializePrintComponents();
            LoadSaleItemsToGrid();
            DisplaySaleDetails();
        }

        public SalesConfirmationForm() : this(new Customer { CustomerId = 0, DiscountPercent = 0 })
        {
        }

        private void InitializePrintComponents()
        {
            this.printDocument1 = new PrintDocument();
            this.printDialog1 = new PrintDialog();

            this.printDocument1.PrintPage += new PrintPageEventHandler(this.printDocument1_PrintPage);
            this.printDialog1.Document = this.printDocument1;
        }

        private DataGridViewTextBoxColumn CreateColumn(string name, string header, bool isReadOnly, string dataPropertyName)
        {
            var col = new DataGridViewTextBoxColumn();
            col.Name = name;
            col.HeaderText = header;
            col.ReadOnly = isReadOnly;
            col.DataPropertyName = dataPropertyName;
            return col;
        }

        private void LoadSaleItemsToGrid()
        {
            DataTable dt = new DataTable();
            var culture = CultureInfo.CreateSpecificCulture("uk-UA");

            dt.Columns.Add("ArticleNumber", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Color", typeof(string));
            dt.Columns.Add("Size", typeof(string));
            dt.Columns.Add("Quantity", typeof(int));
            dt.Columns.Add("Price", typeof(string));
            dt.Columns.Add("Total", typeof(string));
            dt.Columns.Add("ColorSizeId", typeof(int));

            foreach (var item in _items)
            {
                dt.Rows.Add(
                    item.ArticleNumber, item.Name, item.Color, item.Size, item.Quantity,
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
                dataGridView1.Columns.Add(CreateColumn("Quantity", "Кількість", true, "Quantity"));
                dataGridView1.Columns.Add(CreateColumn("Price", "Ціна", true, "Price"));
                dataGridView1.Columns.Add(CreateColumn("Total", "Сума", true, "Total"));

                var idCol = CreateColumn("ColorSizeId", "ID", true, "ColorSizeId");
                idCol.Visible = false;
                dataGridView1.Columns.Add(idCol);

                dataGridView1.DataSource = dt;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.ReadOnly = true;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }

        private void DisplaySaleDetails()
        {
            var culture = CultureInfo.CreateSpecificCulture("uk-UA");

            Label totalLabel = this.Controls.Find("label1", true).FirstOrDefault() as Label;
            if (totalLabel != null)
                totalLabel.Text = $"Загальна сума: {_totalAmount.ToString("N2", culture)} ₴";

            Label discountLabel = this.Controls.Find("label2", true).FirstOrDefault() as Label;
            if (discountLabel != null)
            {
                string customerInfo = _customer.CustomerId != 0 ? $"{_customer.FirstName} {_customer.LastName}, {_customer.DiscountPercent}%" : "Не зареєстрований";
                discountLabel.Text = $"Знижка: {_discountAmount.ToString("N2", culture)} ₴ (Клієнт: {customerInfo})";
            }

            Label finalLabel = this.Controls.Find("label5", true).FirstOrDefault() as Label;
            if (finalLabel != null)
                finalLabel.Text = $"Сума до сплати: {_finalAmount.ToString("N2", culture)} ₴";
        }

        private bool PerformSaleTransaction()
        {
            if (!DbConection.ConnectionDB())
            {
                MessageBox.Show("Не вдалося підключитися до бази даних.", "Помилка БД", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            MySqlTransaction transaction = null;

            try
            {
                transaction = DbConection.msConnection.BeginTransaction();
                DbConection.msCommand.Transaction = transaction;

                string saleQuery = "INSERT INTO `Sale` (customer_id, sale_datetime, sale_total, payment_status, employee_id) VALUES (@customerId, NOW(), @finalAmount, 'Оплачено', @employeeId); SELECT LAST_INSERT_ID();";
                DbConection.msCommand.CommandText = saleQuery;
                DbConection.msCommand.Parameters.Clear();
                DbConection.msCommand.Parameters.AddWithValue("@customerId", _customer.CustomerId == 0 ? (object)DBNull.Value : _customer.CustomerId);
                DbConection.msCommand.Parameters.AddWithValue("@finalAmount", _finalAmount);
                DbConection.msCommand.Parameters.AddWithValue("@employeeId", SessionManager.CurrentEmployeeId);

                // Отримуємо ID продажу (номер чека)
                long saleId = Convert.ToInt64(DbConection.msCommand.ExecuteScalar());
                _currentSaleId = saleId; 

                // Вставка деталей продажу та оновлення залишків
                string itemQuery = "INSERT INTO Sale_Details (sale_id, color_size_id, product_quantity, unit_price) VALUES (@saleId, @csId, @qty, @price);";
                string stockQuery = "UPDATE Color_Size SET stock_quantity = stock_quantity - @qty WHERE color_size_id = @csId;";

                foreach (var item in _items)
                {
                    // Вставка деталей
                    DbConection.msCommand.CommandText = itemQuery;
                    DbConection.msCommand.Parameters.Clear();
                    DbConection.msCommand.Parameters.AddWithValue("@saleId", saleId);
                    DbConection.msCommand.Parameters.AddWithValue("@csId", item.ColorSizeId);
                    DbConection.msCommand.Parameters.AddWithValue("@qty", item.Quantity);
                    DbConection.msCommand.Parameters.AddWithValue("@price", item.Price);
                    DbConection.msCommand.ExecuteNonQuery();

                    // Оновлення залишку
                    DbConection.msCommand.CommandText = stockQuery;
                    DbConection.msCommand.Parameters.Clear();
                    DbConection.msCommand.Parameters.AddWithValue("@qty", item.Quantity);
                    DbConection.msCommand.Parameters.AddWithValue("@csId", item.ColorSizeId);
                    DbConection.msCommand.ExecuteNonQuery();
                }

                transaction.Commit();
                DbConection.CloseDB();
                return true;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                DbConection.CloseDB();
                MessageBox.Show($"Помилка оформлення продажу: {ex.Message}", "Помилка транзакції", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Друк чека

        private void PrintReceipt()
        {
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                printDocument1.Print();
            }
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font regularFont = new Font("Arial", 9);
            Font boldFont = new Font("Arial", 9, FontStyle.Bold);
            Font headerFont = new Font("Arial", 14, FontStyle.Bold);

            float fontHeight = regularFont.GetHeight(g);
            float yPos = 20;
            float xMargin = 50;
            float lineSpacing = fontHeight + 4;
            var culture = CultureInfo.CreateSpecificCulture("uk-UA");

            string employeeName = $"{SessionManager.CurrentEmployeeFirstName} {SessionManager.CurrentEmployeeLastName}";
            string customerName = _customer.CustomerId != 0 ? $"{_customer.FirstName} {_customer.LastName}" : "Не зареєстрований";
 
            g.DrawString($"Чек №{_currentSaleId}", headerFont, Brushes.Black, xMargin, yPos);
            yPos += lineSpacing * 2;
 
            g.DrawString($"Дата: {DateTime.Now:yyyy-MM-dd HH:mm}", regularFont, Brushes.Black, xMargin, yPos);
            yPos += lineSpacing;
            g.DrawString($"Клієнт: {customerName}", regularFont, Brushes.Black, xMargin, yPos);
            yPos += lineSpacing;
            g.DrawString($"Продавець: {employeeName}", regularFont, Brushes.Black, xMargin, yPos);
            yPos += lineSpacing * 2;

            float nameColWidth = 90;  
            float colorColWidth = 80; 
            float sizeColWidth = 60;  
            float qtyColWidth = 50;
            float priceColWidth = 80;
            float totalColWidth = 100; 

            float xName = xMargin;
            float xColor = xName + nameColWidth;     
            float xSize = xColor + colorColWidth + 5; 
            float xQty = xSize + sizeColWidth;
            float xPrice = xQty + qtyColWidth + 5;   
            float xTotal = xPrice + priceColWidth + 5; 

            StringFormat centerAlign = new StringFormat { Alignment = StringAlignment.Center };
            StringFormat rightAlign = new StringFormat { Alignment = StringAlignment.Far };

            // Лінія-розділювач
            g.DrawString(new string('-', 150), regularFont, Brushes.Black, xMargin, yPos);
            yPos += lineSpacing;

            // Заголовки стовпців
            g.DrawString("Назва", boldFont, Brushes.Black, xName, yPos);
            g.DrawString("Колір", boldFont, Brushes.Black, new RectangleF(xColor, yPos, colorColWidth, fontHeight), centerAlign);
            g.DrawString("Розмір", boldFont, Brushes.Black, new RectangleF(xSize, yPos, sizeColWidth, fontHeight), centerAlign); // Тепер поміститься
            g.DrawString("К-сть", boldFont, Brushes.Black, new RectangleF(xQty, yPos, qtyColWidth, fontHeight), centerAlign);
            g.DrawString("Ціна (грн)", boldFont, Brushes.Black, new RectangleF(xPrice, yPos, priceColWidth, fontHeight), rightAlign);
            g.DrawString("Сума (грн)", boldFont, Brushes.Black, new RectangleF(xTotal, yPos, totalColWidth, fontHeight), rightAlign);
            yPos += lineSpacing;

            // Лінія-розділювач
            g.DrawString(new string('-', 150), regularFont, Brushes.Black, xMargin, yPos);
            yPos += lineSpacing;

            // Рядки товарів
            foreach (var item in _items)
            {
                string name = item.Name.Length > 12 ? item.Name.Substring(0, 12) + "..." : item.Name;
                string priceString = item.Price.ToString("N2", culture);
                string totalString = item.TotalPrice.ToString("N2", culture);

                g.DrawString(name, regularFont, Brushes.Black, xName, yPos);

                g.DrawString(item.Color, regularFont, Brushes.Black, new RectangleF(xColor, yPos, colorColWidth, fontHeight), centerAlign);

                g.DrawString(item.Size, regularFont, Brushes.Black, new RectangleF(xSize, yPos, sizeColWidth, fontHeight), centerAlign);
                g.DrawString(item.Quantity.ToString(), regularFont, Brushes.Black, new RectangleF(xQty, yPos, qtyColWidth, fontHeight), centerAlign);
                g.DrawString(priceString, regularFont, Brushes.Black, new RectangleF(xPrice, yPos, priceColWidth, fontHeight), rightAlign);
                g.DrawString(totalString, regularFont, Brushes.Black, new RectangleF(xTotal, yPos, totalColWidth, fontHeight), rightAlign);

                yPos += lineSpacing;
            }
 
            yPos += lineSpacing;
            g.DrawString(new string('-', 150), regularFont, Brushes.Black, xMargin, yPos);
            yPos += lineSpacing;

            float xSummary = xTotal + totalColWidth;

            g.DrawString($"Загальна сума: {_totalAmount.ToString("N2", culture)} грн",
                         boldFont, Brushes.Black, xSummary, yPos, rightAlign);
            yPos += lineSpacing;

            if (_customer.DiscountPercent > 0)
            {
                string discountText = $"Знижка ({_customer.DiscountPercent}%): {_discountAmount.ToString("N2", culture)} грн";
                g.DrawString(discountText, regularFont, Brushes.Black, xSummary, yPos, rightAlign);
                yPos += lineSpacing;

                string finalString = $"До сплати: {_finalAmount.ToString("N2", culture)} грн";
                g.DrawString(finalString, new Font("Arial", 11, FontStyle.Bold), Brushes.Black, xSummary, yPos, rightAlign);
            }
            else
            {
                string finalString = $"До сплати: {_finalAmount.ToString("N2", culture)} грн";
                g.DrawString(finalString, new Font("Arial", 11, FontStyle.Bold), Brushes.Black, xSummary, yPos, rightAlign);
            }

            e.HasMorePages = false;
        }


        private void button1_Click(object sender, EventArgs e) 
        {
            if (_items == null || _items.Count == 0)
            {
                MessageBox.Show("Неможливо оформити продаж: кошик порожній.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (PerformSaleTransaction())
            {
                PrintReceipt();
                BasketManager.ClearBasket();
                this.DialogResult = DialogResult.OK;
            }
        }

        private void button2_Click(object sender, EventArgs e) // Кнопка "Повернутися" 
        {
            this.DialogResult = DialogResult.Cancel;
        }
        private void close_Click(object sender, EventArgs e) { Application.Exit(); }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }

        private void panel2_Paint(object sender, PaintEventArgs e) { }
    }
}