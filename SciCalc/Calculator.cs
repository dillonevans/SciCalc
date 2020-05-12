using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SciCalc
{
    /// <summary>
    /// <b> Author: Dillon Evans </b>
    /// <br> This Class is Used to Perform Calculations using Diijkstra's Shunting Yard Algorithm. </br>
    /// <br> Parses input tokens based on the constants defined within the <see cref="Tokens"/> class. </br>
    /// </summary>
    class Calculator
    {
        public enum Mode { DEG, RAD}
        private Mode currentMode;

        public Mode CurrentMode
        {
            set { currentMode = value; }
        }

        /// <summary>
        /// Converts the given expression into Reverse-Polish Notation and Applies the Appropriate Operators or Functions
        /// to the current token
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private double EvaluateExpression(string expression)
        {
            expression = ConvertToPostFix(expression); //Convert the expression to post fix
            Stack<double> valueStack = new Stack<double>();
            string[] tokens = expression.Trim().Split(' ');
            double l, r; //The left and right operands of an expression

            //Read in each token of the given expression
            foreach (string token in tokens)
            {
                if (token != " ") //Ignore Whitespaces
                {
                    //If the Current Token is a Number
                    if (Tokens.IsNumeric(token))
                    {
                        //Push the Token Onto the Value Stack
                        valueStack.Push(Double.Parse(token));
                    }

                    //If the Current Token is an Operator
                    else if (Tokens.IsOperator(token))
                    {
                        if (token == Tokens.FACT_OP)
                        {
                            //Calculates the Value of the Factorial of the Top of The Stack and Stores the Value
                            valueStack.Push(Factorial(valueStack.Pop()));
                        }
                        else
                        {
                            //Ensure that the stack has two variables to pop
                            if (valueStack.Count >=2)
                            {
                                //Pop the Stack Twice to Obtain the Operands and Apply the Operator
                                l = valueStack.Pop();
                                r = valueStack.Pop();
                                valueStack.Push(EvaluateExpression(l, r, token));
                            }
                        }
                    }

                    //If the current token identifies a Function
                    else if (Tokens.IsFunction(token))
                    {
                        //Apply the function to the value on the top of the stack and store the result.
                        valueStack.Push(EvaluateFunction(token, valueStack.Pop()));
                    }
                }
            }
            return valueStack.Pop();
        }

        /// <summary>
        /// Evaluates a given expression valid within the grammar
        /// </summary>
        /// <param name="rightOperand"></param>
        /// <param name="leftOperand"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        private static double EvaluateExpression(double rightOperand, double leftOperand, string op)
        {
            switch (op)
            {
                case Tokens.ADD_OP:
                    return leftOperand + rightOperand;
                case Tokens.SUB_OP:
                    return leftOperand - rightOperand;
                case Tokens.MULT_OP:
                    return leftOperand * rightOperand;
                case Tokens.DIV_OP:
                    return leftOperand / rightOperand;
                case Tokens.MOD_OP:
                    return leftOperand % rightOperand;
                case Tokens.EXP_OP:
                    return Math.Pow(leftOperand, rightOperand);
            }
            return 0;
        }

        /// <summary>
        /// Evaluates the function on the top of the stack
        /// </summary>
        /// <param name="func"> The function to evaluate </param>
        /// <param name="val"> The value to insert into the function </param>
        /// <returns> The result of the function evaluation </returns>
        private double EvaluateFunction(string func, double val)
        {
            //Perform Token Lookup and Return The Evaluation of the Associated Function for the given value
            double modifier = currentMode == Mode.DEG ? (Math.PI / 180.0) : 1;
            
            switch (func)
            {
                case Tokens.SIN:
                    return Math.Sin(val * modifier);
                case Tokens.COS:
                    return Math.Cos(val * modifier);
                case Tokens.TAN:
                    return Math.Tan(val * modifier);
                case Tokens.SQRT:
                    return Math.Sqrt(val);
                case Tokens.NATURAL_LOG:
                    return Math.Log(val);
                case Tokens.LOG:
                    return Math.Log10(val);
            }
            return 0;
        }

        /// <summary>
        /// Evaluate the constant represented by the token
        /// </summary>
        /// <param name="constant"> The token representing the constant </param>
        /// <returns> The value of the constant </returns>
        private static double EvaluateConstant(string constant)
        {
            //Perform Token Lookup and Return Constant Value (If It is a Valid Token)
            switch (constant)
            {
                case Tokens.PI:
                    return Math.PI;
                case Tokens.E:
                    return Math.E;
                default:
                    return 0;
            }  
        }

      /// <summary>
      /// Utilizes Diijkstra's Shunting-Yard Algorithm to convert the expression given to
      /// "Reverse-Polish Notation" or Postfix 
      /// </summary>
      /// <param name="expression"> The user input expression in infix</param>
      /// <returns> The given expression in RPN </returns>
        private string ConvertToPostFix(string expression)
        {
            Queue<string> outputQueue = new Queue<string>();
            Stack<string> operatorStack = new Stack<string>();
            StringBuilder postFixString = new StringBuilder();
            int opCount = 0, numCount = 0;
            bool lastTokenIsOperator = false;
            string[] tokens = expression.Split(' ');

            foreach (string token in tokens)
            {
                //If the token is e or Pi, enqueue the numeric value
                if (Tokens.IsConstant(token))
                {
                    lastTokenIsOperator = false;
                    numCount++;
                    outputQueue.Enqueue(EvaluateConstant(token).ToString());
                }

                //If the token is a number, enqueue it to the Output Queue
                else if (Tokens.IsNumeric(token))
                {
                    lastTokenIsOperator = false;
                    numCount++;
                    outputQueue.Enqueue(token);
                }

                // If the token is a function push it onto the Operator Stack
                else if (Tokens.IsFunction(token)) 
                {
                    lastTokenIsOperator = false;
                    operatorStack.Push(token);
                }

                //If the token is a operator push it onto the Operator Stack
                else if (Tokens.IsOperator(token)) 
                {
                    //IE: 2++ is invalid
                    if (lastTokenIsOperator) { throw new Exception("Invalid Expression"); }
                    
                    //If the Operator Stack isn't empty
                    if (operatorStack.Count != 0)
                    {
                        //While there are still left associative operators, a function, or an operator of higher precedence
                        //Pop the top of the op stack and Enqueue it. If a left parenthesis is encountered, halt.
                        while (Tokens.IsFunction(operatorStack.Peek()) || (OperatorPrecedence(token) < OperatorPrecedence(operatorStack.Peek()) || (OperatorPrecedence(token) == OperatorPrecedence(operatorStack.Peek()) && IsLeftAssociative(token)) && operatorStack.Peek() != (Tokens.LEFT_PAREN_OP)))
                        {
                            outputQueue.Enqueue(operatorStack.Pop());
                            if (operatorStack.Count == 0)
                            { 
                                break;
                            }
                        }
                    }
                    //Push the current token to the Operator Stack
                    operatorStack.Push(token); 
            
                    /*Other operators are evaluated in the form "a *|+|-|/ b" but because factorial is a special case 
                    it's an exception to the token classification */
                    if (token != Tokens.FACT_OP)
                    {
                        lastTokenIsOperator = true;
                        opCount++;
                    }
                }

                //If the token is a left parentheses, push it onto the Operator Stack
                else if (token == Tokens.LEFT_PAREN_OP) 
                {
                    lastTokenIsOperator = false;
                    operatorStack.Push(token);
                }

                //If the Token is a right parenthesis, pop the stack until a left parenthesis is found
                else if (token == Tokens.RIGHT_PAREN_OP) 
                {
                    lastTokenIsOperator = false;
                    while (operatorStack.Peek() != Tokens.LEFT_PAREN_OP)
                    {
                        outputQueue.Enqueue(operatorStack.Pop());
                    }
                    operatorStack.Pop();
                }
            }

            //Enqueue any remaining tokens to the Output Queue
            while (operatorStack.Count != 0)
            {
                outputQueue.Enqueue(operatorStack.Pop());
            }

            //Append whatever is remaining in the output queue to the string
            foreach (string str in outputQueue)
            {
                postFixString.Append(str + " ");
            }

            Debug.WriteLine("Post Fix: {0}", postFixString);

            //In the case of 2+ or +2, throw an exception
            if (opCount == numCount) { throw new Exception("Invalid Expression"); }
            return postFixString.ToString();
        }
       
        /// <summary>
        /// Determines the precedence of the operator for order of evaluation
        /// </summary>
        /// <param name="op"> The operator </param>
        /// <returns> The precedence of the operator </returns>
        private static int OperatorPrecedence(string op)
        {
            //Perform Token Lookup and Return Precedence
            switch (op)
            {
                case Tokens.EXP_OP: 
                    return 4;
                case Tokens.FACT_OP:
                    return 3;
                case Tokens.MULT_OP:
                case Tokens.DIV_OP:
                case Tokens.MOD_OP:
                    return 2;
                case Tokens.ADD_OP:
                case Tokens.SUB_OP:
                    return 1;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Determines if the operator is left associative
        /// </summary>
        /// <param name="op"> The operator </param>
        /// <returns> True if the operator is lest associative </returns>
        private static bool IsLeftAssociative(string op)
        {
            switch (op)
            {
                case Tokens.ADD_OP:
                case Tokens.SUB_OP:
                case Tokens.MULT_OP:
                case Tokens.DIV_OP:
                case Tokens.MOD_OP:
                case Tokens.FACT_OP:
                    return true;
                default:
                    return false;
            }
        }
    
        /// <summary>
        /// Calculates n!
        /// </summary>
        /// <param name="n"> The value to find n! for </param>
        /// <returns> n! </returns>
        private static double Factorial(double n)
        {
            if (n == 0)
            {
                return 1;
            }
            if (n <= 2)
            {
                return n;
            }
            return n * Factorial(n - 1);
        }

        /// <summary>
        /// Returns a string containing the result of evaluation
        /// </summary>
        /// <param name="expression"> The expression to evaluate </param>
        /// <returns> The result of computation if no errors arise </returns>
        public string GetComputationString(string expression)
        {
            string output;
            try
            {
               return EvaluateExpression(expression).ToString();
            }
            catch (Exception e)
            {
                output = "ERROR:" + e.Message;
            }
            return output;
        }  
    }
}
