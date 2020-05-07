using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace SciCalc
{
    public partial class SciCalcForm : Form
    {
        private string displayString = "", infix;
        private static Dictionary<string, Button> buttonMap = new Dictionary<string, Button>();

        public SciCalcForm()
        {
            InitializeComponent();
        }

        private void ButtonHandler(object sender, EventArgs e)
        {
            Button currentButton = (Button)sender;
            char token = currentButton.Text[0];
            string text = currentButton.Text;
  
            if (!(displayString.Contains("+") || displayString.Contains("−") || displayString.Contains("/") || displayString.Contains("x")) && displayString.Length >= 1)
            {
                if (Tokens.IsFunction(text))
                {
                    displayString = text + " (" + displayString + ")";
                    infix = text + " ( " + infix + " ) ";
                    text = "";
                }
            }
            if (Char.IsDigit(token) || token == '.' || token == 'π' || token == 'e')
            {
                infix += token; //Append number as usual
            }
            else if (text == Tokens.NEGATIVE_TOKEN)
            {
                infix += " " + text;
            }
            else
            {
                infix += " " + text + " ";
            }

            displayString += text;
            textBox1.Text = displayString;
            Debug.WriteLine(infix);
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            infix = ""; displayString = "";
            textBox1.Text = displayString;
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {

            Debug.WriteLine(e.KeyChar);

            //Lookup the key and perform the button click
            if (buttonMap.ContainsKey(e.KeyChar.ToString()))
            {
                buttonMap[e.KeyChar.ToString()].PerformClick();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string token;
            this.ActiveControl = EqualsButton;

            foreach (Button b in groupBox3.Controls)
            {
                switch (b.Text)
                {
                    case Tokens.SUB_OP:
                        token = "-";
                        break;
                    case Tokens.MULT_OP:
                        token = "*";
                        break;
                    default:
                        token = b.Text;
                        break;
                }
                buttonMap.Add(token, b);
            }
        }

        /// <summary>
        /// Upon Applying The Equals Button, Evaluate The Infix Expression
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EqualsButton_Click(object sender, EventArgs e)
        {

            string result = Calculator.EvaluateExpression(infix.Trim()).ToString();
            textBox1.Text = result;
            infix = result;
            displayString = result;
        }
    }
}
