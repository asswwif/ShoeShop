using System;
using System.Data;
using System.Windows.Forms;
using System.Drawing; 

namespace WindowsFormsApp2
{
    public partial class ReturnProcessingForm : Form
    {
        private ReturnService _returnService;

        private int _currentEmployeeId = 1;

        public ReturnProcessingForm()
        {
            InitializeComponent();
            _returnService = new ReturnService();
            SetupDataGridView();
            this.MinimumSize = new Size(900, 500);
            this.MaximumSize = new Size(1800, 960);
        }

        private void SetupDataGridView()
        {
            dataGridView1.DataSource = null;
            dataGridView1.Columns.Clear();
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AllowUserToAddRows = false;

            dataGridView1.Columns.Add(new DataGridViewCheckBoxColumn
            {
                HeaderText = "Повернути",
                Name = "colReturn",
                Width = 70
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Товар", Name = "colProductName", DataPropertyName = "product_name", ReadOnly = true });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Колір", Name = "colColor", DataPropertyName = "color_name", ReadOnly = true });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Розмір", Name = "colSize", DataPropertyName = "size_value", ReadOnly = true });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "К-ть у чеку", Name = "colQtySaled", DataPropertyName = "product_quantity", ReadOnly = true });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Ціна", Name = "colPrice", DataPropertyName = "unit_price", ReadOnly = true });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "К-ть для повернення",
                Name = "colQtyReturn",
                DefaultCellStyle = new DataGridViewCellStyle { NullValue = "0", BackColor = Color.LightYellow },
                Width = 120
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Причина повернення",
                Name = "colReason",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.LightYellow }
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSaleDetailId", DataPropertyName = "sale_detail_id", Visible = false });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "colColorSizeId", DataPropertyName = "color_size_id", Visible = false });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox1.Text, out int saleId) || saleId <= 0)
            {
                MessageBox.Show("Будь ласка, введіть коректний номер продажу.", "Помилка вводу", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataTable details = _returnService.GetSaleDetailsForReturn(saleId);

            if (details.Rows.Count == 0)
            {
                MessageBox.Show($"Продаж №{saleId} не знайдено, або він не містить товарів.", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetupDataGridView();
                return;
            }

            dataGridView1.DataSource = details;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.DataSource == null || dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Спочатку завантажте деталі продажу.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool itemsToReturn = false;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                // Перевіряємо, чи позначено хоча б один товар
                if (row.Cells["colReturn"].Value is true)
                {
                    itemsToReturn = true;
                    break;
                }
            }

            if (!itemsToReturn)
            {
                MessageBox.Show("Оберіть хоча б один товар для повернення.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool allSuccess = true;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["colReturn"].Value is true)
                {
                    // Збір та валідація введених даних
                    if (!int.TryParse(row.Cells["colQtyReturn"].Value?.ToString(), out int qtyReturn) || qtyReturn <= 0)
                    {
                        MessageBox.Show($"Введіть коректну кількість для повернення в рядку з товаром '{row.Cells["colProductName"].Value}'.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        allSuccess = false;
                        break;
                    }

                    int qtySaled = Convert.ToInt32(row.Cells["colQtySaled"].Value);
                    if (qtyReturn > qtySaled)
                    {
                        MessageBox.Show($"Неможливо повернути {qtyReturn} одиниць товару '{row.Cells["colProductName"].Value}'. В чеку було лише {qtySaled}.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        allSuccess = false;
                        break;
                    }

                    int saleDetailId = Convert.ToInt32(row.Cells["colSaleDetailId"].Value);
                    int colorSizeId = Convert.ToInt32(row.Cells["colColorSizeId"].Value);
                    string reason = row.Cells["colReason"].Value?.ToString() ?? "Не вказано";

                    if (!_returnService.ProcessReturn(saleDetailId, _currentEmployeeId, colorSizeId, qtyReturn, reason))
                    {
                        allSuccess = false;
                        break;
                    }
                }
            }

            if (allSuccess)
            {
                MessageBox.Show("Повернення успішно оформлено! Склад оновлено.", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Очищуємо форму після успішного оформлення
                textBox1.Clear();
                SetupDataGridView(); 
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ReturnsForm returnForm = new ReturnsForm();
            returnForm.Show();
            this.Hide();
        }
 
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        private void close_Click(object sender, EventArgs e)
        {
            // Вихід із програми
            Application.Exit();
        }

        private void panel2_Paint(object sender, PaintEventArgs e) { }
    }
}