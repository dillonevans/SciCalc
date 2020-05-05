using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SciCalc
{
    public partial class SciCalcForm : Form
    {
        private string displayString = "", infix;


        public SciCalcForm()
        {
            InitializeComponent();
        }


        private void ButtonHandler(object sender, EventArgs e)
        {
            Button currentButton = (Button)sender;
            char token = currentButton.Text[0];
            string text = currentButton.Text;

            if (text == "x^n") { text = "^"; }

            if (!(displayString.Contains("+") || displayString.Contains("-") || displayString.Contains("/") || displayString.Contains("x")) && displayString.Length >= 1)
            {
                if (text == "ln" || text == "cos" || text == "sin" || text == "tan" || text == "sqrt")
                {
                    displayString = text + " (" + displayString + ")";
                    infix = text + " ( " + infix + " ) ";
                    text = "";
                }
            }
            if (Char.IsDigit(token) || token == '.' || token == 'π' || token == 'e')
            {
                infix += token;
                Debug.WriteLine(infix);
            }
            else
            {
                infix += " " + text + " ";
            }

            displayString += text;
            textBox1.Text = displayString;
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            infix = ""; displayString = "";
            textBox1.Text = displayString;
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {

            Debug.WriteLine(e.KeyChar);
            //Lookup the key and activate the associated button
            switch (e.KeyChar)
            {
                case '0':
                    ZeroButton.PerformClick();
                    break;
                case '1':
                    OneButton.PerformClick();
                    break;
                case '2':
                    TwoButton.PerformClick();
                    break;
                case '3':
                    ThreeButton.PerformClick();
                    break;
                case '4':
                    FourButton.PerformClick();
                    break;
                case '5':
                    FiveButton.PerformClick();
                    break;
                case '6':
                    SixButton.PerformClick();
                    break;
                case '7':
                    SevenButton.PerformClick();
                    break;
                case '8':
                    EightButton.PerformClick();
                    break;
                case '9':
                    NineButton.PerformClick();
                    break;
                case '+':
                    AdditionButton.PerformClick();
                    break;
                case '-':
                    SubtractionButton.PerformClick();
                    break;
                case '*':
                case 'x':
                    MultiplicationButton.PerformClick();
                    break;
                case '/':
                    DivisionButton.PerformClick();
                    break;
                case 'e':
                    eButton.PerformClick();
                    break;
                case '!':
                    FactorialButton.PerformClick();
                    break;
                case '(':
                    LeftParenButton.PerformClick();
                    break;
                case ')':
                    RightParenButton.PerformClick();
                    break;
                case '^':
                    ExponentButton.PerformClick();
                    break;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.ActiveControl = EqualsButton;
        }

        private void EqualsButton_Click(object sender, EventArgs e)
        {

            string result = Calculator.EvaluateExpression(infix.Trim()).ToString();
            textBox1.Text = result;
            infix = result;
            displayString = result;
        }
    }
}
