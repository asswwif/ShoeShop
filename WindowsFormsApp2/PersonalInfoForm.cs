using System;
using System.Windows.Forms;

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

            // Форматування номера телефону для відображення
            if (!string.IsNullOrEmpty(SessionManager.CurrentEmployeePhone))
                textBox3.Text = InfoValidator.FormatPhoneForDisplay(SessionManager.CurrentEmployeePhone);
            else
                textBox3.Text = "";

            if (string.IsNullOrEmpty(SessionManager.CurrentEmployeeFirstName) &&
                string.IsNullOrEmpty(SessionManager.CurrentEmployeeLastName) &&
                string.IsNullOrEmpty(SessionManager.CurrentEmployeePhone))
            {
                MessageBox.Show("Будь ласка, заповніть свої особисті дані!", "Інформація",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string newLastName = textBox1.Text.Trim();
            string newFirstName = textBox2.Text.Trim();
            string rawPhone = textBox3.Text.Trim();

            // Валідація прізвища
            if (!InfoValidator.ValidateName(newLastName, "Прізвище", out string error))
            {
                MessageBox.Show(error, "Помилка валідації", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return;
            }

            // Валідація імені
            if (!InfoValidator.ValidateName(newFirstName, "Ім'я", out error))
            {
                MessageBox.Show(error, "Помилка валідації", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Focus();
                return;
            }

            // Валідація номера телефону
            if (!InfoValidator.ValidatePhone(rawPhone, out string normalizedPhone, out error))
            {
                MessageBox.Show(error, "Помилка валідації", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox3.Focus();
                return;
            }

            // Оновлення у базі
            bool success = _employeeService.UpdatePersonalInfo(
                SessionManager.CurrentEmployeeId,
                newFirstName,
                newLastName,
                normalizedPhone
            );

            if (success)
            {
                MessageBox.Show("Ваші дані успішно оновлено!", "Успіх",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadEmployeeData();
            }
            else
            {
                MessageBox.Show("Не вдалося оновити дані. Зверніться до адміністратора.",
                    "Помилка оновлення", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
