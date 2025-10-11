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
    public partial class SignUpForm : Form
    {
        public SignUpForm()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SignInForm loginForm = new SignInForm();
            loginForm.Show();
            this.Hide();
        }

        private void register_showPass_CheckedChanged(object sender, EventArgs e)
        {
            register_password.PasswordChar = register_showPass.Checked ? '\0' : '*';
            register_cPassword.PasswordChar = register_showPass.Checked ? '\0' : '*';
        }

        private void pictureBox1_Click(object sender, EventArgs e) { }
 

        private string FormatPhoneNumber(string inputPhone)
        {
            if (string.IsNullOrWhiteSpace(inputPhone))
            {
                return "";
            }

            string digitsOnly = Regex.Replace(inputPhone, @"[^\d]", "");

            if (digitsOnly.Length == 10 && digitsOnly.StartsWith("0"))
            {
                return $"+38{digitsOnly}";
            }
            else if (digitsOnly.Length == 12 && digitsOnly.StartsWith("380"))
            {
                return $"+{digitsOnly}";
            }

            return null;
        }

        private void register_btn_Click(object sender, EventArgs e)
        {
            // Збираємо дані з полів (тільки username і password)
            string username = register_username.Text.Trim();
            string password = register_password.Text.Trim();
            string confirmPassword = register_cPassword.Text.Trim();

            // Валідація username (логін)
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Введіть ім'я користувача (логін)!", "Помилка валідації",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                register_username.Focus();
                return;
            }

            if (username.Length < 3)
            {
                MessageBox.Show("Ім'я користувача має містити мінімум 3 символи!", "Помилка валідації",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                register_username.Focus();
                return;
            }

            // Валідація пароля
            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введіть пароль!", "Помилка валідації",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                register_password.Focus();
                return;
            }

            if (password.Length < 4)
            {
                MessageBox.Show("Пароль має містити мінімум 4 символи!", "Помилка валідації",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                register_password.Focus();
                return;
            }

            // Перевірка співпадіння паролів
            if (password != confirmPassword)
            {
                MessageBox.Show("Паролі не співпадають!", "Помилка валідації",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                register_cPassword.Focus();
                return;
            }

            try
            {
                if (!DbConection.ConnectionDB())
                {
                    MessageBox.Show("Не вдалося підключитися до бази даних!", "Помилка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Перевіряємо чи існує користувач
                string checkQuery = "SELECT COUNT(*) FROM Employee WHERE username=@username";
                DbConection.msCommand.CommandText = checkQuery;
                DbConection.msCommand.Parameters.Clear();
                DbConection.msCommand.Parameters.AddWithValue("@username", username);

                int count = Convert.ToInt32(DbConection.msCommand.ExecuteScalar());

                if (count > 0)
                {
                    DbConection.CloseDB();
                    MessageBox.Show("Такий логін вже існує! Оберіть інший.", "Помилка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    register_username.Focus();
                    return;
                }

                // Хешуємо пароль
                string passwordHash = PasswordHelper.HashPassword(password);

                string insertQuery = @"INSERT INTO Employee (username, password_hash, first_name, last_name, position, phone_number) 
                                      VALUES (@username, @passwordHash, '', '', @position, '')";

                DbConection.msCommand.CommandText = insertQuery;
                DbConection.msCommand.Parameters.Clear();
                DbConection.msCommand.Parameters.AddWithValue("@username", username);
                DbConection.msCommand.Parameters.AddWithValue("@passwordHash", passwordHash);
                DbConection.msCommand.Parameters.AddWithValue("@position", "Продавець");

                int rows = DbConection.msCommand.ExecuteNonQuery();
                DbConection.CloseDB();

                if (rows > 0)
                {
                    MessageBox.Show("Реєстрація успішна!\n\nБудь ласка, заповніть свої особисті дані в профілі.", "Успіх",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Ініціалізуємо сесію для новоствореного користувача
                    if (SessionManager.InitializeSession(username))
                    {
                        SellerMainForm mainForm = new SellerMainForm();
                        mainForm.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Помилка ініціалізації сесії. Спробуйте увійти вручну.", "Увага",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        SignInForm loginForm = new SignInForm();
                        loginForm.Show();
                        this.Hide();
                    }
                }
                else
                {
                    MessageBox.Show("Помилка при створенні користувача!", "Помилка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                DbConection.CloseDB();
            }
        }

        private void SignUpForm_Load(object sender, EventArgs e) { }
    }
}
