using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;

namespace Calc
{
    public static class Lexer
    {
        public class ErrorInputException : Exception
        {
            public ErrorInputException(string message, int place)
                : base($"Error in character #{place + 1}: {message}") { }
        }

        /// <summary>
        /// Lexical processing expression
        /// </summary>
        /// <param name="input">expression</param>
        /// <returns>array of simple tokens</returns>
        public static Token[] Process(string input)
        {
            // Replacing dots with commas 
            // for supporting both as decimal separators
            string buf = input.Replace('.', ',');
            input = buf;
            List<Token> Tokens = new List<Token>();

            // Flag that shows if current character 
            // is not below to previous number if it is a digit
            bool digit = true;

            for (int i = 0; i < input.Length; i++)
            {
                if (((input[i] > '0' && input[i] <= '9') || input[i] == ',') && digit)
                {
                    decNumber(ref i, input, ref digit, Tokens);
                }

                else if (input[i] == '0' && digit)
                {
                    if (i < input.Length - 1 && (input[i + 1] == 'x' || input[i + 1] == 'X'))
                    {
                        hexNumber(ref i, input, ref digit, Tokens);
                    }

                    else if (i < input.Length - 1 && (input[i + 1] >= '0' && input[i + 1] <= '7'))
                    {
                        octNumber(ref i, input, ref digit, Tokens);
                    }

                    // For processing just zeros
                    else 
                    {
                        decNumber(ref i, input, ref digit, Tokens);
                    }
                }

                else
                {
                    op(i, input, Tokens);
                    digit = true;
                }
            }

            return Tokens.ToArray();
        }

        /// <summary>
        /// Processing numbers as decimal
        /// </summary>
        /// <param name="i">indexer</param>
        /// <param name="input">expression</param>
        /// <param name="digit">digit flag</param>
        /// <param name="Tokens">using Tokens' list</param>
        private static void decNumber(ref int i, string input, ref bool digit, List<Token> Tokens)
        {
            StringBuilder number = new StringBuilder();

            // Flag for catching two commas in one number
            bool fraction = false;
            while ((input[i] >= '0' && input[i] <= '9') ||
                    input[i] == ',')
            {
                if (input[i] != ',' || (input[i] == ',' && !fraction))
                {
                    number.Append(input[i]);

                    if (input[i] == ',')
                    {
                        fraction = true;
                    }

                    if (i < input.Length - 1)
                    {
                        i++;
                    }

                    else
                    {
                        break;
                    }
                }

                else
                {
                    throw new ErrorInputException("digit cannot have two commas", i);
                }
            }

            if (i <= input.Length - 1)
            {
                i--;
                digit = false;
            }

            double value;
            if (double.TryParse(number.ToString(), out value))
            {
                Token token = new Number(Token.Types.Number, value);
                Tokens.Add(token);
            }
        }

        /// <summary>
        /// Processing number as HEX
        /// </summary>
        /// <param name="i">indexer</param>
        /// <param name="input">expression</param>
        /// <param name="digit">digit flag</param>
        /// <param name="Tokens">using Tokens' list</param>
        private static void hexNumber(ref int i, string input, ref bool digit, List<Token> Tokens)
        {
            StringBuilder number = new StringBuilder();
            i += 2;

            while ((input[i] >= '0' && input[i] <= '9') ||
                (input[i] >= 'a' && input[i] <= 'f') ||
                (input[i] >= 'A' && input[i] <= 'F'))
            {
                number.Append(input[i]);

                if (i < input.Length - 1)
                {
                    i++;
                }

                else
                {
                    break;
                }
            }

            isInteger(input, i);

            if (i <= input.Length - 1)
            {
                i--;
                digit = false;
            }

            int value;
            CultureInfo culture = new CultureInfo("en-GB");
            if (int.TryParse(number.ToString(),
                NumberStyles.AllowHexSpecifier, culture, out value))
            {
                Token token = new Number(Token.Types.Number, value);
                Tokens.Add(token);
            }
        }

        /// <summary>
        /// Processing numer as Oct
        /// </summary>
        /// <param name="i">indexer</param>
        /// <param name="input">expression</param>
        /// <param name="digit">digit flag</param>
        /// <param name="Tokens">using Tokens' array</param>
        private static void octNumber(ref int i, string input, ref bool digit, List<Token> Tokens)
        {
            StringBuilder number = new StringBuilder();
            i++;

            while (input[i] >= '0' && input[i] <= '7')
            {
                number.Append(input[i]);

                if (i < input.Length - 1)
                {
                    i++;
                }

                else
                {
                    break;
                }
            }

            isInteger(input, i);

            if (i <= input.Length - 1)
            {
                i--;
                digit = false;
            }

            int value = 0;
            // Transformation oct to dec
            for (int j = number.Length - 1; j >= 0; j--)
            {
                value += int.Parse(number[j].ToString()) * (int)Math.Pow(8, number.Length - 1 - j);
            }

            Token oct = new Number(Token.Types.Number, value);
            Tokens.Add(oct);
        }

        /// <summary>
        /// Processing operation symbols
        /// </summary>
        /// <param name="i">indexer</param>
        /// <param name="input">expression</param>
        /// <param name="Tokens">using Tokens' list</param>
        private static void op(int i, string input, List<Token> Tokens)
        {
            switch (input[i])
            {
                case '-':
                    Token minus = new Operator(Token.Types.LowOp, (a, b) => a - b, input[i].ToString());
                    Tokens.Add(minus);
                    return;
                case '+':
                    Token plus = new Operator(Token.Types.LowOp, (a, b) => a + b, input[i].ToString());
                    Tokens.Add(plus);
                    return;
                case '*':
                    Token mult = new Operator(Token.Types.MiddleOp, (a, b) => a * b, input[i].ToString());
                    Tokens.Add(mult);
                    return;
                case '/':
                    Token devide = new Operator(Token.Types.MiddleOp, (a, b) => a / b, input[i].ToString());
                    Tokens.Add(devide);
                    return;
                case '^':
                    Token pow = new Operator(Token.Types.HighOp, (a, b) => Math.Pow(a, b), input[i].ToString());
                    Tokens.Add(pow);
                    return;
                case '(':
                    Token left = new Token(Token.Types.LeftBracket);
                    Tokens.Add(left);
                    return;
                case ')':
                    Token right = new Token(Token.Types.RightBracket);
                    Tokens.Add(right);
                    return;
                case ' ':
                case ',':
                    return;
            }

            if (i != input.Length - 1 || (input[i] < '0' || input[i] > '9'))
            {
                throw new ErrorInputException("not allowed character", i);
            }
        }

        /// <summary>
        /// Checking HEX and OCT numbers on whether they are integer or not
        /// </summary>
        /// <param name="input">expression</param>
        /// <param name="i">indexer</param>
        private static void isInteger(string input, int i)
        {
            if (input[i] == ',')
            {
                throw new ErrorInputException("fractional HEX and OCT numbers are not supported", i);
            }
        }
    }
}
