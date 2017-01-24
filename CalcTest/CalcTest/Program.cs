using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalcTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter expression:");
            string expr = Console.ReadLine();

            Calc.Lexer lexer = new Calc.Lexer(expr);

            try
            {
                Calc.Token[] tokens = lexer.Process();

                foreach (var t in tokens)
                {
                    Console.WriteLine(t);
                }
            }

            catch (Calc.Lexer.InputErrorException e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        }
    }
}
