namespace Lab_sp.View.Forms
{
    partial class AuthForm
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
            this.infoLabel = new System.Windows.Forms.Label();
            this.enterBox = new System.Windows.Forms.PictureBox();
            this.textBoxPass = new System.Windows.Forms.TextBox();
            this.textBoxLogin = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.exit = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.enterBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.exit)).BeginInit();
            this.SuspendLayout();
            // 
            // infoLabel
            // 
            this.infoLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.infoLabel.AutoSize = true;
            this.infoLabel.ForeColor = System.Drawing.Color.SteelBlue;
            this.infoLabel.Location = new System.Drawing.Point(175, 309);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(238, 13);
            this.infoLabel.TabIndex = 3;
            this.infoLabel.Text = "Для продолжения требуется авторизоваться";
            // 
            // enterBox
            // 
            this.enterBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.enterBox.Image = global::Lab_sp.Properties.Resources.enter1;
            this.enterBox.Location = new System.Drawing.Point(239, 263);
            this.enterBox.Name = "enterBox";
            this.enterBox.Size = new System.Drawing.Size(126, 33);
            this.enterBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.enterBox.TabIndex = 2;
            this.enterBox.TabStop = false;
            this.enterBox.Click += new System.EventHandler(this.enterBox_Click);
            this.enterBox.MouseEnter += new System.EventHandler(this.enterBox_MouseEnter);
            this.enterBox.MouseLeave += new System.EventHandler(this.enterBox_MouseLeave);
            // 
            // textBoxPass
            // 
            this.textBoxPass.Font = new System.Drawing.Font("Gill Sans Ultra Bold", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxPass.Location = new System.Drawing.Point(224, 220);
            this.textBoxPass.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxPass.Name = "textBoxPass";
            this.textBoxPass.PasswordChar = '•';
            this.textBoxPass.Size = new System.Drawing.Size(159, 21);
            this.textBoxPass.TabIndex = 5;
            this.textBoxPass.Tag = "";
            this.textBoxPass.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxLogin
            // 
            this.textBoxLogin.Location = new System.Drawing.Point(224, 184);
            this.textBoxLogin.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxLogin.Name = "textBoxLogin";
            this.textBoxLogin.Size = new System.Drawing.Size(159, 20);
            this.textBoxLogin.TabIndex = 4;
            this.textBoxLogin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.ForeColor = System.Drawing.Color.SteelBlue;
            this.label2.Location = new System.Drawing.Point(159, 184);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 20);
            this.label2.TabIndex = 6;
            this.label2.Text = "Логин:";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.ForeColor = System.Drawing.Color.SteelBlue;
            this.label3.Location = new System.Drawing.Point(147, 220);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "Пароль:";
            // 
            // exit
            // 
            this.exit.Image = global::Lab_sp.Properties.Resources.cross1;
            this.exit.Location = new System.Drawing.Point(572, 12);
            this.exit.Name = "exit";
            this.exit.Size = new System.Drawing.Size(23, 22);
            this.exit.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.exit.TabIndex = 8;
            this.exit.TabStop = false;
            this.exit.Click += new System.EventHandler(this.exit_Click);
            this.exit.MouseEnter += new System.EventHandler(this.exit_MouseEnter);
            this.exit.MouseLeave += new System.EventHandler(this.exit_MouseLeave);
            // 
            // AuthForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BackgroundImage = global::Lab_sp.Properties.Resources.welcome;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(607, 353);
            this.Controls.Add(this.exit);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxPass);
            this.Controls.Add(this.textBoxLogin);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.enterBox);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximumSize = new System.Drawing.Size(607, 353);
            this.MinimumSize = new System.Drawing.Size(607, 353);
            this.Name = "AuthForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AuthForm";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AuthForm_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.enterBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.exit)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.PictureBox enterBox;
        private System.Windows.Forms.TextBox textBoxPass;
        private System.Windows.Forms.TextBox textBoxLogin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox exit;
    }
}