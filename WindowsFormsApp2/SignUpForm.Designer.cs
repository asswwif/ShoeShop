namespace WindowsFormsApp2
{
    partial class SignUpForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.register_btn = new System.Windows.Forms.Button();
            this.register_showPass = new System.Windows.Forms.CheckBox();
            this.register_password = new System.Windows.Forms.TextBox();
            this.register_username = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.close = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.register_loginBtn = new System.Windows.Forms.Button();
            this.register_cPassword = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // register_btn
            // 
            this.register_btn.BackColor = System.Drawing.Color.LightPink;
            this.register_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.register_btn.Font = new System.Drawing.Font("Arial Rounded MT Bold", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.register_btn.ForeColor = System.Drawing.Color.White;
            this.register_btn.Location = new System.Drawing.Point(694, 738);
            this.register_btn.Margin = new System.Windows.Forms.Padding(6);
            this.register_btn.Name = "register_btn";
            this.register_btn.Size = new System.Drawing.Size(250, 56);
            this.register_btn.TabIndex = 17;
            this.register_btn.Text = "Зареєструватися";
            this.register_btn.UseVisualStyleBackColor = false;
            this.register_btn.Click += new System.EventHandler(this.register_btn_Click);
            // 
            // register_showPass
            // 
            this.register_showPass.AutoSize = true;
            this.register_showPass.Font = new System.Drawing.Font("Arial", 7.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.register_showPass.Location = new System.Drawing.Point(692, 658);
            this.register_showPass.Margin = new System.Windows.Forms.Padding(6);
            this.register_showPass.Name = "register_showPass";
            this.register_showPass.Size = new System.Drawing.Size(205, 28);
            this.register_showPass.TabIndex = 16;
            this.register_showPass.Text = "Показати пароль";
            this.register_showPass.UseVisualStyleBackColor = true;
            this.register_showPass.CheckedChanged += new System.EventHandler(this.register_showPass_CheckedChanged);
            // 
            // register_password
            // 
            this.register_password.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.register_password.Location = new System.Drawing.Point(684, 442);
            this.register_password.Margin = new System.Windows.Forms.Padding(6);
            this.register_password.Name = "register_password";
            this.register_password.PasswordChar = '*';
            this.register_password.Size = new System.Drawing.Size(628, 44);
            this.register_password.TabIndex = 15;
            // 
            // register_username
            // 
            this.register_username.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.register_username.Location = new System.Drawing.Point(686, 300);
            this.register_username.Margin = new System.Windows.Forms.Padding(6);
            this.register_username.Name = "register_username";
            this.register_username.Size = new System.Drawing.Size(628, 44);
            this.register_username.TabIndex = 14;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(692, 387);
            this.label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(124, 36);
            this.label4.TabIndex = 13;
            this.label4.Text = "Пароль";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(688, 244);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(256, 36);
            this.label3.TabIndex = 12;
            this.label3.Text = "Ім\'я користувача";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 13.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(692, 140);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(221, 44);
            this.label2.TabIndex = 11;
            this.label2.Text = "Реєстрація";
            // 
            // close
            // 
            this.close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.close.AutoSize = true;
            this.close.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.close.Location = new System.Drawing.Point(1320, 15);
            this.close.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(39, 37);
            this.close.TabIndex = 10;
            this.close.Text = "X";
            this.close.Click += new System.EventHandler(this.label1_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.LightPink;
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.register_loginBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(616, 933);
            this.panel1.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial Rounded MT Bold", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(206, 490);
            this.label6.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(214, 44);
            this.label6.TabIndex = 9;
            this.label6.Text = "ShoeStore";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::WindowsFormsApp2.Properties.Resources.icons8_online_store_100;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox1.Location = new System.Drawing.Point(215, 242);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(6);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(200, 192);
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 13.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(180, 772);
            this.label5.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(277, 44);
            this.label5.TabIndex = 9;
            this.label5.Text = "Вже є акаунт?";
            // 
            // register_loginBtn
            // 
            this.register_loginBtn.BackColor = System.Drawing.Color.LightPink;
            this.register_loginBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.register_loginBtn.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.register_loginBtn.ForeColor = System.Drawing.Color.White;
            this.register_loginBtn.Location = new System.Drawing.Point(24, 840);
            this.register_loginBtn.Margin = new System.Windows.Forms.Padding(6);
            this.register_loginBtn.Name = "register_loginBtn";
            this.register_loginBtn.Size = new System.Drawing.Size(564, 56);
            this.register_loginBtn.TabIndex = 9;
            this.register_loginBtn.Text = "Увійти";
            this.register_loginBtn.UseVisualStyleBackColor = false;
            this.register_loginBtn.Click += new System.EventHandler(this.button2_Click);
            // 
            // register_cPassword
            // 
            this.register_cPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.register_cPassword.Location = new System.Drawing.Point(684, 583);
            this.register_cPassword.Margin = new System.Windows.Forms.Padding(6);
            this.register_cPassword.Name = "register_cPassword";
            this.register_cPassword.PasswordChar = '*';
            this.register_cPassword.Size = new System.Drawing.Size(628, 44);
            this.register_cPassword.TabIndex = 19;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(692, 527);
            this.label7.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(298, 36);
            this.label7.TabIndex = 18;
            this.label7.Text = "Підтвердіть пароль";
            // 
            // SignUpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1370, 933);
            this.Controls.Add(this.register_cPassword);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.register_btn);
            this.Controls.Add(this.register_showPass);
            this.Controls.Add(this.register_password);
            this.Controls.Add(this.register_username);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.close);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "SignUpForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.SignUpForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button register_btn;
        private System.Windows.Forms.CheckBox register_showPass;
        private System.Windows.Forms.TextBox register_password;
        private System.Windows.Forms.TextBox register_username;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label close;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button register_loginBtn;
        private System.Windows.Forms.TextBox register_cPassword;
        private System.Windows.Forms.Label label7;
    }
}