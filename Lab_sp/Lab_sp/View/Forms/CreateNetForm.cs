using Lab_sp.Core;
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
    public partial class CreateNetForm : Form
    {
        public CreateNetForm()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                int height = int.Parse(textBoxHeight.Text);
                int width = int.Parse(textBoxWidth.Text);
                if (height < 20 || height > 100 || width < 20 || width > 100)
                    throw new ArgumentException();

                Settings.Instance.Size = new Size(width, height);
                if (comboBoxType.SelectedIndex == 0)
                    Settings.Instance.IsColor = false;
                else Settings.Instance.IsColor = true;
                DialogResult = DialogResult.OK;
            }
            catch
            {
                MessageBox.Show("Неверно введены параметры настройки сети (19 < width, height < 101)!");
            }
        }
    }
}
