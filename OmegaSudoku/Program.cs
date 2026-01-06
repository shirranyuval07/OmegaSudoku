using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaSudoku
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Omega Sudoku!");
            SquareCell sc = new SquareCell(3,5);
            Console.WriteLine("Created a SquareCell at: " + sc);
        }
    }
}
