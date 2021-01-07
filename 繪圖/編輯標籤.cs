using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 繪圖用lib;

namespace 繪圖
{
    public partial class 編輯標籤 : Form
    {
        public 編輯標籤()
        {
            InitializeComponent();
        }
        public 編輯標籤(Formula fml)
        {
            InitializeComponent();
            textBox1.Text = fml.name;
            string s = "";
            foreach (var ele in fml.EleL)
            {
                s += ele.ToString() + " ";
            }
            if (s.Length > 0)
                s = (s.Last() == ' ') ? s.Remove(s.Count() - 1) : s;
            textBox6.Text = s;
        }
        public List<Element> eleL = new List<Element>();
        public string name;
        public double Xleft, Xright, Yup, Ydown;

        private void 編輯公式_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            輸入算式 f = new 輸入算式(textBox6.Text);
            if(f.ShowDialog() == DialogResult.OK)
            {
                eleL = f.EleL;
                string s = "";
                foreach (var ele in f.EleL)
                {
                    s += ele.ToString() + " ";
                }
                if (s.Count() > 0)
                    s = (s.Last() == ' ') ? s.Remove(s.Count() - 1) : s;
                textBox6.Text = s;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            name = textBox1.Text;
            if (name.Contains(" ") || name.Contains("\\") || name.Contains("%") || name.Contains("$") || name.Contains("#") || name.Contains("!") || name.Contains("@") || name.Contains("^") || name.Contains("&") || name.Contains("*"))
            {
                MessageBox.Show("請勿包含特殊字元", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if(name == "")
            {
                MessageBox.Show("請輸入名稱", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DialogResult = DialogResult.OK;
        }
    }
}
