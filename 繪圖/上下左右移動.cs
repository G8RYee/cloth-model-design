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
    public partial class 上下左右移動 : Form
    {
        public 上下左右移動()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
        }
        public float updown, leftright;
        public bool tufd, tlfr, utinch_fcm, ltinch_fcm;
        private void button2_Click(object sender, EventArgs e)
        {
            float u, l;
            if (float.TryParse(textBox1.Text, out u) && float.TryParse(textBox2.Text, out l))
            {
                updown = u;
                leftright = l;
                tufd = comboBox1.SelectedIndex == 0;
                tlfr = comboBox2.SelectedIndex == 0;
                utinch_fcm = comboBox3.SelectedIndex == 0;
                ltinch_fcm = comboBox4.SelectedIndex == 0;
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("請輸入數字", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
    }
}
