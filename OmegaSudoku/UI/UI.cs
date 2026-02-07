using OmegaSudoku.Core;
using OmegaSudoku.Exceptions;
using OmegaSudoku.Logic;
using OmegaSudoku.Utils;
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

namespace OmegaSudoku.UI
{
    static class UI
    {
        public static void Start()
        {
            Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) =>
            {
                e.Cancel = true;
            };
            Console.WriteLine("Welcome! What would you like to do?");
            Console.WriteLine("1. Solve a sudoku");
            Console.WriteLine("2. Generate and solve a sudoku board");
            Console.WriteLine("3. Read From file");
            Console.WriteLine("4. Put board into file (between 4x4 to 25x25)");
            Console.WriteLine("5. Check 50k hard 9x9 boards.");
            Console.WriteLine("6. Break from the app");
            Console.WriteLine("C. Clear Console");
            Console.WriteLine("Enter 'menu' to look at the menu again");
            while (true)
            {
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        StartSudokuSolver();
                        Console.WriteLine("Welcome back to the menu. Press 'menu' to look at the menu again");
                        break;
                    case "2":
                        StartSudokuSolverGenerated();
                        Console.WriteLine("Welcome back to the menu. Press 'menu' to look at the menu again");
                        break;
                    case "3":
                        ReadFromFile();
                        Console.WriteLine("Welcome back to the menu. Press 'menu' to look at the menu again");
                        break;
                    case "4":
                        StartInputIntoFile();
                        Console.WriteLine("Welcome back to the menu. Press 'menu' to look at the menu again");
                        break;
                    case "5":
                        StartSudokuSolver2();
                        Console.WriteLine("Welcome back to the menu. Press 'menu' to look at the menu again");
                        break;
                    case "6":
                        {
                            Console.WriteLine("Thank you for using Omega Sudoku Solver");
                            break;
                        }
                    case "C":
                        Console.Clear();
                        Console.Write("\x1b[3J");
                        Console.WriteLine("Welcome back to the menu. Press 'menu' to look at the menu again");
                        break;

                    case "menu":
                        {
                            Console.WriteLine("Welcome! What would you like to do?");
                            Console.WriteLine("1. Solve a sudoku");
                            Console.WriteLine("2. Generate and solve a sudoku board");
                            Console.WriteLine("3. Read From file");
                            Console.WriteLine("4. Put board into file and solve (between 4x4 to 25x25)");
                            Console.WriteLine("5. Check 50k hard 9x9 boards.");
                            Console.WriteLine("6. Break from the app");
                            Console.WriteLine("C. Clear Console");
                            Console.WriteLine("Enter 'menu' to look at the menu again");
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("Insert a correct Input !");
                            break;
                        }

                }
                if (input == "6")
                    break;
            }
        }

        public static void StartSudokuSolver()
        {
            Console.WriteLine("Welcome to Omega Sudoku! \n enter HALAS to stop \n OMEGA");
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
                if (input == "OMEGA")
                {
                    Console.WriteLine("Some say that Ω Is the best");
                    continue;
                }
                Console.WriteLine(input.Length + " is the input length");
                try
                {

                    double counter = input.Count(c => c == Constants.emptyCell);
                    Console.WriteLine((double)(counter / input.Length));
                    if (counter == input.Length || counter <= input.Length * 0.95)
                    {
                        board = new FastSudokuBoard(input);
                    }
                    else
                    {
                        board = new SudokuBoard(input);
                    }



                    board.PrintBoard();
                    start = Stopwatch.GetTimestamp();
                    bool solved = Solver.Solve(board);
                    if (!solved)
                        throw new UnsolvableSudokuException("Sudoku could not be solved");
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
        }
        //fast checker without checking if sol is right only checking the time
        public static void StartSudokuSolver2()
        {
            Console.WriteLine("Welcome to Omega Sudoku Solver (FAST MODE)");

            // Load puzzles
            List<string> puzzles = new List<string>();
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(baseDir, "FilesData", "17_clue.txt");
            using (var reader = new StreamReader(filePath))
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
                // Allocate new board each puzzle
                SudokuBoard board = new SudokuBoard(puzzle);

                Solver.Solve(board);

                count++;

                // Progress every 10k puzzles
                if (count % 10000 == 0)
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
        public static void StartInputIntoFile()
        {
            Console.WriteLine("Input string to put into the file. Enter HALAS to stop");
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(baseDir, "FilesData", "boards.txt");
            while (true)
            {
                string input = Console.ReadLine();
                if (input == "HALAS")
                    break;
                try
                {
                    SudokuBoard board = new SudokuBoard(input); //validate through creating a board
                    File.AppendAllText(filePath, input + Environment.NewLine);
                    Console.WriteLine("String successfully written to file.");

                }
                catch (IOException ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
                catch (SudokuException ex)
                {
                    Console.WriteLine($"A Sudoku Board Exception occured: {ex.Message}");
                }
            }

        }
        public static void ReadFromFile()
        {
            List<string> listA = new List<string>();

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(baseDir, "FilesData", "boards.txt");
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    listA.Add(line);
                }
            }
            foreach (string line in listA)
            {
                try
                {
                    SudokuBoard board = new SudokuBoard(line);
                    board.PrintBoard();
                    bool solved = Solver.Solve(board);
                    if (solved)
                    {
                        Console.WriteLine("solved succesfully");
                        board.PrintBoard();
                    }
                    else
                        Console.WriteLine("Failed to solve");
                }
                catch (SudokuException ex)
                {
                    Console.WriteLine($"A Sudoku Board Exception occured: {ex.Message}");
                }
            }
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
                    Console.WriteLine("Do you wish to stop? (Y/any other key)");
                    string answer = Console.ReadLine();
                    if (answer == "Y")
                        break;
                    Console.WriteLine("Give board size");
                    string s = UI.GeneratorNxN(int.Parse(Console.ReadLine()));
                    ISudokuBoard board = null;


                    double counter = s.Count(c => c == Constants.emptyCell);
                    Console.WriteLine((double)(counter / s.Length));
                    if (counter <= 0.95 * s.Length)
                    {
                        board = new FastSudokuBoard(s);
                    }
                    else
                    {
                        board = new SudokuBoard(s);
                    }
                    double ratio = (double)s.Count(c => c == Constants.emptyCell) / s.Length;
                    Console.WriteLine($"Difficulty Ratio: {ratio:P2}");


                    Console.WriteLine("--- Initial Board ---");
                    board.PrintBoard();

                    long start = Stopwatch.GetTimestamp();
                    bool solved = Solver.Solve(board);
                    long end = Stopwatch.GetTimestamp();

                    if (solved)
                    {
                        Console.WriteLine("\n--- SOLVED ---");
                        board.PrintBoard();
                    }
                    else //shouldn't happen. sanity check
                    {
                        Console.WriteLine("\n!!! FAILED TO SOLVE !!!");
                    }

                    TimeSpan elapsed = TimeSpan.FromSeconds((end - start) / (double)Stopwatch.Frequency);
                    Console.WriteLine($"Time: {elapsed:mm\\:ss\\.ffffff}");

                    Console.WriteLine("Would you like to clear the console? (Y/any other key)");
                    string clear = Console.ReadLine();
                    if (clear == "Y" || clear == "y")
                    {
                        Console.Clear();
                        Console.Write("\x1b[3J");
                    }

                }
                catch (SudokuException ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
        }
        public static string GeneratorNxN(int boardLength)
        {
            if (boardLength > 25 || boardLength < 4)
                throw new InvalidPuzzleException("Board length must be between 4 and 25");
            Constants.boardLen = boardLength;
            Constants.SetSymbol();

            int totalCells = boardLength * boardLength;

            string emptyBoard = new string(Constants.emptyCell, totalCells);
            SudokuBoard board = new SudokuBoard(emptyBoard);
            bool solved = Solver.Solve(board);
            if (!solved)
                throw new UnsolvableSudokuException("Generated board is unsolvable");
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
                board.board[coord.Item1* boardLength + coord.Item2].Value = Constants.emptyCell;
            }

            return board.ToString();
        }
    }
}
