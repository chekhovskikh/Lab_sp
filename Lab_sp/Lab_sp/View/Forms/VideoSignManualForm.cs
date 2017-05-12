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
    public partial class VideoSignManualForm : Form
    {
        public VideoSignManualForm()
        {
            InitializeComponent();
            SelectFormAccordingToLevelAccess();
        }

        private void VideoSignManualForm_Load(object sender, EventArgs e)
        {
            RefreshTable();
        }

        private void RefreshTable()
        {
            dataGridView.Rows.Clear();
            List<FullInfoVideoSign> signs = Settings.Instance.DataLayer.AllFullInfoVideoSigns();
            foreach (var sign in signs)
            {
                int i = dataGridView.Rows.Add(sign.Image, sign.SignImage, sign.Name, sign.Gost, sign.Time, sign.Type);
                dataGridView.Rows[i].Tag = sign.Id;
            }
        }

        /// <summary>
        /// Изменить визуальное представление формы соответственно уровню доступа пользователя
        /// </summary>
        private void SelectFormAccordingToLevelAccess()
        {
            switch (Settings.Instance.Role)
            {
                case Settings.UserRole.Admin:
                    Text += " (Администратор)";
                    break;
                default:
                    Text += " (Пользователь)";
                    break;
            }
        }

        private void removeAllMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Instance.DataLayer.RemoveAllVideoSigns();
            VideoSignManualForm_Load(sender, e);
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dataGridView.SelectedRows)
                    Settings.Instance.DataLayer.RemoveVideoSign((int)row.Tag);
            }
            catch { MessageBox.Show("Неккоректные данные пользователя!"); }
            RefreshTable();
        }
    }
}
