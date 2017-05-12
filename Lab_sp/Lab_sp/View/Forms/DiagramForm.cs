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
    public partial class DiagramForm : Form
    {
        public DiagramForm(List<double> diagram)
        {
            InitializeComponent();
            for (int i = 0; i < diagram.Count; i++)
                chart1.Series[0].Points.AddXY(i+1, diagram[i]);
        }
    }
}
