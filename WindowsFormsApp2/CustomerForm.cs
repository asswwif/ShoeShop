using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class CustomerForm : Form
    {
        public CustomerForm()
        {
            InitializeComponent();
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
                // Очищаємо сесію перед виходом
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
    }
}
