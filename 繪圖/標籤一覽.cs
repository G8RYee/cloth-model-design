using System;
using System.Collections.Generic;
using System.Windows.Forms;
using 繪圖用lib;

namespace 繪圖
{
    public partial class 標籤一覽 : Form
    {
        public List<Formula> FL = new List<Formula>();
        public 標籤一覽(List<Formula> fl)
        {
            InitializeComponent();
            listView1.Focus();
            FL = fl;
            listView1.Items.Clear();
            for (int i = 0; i < FL.Count;i++)
            {
                string s = "";
                foreach (var ele in FL[i].EleL)
                {
                    s += ele.ToString() + " ";
                }
                ListViewItem l = new ListViewItem(new string[] { FL[i].name, s });
                listView1.Items.Add(l);
            }
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            編輯標籤 f = new 編輯標籤();
            f.ShowDialog();
            if (f.DialogResult == DialogResult.OK)
            {
                Formula fml = new Formula(f.name, f.eleL);
                FL.Add(fml);
                string s = "";
                foreach (var ele in fml.EleL)
                {
                    s += ele.ToString();
                }
                listView1.Items.Add(new ListViewItem(new string[] { fml.name, s }));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(listView1.FocusedItem != null)
            {
                int index = listView1.FocusedItem.Index;

                編輯標籤 f = new 編輯標籤(FL[index]);
                f.ShowDialog();
                if (f.DialogResult == DialogResult.OK)
                {
                    FL[index].name = f.name;
                    FL[index].EleL = f.eleL;
                    string s = "";
                    foreach (var ele in FL[index].EleL)
                    {
                        s += ele.ToString();
                    }
                    listView1.Items[index] = new ListViewItem(new string[] { FL[index].name, s });
                }
            }
        }

        /*private void button3_Click(object sender, EventArgs e)
        {
            int index = listView1.FocusedItem.Index;
            Rename f = new Rename(FL[index].name);
            f.ShowDialog();
            if (f.DialogResult == DialogResult.OK)
            {
                FL[index].name = f.ans;
                string s = "";
                foreach (var ele in FL[index].EleL)
                {
                    s += ele.ToString() + " ";
                }
                listView1.FocusedItem = new ListViewItem(new string[] { f.ans, s });
            }
        }*/

        private void button4_Click(object sender, EventArgs e)
        {
            listView1.Items.Remove(listView1.FocusedItem);
            FL.RemoveAt(listView1.FocusedItem.Index);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void 標籤一覽_Load(object sender, EventArgs e)
        {

        }
    }
}
