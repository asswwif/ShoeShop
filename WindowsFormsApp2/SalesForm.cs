using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class SalesForm : Form
    {
        private SalesService _salesService;

        public SalesForm()
        {
            InitializeComponent();
            _salesService = new SalesService();
            dateTimePicker1.Format = DateTimePickerFormat.Short;
            dateTimePicker2.Format = DateTimePickerFormat.Short;
            dateTimePicker1.Value = DateTime.Today.AddMonths(-1);
            dateTimePicker2.Value = DateTime.Today;
            SetupDataGridView();
            LoadSalesHistory();
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
                    button3.BackColor = Color.Maroon;
                    button3.Text = $"Кошик ({itemCount})";
                    button3.Font = new Font("Arial", 12, FontStyle.Bold);
                    button3.ForeColor = Color.White;
                }
                else
                {
                    // Кошик порожній - стандартний вигляд
                    button3.BackColor = Color.LightPink;
                    button3.Text = "Кошик";
                    button3.Font = new Font("Arial", 12, FontStyle.Bold);
                    button3.ForeColor = Color.White;
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

        private void SetupDataGridView()
        {
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.ReadOnly = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.RowTemplate.MinimumHeight = 50;
        }

        private void LoadSalesHistory()
        {
            try
            {
                DateTime startDate = dateTimePicker1.Value.Date;
                DateTime endDate = dateTimePicker2.Value.Date.AddDays(1).AddSeconds(-1);

                DataTable salesData = _salesService.GetSalesHistory(startDate, endDate);

                dataGridView1.DataSource = salesData;

                if (dataGridView1.Columns.Contains("Товари (Деталі)"))
                {
                    dataGridView1.Columns["Товари (Деталі)"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                }

                if (dataGridView1.Columns.Contains("sale_id"))
                    dataGridView1.Columns["sale_id"].HeaderText = "Чек";
                if (dataGridView1.Columns.Contains("sale_datetime"))
                    dataGridView1.Columns["sale_datetime"].HeaderText = "Дата/Час";
                if (dataGridView1.Columns.Contains("CustomerName"))
                    dataGridView1.Columns["CustomerName"].HeaderText = "Клієнт";
                if (dataGridView1.Columns.Contains("EmployeeName"))
                    dataGridView1.Columns["EmployeeName"].HeaderText = "Працівник";
                if (dataGridView1.Columns.Contains("sale_total"))
                    dataGridView1.Columns["sale_total"].HeaderText = "Сума (грн)";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження даних: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private void button3_Click(object sender, EventArgs e)
        {
            BasketForm bascetForm = new BasketForm();
            bascetForm.Show();
            this.Hide();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            PersonalInfoForm personalInfoForm = new PersonalInfoForm();
            personalInfoForm.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ReturnsForm returnForm = new ReturnsForm();
            returnForm.Show();
            this.Hide();
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

        private void button7_Click(object sender, EventArgs e) { LoadSalesHistory(); }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void panel4_Paint(object sender, PaintEventArgs e) { }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                // Перевірка чи є дані
                if (dataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("Немає даних для експорту. Спочатку завантажте дані.", "Увага",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Діалог збереження файлу з вибором формату
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel файли (*.xlsx)|*.xlsx|Word документи (*.docx)|*.docx|PDF файли (*.pdf)|*.pdf";
                saveDialog.FilterIndex = 1;
                saveDialog.FileName = $"Звіт_Продажі_{DateTime.Now:yyyy-MM-dd_HH-mm}";
                saveDialog.Title = "Зберегти звіт";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    Cursor = Cursors.WaitCursor;

                    try
                    {
                        var data = _salesService.GetSalesHistoryForExport(
                            dateTimePicker1.Value.Date,
                            dateTimePicker2.Value.Date
                        );

                        // Перевірка чи отримали дані
                        if (data.Rows.Count == 0)
                        {
                            MessageBox.Show("Немає даних для експорту за обраний період.", "Увага",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            Cursor = Cursors.Default;
                            return;
                        }

                        // Визначаємо формат за розширенням файлу
                        string extension = System.IO.Path.GetExtension(saveDialog.FileName).ToLower();

                        if (extension == ".xlsx")
                        {
                            // Генеруємо звіт Excel
                            using (ExcelReportService excelService = new ExcelReportService())
                            {
                                excelService.GenerateSalesReport(
                                    data,
                                    saveDialog.FileName,
                                    dateTimePicker1.Value.Date,
                                    dateTimePicker2.Value.Date
                                );
                            }
                        }
                        else if (extension == ".docx" || extension == ".pdf")
                        {
                            // Генеруємо звіт Word
                            using (WordReportService wordService = new WordReportService())
                            {
                                wordService.GenerateSalesReport(
                                    data,
                                    saveDialog.FileName,
                                    dateTimePicker1.Value.Date,
                                    dateTimePicker2.Value.Date
                                );
                            }
                        }

                        Cursor = Cursors.Default;

                        System.Threading.Thread.Sleep(500);

                        // Перевіряємо існування файлу
                        if (!System.IO.File.Exists(saveDialog.FileName))
                        {
                            MessageBox.Show("Файл було створено, але не вдалося підтвердити його збереження.", "Увага",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Питаємо чи відкрити файл
                        DialogResult result = CustomConfirmDialog.Show(
                            "Звіт успішно створено! Відкрити файл?",
                            "Успіх"
                        );

                        if (result == DialogResult.Yes)
                        {
                            try
                            {
                                System.Threading.Thread.Sleep(300);

                                var startInfo = new System.Diagnostics.ProcessStartInfo
                                {
                                    FileName = saveDialog.FileName,
                                    UseShellExecute = true
                                };
                                System.Diagnostics.Process.Start(startInfo);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Файл збережено, але не вдалося його відкрити: {ex.Message}\n\nВи можете знайти файл за шляхом:\n{saveDialog.FileName}",
                                    "Інформація",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Cursor = Cursors.Default;
                        MessageBox.Show($"Помилка під час створення звіту: {ex.Message}\n\nДеталі: {ex.StackTrace}",
                            "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show($"Помилка експорту звіту: {ex.Message}\n\nДеталі: {ex.StackTrace}",
                    "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

