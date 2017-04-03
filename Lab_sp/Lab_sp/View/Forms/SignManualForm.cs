﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lab_sp.Core;
using Lab_sp.Entities;

namespace Lab_sp.View.Forms
{
    public partial class SignManualForm : Form
    {
        public SignManualForm()
        {
            InitializeComponent();
        }

        private void Manual_Load(object sender, EventArgs e)
        {
            List<Sign> signs = Settings.Instance.DataLayer.AllSigns();
            foreach (var sign in signs)
                dataGridView.Rows.Add(sign.Image, sign.Name, sign.Gost, sign.Type);
        }
    }
}
