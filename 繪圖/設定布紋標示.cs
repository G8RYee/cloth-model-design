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
    public partial class 設定布紋標示 : Form
    {
        public bool visible;
        public int loc_type, dir_type;
        public PointF Loc = new PointF(-1, -1);
        public double Dir = -1;
        public double size = 100;

        public 設定布紋標示()
        {
            InitializeComponent();
            radioButton2.Checked = true;
        }
        public 設定布紋標示(bool visible, int location, int direction, float x = -1, float y = -1, double dir = -1, double size = 100)
        {
            InitializeComponent();
            if (visible)
                radioButton1.Checked = true;
            else
                radioButton2.Checked = true;

            if (location == 0)
                radioButton3.Checked = true;
            else if (location == 1)
                radioButton4.Checked = true;
            else if (location == 2)
                radioButton5.Checked = true;
            else if (location == 3)
                radioButton6.Checked = true;
            else if (location == 4)
            {
                radioButton7.Checked = true;
                Loc = new PointF(x, y);
                textBox1.Text = x.ToString();
                textBox2.Text = y.ToString();
            }

            if (direction == 0)
                radioButton8.Checked = true;
            else if (direction == 1)
                radioButton9.Checked = true;
            else if (direction == 2)
                radioButton10.Checked = true;
            else if (direction == 3)
                radioButton11.Checked = true;
            else if (direction == 4)
            {
                radioButton12.Checked = true;
                Dir = dir;
                textBox4.Text = dir.ToString();
            }
            comboBox1.Text = size.ToString();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                groupBox2.Enabled = true;
                groupBox3.Enabled = true;
                groupBox4.Enabled = true;
            }
            else
            {
                groupBox2.Enabled = false;
                groupBox3.Enabled = false;
                groupBox4.Enabled = false;
            }
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton7.Checked == true)
            {
                textBox1.Enabled = true;
                textBox2.Enabled = true;
            }
            else
            {
                textBox1.Enabled = false;
                textBox2.Enabled = false;
            }
        }

        private void radioButton12_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton12.Checked == true)
            {
                textBox4.Enabled = true;
            }
            else
            {
                textBox4.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            visible = radioButton1.Checked;

            loc_type = radioButton3.Checked ? 0 :
                       radioButton4.Checked ? 1 :
                       radioButton5.Checked ? 2 :
                       radioButton6.Checked ? 3 : 4;
            if (loc_type == 4)
            {
                try
                {
                    float x, y;
                    float.TryParse(textBox1.Text, out x);
                    float.TryParse(textBox2.Text, out y);
                    Loc = new PointF(x, y);
                }
                catch
                {
                    MessageBox.Show("指定位置請輸入數字", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            dir_type = radioButton8.Checked ? 0 :
                       radioButton9.Checked ? 1 :
                       radioButton10.Checked ? 2 :
                       radioButton11.Checked ? 3 : 4;
            if (dir_type == 4)
            {
                try
                {
                    double.TryParse(textBox4.Text, out Dir);
                }
                catch
                {
                    MessageBox.Show("指定角度請輸入數字", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            try
            {
                double.TryParse(comboBox1.Text, out size);
            }
            catch
            {
                MessageBox.Show("縮放大小請輸入數字", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (size < 50)
                size = 50;
            this.DialogResult = DialogResult.OK;
        }
    }
}
