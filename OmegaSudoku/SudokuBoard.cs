using System;
using System.Collections.Generic;
using System.Linq;

namespace OmegaSudoku
{
    class SudokuBoard
    {
        public SquareCell[,] board;

        private int boxLen;

        private int[] rowUsed;
        private int[] colUsed;
        private int[] boxUsed;

        private HashSet<SquareCell> emptyCells;

        public bool HasEmptyCells => emptyCells.Count > 0;

        public HashSet<SquareCell> EmptyCells => emptyCells;
        public int[] RowUsed => rowUsed;
        public int[] ColUsed => colUsed;
        public int[] BoxUsed => boxUsed;

        public int numOfFilledCells { get; set; }

        public SudokuBoard(string boardString)
        {
            Constants.boardLen = (int)Math.Sqrt(boardString.Length);
            Constants.SetSymbol();
            boxLen = (int)Math.Sqrt(Constants.boardLen);

            rowUsed = new int[Constants.boardLen];
            colUsed = new int[Constants.boardLen];
            boxUsed = new int[Constants.boardLen];

            emptyCells = new HashSet<SquareCell>();
            board = new SquareCell[Constants.boardLen, Constants.boardLen];

            for (int row = 0; row < Constants.boardLen; row++)
            {
                for (int col = 0; col < Constants.boardLen; col++)
                {
                    char value = boardString[row * Constants.boardLen + col];
                    board[row, col] = new SquareCell(row, col, value);

                    if (value == Constants.emptyCell)
                    {
                        emptyCells.Add(board[row, col]);
                        board[row, col].PossibleMask = CreateFullMask(); // all options initially
                    }
                    else
                    {
                        int bit = SudokuHelper.BitFromChar(value);
                        rowUsed[row] |= bit;
                        colUsed[col] |= bit;
                        boxUsed[BoxIndex(row, col)] |= bit;

                        board[row, col].PossibleMask = bit;
                    }
                }
            }

            InitializePossibleValues();

            if (!IsValidBoard())
                throw new Exception("The provided board is not valid.");
        }

        private int CreateFullMask() => (1 << Constants.boardLen) - 1;



        public int BoxIndex(int row, int col) => (row / boxLen) * boxLen + (col / boxLen);

        private bool IsValidPlace(int row, int col, char value)
        {
            int bit = SudokuHelper.BitFromChar(value);
            return (rowUsed[row] & bit) == 0
                && (colUsed[col] & bit) == 0
                && (boxUsed[BoxIndex(row, col)] & bit) == 0;
        }

        public void InitializePossibleValues()
        {
            for (int row = 0; row < Constants.boardLen; row++)
            {
                for (int col = 0; col < Constants.boardLen; col++)
                {
                    if (board[row, col].Value == Constants.emptyCell)
                    {
                        int mask = 0;
                        foreach (char d in Constants.symbols)
                        {
                            if (IsValidPlace(row, col, d))
                                mask |= SudokuHelper.BitFromChar(d);
                        }
                        board[row, col].PossibleMask = mask;
                    }
                }
            }
        }

        public SquareCell GetBestCell()
        {
            if (emptyCells.Count == 0)
                return null;

            SquareCell bestCell = null;
            int minOptions = int.MaxValue;
            int maxDegree = -1;

            foreach (var cell in emptyCells)
            {
                int options = cell.PossibleCount;

                // Naked single: immediately return
                if (options == 1)
                    return cell;

                if (options < minOptions)
                {
                    minOptions = options;
                    bestCell = cell;
                    maxDegree = CountNumEmpty(cell.Row, cell.Col); // degree heuristic
                }
                else if (options == minOptions)
                {
                    int degree = CountNumEmpty(cell.Row, cell.Col);
                    if (degree > maxDegree)
                    {
                        maxDegree = degree;
                        bestCell = cell;
                    }
                }
            }

            return bestCell;
        }

        // Degree heuristic: counts empty neighbors
        public int CountNumEmpty(int row, int col)
        {
            SquareCell cell = board[row, col];
            return cell.GetNeighbors().Count(n => board[n.Item1, n.Item2].Value == Constants.emptyCell);
        }

