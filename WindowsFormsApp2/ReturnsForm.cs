using System;
using System.Data;
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
        }

        private void SetupDateTimePickers()
        {
            dateTimePicker1.Format = DateTimePickerFormat.Short;
            dateTimePicker2.Format = DateTimePickerFormat.Short;

            // За замовчуванням показуємо повернення за останній місяць
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

                // Отримуємо дані з фільтрацією по даті
                DataTable returnsData = _returnService.GetReturnsDataByDateRange(startDate, endDate);

                dataGridView1.DataSource = returnsData;

                // Перевірка, чи є дані
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
            SellerMainForm mainForm = new SellerMainForm();
            mainForm.Show();
            this.Hide();
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
    }
}