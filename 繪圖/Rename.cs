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
    public partial class Rename : Form
    {
        public string ans;
        public Rename()
        {
            InitializeComponent();
        }
        public Rename(string s)
        {
            InitializeComponent();
            textBox1.Text = s;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ans = textBox1.Text;
            if (ans.Contains(" ") || ans.Contains("\\") || ans.Contains("%") || ans.Contains("$") || ans.Contains("#") || ans.Contains("!") || ans.Contains("@") || ans.Contains("^") || ans.Contains("&") || ans.Contains("*"))
                MessageBox.Show("請勿包含特殊字元", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                DialogResult = DialogResult.OK;
        }
    }
}
