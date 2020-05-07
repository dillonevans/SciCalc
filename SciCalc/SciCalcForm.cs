using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace SciCalc
{
    public partial class SciCalcForm : Form
    {
        //Object & Variable Declarations
        private string displayString = "", infix;
        private bool lastTokenIsOperator = false;
        private readonly Dictionary<string, Button> buttonMap = new Dictionary<string, Button>();

        /// <summary>
        /// Constructor for SciCalcForm
        /// </summary>
        public SciCalcForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Universal Button Handler. Handles Input for Every Button Aside from Clear and Equals
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonHandler(object sender, EventArgs e)
        {
            Button currentButton = (Button)sender;
            char token = currentButton.Text[0];
            string text = currentButton.Text;
            if (DisplayBox.Text == "ERROR")
            {
                displayString = "";
                DisplayBox.Clear();
            }

            if (!(displayString.Contains("+") || displayString.Contains("−") || displayString.Contains("/") || displayString.Contains("x")) && displayString.Length >= 1)
            {
                if (Tokens.IsFunction(text)) //Encase whatever is on the display with the function
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

            if (lastTokenIsOperator && Tokens.IsOperator(text))
            {
                displayString = "ERROR";
                DisplayBox.Text = displayString;
                infix = "";
                EqualsButton.Enabled = false;
            }
            else
            {
                lastTokenIsOperator = Tokens.IsOperator(text) ? true : false;
                displayString += text;
                DisplayBox.Text = displayString;
                EqualsButton.Enabled = true;
                Debug.WriteLine(infix);
            }

        }

        /// <summary>
        /// Clears the Display and Resets the Infix String
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearButton_Click(object sender, EventArgs e)
        {
            infix = ""; displayString = "";
            DisplayBox.Text = displayString;
        }

        /// <summary>
        /// Upon Key Press, Perform the Associated Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Lookup the key and perform the button click
            if (buttonMap.ContainsKey(e.KeyChar.ToString()))
            {
                buttonMap[e.KeyChar.ToString()].PerformClick();
            }
        }
    
        /// <summary>
        /// Upon Loading the Form, Import All The Buttons to a Dictionary
        /// that Serves as a Lookup Table for User Input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            string token;
            this.ActiveControl = EqualsButton;

            foreach (Button b in groupBox3.Controls)
            {
                //The '*' key represents multiplication and the '-' key represents subtraction 
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
        /// Upon Clicking The Equals Button, Evaluate The Infix Expression and display the
        /// result to the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EqualsButton_Click(object sender, EventArgs e)
        {

            string result = Calculator.EvaluateExpression(infix.Trim()).ToString();
            DisplayBox.Text = result;
            infix = result;
            displayString = result;
        }
    }
}
