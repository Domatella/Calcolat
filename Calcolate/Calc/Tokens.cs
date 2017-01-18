using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc
{
    public delegate double Operation(double t1, double t2);

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

        public Token(Types type)
        {
            Type = type;
        }

        public Types Type { get; set; }

        public override string ToString()
        {
            return $"Type: {Type.ToString()}";
        }
    }

    public class Number : Token
    {
        public Number(Types type, double value) : base(type)
        {
            Value = value;
        }

        public double Value { get; set; }

        public override string ToString()
        {
            return base.ToString() + $", value: {Value.ToString()}";
        }
    }

    public class Operator : Token
    {
        private string op;

        public Operator(Types type, Operation action, string op) : base(type)
        {
            Action = action;
            this.op = op;
        }

        public Operation Action { get; set; }

        public override string ToString()
        {
            return base.ToString() + $", operation: {op}";
        }
    }
}
