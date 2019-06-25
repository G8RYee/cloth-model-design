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
    public partial class 等分 : Form
    {
        public int ans = 0;
        public 等分()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox1.Text, out ans))
                MessageBox.Show("請輸入整數數字", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                DialogResult = DialogResult.OK;
        }
    }
}
