using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class SellerMainForm : Form
    {
        public SellerMainForm()
        {
            InitializeComponent();
            UpdateBasketButton();
            this.MinimumSize = new Size(900, 500);
            this.MaximumSize = new Size(1800, 960);

            // Підписуємось на зміни кошика
            BasketManager.BasketChanged += OnBasketChanged;
        }

        // Обробник події зміни кошика
        private void OnBasketChanged(object sender, EventArgs e)
        {
            UpdateBasketButton();
        }

        public void UpdateBasketButton()
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
            // Коли форма стає видимою - оновлюємо кнопку
            if (this.Visible)
            {
                UpdateBasketButton();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Відписуємось від події при закритті форми
            BasketManager.BasketChanged -= OnBasketChanged;
            base.OnFormClosing(e);
        }

        private void label1_Click(object sender, EventArgs e) { }

        private void close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label3_Click(object sender, EventArgs e) { }

        private void button1_Click(object sender, EventArgs e)
        {
            CustomerForm customerForm = new CustomerForm();
            customerForm.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ReturnsForm returnForm = new ReturnsForm();
            returnForm.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            BasketForm basketForm = new BasketForm();
            basketForm.Owner = this;
            basketForm.Show();
            this.Hide();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        private void panel3_Paint(object sender, PaintEventArgs e) { }

        private void products1_Load(object sender, EventArgs e) { }

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

        private void button5_Click(object sender, EventArgs e)
        {
            PersonalInfoForm personalInfoForm = new PersonalInfoForm();
            personalInfoForm.Show();
            this.Hide();
        }

        private void panel1_Paint(object sender, PaintEventArgs e) { }

        private void button7_Click(object sender, EventArgs e)
        {
            SalesForm salesForm = new SalesForm();
            salesForm.Show();
            this.Hide();
        }

        private void products1_Load_1(object sender, EventArgs e) { }

    }
}