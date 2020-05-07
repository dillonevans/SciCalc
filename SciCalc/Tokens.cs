using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SciCalc
{
    /// <summary>
    /// A class containing valid tokens within the grammar
    /// </summary>
    class Tokens
    {

        //Valid Operator Tokens
        public const string ADD_OP = "+";
        public const string DIV_OP = "/";
        public const string MULT_OP = "x";
        public const string SUB_OP = "−";
        public const string MOD_OP = "%";
        public const string FACT_OP = "!";
        public const string EXP_OP = "^";
        public const string LEFT_PAREN_OP = "(";
        public const string RIGHT_PAREN_OP = ")";
        public const string DECIMAL_OP = ".";

        //Valid Function Tokens
        public const string SIN = "sin";
        public const string COS = "cos";
        public const string TAN = "tan";
        public const string SQRT = "sqrt";
        public const string NATURAL_LOG = "ln";
        public const string LOG = "log";

        //Not to be confused with SUB_OP, this represents a negative
        public const string NEGATIVE_TOKEN = "-";

        //Valid Constant Tokens
        public const string PI = "π";
        public const string E = "e";

        /// <summary>
        /// Determines if the given token is a mathematical operator
        /// </summary>
        /// <param name="token"> The parsed token </param>
        /// <returns> True if the token is a function </returns>
        public static bool IsFunction(string token)
        {
            switch (token)
            {
                case SIN:
                case COS:
                case TAN:
                case SQRT:
                case NATURAL_LOG:
                case LOG:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines if the given token is a mathematical operator
        /// </summary>
        /// <param name="token"> The parsed token </param>
        /// <returns></returns>
        public static bool IsOperator(String token)
        {
            switch (token)
            {
                case ADD_OP:
                case SUB_OP:
                case MULT_OP:
                case DIV_OP:
                case MOD_OP:
                case FACT_OP:
                case EXP_OP:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines if the given token is numeric
        /// </summary>
        /// <param name="token"> The parsed token </param>
        /// <returns> True if the token is numeric </returns>
        public static bool IsNumeric(string token)
        {
            if (token.Length >= 2)
            {
                if (token.StartsWith("-") && (Char.IsDigit(token[1] )|| IsConstant(token[1].ToString()))) //token is a negative number
                {
                    return true;
                }
            }
            foreach (char parse in token)
            {
                if (Char.IsWhiteSpace(parse))
                {
                    return false;
                }
                if (!Char.IsDigit(parse) && !(parse== DECIMAL_OP[0] || parse == NEGATIVE_TOKEN[0]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Determines if the given token is a constant
        /// </summary>
        /// <param name="token"> The parsed token </param>
        /// <returns> True if the token is a constant </returns>
        public static bool IsConstant(string token)
        {
            switch (token)
            {
                case PI:
                case E:
                    return true;
                default:
                    return false;
            }
        }
    }

}
