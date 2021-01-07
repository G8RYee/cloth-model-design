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
    public partial class 依照公式調整 : Form
    {
        string[] rowname = { "1.肩寛(SW)" ,"2.前胸吊帶距(DFS)","3.乳高(BH)","4.乳深(BR)","5.單乳寬(SBW)","6.前胸寬(FBW)","7.胸上圍(UpB)","8.胸圍(B)",
                             "9.胸下圍(UdB)","10.腋下->胸下(ArtoUdB)","11.腋下->腰(ArtoW)","12.腋下->大腿(ArtoT)","13.胸下->腰(UdBtoW)","14.胸下->大腿(UdBtoT)",
                             "15.腰圍(W)","16.腹高(AH)","17.腹圍(Ab)","18.胸下->Y(UdBtoY)","19.臀圍(Hip)","20.臀長(HL)","21.平口側邊長(WtoT)","22.斜大腿圍(OTS)",
                             "23.背長(BL)","24.背寬(BW)","25.股上長(CD)","26.提臀高(HLH)","27.四角提臀高(BHLB)","28.褲檔長(CL)","29.總檔長(UdBtoC)"
        };
        public double[] custom_size = new double[29];
        public 依照公式調整(double[] standard)
        {
            InitializeComponent();
            for(int i = 0; i < 29; i++)
            {
                Standard_Size_Datagridview.Rows.Add();
                Standard_Size_Datagridview.Rows[i].Cells[0].Value = rowname[i];
                Standard_Size_Datagridview.Rows[i].Cells[1].Value = standard[i].ToString();
            }
            for (int i = 0; i < 29; i++)
            {
                Custom_Size_Datagridview.Rows.Add();
                Custom_Size_Datagridview.Rows[i].Cells[0].Value = rowname[i];
                Custom_Size_Datagridview.Rows[i].Cells[1].Value = 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < 29; i++)
                    double.TryParse(Custom_Size_Datagridview.Rows[i].Cells[1].Value.ToString(), out custom_size[i]);
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
            if (f.ShowDialog() == DialogResult.OK)
            {
                for(int i = 0; i < 29; i++)
                {
                    Custom_Size_Datagridview.Rows[i].Cells[1].Value = f.custom_size[i];
                }
                label2.Text = "顧客編號 " + f.custom_number;
            }
        }
    }
}
