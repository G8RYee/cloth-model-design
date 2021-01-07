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
    public partial class 輸出設定 : Form
    {
        public 輸出設定()
        {
            InitializeComponent();
        }
        public bool[] output_containt = new bool[8];//線 縫份 輔 距 輔距 點 布紋 字 
        private void button1_Click(object sender, EventArgs e)
        {
            output_containt[0] = checkBox1.Checked;
            output_containt[1] = checkBox2.Checked;
            output_containt[2] = checkBox3.Checked;
            output_containt[3] = checkBox4.Checked;
            output_containt[4] = checkBox5.Checked;
            output_containt[5] = checkBox6.Checked;
            output_containt[6] = checkBox7.Checked;
            output_containt[7] = checkBox8.Checked;
            DialogResult = DialogResult.OK;
        }
    }
}
