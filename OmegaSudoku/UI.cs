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
            Console.WriteLine("Welcome to Omega Sudoku!");
            Stopwatch stopwatch = Stopwatch.StartNew();



            try
            {

                // Simulate some work
                Thread.Sleep(1000);
                SudokuBoard board = new SudokuBoard("800000070006010053040600000000080400003000700020005038000000800004050061900002000");
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
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                Console.WriteLine("RunTime: " + elapsedTime);
                Console.WriteLine("Thank you for using Omega Sudoku Solver!");
            }
        }
    }
}
