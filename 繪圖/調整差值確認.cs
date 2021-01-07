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
    public partial class 調整差值確認 : Form
    {
        /*public 調整差值確認(double[] standard, double[] custom, List<Formula> FormulaList)
        {
            InitializeComponent();
            for(int i = 0; i < FormulaList.Count; i++)
            {
                string s = "";
                foreach (var ele in FormulaList[i].EleL)
                {
                    s += ele.ToString();
                }

                var element_comvert = new List<Element>();
                foreach (var ele in FormulaList[i].EleL)
                {
                    if (ele.GetType().Equals(typeof(VariableElement)))
                    {
                        string temps = ele.ToString();
                        temps = temps.Remove(temps.Length - 1, 1);
                        temps = temps.Remove(0, 1);
                        int index = 0;
                        int.TryParse(temps, out index);
                        double v = custom[index - 1] - standard[index - 1];
                        element_comvert.Add(new NumberElement(v.ToString()));
                    }
                    else
                    {
                        element_comvert.Add(ele);
                    }
                }
                InfixToPostfix itp = new InfixToPostfix();
                var PFele = itp.ConvertFromInfixToPostFix(element_comvert);
                PostFixEvaluator pfe = new PostFixEvaluator();
                double ans = pfe.Evaluate(PFele);
                
                Size_Check_Datagridview.Rows.Add();
                Size_Check_Datagridview.Rows[i].Cells[0].Value = FormulaList[i].name;
                Size_Check_Datagridview.Rows[i].Cells[1].Value = ans;
                Size_Check_Datagridview.Rows[i].Cells[2].Value = s;
            }
        }*/

        public 調整差值確認(List<double> shift, List<main.FormulaToLine> ftl)
        {
            InitializeComponent();
            for (int i = 0; i < ftl.Count; i++)
            {
                string s = "";
                foreach (var ele in ftl[i].formula_eleL)
                {
                    s += ele.ToString();
                }

                Size_Check_Datagridview.Rows.Add();
                Size_Check_Datagridview.Rows[i].Cells[0].Value = ftl[i].name;
                Size_Check_Datagridview.Rows[i].Cells[1].Value = shift[i];
                if (shift[i] < 0)
                    Size_Check_Datagridview.Rows[i].Cells[1].Style.ForeColor = Color.Red;
                Size_Check_Datagridview.Rows[i].Cells[2].Value = s;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
