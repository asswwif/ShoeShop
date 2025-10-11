using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WindowsFormsApp2
{
    public partial class SellerMainForm : Form

    {
        public SellerMainForm()
        {
            InitializeComponent(); 
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
    }
}
