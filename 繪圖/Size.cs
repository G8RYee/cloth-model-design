using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 繪圖
{
    public partial class Size : Form
    {
        public Size()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }
        public int width, height;
        private void button1_Click(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex == 0)//A4
            {
                width = (int)(8.3 * 72);
                height = (int)(11.7 * 72);
                DialogResult = DialogResult.OK;
            }
            if (comboBox1.SelectedIndex == 1)//A3
            {
                width = (int)(11.7 * 72);
                height = (int)(16.5 * 72);
                DialogResult = DialogResult.OK;
            }
            if (comboBox1.SelectedIndex == 2)//A2
            {
                width = (int)(16.5 * 72);
                height = (int)(23.4 * 72);
                DialogResult = DialogResult.OK;
            }
            if (comboBox1.SelectedIndex == 3)//A1
            {
                width = (int)(23.4 * 72);
                height = (int)(33.1 * 72);
                DialogResult = DialogResult.OK;
            }
            if (comboBox1.SelectedIndex == 4)//A0
            {
                width = (int)(33.1 * 72);
                height = (int)(46.8 * 72);
                DialogResult = DialogResult.OK;
            }
        }
    }
}
