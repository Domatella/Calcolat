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
            string exp = Console.ReadLine();

            try
            {
                Calc.Token[] tokens = Calc.Lexer.Process(exp);

                foreach (var t in tokens)
                {
                    Console.WriteLine(t);
                }
            }
            catch (Calc.Lexer.ErrorInputException e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        }
    }
}
