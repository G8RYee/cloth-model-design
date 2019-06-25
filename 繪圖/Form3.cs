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
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }
        public int width, height;
        public Form3(int w, int h)
        {
            InitializeComponent();
            width = w;
            height = h;
            textBox1.Text = h + "";
            textBox2.Text = w + "";
        }
        private void button1_Click(object sender, EventArgs e)
        {
            int w, h;
            if (int.TryParse(textBox1.Text, out h) && int.TryParse(textBox2.Text, out w))
            {
                width = w;
                height = h;
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("請輸入整數", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
