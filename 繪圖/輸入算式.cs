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
    public partial class 輸入算式 : Form
    {
        public 輸入算式()
        {
            InitializeComponent();
        }
        public 輸入算式(string formula)
        {
            InitializeComponent();
            textBox1.Text = formula;
        }
        public List<Element> EleL = new List<Element>();
        int count = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
                if (textBox1.Text.Last() == ']')
                    button17_Click(sender, e);
                else if (textBox1.Text.Last() == ')')
                    return;
            textBox1.Text += "1";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
                if (textBox1.Text.Last() == ']')
                    button17_Click(sender, e);
                else if (textBox1.Text.Last() == ')')
                    return;
            textBox1.Text += "2";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
                if (textBox1.Text.Last() == ']')
                    button17_Click(sender, e);
                else if (textBox1.Text.Last() == ')')
                    return;
            textBox1.Text += "3";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
                if (textBox1.Text.Last() == ']')
                    button17_Click(sender, e);
                else if (textBox1.Text.Last() == ')')
                    return;
            textBox1.Text += "4";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
                if (textBox1.Text.Last() == ']')
                    button17_Click(sender, e);
                else if (textBox1.Text.Last() == ')')
                    return;
            textBox1.Text += "5";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
                if (textBox1.Text.Last() == ']')
                    button17_Click(sender, e);
                else if (textBox1.Text.Last() == ')')
                    return;
            textBox1.Text += "6";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
                if (textBox1.Text.Last() == ']')
                    button17_Click(sender, e);
                else if (textBox1.Text.Last() == ')')
                    return;
            textBox1.Text += "7";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
                if (textBox1.Text.Last() == ']')
                    button17_Click(sender, e);
                else if (textBox1.Text.Last() == ')')
                    return;
            textBox1.Text += "8";
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
                if (textBox1.Text.Last() == ']')
                    button17_Click(sender, e);
                else if (textBox1.Text.Last() == ')')
                    return;
            textBox1.Text += "9";
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
                if (textBox1.Text.Last() == ']')
                    button17_Click(sender, e);
                else if (textBox1.Text.Last() == ')')
                    return;
            textBox1.Text += "0";
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
                if (Char.IsDigit(textBox1.Text.Last()))
                    textBox1.Text += ".";
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
                if (textBox1.Text.Last() == '+' || textBox1.Text.Last() == '-' || textBox1.Text.Last() == '*' || textBox1.Text.Last() == '/')
                    button17_Click(sender, e);
            if (textBox1.Text.Length > 0)
                textBox1.Text += "/";
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
                if (textBox1.Text.Last() == '+' || textBox1.Text.Last() == '-' || textBox1.Text.Last() == '*' || textBox1.Text.Last() == '/')
                    button17_Click(sender, e);
            if (textBox1.Text.Length > 0)
                textBox1.Text += "*";
        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
                if (textBox1.Text.Last() == '+' || textBox1.Text.Last() == '-' || textBox1.Text.Last() == '*' || textBox1.Text.Last() == '/')
                    button17_Click(sender, e);
            if (textBox1.Text.Length > 0)
                textBox1.Text += "+";
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
                if (textBox1.Text.Last() == '+' || textBox1.Text.Last() == '-' || textBox1.Text.Last() == '*' || textBox1.Text.Last() == '/')
                    button17_Click(sender, e);
            if (textBox1.Text.Length > 0)
                textBox1.Text += "-";
        }

        private void button16_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void button17_Click(object sender, EventArgs e)
        {
            if(textBox1.Text.Length > 0)
            {
                if (textBox1.Text.Last() == ']')
                {
                    while (textBox1.Text.Last() != '[')
                        textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - 1);
                    textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - 1);
                }
                else if(textBox1.Text.Last() == '(')
                {
                    textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - 1);
                    count--;
                }
                else if (textBox1.Text.Last() == ')')
                {
                    textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - 1);
                    count++;
                }
                else
                    textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - 1);
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
            {
                count++;
                textBox1.Text += "(";
            }
            else if (textBox1.Text.Last() == '+' || textBox1.Text.Last() == '-' || textBox1.Text.Last() == '*' || textBox1.Text.Last() == '/')
            {
                count++;
                textBox1.Text += "(";
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            if (count == 0)
                return;
            if (textBox1.Text.Length > 0)
                if (textBox1.Text.Last() == '+' || textBox1.Text.Last() == '-' || textBox1.Text.Last() == '*' || textBox1.Text.Last() == '/')
                    button17_Click(sender, e);
            textBox1.Text += ")";
            count--;
        }

        private void button20_Click(object sender, EventArgs e)
        {
            int size = listBox1.SelectedIndex;
            if (textBox1.Text.Length == 0)
                textBox1.Text += "[" + (size + 1) + "]";
            else if (textBox1.Text.Last() == '+' || textBox1.Text.Last() == '-' || textBox1.Text.Last() == '*' || textBox1.Text.Last() == '/')
                textBox1.Text += "[" + (size + 1) + "]";
        }

        private void button21_Click(object sender, EventArgs e)
        {
            Parser p = new Parser();
            List<Element> el = p.Parse(textBox1.Text);
            if (el.Count != 0)
                if (el.Last().GetType().Equals(typeof(OperatorElement)))
                    el.RemoveAt(el.Count - 1);
            string s = "";
            foreach (var ele in el)
                if (ele.GetType().Equals(typeof(NumberElement)))
                    s += ((NumberElement)ele).ToString() + " ";
                else if(ele.GetType().Equals(typeof(OperatorElement)))
                    s += ((OperatorElement)ele).ToString() + " ";
                else
                    s += ((VariableElement)ele).ToString() + " ";
           // MessageBox.Show(s);
            EleL = el;
            DialogResult = DialogResult.OK;
            return;
            /*
            s = "";
            InfixToPostfix i = new InfixToPostfix();
            el = i.ConvertFromInfixToPostFix(el);
            foreach (var ele in el)
                if (ele.GetType().Equals(typeof(NumberElement)))
                    s += ((NumberElement)ele).ToString() + " ";
                else if (ele.GetType().Equals(typeof(OperatorElement)))
                    s += ((OperatorElement)ele).ToString() + " ";
                else
                    s += ((VariableElement)ele).ToString() + " ";
            MessageBox.Show(s);
            s = "";
            PostFixEvaluator pfe = new PostFixEvaluator();
            */
            //return pfe.Evaluate(e);
            //MessageBox.Show(pfe.Evaluate(el).ToString());
        }



        public double Calculate(String s)
        {
            Parser p = new Parser();
            List<Element> e = p.Parse(s);
            InfixToPostfix i = new InfixToPostfix();
            e = i.ConvertFromInfixToPostFix(e);

            PostFixEvaluator pfe = new PostFixEvaluator();
            return pfe.Evaluate(e);
        }

        /*class Program
        {
            public double Calculate(String s)
            {
                Parser p = new Parser();
                List<Element> e = p.Parse(s);
                InfixToPostfix i = new InfixToPostfix();
                e = i.ConvertFromInfixToPostFix(e);

                PostFixEvaluator pfe = new PostFixEvaluator();
                return pfe.Evaluate(e);
            }

            static void Main(string[] args)
            {
                Program c = new Program();
                double d = c.Calculate("4+6+9*8-(5*6+9)^2");
                Console.WriteLine(d);
            }
        }*/

    }
}