        public bool PlaceNumber(int row, int col, char value, Stack<Move> moves)
        {
            if (!IsValidPlace(row, col, value))
                return false;

            SquareCell cell = board[row, col];
            int bit = SudokuHelper.BitFromChar(value);

            // Save current mask of this cell
            moves.Push(new Move(cell, cell.PossibleMask));

            // Set value
            cell.Value = value;
            emptyCells.Remove(cell);
            cell.PossibleMask = 0;

            // Update board usage
            rowUsed[row] = SudokuHelper.AddBit(rowUsed[row],bit);
            colUsed[col] = SudokuHelper.AddBit(colUsed[col],bit);
            boxUsed[BoxIndex(row, col)] = SudokuHelper.AddBit(boxUsed[BoxIndex(row,col)],bit);

            // Update neighbors
            foreach (var loc in cell.GetNeighbors())
            {
                SquareCell neighbor = board[loc.Item1, loc.Item2];
                if (neighbor.Value == Constants.emptyCell && neighbor.Contains(value))
                {
                    moves.Push(new Move(neighbor, neighbor.PossibleMask)); // save mask
                    neighbor.RemovePossibleValue(value);
                }
            }
            return true;
        }

        public void RemoveNumbers(Stack<Move> moves)
        {
            while (moves.Count > 0)
            {
                Move move = moves.Pop();
                SquareCell cell = move.Cell;

                if (cell.Value != Constants.emptyCell)
                {
                    int bit = SudokuHelper.BitFromChar(cell.Value);
                    rowUsed[cell.Row] = SudokuHelper.ClearBit(rowUsed[cell.Row],bit);
                    colUsed[cell.Col] = SudokuHelper.ClearBit(colUsed[cell.Col], bit);
                    boxUsed[BoxIndex(cell.Row, cell.Col)] = SudokuHelper.ClearBit(boxUsed[BoxIndex(cell.Row, cell.Col)], bit);

                    cell.Value = Constants.emptyCell;
                    emptyCells.Add(cell);
                }

                cell.PossibleMask = move.PreviousMask;
            }
        }

        public void PrintBoard()
        {
            Console.WriteLine();
            for (int row = 0; row < Constants.boardLen; row++)
            {
                if (row % boxLen == 0 && row != 0)
                    Console.WriteLine(new string('-', (boxLen * Constants.boardLen) - 3));

                for (int col = 0; col < Constants.boardLen; col++)
                {
                    if (col % boxLen == 0 && col != 0)
                        Console.Write(" | ");

                    Console.Write(board[row, col].Value + " ");
                }
                Console.WriteLine();
            }
        }

        public bool IsHiddenSingle(int row, int col, char value)
        {
            bool hiddenInRow = true;
            bool hiddenInCol = true;
            bool hiddenInBox = true;

            for (int c = 0; c < Constants.boardLen && hiddenInRow; c++)
            {
                if (c != col && board[row, c].Value == Constants.emptyCell && board[row, c].Contains(value))
                    hiddenInRow = false;
            }

            for (int r = 0; r < Constants.boardLen && hiddenInCol; r++)
            {
                if (r != row && board[r, col].Value == Constants.emptyCell && board[r, col].Contains(value))
                    hiddenInCol = false;
            }

            int boxRowStart = (row / boxLen) * boxLen;
            int boxColStart = (col / boxLen) * boxLen;
            for (int r = boxRowStart; r < boxRowStart + boxLen && hiddenInBox; r++)
            {
                for (int c = boxColStart; c < boxColStart + boxLen && hiddenInBox; c++)
                {
                    if ((r != row || c != col) && board[r, c].Value == Constants.emptyCell && board[r, c].Contains(value))
                        hiddenInBox = false;
                }
            }

            return hiddenInBox || hiddenInCol || hiddenInRow;
        }

        public bool IsNakedSingle(int row, int col)
        {
            return board[row, col].Value == Constants.emptyCell && SudokuHelper.CountBits(board[row, col].PossibleMask) == 1;
        }

        public Dictionary<int,List<SquareCell>> GetNakedPairs()
        {
            Dictionary<int, List<SquareCell>> pairs = new Dictionary<int,List<SquareCell>>();
            foreach (SquareCell cell in emptyCells)
            {
                if (SudokuHelper.CountBits(cell.PossibleMask) == 2)
                {
                    if (!pairs.ContainsKey(cell.PossibleMask))
                        pairs[cell.PossibleMask] = new List<SquareCell>();
                    pairs[cell.PossibleMask].Add(cell);
                }

            }
            var nakedPairs = pairs.Where(kvp => kvp.Value.Count == 2).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return nakedPairs;
        }


        public override string ToString()
        {
            string str = "";
            for (int row = 0; row < Constants.boardLen; row++)
                for (int col = 0; col < Constants.boardLen; col++)
                    str += board[row, col].Value;
            return str;
        }

        public bool IsValidBoard()
        {
            for (int row = 0; row < Constants.boardLen; row++)
                for (int col = 0; col < Constants.boardLen; col++)
                    if (board[row, col].Value == Constants.emptyCell && board[row, col].Failed())
                        return false;
            return true;
        }
    }
}
