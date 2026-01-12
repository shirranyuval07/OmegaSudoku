using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OmegaSudoku
{
    static class UI
    {
        public static void StartSudokuSolver()
        {
            Console.WriteLine("Welcome to Omega Sudoku! \n enter HALAS to stop");
            Stopwatch stopwatch = Stopwatch.StartNew();
            while(true)
            {
                Console.WriteLine("give sudoku board");

                string input = Console.ReadLine();
                if (input == "HALAS")
                    break;
                Console.WriteLine(input.Length);
               /* try
                {

                    SudokuBoard board = new SudokuBoard(input);
                    board.PrintBoard();
                    Solver.Solve(board);
                    Console.WriteLine("after Solving: ");
                    board.PrintBoard();


                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
                finally
                {
                    // 2. Stop the stopwatch
                    stopwatch.Stop();

                    // 3. Get elapsed time
                    TimeSpan ts = stopwatch.Elapsed;
                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                    Console.WriteLine("RunTime: " + elapsedTime);
                    Console.WriteLine("Thank you for using Omega Sudoku Solver!");
                }*/
            }


        }
    }
}
