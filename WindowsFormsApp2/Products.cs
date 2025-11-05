using System;
using System.Data;
using System.Windows.Forms;
using System.Globalization;
using System.Linq;

namespace WindowsFormsApp2
{
    public partial class Products : UserControl
    {
        public Products()
        {
            InitializeComponent();
            InitializeFilters();
            LoadProducts();
        }

        private void InitializeFilters()
        {
            try
            {
                if (DbConection.ConnectionDB())
                {
                    FillCategoriesComboBox();
                    FillColorsComboBox();
                    FillSizesComboBox();
                    FillBrandsComboBox();

                    DbConection.CloseDB();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка ініціалізації фільтрів: {ex.Message}", "Помилка");
            }
        }

        private void FillCategoriesComboBox()
        {
            string query = "SELECT category_id, category_name FROM Category ORDER BY category_name";
            DbConection.msCommand.CommandText = query;

            DataTable dt = new DataTable();
            DbConection.msDataAdapter.SelectCommand = DbConection.msCommand;
            DbConection.msDataAdapter.Fill(dt);

            DataRow newRow = dt.NewRow();
            newRow["category_id"] = 0;
            newRow["category_name"] = "Всі категорії";
            dt.Rows.InsertAt(newRow, 0);

            comboBox5.DisplayMember = "category_name";
            comboBox5.ValueMember = "category_id";
            comboBox5.DataSource = dt;
            comboBox5.SelectedIndex = 0;
        }

        private void FillColorsComboBox()
        {
            string query = "SELECT color_id, color_name FROM Color ORDER BY color_name";
            DbConection.msCommand.CommandText = query;

            DataTable dt = new DataTable();
            DbConection.msDataAdapter.SelectCommand = DbConection.msCommand;
            DbConection.msDataAdapter.Fill(dt);

            DataRow newRow = dt.NewRow();
            newRow["color_id"] = 0;
            newRow["color_name"] = "Всі кольори";
            dt.Rows.InsertAt(newRow, 0);

            comboBox1.DisplayMember = "color_name";
            comboBox1.ValueMember = "color_id";
            comboBox1.DataSource = dt;
            comboBox1.SelectedIndex = 0;
        }

        private void FillSizesComboBox()
        {
            string query = "SELECT size_id, size_value FROM Size ORDER BY CAST(size_value AS UNSIGNED), size_value";
            DbConection.msCommand.CommandText = query;

            DataTable dt = new DataTable();
            DbConection.msDataAdapter.SelectCommand = DbConection.msCommand;
            DbConection.msDataAdapter.Fill(dt);

            DataRow newRow = dt.NewRow();
            newRow["size_id"] = 0;
            newRow["size_value"] = "Всі розміри";
            dt.Rows.InsertAt(newRow, 0);

            comboBox2.DisplayMember = "size_value";
            comboBox2.ValueMember = "size_id";
            comboBox2.DataSource = dt;
            comboBox2.SelectedIndex = 0;
        }

        private void FillBrandsComboBox()
        {
            string query = "SELECT brand_id, brand_name FROM Brand ORDER BY brand_name";
            DbConection.msCommand.CommandText = query;

            DataTable dt = new DataTable();
            DbConection.msDataAdapter.SelectCommand = DbConection.msCommand;
            DbConection.msDataAdapter.Fill(dt);

            DataRow newRow = dt.NewRow();
            newRow["brand_id"] = 0;
            newRow["brand_name"] = "Всі бренди";
            dt.Rows.InsertAt(newRow, 0);

            comboBox4.DisplayMember = "brand_name";
            comboBox4.ValueMember = "brand_id";
            comboBox4.DataSource = dt;
            comboBox4.SelectedIndex = 0;
        }

        private void LoadProducts()
        {
            try
            {
                if (DbConection.ConnectionDB())
                {
                    string searchText = textBox1.Text.Trim();

                    string query = @"
                SELECT 
                    cs.color_size_id, 
                    p.article_number AS 'Артикул',
                    p.product_name AS 'Назва',
                    COALESCE(b.brand_name, '') AS 'Бренд',
                    CONCAT(CAST(p.price AS CHAR), ' ₴') AS 'Ціна', 
                    c.color_name AS 'Колір',
                    s.size_value AS 'Розмір',
                    cs.stock_quantity AS 'Кількість'
                FROM Product p
                LEFT JOIN Brand b ON p.brand_id = b.brand_id
                INNER JOIN Color_Size cs ON p.product_id = cs.product_id
                INNER JOIN Color c ON cs.color_id = c.color_id
                INNER JOIN Size s ON cs.size_id = s.size_id
                WHERE cs.stock_quantity > 0";

                    DbConection.msCommand.Parameters.Clear();

                    if (!string.IsNullOrEmpty(searchText))
                    {
                        query += " AND (p.article_number LIKE @searchText OR p.product_name LIKE @searchText)";
                        DbConection.msCommand.Parameters.AddWithValue("@searchText", "%" + searchText + "%");
                    }

                    if (comboBox5.SelectedValue != null && Convert.ToInt32(comboBox5.SelectedValue) > 0)
                    {
                        query += " AND p.category_id = @categoryId";
                        DbConection.msCommand.Parameters.AddWithValue("@categoryId", comboBox5.SelectedValue);
                    }
                    if (comboBox1.SelectedValue != null && Convert.ToInt32(comboBox1.SelectedValue) > 0)
                    {
                        query += " AND c.color_id = @colorId";
                        DbConection.msCommand.Parameters.AddWithValue("@colorId", comboBox1.SelectedValue);
                    }
                    if (comboBox2.SelectedValue != null && Convert.ToInt32(comboBox2.SelectedValue) > 0)
                    {
                        query += " AND s.size_id = @sizeId";
                        DbConection.msCommand.Parameters.AddWithValue("@sizeId", comboBox2.SelectedValue);
                    }
                    if (comboBox4.SelectedValue != null && Convert.ToInt32(comboBox4.SelectedValue) > 0)
                    {
                        query += " AND b.brand_id = @brandId";
                        DbConection.msCommand.Parameters.AddWithValue("@brandId", comboBox4.SelectedValue);
                    }

                    query += " ORDER BY p.product_name, c.color_name, s.size_value";

                    DbConection.msCommand.CommandText = query;

                    DataTable table = new DataTable();
                    DbConection.msDataAdapter.SelectCommand = DbConection.msCommand;
                    DbConection.msDataAdapter.Fill(table);

                    dataGridView1.DataSource = table;

                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dataGridView1.AutoResizeColumns();


                    dataGridView1.ReadOnly = true;
                    dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dataGridView1.AllowUserToAddRows = false;

                    if (dataGridView1.Columns["color_size_id"] != null)
                    {
                        dataGridView1.Columns["color_size_id"].Visible = false;
                    }

                    if (dataGridView1.Columns["AddToCartButton"] == null)
                    {
                        DataGridViewButtonColumn btn = new DataGridViewButtonColumn();
                        btn.HeaderText = "Дія";
                        btn.Text = "+";
                        btn.Name = "AddToCartButton";
                        btn.UseColumnTextForButtonValue = true;
                        dataGridView1.Columns.Add(btn);
                    }

                    DbConection.CloseDB();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження товарів: {ex.Message}", "Помилка");
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView1.Columns[e.ColumnIndex].Name == "AddToCartButton")
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];

                if (selectedRow.Cells["Кількість"].Value == DBNull.Value || int.Parse(selectedRow.Cells["Кількість"].Value.ToString()) < 1)
                {
                    MessageBox.Show("Цей товар закінчився або його кількість недоступна.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string name = selectedRow.Cells["Назва"].Value.ToString();
                string size = selectedRow.Cells["Розмір"].Value.ToString();
                string color = selectedRow.Cells["Колір"].Value.ToString();

                DialogResult dialogResult = CustomConfirmDialog.Show(
                    $"Додати товар '{name} ({size}, {color})' у кошик в кількості 1 шт.?", "Додати товар");

                if (dialogResult == DialogResult.Yes)
                {
                    if (selectedRow.Cells["color_size_id"].Value == DBNull.Value)
                    {
                        MessageBox.Show("Неможливо визначити ID товару для додавання.", "Помилка");
                        return;
                    }
                    int colorSizeId = Convert.ToInt32(selectedRow.Cells["color_size_id"].Value);

                    string priceText = selectedRow.Cells["Ціна"].Value.ToString();
                    decimal price;

                    priceText = priceText.Replace(" ₴", "").Trim();

                    if (priceText.Contains(","))
                    {
                        priceText = priceText.Replace(',', '.');
                    }

                    if (!decimal.TryParse(priceText, NumberStyles.Currency, CultureInfo.InvariantCulture, out price))
                    {
                        MessageBox.Show("Помилка парсингу ціни. Не вдалося перетворити значення ціни на число.", "Помилка");
                        return;
                    }

                    var item = new BasketItem
                    {
                        ColorSizeId = colorSizeId,
                        ArticleNumber = selectedRow.Cells["Артикул"].Value.ToString(),
                        Name = name,
                        Color = color,
                        Size = size,
                        Price = price,
                        Quantity = 1
                    };

                    BasketManager.AddItem(item);
                    MessageBox.Show($"Товар '{name} ({size}, {color})' додано до кошика.", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    NotifyBasketChanged();
                }
            }
        }

        private void NotifyBasketChanged()
        {
            try
            {
                SellerMainForm mainForm = Application.OpenForms.OfType<SellerMainForm>().FirstOrDefault();
                mainForm?.UpdateBasketButton();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка оновлення кошика: {ex.Message}");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e) { }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            LoadProducts();
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e) { }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) { }
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e) { }

        private void button3_Click(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";

            if (comboBox5.Items.Count > 0) comboBox5.SelectedIndex = 0;
            if (comboBox1.Items.Count > 0) comboBox1.SelectedIndex = 0;
            if (comboBox2.Items.Count > 0) comboBox2.SelectedIndex = 0;
            if (comboBox4.Items.Count > 0) comboBox4.SelectedIndex = 0;

            LoadProducts();
        }

        private void panel2_Paint(object sender, PaintEventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void Products_Load(object sender, EventArgs e) { }
    }
}
