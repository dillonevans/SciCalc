using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SciCalc
{
    class Calculator
    {

        public static double EvaluateExpression(string expression)
        {
            expression = ConvertToPostFix(CleanUpString(expression));
            Stack<double> valueStack = new Stack<double>();
            string[] tokens = expression.Trim().Split(' ');
            double l, r;
            Debug.WriteLine(tokens.Length);
            foreach (string token in tokens)
            {
                if (token != " ")
                {

                    Debug.WriteLine("This is a token: " + token);
                    if (IsNumeric(token))
                    {
                        if (token == "π")
                        {
                            valueStack.Push(Math.PI);
                        }
                        else if (token == "e")
                        {
                            Debug.WriteLine("E");
                            valueStack.Push(Math.E);
                        }
                        else
                        {
                            valueStack.Push(Double.Parse(token));
                        }
                    }
                    else if (IsOperator(token))
                    {
                        if (token != "!")
                        {
                            l = valueStack.Pop();
                            r = valueStack.Pop();
                            valueStack.Push(EvaluateExpression(l, r, token));
                        }
                        else
                        {
                            valueStack.Push(Factorial(valueStack.Pop()));
                        }
                    }
                    else if (IsFunction(token))
                    {
                        valueStack.Push(EvaluateFunction(token, valueStack.Pop()));
                    }
                }
            }
            return valueStack.Pop();

            // Console.WriteLine("HERE");

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
                case "+":
                    return leftOperand + rightOperand;
                case "-":
                    return leftOperand - rightOperand;
                case "x":
                    return leftOperand * rightOperand;
                case "/":
                    return leftOperand / rightOperand;
                case "%":
                    return leftOperand % rightOperand;
                case "^":
                    return Math.Pow(leftOperand, rightOperand);
            }
            return 0;
        }

        /// <summary>
        /// Evaluates the function on the stack
        /// </summary>
        /// <param name="func"> The function to evaluate </param>
        /// <param name="val"> The value to insert into the function </param>
        /// <returns> The result of the function evaluation </returns>
        private static double EvaluateFunction(string func, double val)
        {
            switch (func)
            {
                case "sin":
                    return Math.Sin(val);
                case "cos":
                    return Math.Cos(val);
                case "tan":
                    return Math.Tan(val);
                case "sqrt":
                    return Math.Sqrt(val);
                case "ln":
                    return Math.Log(val);
            }
            return 0;
        }

        /**
         * Utilizes Diijkstra's Shunting-Yard Algorithm to convert the expression given
         * to Reverse-Polish Notation or Postfix form
         * @param expression The user input expression in infix
         * @return The given expression in RPN
         */
        private static string ConvertToPostFix(string expression)
        {
            Queue<string> output = new Queue<string>();
            Stack<string> operatorStack = new Stack<string>();
            StringBuilder outString = new StringBuilder();
            string[] tokens = expression.Split(' ');

            foreach (string token in tokens)
            {
                if (IsNumeric(token))
                {
                    if (token == "π")
                    {
                        output.Enqueue(Math.PI.ToString());
                    }
                    else if (token == "e")
                    {
                        output.Enqueue(Math.E.ToString());
                    }
                    else
                    {
                        output.Enqueue(token);

                    }
                }
                else if (IsFunction(token))
                {
                    operatorStack.Push(token);
                }
                else if (IsOperator(token))
                {
                    if (operatorStack.Count != 0)
                    {
                        while (IsFunction(operatorStack.Peek()) || (OperatorPrecedence(token) < OperatorPrecedence(operatorStack.Peek()) || (OperatorPrecedence(token) == OperatorPrecedence(operatorStack.Peek()) && IsLeftAssociative(token)) && operatorStack.Peek() != ("(")))
                        {
                            output.Enqueue(operatorStack.Pop());
                            if (operatorStack.Count == 0)
                            {
                                break;
                            }
                        }
                    }
                    operatorStack.Push(token);
                }
                else if (token == "(")
                {
                    operatorStack.Push(token);
                }
                else if (token == (")"))
                {
                    while (operatorStack.Peek() != ("("))
                    {
                        output.Enqueue(operatorStack.Pop());
                    }
                    operatorStack.Pop();
                }
            }
            while (operatorStack.Count != 0)
            {
                output.Enqueue(operatorStack.Pop());
            }
            foreach (string str in output)
            {
                outString.Append(str + " ");
            }

            return outString.ToString();
        }

        private static bool IsNumeric(string token)
        {
            foreach (char parse in token)
            {
                if (Char.IsWhiteSpace(parse))
                {
                    return false;
                }
                if (!Char.IsDigit(parse) && !(parse == '.' || parse == 'π' || parse == 'e'))
                {
                    return false;
                }
            }
            return true;
        }
        private static bool IsFunction(string token)
        {
            switch (token)
            {
                case "sin":
                case "cos":
                case "tan":
                case "sqrt":
                case "ln":
                    return true;
                default:
                    return false;
            }
        }
        private static int OperatorPrecedence(string op)
        {
            switch (op)
            {
                case "^": //Expontiation
                    return 4;
                case "!":
                    return 3;
                case "x":
                case "/":
                case "%":
                    return 2;
                case "+":
                case "-":
                    return 1;
                default:
                    return 0;
            }
        }

        public static string CleanUpString(string expr)
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
            return editedString.ToString();
        }

        private static bool IsLeftAssociative(string op)
        {
            switch (op)
            {
                case "+":
                case "-":
                case "x":
                case "/":
                case "%":
                case "!":
                    return true;
                case "^":
                default:
                    return false;
            }
        }

        private static bool IsOperator(String token)
        {
            switch (token)
            {
                case "+":
                case "-":
                case "x":
                case "/":
                case "^":
                case "%":
                case "!":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Calculates n!
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
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
