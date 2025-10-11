using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace WindowsFormsApp2
{
    public partial class PersonalInfoForm : Form
    {
        private EmployeeService _employeeService;

        public PersonalInfoForm()
        {
            InitializeComponent();
            _employeeService = new EmployeeService();
            LoadEmployeeData();
        }

        private void LoadEmployeeData()
        {
            label1.Text = $"Користувач: {SessionManager.CurrentEmployeeUsername}";
            label6.Text = $"Посада: {SessionManager.CurrentEmployeePosition}";

            textBox1.Text = SessionManager.CurrentEmployeeLastName;
            textBox2.Text = SessionManager.CurrentEmployeeFirstName;

            // Форматуємо номер телефону для відображення (якщо він є)
            if (!string.IsNullOrEmpty(SessionManager.CurrentEmployeePhone))
            {
                textBox3.Text = FormatPhoneNumber(SessionManager.CurrentEmployeePhone);
            }
            else
            {
                textBox3.Text = ""; 
            }

            // Якщо дані порожні, показуємо підказку користувачу
            if (string.IsNullOrEmpty(SessionManager.CurrentEmployeeFirstName) &&
                string.IsNullOrEmpty(SessionManager.CurrentEmployeeLastName) &&
                string.IsNullOrEmpty(SessionManager.CurrentEmployeePhone))
            {
                MessageBox.Show("Будь ласка, заповніть свої особисті дані!", "Інформація",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private string FormatPhoneNumber(string inputPhone)
        {
            if (string.IsNullOrWhiteSpace(inputPhone))
            {
                return ""; 
            }

            // Видаляємо всі нецифрові символи
            string digitsOnly = Regex.Replace(inputPhone, @"[^\d]", "");

            if (digitsOnly.Length == 10 && digitsOnly.StartsWith("0"))
            {
                return $"+38{digitsOnly}";
            }
            else if (digitsOnly.Length == 12 && digitsOnly.StartsWith("380"))
            {
                return $"+{digitsOnly}";
            }
            else
            {
                // Некоректний формат
                return null;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Збір даних з полів
            string newLastName = textBox1.Text.Trim();
            string newFirstName = textBox2.Text.Trim();
            string rawPhone = textBox3.Text.Trim(); 

            // Валідація ім'я
            if (string.IsNullOrEmpty(newFirstName))
            {
                MessageBox.Show("Поле 'Ім'я' не може бути порожнім!", "Помилка валідації",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Focus();
                return;
            }

            // Валідація прізвища
            if (string.IsNullOrEmpty(newLastName))
            {
                MessageBox.Show("Поле 'Прізвище' не може бути порожнім!", "Помилка валідації",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return;
            }

            // Валідація номеру телефону
            if (string.IsNullOrWhiteSpace(rawPhone))
            {
                MessageBox.Show("Поле 'Номер телефону' не може бути порожнім!", "Помилка валідації",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox3.Focus();
                return;
            }

            // Форматування та валідація формату номера
            string newPhone = FormatPhoneNumber(rawPhone);

            if (newPhone == null)
            {
                MessageBox.Show("Введіть коректний номер телефону!\n\nПриклади:\n• 0501234567\n• +380501234567\n• 380501234567",
                    "Помилка валідації", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox3.Focus();
                return;
            }

            // Викликаємо сервіс для оновлення БД
            bool success = _employeeService.UpdatePersonalInfo(
                SessionManager.CurrentEmployeeId,
                newFirstName,
                newLastName,
                newPhone // Передаємо відформатований номер
            );

            if (success)
            {
                MessageBox.Show("Ваші дані успішно оновлено!", "Успіх",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Оновлюємо відображення на формі після успішного збереження
                LoadEmployeeData();
            }
            else
            {
                MessageBox.Show("Не вдалося оновити дані. Зверніться до адміністратора.", "Помилка оновлення",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            SellerMainForm mainForm = new SellerMainForm();
            mainForm.Show();
            this.Hide();
        }

        private void button4_Click_1(object sender, EventArgs e)
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

        private void panel3_Paint(object sender, PaintEventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void label6_Click(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void textBox3_TextChanged(object sender, EventArgs e) { }

        private void close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void panel2_Paint(object sender, PaintEventArgs e) { }
    }
}