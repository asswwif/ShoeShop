using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class CustomerForm : Form
    {
        public CustomerForm()
        {
            InitializeComponent();
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

        private void close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void sales1_Load(object sender, EventArgs e) { }

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
            ReturnsForm returnForm = new ReturnsForm();
            returnForm.Show();
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

        private void button6_Click(object sender, EventArgs e)
        {
            SalesForm salesForm = new SalesForm();
            salesForm.Show();
            this.Hide();
        }

        private void sales1_Load_1(object sender, EventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }

        private void customerSelector1_Load(object sender, EventArgs e) { }
 
    }
}

