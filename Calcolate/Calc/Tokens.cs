using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc
{
    public class Token
    {
        public enum Types
        {
            Number,
            LowOp,
            MiddleOp,
            HighOp,
            LeftBracket,
            RightBracket
        }

        public Types Type { get; set; }
        public string Value { get; set; }

        public Token(Types type, string value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return $"Type: {Type.ToString()}, value: {Value}";
        }
    }
}
