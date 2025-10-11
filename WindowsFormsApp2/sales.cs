using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Text.RegularExpressions;

namespace WindowsFormsApp2
{
    public partial class sales : UserControl
    {
        private CustomerService customerService;
        private DiscountCardService discountCardService;
        private List<Customer> customers;
        private List<Customer> filteredCustomers; 
        private Customer selectedCustomer;

        public sales()
        {
            InitializeComponent();
            customerService = new CustomerService();
            discountCardService = new DiscountCardService();
            LoadCustomersData();
        }

        private void LoadCustomersData()
        {
            try
            {
                customers = customerService.GetAllCustomersWithDiscounts();
                filteredCustomers = new List<Customer>(customers); // Ініціалізуємо відфільтровані клієнти
                UpdateDataGridViewWithFiltered();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження даних: {ex.Message}", "Помилка",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateDataGridView()
        {
            if (customers == null) return;

            // Створюємо DataTable для відображення
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("ID", typeof(int));
            dataTable.Columns.Add("Ім'я", typeof(string));
            dataTable.Columns.Add("Прізвище", typeof(string));
            dataTable.Columns.Add("Телефон", typeof(string));
            dataTable.Columns.Add("Дата народження", typeof(string));
            dataTable.Columns.Add("Знижка %", typeof(int));

            // Заповнюємо DataTable
            foreach (var customer in customers)
            {
                string birthDateString = "";
                if (customer.BirthDate != DateTime.MinValue && customer.BirthDate.Year > 1900)
                {
                    birthDateString = customer.BirthDate.ToString("dd.MM.yyyy");
                }

                dataTable.Rows.Add(
                  customer.CustomerId,
                  customer.FirstName,
                  customer.LastName,
                  customer.PhoneNumber,
                  birthDateString,
                  customer.DiscountPercent
                );
            }

            dataGridView1.DataSource = dataTable;

            // Налаштування відображення
            if (dataGridView1.Columns["ID"] != null)
                dataGridView1.Columns["ID"].Visible = false;

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void UpdateDataGridViewWithFiltered()
        {
            var customersToDisplay = filteredCustomers ?? customers;

            if (customersToDisplay == null) return;

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("ID", typeof(int));
            dataTable.Columns.Add("Ім'я", typeof(string));
            dataTable.Columns.Add("Прізвище", typeof(string));
            dataTable.Columns.Add("Телефон", typeof(string));
            dataTable.Columns.Add("Дата народження", typeof(string));
            dataTable.Columns.Add("Знижка %", typeof(int));

            foreach (var customer in customersToDisplay)
            {
                string birthDateString = "";
                if (customer.BirthDate != DateTime.MinValue && customer.BirthDate.Year > 1900)
                {
                    birthDateString = customer.BirthDate.ToString("dd.MM.yyyy");
                }

                dataTable.Rows.Add(
                    customer.CustomerId,
                    customer.FirstName,
                    customer.LastName,
                    customer.PhoneNumber,
                    birthDateString,
                    customer.DiscountPercent
                );
            }

            dataGridView1.DataSource = dataTable;

            // Налаштування відображення
            if (dataGridView1.Columns["ID"] != null)
                dataGridView1.Columns["ID"].Visible = false;

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        // Метод для фільтрації клієнтів
        private void FilterCustomers(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                // Якщо пошукове поле порожнє, показуємо всіх клієнтів
                filteredCustomers = new List<Customer>(customers);
            }
            else
            {
                searchText = searchText.ToLower().Trim();

                // Фільтруємо клієнтів за ім'ям, прізвищем або телефоном
                filteredCustomers = customers.Where(c =>
                    c.FirstName.ToLower().Contains(searchText) ||
                    c.LastName.ToLower().Contains(searchText) ||
                    c.PhoneNumber.Contains(searchText) ||
                    c.FullName.ToLower().Contains(searchText)
                ).ToList();
            }

            UpdateDataGridViewWithFiltered();
        }

        // Метод для форматування номера телефону 
        private string FormatPhoneNumber(string inputPhone)
        {
            if (string.IsNullOrWhiteSpace(inputPhone))
            {
                return "";
            }

            // Видаляємо всі нецифрові символи
            string digitsOnly = Regex.Replace(inputPhone, @"[^\d]", "");

            // Перевіряємо різні формати
            if (digitsOnly.Length == 10 && digitsOnly.StartsWith("0"))
            {
                return $"+38{digitsOnly}";
            }
            else if (digitsOnly.Length == 12 && digitsOnly.StartsWith("380"))
            {
                return $"+{digitsOnly}";
            }
            else if (digitsOnly.Length == 9)
            {
                return $"+380{digitsOnly}";
            }

            return null;
        }

        // Валідація введених даних
        private bool ValidateInput()
        {
            // Перевірка імені
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Поле 'Ім'я' не може бути порожнім!", "Помилка валідації",
                       MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return false;
            }

            // Перевірка прізвища
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Поле 'Прізвище' не може бути порожнім!", "Помилка валідації",
                       MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Focus();
                return false;
            }

            // Перевірка телефону
            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Поле 'Номер телефону' не може бути порожнім!", "Помилка валідації",
                       MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox3.Focus();
                return false;
            }

            // Валідація та форматування номера телефону
            string formattedPhone = FormatPhoneNumber(textBox3.Text.Trim());
            if (formattedPhone == null)
            {
                MessageBox.Show("Введіть коректний номер телефону!\n\nПриклади:\n• 0501234567\n• +380501234567\n• 380501234567",
                       "Помилка валідації", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox3.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(textBox4.Text))
            {
                if (!DateTime.TryParseExact(textBox4.Text, "dd.MM.yyyy",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthDate))
                {
                    MessageBox.Show("Введіть дату у форматі ДД.ММ.РРРР (наприклад: 15.04.1985)\nабо залиште поле порожнім",
                           "Помилка валідації", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox4.Focus();
                    return false;
                }

                if (birthDate > DateTime.Now)
                {
                    MessageBox.Show("Дата народження не може бути в майбутньому!", "Помилка валідації",
                           MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox4.Focus();
                    return false;
                }

                if (birthDate < new DateTime(1900, 1, 1))
                {
                    MessageBox.Show("Введіть реалістичну дату народження!", "Помилка валідації",
                           MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox4.Focus();
                    return false;
                }
            }

            return true;
        }

        private DateTime? GetBirthDate()
        {
            if (string.IsNullOrWhiteSpace(textBox4.Text))
            {
                return null;
            }

            if (DateTime.TryParseExact(textBox4.Text, "dd.MM.yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthDate))
            {
                return birthDate;
            }

            return null;
        }

        // Метод для оновлення даних
        public void RefreshData()
        {
            LoadCustomersData();
            if (!string.IsNullOrWhiteSpace(textBox5.Text))
            {
                FilterCustomers(textBox5.Text);
            }
        }

        // Метод для отримання вибраного клієнта
        private Customer GetSelectedCustomer()
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int customerId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);
                return customers?.FirstOrDefault(c => c.CustomerId == customerId);
            }
            return null;
        }

        // Метод для очищення форми
        private void ClearForm()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            selectedCustomer = null;
        }

        // Метод для заповнення форми даними клієнта
        private void FillForm(Customer customer)
        {
            if (customer != null)
            {
                textBox1.Text = customer.FirstName;
                textBox2.Text = customer.LastName;
                textBox3.Text = customer.PhoneNumber;

                if (customer.BirthDate != DateTime.MinValue && customer.BirthDate.Year > 1900)
                {
                    textBox4.Text = customer.BirthDate.ToString("dd.MM.yyyy");
                }
                else
                {
                    textBox4.Text = "";
                }

                selectedCustomer = customer;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                Customer customer = GetSelectedCustomer();
                if (customer != null)
                {
                    FillForm(customer);
                }
            }
        }

        // Кнопка Додати
        private void button1_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                DateTime? birthDate = GetBirthDate();

                // Форматуємо номер телефону перед збереженням
                string formattedPhone = FormatPhoneNumber(textBox3.Text.Trim());

                Customer newCustomer = new Customer
                {
                    FirstName = textBox1.Text.Trim(),
                    LastName = textBox2.Text.Trim(),
                    PhoneNumber = formattedPhone, // Зберігаємо відформатований номер
                    BirthDate = birthDate ?? DateTime.MinValue
                };

                if (customerService.AddCustomer(newCustomer))
                {
                    MessageBox.Show("Клієнта успішно додано!", "Успіх",
                           MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshData();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Помилка при додаванні клієнта!", "Помилка",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка додавання: {ex.Message}", "Помилка",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Кнопка Оновити
        private void button2_Click(object sender, EventArgs e)
        {
            if (selectedCustomer == null)
            {
                MessageBox.Show("Оберіть клієнта зі списку для оновлення!", "Увага",
                       MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateInput()) return;

            try
            {
                DateTime? birthDate = GetBirthDate();

                // Форматуємо номер телефону перед збереженням
                string formattedPhone = FormatPhoneNumber(textBox3.Text.Trim());

                selectedCustomer.FirstName = textBox1.Text.Trim();
                selectedCustomer.LastName = textBox2.Text.Trim();
                selectedCustomer.PhoneNumber = formattedPhone; // Зберігаємо відформатований номер
                selectedCustomer.BirthDate = birthDate ?? DateTime.MinValue;

                if (customerService.UpdateCustomer(selectedCustomer))
                {
                    MessageBox.Show("Дані клієнта успішно оновлено!", "Успіх",
                           MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshData();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Помилка при оновленні даних!", "Помилка",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка оновлення: {ex.Message}", "Помилка",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Кнопка Видалити
        private void button3_Click(object sender, EventArgs e)
        {
            Customer customer = GetSelectedCustomer();
            if (customer == null)
            {
                MessageBox.Show("Оберіть клієнта зі списку для видалення!", "Увага",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string message = $"Ви впевнені, що хочете видалити клієнта:\n{customer.FullName}?\n\nЦю дію неможливо скасувати!";
            string title = "Підтвердження видалення";

            DialogResult result = CustomConfirmDialog.Show(message, title);

            if (result == DialogResult.Yes)
            {
                try
                {
                    if (customerService.DeleteCustomer(customer.CustomerId))
                    {
                        MessageBox.Show("Клієнта успішно видалено!", "Успіх",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                        RefreshData();
                        ClearForm();
                    }
                    else
                    {
                        MessageBox.Show("Помилка при видаленні клієнта! Можливо, клієнт не знайдений або існують пов'язані дані.", "Помилка",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка видалення: {ex.Message}", "Помилка",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Кнопка Пошук 
        private void button4_Click(object sender, EventArgs e)
        {
            // Перевірка чи поле пошуку не порожнє
            if (string.IsNullOrWhiteSpace(textBox5.Text))
            {
                MessageBox.Show("Введіть текст для пошуку!", "Увага",
                       MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox5.Focus();
                return;
            }

            FilterCustomers(textBox5.Text);

            // Повідомлення про результати пошуку
            if (filteredCustomers != null && filteredCustomers.Count == 0)
            {
                MessageBox.Show("Клієнтів за вказаним запитом не знайдено!", "Результат пошуку",
                       MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (filteredCustomers != null)
            {
                MessageBox.Show($"Знайдено клієнтів: {filteredCustomers.Count}", "Результат пошуку",
                       MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Кнопка додати дисконтну карту
        private void button5_Click(object sender, EventArgs e)
        {
            Customer customer = GetSelectedCustomer();
            if (customer == null)
            {
                MessageBox.Show("Оберіть клієнта зі списку для додавання дисконтної картки!", "Увага",
                       MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (customer.DiscountPercent > 0)
            {
                MessageBox.Show($"У клієнта {customer.FullName} вже є дисконтна картка зі знижкою {customer.DiscountPercent}%!",
                       "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DiscountCardForm discountForm = new DiscountCardForm(customer);
            if (discountForm.ShowDialog() == DialogResult.OK)
            {
                RefreshData();
                MessageBox.Show("Дані оновлено!", "Інформація",
                       MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Кнопка Очистити 
        private void button6_Click(object sender, EventArgs e)
        {
            textBox5.Clear();
            filteredCustomers = new List<Customer>(customers);
            UpdateDataGridViewWithFiltered();

            MessageBox.Show($"Фільтр скинуто. Показано всіх клієнтів ({customers.Count})", "Інформація",
                   MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void textBox5_TextChanged(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click_1(object sender, EventArgs e) { }
        private void panel2_Paint(object sender, PaintEventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void textBox3_TextChanged(object sender, EventArgs e) { }
        private void textBox4_TextChanged(object sender, EventArgs e) { }
        private void label1_Click_1(object sender, EventArgs e) { }
        private void textBox4_TextChanged_1(object sender, EventArgs e) { }
        private void sales_Load(object sender, EventArgs e) { }
    }
}