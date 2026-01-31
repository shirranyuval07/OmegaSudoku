using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
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
            int count = 0;
            long start = 0;
            long end = 0;
            long elapsedTicks = 0;
            TimeSpan elapsed = TimeSpan.Zero;
            Stopwatch stopwatch = new Stopwatch();
            ISudokuBoard board = null;

            while (true)
            {
                Console.WriteLine("give sudoku board");

                string input = Console.ReadLine();
                if (input == "HALAS")
                    break;
                Console.WriteLine(input.Length +" is the input length");
                try
                {
                    if(input.Length <=256)
                        board = new SudokuBoard(input);
                    else
                    {
                        double counter = input.Count(c => c == '0');
                        Console.WriteLine((double)(counter /625));
                        if (counter == 625 || counter <= 625*0.95)
                        {
                            board = new FastSudokuBoard(input);
                        }
                        else
                        {
                            board = new SudokuBoard(input);
                        }
                    }



                    board.PrintBoard();
                    start = Stopwatch.GetTimestamp();
                    bool solved = Solver.Solve(board);
                    if(!solved)
                        throw new SudokuException("Sudoku could not be solved");
                    Console.WriteLine("after Solving: ");
                    board.PrintBoard();
                    end = Stopwatch.GetTimestamp();
                    elapsedTicks = end - start;
                    elapsed = TimeSpan.FromSeconds(elapsedTicks / (double)Stopwatch.Frequency);
                    Console.WriteLine(elapsed.ToString(@"mm\:ss\.ffffff"));

                }
                catch (SudokuException ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
            //checking 50k 17_clue sudokus to notice exactly where it falls
            
            /*
            List<string> listA = new List<string>();

            using (var reader = new StreamReader(@"C:\Users\Owner\Yuval_Omega\OmegaSudoku\OmegaSudoku\bin\Debug\17_clue.txt"))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    listA.Add(line);
                }
            }*/
            TimeSpan maxTimeSpan = TimeSpan.Zero;
            TimeSpan overall = TimeSpan.Zero;
            string longestPuzzle = "";
            /*
            foreach (string puzzle in listA)
            {
                start = 0;
                end = 0;
                elapsedTicks = 0;
                elapsed = TimeSpan.Zero;
                stopwatch = new Stopwatch();
                try
                {
                    start = Stopwatch.GetTimestamp();
                    // Arrange
                    SudokuBoard board = new SudokuBoard(puzzle);
                    // Act
                    Solver.Solve(board);
                    end = Stopwatch.GetTimestamp();
                    elapsedTicks = end - start;
                    elapsed = TimeSpan.FromSeconds(elapsedTicks / (double)Stopwatch.Frequency);
                    Console.Write("Number: " + count++ + " ,time is: " + elapsed.ToString(@"mm\:ss\.ffffff") + "\r");
                    overall += elapsed;
                    if (elapsed > maxTimeSpan)
                    {
                        maxTimeSpan = elapsed;
                        longestPuzzle = puzzle;
                    }

                    if (elapsed.TotalSeconds > 1)
                        throw new Exception("TestSolveSudoku failed: Sudoku Took too long");

                }
                catch (Exception ex)
                {
                    // Assert
                    throw new Exception("TestSolveSudoku failed: An exception occurred while solving the puzzle.", ex);
                }
            }

            Console.WriteLine("Longest puzzle is {0} \n Run time is: {1}",longestPuzzle,maxTimeSpan);

            Console.WriteLine("Overall time for 49,150 puzzles: " + overall.ToString(@"mm\:ss\.ffffff"));

            Console.WriteLine("Average time is: " + overall.TotalMilliseconds / 49150 + " milliseconds");
            */
            //checking 9 million sudokus from csv file
            List<string> listPuzzle = new List<string>();
            List<string> listSol = new List<string>();
            int i = 0;
            using (var reader = new StreamReader(@"C:\Users\Owner\Yuval_Omega\OmegaSudoku\OmegaSudoku\bin\Debug\sudoku.csv"))
            {
                while (!reader.EndOfStream) // && i < 10000
                {
                    var line = reader.ReadLine();
                    
                    var strings = line.Split(',');

                    listPuzzle.Add(strings[0]);
                    listSol.Add(strings[1]);
                }
            }

            var puzzleAndSol = listPuzzle.Zip(listSol, (puzzle, sol) => new { puzzle, sol });
            maxTimeSpan = TimeSpan.Zero;
            overall = TimeSpan.Zero;
            longestPuzzle = "";
            Console.WriteLine();

            foreach (var item in puzzleAndSol)
            {
                //if (count == 10000)
                  //  break;
                string puzzle = item.puzzle;
                string sol = item.sol;
                start = 0;
                end = 0;
                elapsedTicks = 0;
                elapsed = TimeSpan.Zero;
                stopwatch = new Stopwatch();
                try
                {
                    start = Stopwatch.GetTimestamp();
                    // Arrange
                    board = new SudokuBoard(puzzle);
                    // Act
                    if (!board.IsValidBoard())
                        continue;
                    Solver.Solve(board);
                    if (board.ToString() != sol)
                        throw new SudokuException("TestSolveSudoku failed: Sudoku solution is incorrect");
                    end = Stopwatch.GetTimestamp();
                    elapsedTicks = end - start;
                    elapsed = TimeSpan.FromSeconds(elapsedTicks / (double)Stopwatch.Frequency);
                    Console.Write("Number: "+count++ + " ,time is: "+ elapsed.ToString(@"mm\:ss\.ffffff") + "\r");
                    overall += elapsed;
                    if (elapsed > maxTimeSpan)
                    {
                        maxTimeSpan = elapsed;
                        longestPuzzle = puzzle;
                    }

                    if (elapsed.TotalSeconds > 1)
                        throw new SudokuException("TestSolveSudoku failed: Sudoku Took too long");

                }
                catch (SudokuException ex)
                {
                    // Assert
                    Console.WriteLine(puzzle);
                    throw new SudokuException("TestSolveSudoku failed: An exception occurred while solving the puzzle.", ex);
                }
            }

            Console.WriteLine("Longest puzzle is {0} \n Run time is: {1}", longestPuzzle, maxTimeSpan);

            Console.WriteLine("Overall time for 9,000,000 puzzles: " + overall.ToString(@"mm\:ss\.ffffff"));

            Console.WriteLine("Average time is: " + overall.TotalMilliseconds / 9000000 + " milliseconds");

            
            Console.WriteLine("Thank you for using Omega Sudoku Solver!");

        }
        public static void StartSudokuSolver2()
        {
            Console.WriteLine("Welcome to Omega Sudoku Solver (FAST MODE)");

            // Load puzzles
            List<string> puzzles = new List<string>();
            using (var reader = new StreamReader(
                @"C:\Users\Owner\Yuval_Omega\OmegaSudoku\OmegaSudoku\bin\Debug\sudoku.csv"))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var parts = line.Split(',');
                    puzzles.Add(parts[0]);
                }
            }

            Console.WriteLine($"Loaded {puzzles.Count:N0} puzzles");
            Console.WriteLine("Starting solve...\n");
            Console.WriteLine();
            int count = 0;

            var totalSw = Stopwatch.StartNew();

            foreach (var puzzle in puzzles)
            {
                // Allocate new board each puzzle (no LoadFromString)
                SudokuBoard board = new SudokuBoard(puzzle);

                Solver.Solve(board);

                count++;

                // Progress every 100k puzzles
                if (count % 100000 == 0)
                {
                    double elapsedSec = totalSw.Elapsed.TotalSeconds;
                    double rate = count / elapsedSec;
                    Console.Write($"Solved {count:N0} | {rate:N0} / sec | Elapsed {elapsedSec:F1}s \r");
                }
            }

            totalSw.Stop();

            Console.WriteLine("\nDONE");
            Console.WriteLine($"Total puzzles solved: {count:N0}");
            Console.WriteLine($"Total time: {totalSw.Elapsed}");
            Console.WriteLine($"Average per puzzle: {totalSw.Elapsed.TotalMilliseconds / count:F6} ms");
        }
        public static void StartSudokuSolverGenerated()
        {
            Console.WriteLine("Welcome to Omega Sudoku!");

            int j = 0;

            while (true)
            {
                try
                {
                    j++;
                    Console.WriteLine("Give board size");
                    string s = UI.GeneratorNxN(int.Parse(Console.ReadLine()));
                    ISudokuBoard board = null;

                    if (s.Length <= 256)
                        board = new SudokuBoard(s);
                    else
                    {
                        double counter = s.Count(c => c == '0');
                        Console.WriteLine((double)(counter / 625));
                        if (counter <= 0.95 * 625) // alot of try and error
                        {
                            board = new FastSudokuBoard(s);
                        }
                        else
                        {
                            board = new SudokuBoard(s);
                        }
                    }
                    double ratio = (double)s.Count(c => c == '0') / s.Length;
                    Console.WriteLine($"Difficulty Ratio: {ratio:P2}");


                    Console.WriteLine("--- Initial Board ---");
                    board.PrintBoard();

                    Console.WriteLine("Look at the board for a second");
                    Thread.Sleep(1000);
                    long start = Stopwatch.GetTimestamp();
                    bool solved = Solver.Solve(board);
                    long end = Stopwatch.GetTimestamp();

                    if (solved)
                    {
                        Console.WriteLine("\n--- SOLVED ---");
                        board.PrintBoard();
                    }
                    else
                    {
                        Console.WriteLine("\n!!! FAILED TO SOLVE !!!");
                    }

                    TimeSpan elapsed = TimeSpan.FromSeconds((end - start) / (double)Stopwatch.Frequency);
                    Console.WriteLine($"Time: {elapsed:mm\\:ss\\.ffffff}");

                    Console.WriteLine("\nWaiting 5 seconds...");
                    Thread.Sleep(5000);
                    Console.Write("\x1b[3J");
                    Console.Clear();
                    Thread.Sleep(1000);

                }
                catch (SudokuException ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
        }
        public static string GeneratorNxN(int boardLength)
        {
            if(boardLength > 25 || boardLength < 4)
                throw new InvalidPuzzleException("Board length must be between 4 and 25");
            Constants.boardLen = boardLength;
            Constants.SetSymbol();

            int totalCells = boardLength * boardLength;

            string emptyBoard = new string('0', totalCells);
            SudokuBoard board = new SudokuBoard(emptyBoard);
            bool solved = Solver.Solve(board);
            if(!solved)
                throw new Exception("Generated board is unsolvable");
            Random rnd = new Random();

            int cellsToRemove;

            switch (boardLength)
            {
                case 4:
                    cellsToRemove = 10;
                    break;
                case 9:
                    cellsToRemove = 64;
                    break;
                case 16:
                    cellsToRemove = 200;
                    break;
                case 25:
                    cellsToRemove = 580;
                    break;
                default:
                    cellsToRemove = (int)(totalCells * 0.60);
                    break;
            }
            List<Tuple<int, int>> allCoordinates = new List<Tuple<int, int>>();
            for (int r = 0; r < boardLength; r++)
            {
                for (int c = 0; c < boardLength; c++)
                {
                    allCoordinates.Add(new Tuple<int, int>(r, c));
                }
            }

            int n = allCoordinates.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                var value = allCoordinates[k];
                allCoordinates[k] = allCoordinates[n];
                allCoordinates[n] = value;
            }

            for (int i = 0; i < cellsToRemove; i++)
            {
                var coord = allCoordinates[i];
                board.board[coord.Item1, coord.Item2].Value = '0';
            }

            return board.ToString();
        }
    }
}
