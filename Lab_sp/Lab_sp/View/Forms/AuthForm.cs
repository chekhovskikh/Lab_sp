using Lab_sp.Core;
using Lab_sp.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab_sp.View.Forms
{
    public partial class AuthForm : Form
    {
        public AuthForm()
        {
            InitializeComponent();
        }

        private void enterBox_Click(object sender, EventArgs e)
        {
            bool check = false;
            string login = textBoxLogin.Text;
            string pass = textBoxPass.Text;
            if (Validate(login) && Validate(pass))
            {
                List<User> users = Settings.Instance.DataLayer.AllUsers();
                foreach (var user in users)
                {
                    if (user.Login == login && user.Pass == pass)
                    {
                        Settings.Instance.UserName = login;
                        Settings.Instance.Role = user.Role;
                        DialogResult = DialogResult.OK;
                        check = !check;
                    }
                }
            }

            if (!check)
            {
                if (infoLabel.ForeColor != Color.OrangeRed)
                    infoLabel.Location = new Point(infoLabel.Location.X - 20, infoLabel.Location.Y);
                infoLabel.ForeColor = Color.OrangeRed;
                infoLabel.Text = "Проверьте корректность введенных логина и пароля";
                textBoxPass.Text = "";
            }
        }

        /// <summary>
        /// Проверка на допустимые символы пароля/логина.
        /// Русский алфавит запрещен
        /// </summary>
        /// <param name="s">строка для проверки</param>
        private Boolean Validate(string s)
        {
            string bannedSymbols = "?!.+-/*=@#$%&()№%: ";
            foreach (char c in bannedSymbols)
                if (s.Contains(c) || (c >= 'а' && c <= 'я') || (c >= 'A' && c <= 'Я'))
                    return false;
            return !s.Equals("");
        }

        private void AuthForm_MouseDown(object sender, MouseEventArgs e)
        {
            base.Capture = false;
            Message m = Message.Create(base.Handle, 0xa1, new IntPtr(2), IntPtr.Zero);
            this.WndProc(ref m);
        }

        private void exit_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void exit_MouseLeave(object sender, EventArgs e)
        {
            exit.Image = Properties.Resources.cross1;
        }

        private void exit_MouseEnter(object sender, EventArgs e)
        {
            exit.Image = Properties.Resources.cross2;
        }

        private void enterBox_MouseLeave(object sender, EventArgs e)
        {
            enterBox.Image = Properties.Resources.enter1;
        }

        private void enterBox_MouseEnter(object sender, EventArgs e)
        {
            enterBox.Image = Properties.Resources.enter2;
        }
    }
}
