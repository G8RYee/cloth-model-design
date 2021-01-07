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
    public partial class 輸入基礎尺寸 : Form
    {
        TextBox[] textblist = new TextBox[29];
        public double[] Sizes = new double[29];
        public string Prototype_Number;
        public string Prototype_Name;
        public 輸入基礎尺寸()
        {
            InitializeComponent();
            setTextbList();
        }
        public 輸入基礎尺寸(double[] sizes, string name, string num)
        {
            InitializeComponent();
            setTextbList();
            if (sizes.Length == 29)
            {
                for (int i = 0; i < 29; i++)
                    textblist[i].Text=sizes[i].ToString();
                Sizes = sizes;
            }
            textBox30.Text = num;
            textBox31.Text = name;
        }
        private void setTextbList()
        {
            textblist[0] = textBox1;
            textblist[1] = textBox2;
            textblist[2] = textBox3;
            textblist[3] = textBox4;
            textblist[4] = textBox5;
            textblist[5] = textBox6;
            textblist[6] = textBox7;
            textblist[7] = textBox8;
            textblist[8] = textBox9;
            textblist[9] = textBox10;
            textblist[10] = textBox11;
            textblist[11] = textBox12;
            textblist[12] = textBox13;
            textblist[13] = textBox14;
            textblist[14] = textBox15;
            textblist[15] = textBox16;
            textblist[16] = textBox17;
            textblist[17] = textBox18;
            textblist[18] = textBox19;
            textblist[19] = textBox20;
            textblist[20] = textBox21;
            textblist[21] = textBox22;
            textblist[22] = textBox23;
            textblist[23] = textBox24;
            textblist[24] = textBox25;
            textblist[25] = textBox26;
            textblist[26] = textBox27;
            textblist[27] = textBox28;
            textblist[28] = textBox29;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < 29; i++)
                    double.TryParse(textblist[i].Text,out Sizes[i]);
                Prototype_Number = textBox30.Text;
                Prototype_Name = textBox31.Text;
                DialogResult = DialogResult.OK;
            }
            catch
            {
                MessageBox.Show("請輸入數字", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            匯入尺寸 f = new 匯入尺寸();
            f.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
