using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;

namespace Calc
{
    public class Lexer
    {
        public class InputErrorException : Exception
        {
            public InputErrorException(string message, int place)
                : base($"Error in character #{place + 1}: {message}") { }
        }

        private string input;
        private int i;
        private List<Token> tokens;

        public string InputString { get { return input; } }
        public Token[] Tokens { get { return tokens.ToArray(); } }

        public Lexer(string input)
        {
            this.input = input;
            tokens = new List<Token>();
        }

        /// <summary>
        /// Lexical processing expression
        /// </summary>
        /// <returns>array of simple tokens</returns>
        public Token[] Process()
        {
            // Replacing dots with commas 
            // for supporting both as decimal separators
            string buf = input.Replace('.', ',');
            input = buf;

            for (i = 0; i < input.Length; i++)
            {
                if (((input[i] > '0' && input[i] <= '9') || input[i] == ','))
                {
                    procDecNumber();
                }

                else if (input[i] == '0')
                {
                    if (i < input.Length - 1 && (input[i + 1] == 'x' || input[i + 1] == 'X'))
                    {
                        prcoHexNumber();
                    }

                    else if (i < input.Length - 1 && (input[i + 1] >= '0' && input[i + 1] <= '7'))
                    {
                        procOctNumber();
                    }

                    // For processing just zeros
                    else 
                    {
                        procDecNumber();
                    }
                }

                else
                {
                    procOp();
                }
            }

            return tokens.ToArray();
        }

        /// <summary>
        /// Processing numbers as decimal
        /// </summary>
        private void procDecNumber()
        {
            StringBuilder number = new StringBuilder();

            // Flag for catching two commas in one number
            bool fraction = false;
            bool isLastChar = false;
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
                        isLastChar = true;
                        break;
                    }
                }

                else
                {
                    throw new InputErrorException("digit cannot have two commas", i);
                }
            }

            if (i <= input.Length - 1 && !isLastChar)
            {
                i--;
            }

            double value;
            if (double.TryParse(number.ToString(), out value))
            {
                Token token = new Token(Token.Types.Number, value.ToString());
                tokens.Add(token);
            }
        }

        /// <summary>
        /// Processing number as HEX
        /// </summary>
        private void prcoHexNumber()
        {
            StringBuilder number = new StringBuilder();
            i += 2;

            bool isLastChar = false;
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
                    isLastChar = true;
                    break;
                }
            }

            exceptDigit();

            if (i <= input.Length - 1 && !isLastChar)
            {
                i--;
            }

            int value;
            CultureInfo culture = new CultureInfo("en-GB");
            if (int.TryParse(number.ToString(),
                NumberStyles.AllowHexSpecifier, culture, out value))
            {
                Token token = new Token(Token.Types.Number, value.ToString());
                tokens.Add(token);
            }
        }

        /// <summary>
        /// Processing numer as Oct
        /// </summary>
        private void procOctNumber()
        {
            StringBuilder number = new StringBuilder();
            i++;

            bool isLastChar = false;
            while (input[i] >= '0' && input[i] <= '7')
            {
                number.Append(input[i]);

                if (i < input.Length - 1)
                {
                    i++;
                }

                else
                {
                    isLastChar = true;
                    break;
                }
            }

            exceptDigit();

            if (i <= input.Length - 1 && !isLastChar)
            {
                i--;
            }

            int value = 0;
            // Transformation oct to dec
            for (int j = number.Length - 1; j >= 0; j--)
            {
                value += int.Parse(number[j].ToString()) * (int)Math.Pow(8, number.Length - 1 - j);
            }

            Token oct = new Token(Token.Types.Number, value.ToString());
            tokens.Add(oct);
        }

        /// <summary>
        /// Processing operation symbols
        /// </summary>
        private void procOp()
        {
            switch (input[i])
            {
                case '-':
                    Token minus = new Token(Token.Types.LowOp, input[i].ToString());
                    tokens.Add(minus);
                    return;
                case '+':
                    Token plus = new Token(Token.Types.LowOp, input[i].ToString());
                    tokens.Add(plus);
                    return;
                case '*':
                    Token mult = new Token(Token.Types.MiddleOp, input[i].ToString());
                    tokens.Add(mult);
                    return;
                case '/':
                    Token devide = new Token(Token.Types.MiddleOp, input[i].ToString());
                    tokens.Add(devide);
                    return;
                case '^':
                    Token pow = new Token(Token.Types.HighOp, input[i].ToString());
                    tokens.Add(pow);
                    return;
                case '(':
                    Token left = new Token(Token.Types.LeftBracket, input[i].ToString());
                    tokens.Add(left);
                    return;
                case ')':
                    Token right = new Token(Token.Types.RightBracket, input[i].ToString());
                    tokens.Add(right);
                    return;
                case ' ':
                case ',':
                case '\t':
                case '\r':
                case '\n':
                    return;
            }

            if (i != input.Length - 1 || (input[i] < '0' || input[i] > '9'))
            {
                throw new InputErrorException("not allowed character", i);
            }
        }

        /// <summary>
        /// Checking HEX and OCT numbers on whether they are integer or not
        /// </summary>
        private void exceptDigit()
        {
            if (input[i] == ',')
            {
                throw new InputErrorException("fractional HEX and OCT numbers are not supported", i);
            }
        }
    }
}
