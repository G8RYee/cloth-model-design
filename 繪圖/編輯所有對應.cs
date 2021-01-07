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
    public partial class 編輯所有對應 : Form
    {
        private bool edited = false;
        public 編輯所有對應(List<main.TabpageData> data)
        {
            InitializeComponent();
            FTL_Datagridview.Columns[7].ValueType = typeof(main.FormulaToLine);

            FTL_Datagridview.Columns[1].DefaultCellStyle.BackColor = Color.LightGray;
            FTL_Datagridview.Columns[2].DefaultCellStyle.BackColor = Color.LightGray;
            FTL_Datagridview.Columns[5].DefaultCellStyle.BackColor = Color.LightGray;
            FTL_Datagridview.Columns[6].DefaultCellStyle.BackColor = Color.LightGray;
            foreach (var td in data)
            {
                foreach(var ftl in td.FtoLL)
                {
                    FTL_Datagridview.Rows.Add();
                    FTL_Datagridview.Rows[FTL_Datagridview.Rows.Count - 1].Cells[0].Value = ftl.name;
                    FTL_Datagridview.Rows[FTL_Datagridview.Rows.Count - 1].Cells[1].Value = ftl.fml.name;
                    FTL_Datagridview.Rows[FTL_Datagridview.Rows.Count - 1].Cells[2].Value = td.TabpageName;
                    string s = "";
                    foreach (var ele in ftl.formula_eleL)
                    {
                        if (ele.GetType().Equals(typeof(VariableElement)))
                        {
                            VariableElement vele = (VariableElement)ele;
                            s += vele.getName();
                        }
                        else
                        {
                            s += ele.ToString() + " ";
                        }
                    }
                    FTL_Datagridview.Rows[FTL_Datagridview.Rows.Count - 1].Cells[3].Value = s;
                    FTL_Datagridview.Rows[FTL_Datagridview.Rows.Count - 1].Cells[4].Value = ftl.prop12;
                    string lname = "";
                    if (ftl.type == 0)
                        lname += ftl.L1.name + " " + ftl.L2.name;
                    else if (ftl.type == 1)
                        lname += ftl.L1.name + " " + ftl.C1.name;
                    else if (ftl.type == 2)
                        lname += ftl.C1.name + " " + ftl.C2.name;
                    else if (ftl.type == 3)
                        lname += ftl.L1.name + " " + ftl.unfixed_P1.name;
                    else if (ftl.type == 4)
                        lname += ftl.C1.name + " " + ftl.unfixed_P1.name;
                    else if (ftl.type == 5)
                        lname += ftl.unfixed_P1.name + " " + ftl.unfixed_P2.name;
                    FTL_Datagridview.Rows[FTL_Datagridview.Rows.Count - 1].Cells[5].Value = lname;
                    FTL_Datagridview.Rows[FTL_Datagridview.Rows.Count - 1].Cells[6].Value = ftl.mode == 0 ? "平行移動" : "垂直移動";
                    FTL_Datagridview.Rows[FTL_Datagridview.Rows.Count - 1].Cells[7].Value = ftl;
                }
            }
        }
        //0對應名稱 1標籤名稱 2所在分頁 3套用公式 4左右比例 5套用物件 6移動形式 7FTL
        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (edited)
            {
                if (MessageBox.Show("將放棄編輯內容", "訊息", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                {
                    DialogResult = DialogResult.Cancel;
                    return;
                }
            }
            else
                DialogResult = DialogResult.Cancel;
        }

        private void FTL_Datagridview_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            edited = true;
            try
            {
                main.FormulaToLine ftl = (main.FormulaToLine)FTL_Datagridview.Rows[e.RowIndex].Cells[7].Value;
                if (e.ColumnIndex == 4)
                {
                    string[] s = FTL_Datagridview.Rows[e.RowIndex].Cells[4].Value.ToString().Split(':');
                    double.Parse(s[0]);
                    double.Parse(s[1]);
                    ftl.prop12 = FTL_Datagridview.Rows[e.RowIndex].Cells[4].Value.ToString();
                }
                else if (e.ColumnIndex == 0)
                {
                    ftl.name = FTL_Datagridview.Rows[e.RowIndex].Cells[0].Value.ToString();
                }
            }
            catch
            {

            }
        }

        private void FTL_Datagridview_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                main.FormulaToLine ftl = (main.FormulaToLine)FTL_Datagridview.Rows[e.RowIndex].Cells[7].Value;
                string s = "";
                foreach (var ele in ftl.formula_eleL)
                {
                    if (ele.GetType().Equals(typeof(VariableElement)))
                    {
                        VariableElement vele = (VariableElement)ele;
                        s += vele.getName();
                    }
                    else
                    {
                        s += ele.ToString() + " ";
                    }
                }
                if (s.Length > 0)
                    if (s.Last() == ' ')
                        s = s.Remove(s.Length - 1);
                輸入算式 f = new 輸入算式(s);
                f.ShowDialog();
                if (f.DialogResult == DialogResult.OK)
                {
                    ftl.formula_eleL = f.EleL;
                    s = "";
                    foreach (var ele in ftl.formula_eleL)
                    {
                        if (ele.GetType().Equals(typeof(VariableElement)))
                        {
                            VariableElement vele = (VariableElement)ele;
                            s += vele.getName();
                        }
                        else
                        {
                            s += ele.ToString() + " ";
                        }
                    }
                    if (s.Length > 0)
                        if (s.Last() == ' ')
                            s = s.Remove(s.Length - 1);
                    FTL_Datagridview.Rows[e.RowIndex].Cells[3].Value = s;
                }
            }
        }


    }
}
