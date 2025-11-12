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
    public partial class CustomerSalesHistoryForm : Form
    {
        private Customer _customer;
        private DataTable _salesHistoryData;
        private DataTable _groupedSalesData;

        public CustomerSalesHistoryForm(Customer customer)
        {
            InitializeComponent();
            _customer = customer;
            this.Text = $"Історія покупок - {customer.FullName}";
            this.MinimumSize = new Size(750, 250);
            this.MaximumSize = new Size(1523, 679);
            LoadSalesHistory();
        }

        private void LoadSalesHistory()
        {
            try
            {
                if (!DbConection.ConnectionDB())
                {
                    MessageBox.Show("Не вдалося підключитися до бази даних.", "Помилка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string query = @"
                    SELECT 
                        s.sale_id AS SaleId,
                        s.sale_datetime AS SaleDate,
                        p.article_number AS ArticleNumber,
                        p.product_name AS ProductName,
                        b.brand_name AS BrandName,
                        col.color_name AS Color,
                        sz.size_value AS Size,
                        sd.product_quantity AS Quantity,
                        sd.unit_price AS UnitPrice,
                        (sd.product_quantity * sd.unit_price) AS Total,
                        cat.category_name AS Category
                    FROM sale s
                    INNER JOIN sale_details sd ON s.sale_id = sd.sale_id
                    INNER JOIN color_size cs ON sd.color_size_id = cs.color_size_id
                    INNER JOIN product p ON cs.product_id = p.product_id
                    INNER JOIN brand b ON p.brand_id = b.brand_id
                    INNER JOIN color col ON cs.color_id = col.color_id
                    INNER JOIN size sz ON cs.size_id = sz.size_id
                    INNER JOIN category cat ON p.category_id = cat.category_id
                    WHERE s.customer_id = @customerId
                    ORDER BY s.sale_datetime DESC, p.product_name";

                DbConection.msCommand.CommandText = query;
                DbConection.msCommand.Parameters.Clear();
                DbConection.msCommand.Parameters.AddWithValue("@customerId", _customer.CustomerId);

                _salesHistoryData = new DataTable();
                DbConection.msDataAdapter.Fill(_salesHistoryData);

                DbConection.CloseDB();

                GroupSalesByCheck();
                dataGridView1.DataSource = _groupedSalesData;
                ConfigureDataGridView();
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження історії: {ex.Message}", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                DbConection.CloseDB();
            }
        }

        private void GroupSalesByCheck()
        {
            _groupedSalesData = new DataTable();
            _groupedSalesData.Columns.Add("Чек", typeof(string));
            _groupedSalesData.Columns.Add("Дата/Час", typeof(string));
            _groupedSalesData.Columns.Add("Сума (грн)", typeof(string));
            _groupedSalesData.Columns.Add("Товари (Деталі)", typeof(string));

            if (_salesHistoryData == null || _salesHistoryData.Rows.Count == 0)
                return;

            var culture = CultureInfo.CreateSpecificCulture("uk-UA");

            // Групуємо за SaleId
            var groupedBySale = _salesHistoryData.AsEnumerable()
                .GroupBy(row => Convert.ToInt32(row["SaleId"]))
                .OrderByDescending(g => Convert.ToDateTime(_salesHistoryData.AsEnumerable()
                    .First(r => Convert.ToInt32(r["SaleId"]) == g.Key)["SaleDate"]));

            foreach (var saleGroup in groupedBySale)
            {
                int saleId = saleGroup.Key;
                DataRow firstRow = saleGroup.First();
                DateTime saleDate = Convert.ToDateTime(firstRow["SaleDate"]);
                decimal totalAmount = 0;
                List<string> productDetails = new List<string>();

                foreach (DataRow row in saleGroup)
                {
                    string productName = row["ProductName"]?.ToString() ?? "";
                    string brandName = row["BrandName"]?.ToString() ?? "";
                    string categoryName = row["Category"]?.ToString() ?? "";
                    string color = row["Color"]?.ToString() ?? "";
                    string size = row["Size"]?.ToString() ?? "";
                    int quantity = Convert.ToInt32(row["Quantity"]);
                    decimal unitPrice = Convert.ToDecimal(row["UnitPrice"]);
                    decimal itemTotal = Convert.ToDecimal(row["Total"]);

                    totalAmount += itemTotal;

                    string detail = $"{productName} ({brandName} / {categoryName} / {color} / {size}) – {quantity} од. – {itemTotal.ToString("N2", culture)} грн";
                    productDetails.Add(detail);
                }

                DataRow newRow = _groupedSalesData.NewRow();
                newRow["Чек"] = saleId.ToString();
                newRow["Дата/Час"] = saleDate.ToString("dd.MM.yyyy HH:mm");
                newRow["Сума (грн)"] = totalAmount.ToString("N0", culture);
                newRow["Товари (Деталі)"] = string.Join("\n", productDetails);

                _groupedSalesData.Rows.Add(newRow);
            }
        }

        private void ConfigureDataGridView()
        {
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.RowHeadersVisible = false;

            if (dataGridView1.Columns.Count > 0)
            {
                dataGridView1.Columns["Чек"].FillWeight = 10;  // ~10%
                dataGridView1.Columns["Дата/Час"].FillWeight = 20;  // ~20%
                dataGridView1.Columns["Сума (грн)"].FillWeight = 15;  // ~15%
                dataGridView1.Columns["Товари (Деталі)"].FillWeight = 55;  // ~55%
                dataGridView1.Columns["Товари (Деталі)"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            }

            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.LightBlue;
            dataGridView1.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;
        }

        private void UpdateStatistics()
        {
            Label lblCustomerInfo = this.Controls.Find("lblCustomerInfo", true).FirstOrDefault() as Label;
            Label lblStatistics = this.Controls.Find("lblStatistics", true).FirstOrDefault() as Label;
            Label lblPreferences = this.Controls.Find("lblPreferences", true).FirstOrDefault() as Label;

            if (lblCustomerInfo != null)
            {
                string discountInfo = _customer.DiscountPercent > 0
                    ? $" (Знижка: {_customer.DiscountPercent}%)"
                    : "";
                lblCustomerInfo.Text = $"Клієнт: {_customer.FullName}{discountInfo}";
            }

            if (_salesHistoryData == null || _salesHistoryData.Rows.Count == 0)
            {
                if (lblStatistics != null)
                    lblStatistics.Text = "Покупок не знайдено";
                if (lblPreferences != null)
                    lblPreferences.Text = "";
                return;
            }

            int totalChecks = _groupedSalesData.Rows.Count;
            int totalItems = _salesHistoryData.Rows.Count;
            decimal totalSpent = 0;
            Dictionary<string, int> brandPreferences = new Dictionary<string, int>();
            Dictionary<string, int> sizePreferences = new Dictionary<string, int>();
            Dictionary<string, int> categoryPreferences = new Dictionary<string, int>();

            foreach (DataRow row in _salesHistoryData.Rows)
            {
                if (row["Total"] != DBNull.Value)
                    totalSpent += Convert.ToDecimal(row["Total"]);

                string brand = row["BrandName"]?.ToString() ?? "";
                if (!string.IsNullOrEmpty(brand))
                {
                    if (!brandPreferences.ContainsKey(brand))
                        brandPreferences[brand] = 0;
                    brandPreferences[brand]++;
                }

                string size = row["Size"]?.ToString() ?? "";
                if (!string.IsNullOrEmpty(size))
                {
                    if (!sizePreferences.ContainsKey(size))
                        sizePreferences[size] = 0;
                    sizePreferences[size]++;
                }

                string category = row["Category"]?.ToString() ?? "";
                if (!string.IsNullOrEmpty(category))
                {
                    if (!categoryPreferences.ContainsKey(category))
                        categoryPreferences[category] = 0;
                    categoryPreferences[category]++;
                }
            }

            var culture = CultureInfo.CreateSpecificCulture("uk-UA");

            if (lblStatistics != null)
            {
                lblStatistics.Text = $"Всього чеків: {totalChecks}  |  " +
                                    $"Всього товарів: {totalItems}  |  " +
                                    $"Загальна сума: {totalSpent.ToString("N2", culture)} ₴";
            }

            if (lblPreferences != null)
            {
                string preferences = "Рекомендації: ";

                if (brandPreferences.Count > 0)
                {
                    var favBrand = GetMostFrequent(brandPreferences);
                    preferences += $"Бренд - {favBrand.Key} ({favBrand.Value} шт.)";
                }

                if (sizePreferences.Count > 0)
                {
                    var favSize = GetMostFrequent(sizePreferences);
                    preferences += $"  |  Розмір - {favSize.Key} ({favSize.Value} шт.)";
                }

                if (categoryPreferences.Count > 0)
                {
                    var favCategory = GetMostFrequent(categoryPreferences);
                    preferences += $"  |  Категорія - {favCategory.Key} ({favCategory.Value} шт.)";
                }

                lblPreferences.Text = preferences;
            }
        }

        private KeyValuePair<string, int> GetMostFrequent(Dictionary<string, int> dict)
        {
            return dict.OrderByDescending(x => x.Value).FirstOrDefault();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        private void close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}