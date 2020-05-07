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
        /// <summary>
        /// Converts the given expression into Reverse-Polish Notation and Applies the Appropriate Operators or Functions
        /// to the current token
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static double EvaluateExpression(string expression)
        {
            expression = ConvertToPostFix(CleanUpString(expression)); //Clean up the expression and parse it to post fix
            double l, r;
            Stack<double> valueStack = new Stack<double>();
            string[] tokens = expression.Trim().Split(' ');

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
                            //Pop the Stack Twice to Obtain the Operands and Apply the Operator
                            l = valueStack.Pop();
                            r = valueStack.Pop();
                            valueStack.Push(EvaluateExpression(l, r, token));
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
        private static double EvaluateFunction(string func, double val)
        {
            //Perform Token Lookup and Return The Evaluation of the Associated Function for the given value
            switch (func)
            {
                case Tokens.SIN:
                    return Math.Sin(val);
                case Tokens.COS:
                    return Math.Cos(val);
                case Tokens.TAN:
                    return Math.Tan(val);
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
        private static string ConvertToPostFix(string expression)
        {
            Queue<string> outputQueue = new Queue<string>();
            Stack<string> operatorStack = new Stack<string>();
            StringBuilder postFixString = new StringBuilder();
            string[] tokens = expression.Split(' ');

            foreach (string token in tokens)
            {
                if (Tokens.IsConstant(token))
                {
                    outputQueue.Enqueue(EvaluateConstant(token).ToString());
                }
                //If the token is a number, enqueue it to the Output Queue
                else if (Tokens.IsNumeric(token))
                { 
                    outputQueue.Enqueue(token);
                }
                // If the token is a function push it onto the Operator Stack
                else if (Tokens.IsFunction(token)) 
                {
                    operatorStack.Push(token);
                }
                //If the token is a operator push it onto the Operator Stack
                else if (Tokens.IsOperator(token)) 
                {

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
                }
                //If the token is a left parentheses, push it onto the Operator Stack
                else if (token == Tokens.LEFT_PAREN_OP) 
                {
                    operatorStack.Push(token);
                }
                //If the Token is a right parenthesis, pop the stack until a left parenthesis is found
                else if (token == Tokens.RIGHT_PAREN_OP) 
                {
                    while (operatorStack.Peek() != Tokens.LEFT_PAREN_OP)
                    {
                        outputQueue.Enqueue(operatorStack.Pop());
                        if (operatorStack.Count == 0)
                        {
                            throw new Exception("Imbalanced Parenthetical");
                        }
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
        /// Removes any additional whitespace from the infix string for conversion to post fix
        /// </summary>
        /// <param name="expr"> The infix expression </param>
        /// <returns></returns>
        private static string CleanUpString(string expr)
        {
            StringBuilder editedString = new StringBuilder();

            for (int i = 0; i < expr.Length - 1; i++)
            {
                if (expr[i] != ' ')
                {
                    editedString.Append(expr[i]);
                }
                else if (expr[i] == ' ' && expr[i + 1] != ' ')
                {
                    editedString.Append(expr[i]);
                }
            }
            if (expr[expr.Length - 1] != ' ')
            {
                editedString.Append(expr[expr.Length - 1]);
            }
            Debug.WriteLine(editedString.ToString());
            return editedString.ToString();
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
                case Tokens.EXP_OP:
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
    }
}
