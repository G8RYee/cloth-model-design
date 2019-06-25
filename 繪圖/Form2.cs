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
    public partial class 重新命名 : Form
    {
        public string ans;
        public 重新命名()
        {
            InitializeComponent();
        }
        public 重新命名(string s)
        {
            InitializeComponent();
            textBox1.Text = s;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ans = textBox1.Text;
            if (ans.Contains(" ") || ans.Contains("\\") || ans.Contains("%") || ans.Contains("$") || ans.Contains("#") || ans.Contains("!") || ans.Contains("@") || ans.Contains("^") || ans.Contains("&") || ans.Contains("*"))
                MessageBox.Show("含有非法字元", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                DialogResult = DialogResult.OK;
        }
    }
}
