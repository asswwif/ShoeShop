using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class ReturnsForm : Form
    {
        private ReturnService _returnService;

        public ReturnsForm()
        {
            InitializeComponent();
            _returnService = new ReturnService();
            SetupDateTimePickers();
            SetupDataGridView();
            LoadReturnsData();
            UpdateBasketButton();
            this.MinimumSize = new Size(900, 500);
            this.MaximumSize = new Size(1800, 960);
        }

        private void UpdateBasketButton()
        {
            try
            {
                int itemCount = BasketManager.GetItems().Count;
                if (itemCount > 0)
                {
                    // Кошик не порожній, то підсвічуємо
                    button5.BackColor = Color.Maroon;
                    button5.Text = $"Кошик ({itemCount})";
                    button5.Font = new Font("Arial", 12, FontStyle.Bold);
                    button5.ForeColor = Color.White;
                }
                else
                {
                    // Кошик порожній - стандартний вигляд
                    button5.BackColor = Color.LightPink;
                    button5.Text = "Кошик";
                    button5.Font = new Font("Arial", 12, FontStyle.Bold);
                    button5.ForeColor = Color.White;
                }
            }
            catch (Exception ex)
            {
                // Якщо помилка, то просто ігноруємо
                Console.WriteLine($"Помилка оновлення кошика: {ex.Message}");
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (this.Visible)
            {
                UpdateBasketButton();
            }
        }

        private void SetupDateTimePickers()
        {
            dateTimePicker1.Format = DateTimePickerFormat.Short;
            dateTimePicker2.Format = DateTimePickerFormat.Short;
            dateTimePicker1.Value = DateTime.Today.AddMonths(-1);
            dateTimePicker2.Value = DateTime.Today;
        }

        private void SetupDataGridView()
        {
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.ReadOnly = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.RowTemplate.MinimumHeight = 50;
        }

        private void LoadReturnsData()
        {
            try
            {
                DateTime startDate = dateTimePicker1.Value.Date;
                DateTime endDate = dateTimePicker2.Value.Date.AddDays(1);
                DataTable returnsData = _returnService.GetReturnsDataByDateRange(startDate, endDate);
                dataGridView1.DataSource = returnsData;

                if (returnsData.Rows.Count == 0)
                {
                    MessageBox.Show($"За період з {startDate:dd.MM.yyyy} по {dateTimePicker2.Value.Date:dd.MM.yyyy} повернень не знайдено.",
                        "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                ConfigureColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не вдалося відобразити дані повернень: {ex.Message}", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureColumns()
        {
            if (dataGridView1.Columns.Contains("return_id"))
                dataGridView1.Columns["return_id"].HeaderText = "ID Повернення";
            if (dataGridView1.Columns.Contains("return_datetime"))
                dataGridView1.Columns["return_datetime"].HeaderText = "Дата/Час";
            if (dataGridView1.Columns.Contains("sale_id"))
                dataGridView1.Columns["sale_id"].HeaderText = "Чек продажу";
            if (dataGridView1.Columns.Contains("product_name"))
                dataGridView1.Columns["product_name"].HeaderText = "Товар";
            if (dataGridView1.Columns.Contains("quantity"))
                dataGridView1.Columns["quantity"].HeaderText = "Кількість";
            if (dataGridView1.Columns.Contains("refund_amount"))
                dataGridView1.Columns["refund_amount"].HeaderText = "Сума повернення (грн)";
            if (dataGridView1.Columns.Contains("reason"))
                dataGridView1.Columns["reason"].HeaderText = "Причина";
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePicker1.Value > dateTimePicker2.Value)
            {
                dateTimePicker2.Value = dateTimePicker1.Value;
            }
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePicker2.Value < dateTimePicker1.Value)
            {
                dateTimePicker1.Value = dateTimePicker2.Value;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            LoadReturnsData();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult result = CustomConfirmDialog.Show(
                "Ви дійсно хочете вийти з програми?",
                "Підтвердження");

            if (result == DialogResult.Yes)
            {
                SessionManager.ClearSession();
                SignInForm signInForm = new SignInForm();
                signInForm.Show();
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SellerMainForm mainForm = Application.OpenForms.OfType<SellerMainForm>().FirstOrDefault();
            if (mainForm != null)
            {
                mainForm.UpdateBasketButton();
                mainForm.Show();
            }
            else
            {
                mainForm = new SellerMainForm();
                mainForm.Show();
            }
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CustomerForm customerForm = new CustomerForm();
            customerForm.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            BasketForm bascetForm = new BasketForm();
            bascetForm.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            PersonalInfoForm personalInfoForm = new PersonalInfoForm();
            personalInfoForm.Show();
            this.Hide();
        }

        private void close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ReturnProcessingForm returnProcessingForm = new ReturnProcessingForm();
            returnProcessingForm.Show();
            this.Hide();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SalesForm salesForm = new SalesForm();
            salesForm.Show();
            this.Hide();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void panel4_Paint(object sender, PaintEventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("Немає даних для експорту.", "Увага",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel файли (*.xlsx)|*.xlsx";
                saveDialog.FilterIndex = 1;
                saveDialog.FileName = $"Звіт_Повернення_{DateTime.Now:yyyy-MM-dd_HH-mm}";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    DataTable data = _returnService.GetReturnsDataForExport(
                        dateTimePicker1.Value.Date,
                        dateTimePicker2.Value.Date.AddDays(1)
                    );

                    using (ExcelReportService excelService = new ExcelReportService())
                    {
                        excelService.GenerateReturnsReport(
                            data,
                            saveDialog.FileName,
                            dateTimePicker1.Value.Date,
                            dateTimePicker2.Value.Date
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка експорту в Excel: {ex.Message}", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void panel3_Paint(object sender, PaintEventArgs e) { }

    }
}
