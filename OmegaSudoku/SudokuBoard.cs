using System;
using System.Collections.Generic;
using System.Linq;

namespace OmegaSudoku
{
    class SudokuBoard : ISudokuBoard
    {
        public SquareCell[,] board;

        private int boxLen;

        public int[,] RowCounts;
        public int[,] ColCounts;
        public int[,] BoxCounts;

        private int[] rowUsed;
        private int[] colUsed;
        private int[] boxUsed;
        private int[,] counterEmptyNeighbors;

        private HashSet<SquareCell> emptyCells;

        private int fullmask;

        public bool HasEmptyCells => emptyCells.Count > 0;

        public HashSet<SquareCell> EmptyCells => emptyCells;
        public int[] RowUsed => rowUsed;
        public int[] ColUsed => colUsed;
        public int[] BoxUsed => boxUsed;

        public int numOfFilledCells { get; set; }
        SquareCell[,] ISudokuBoard.board
        {
            get => board;
            set => board = value;
        }

        int ISudokuBoard.boxLen
        {
            get => boxLen;
            set => boxLen = value;
        }

        int[,] ISudokuBoard.RowCounts
        {
            get => RowCounts;
            set => RowCounts = value;
        }

        int[,] ISudokuBoard.ColCounts
        {
            get => ColCounts;
            set => ColCounts = value;
        }

        int[,] ISudokuBoard.BoxCounts
        {
            get => BoxCounts;
            set => BoxCounts = value;
        }

        int[] ISudokuBoard.rowUsed
        {
            get => rowUsed;
            set => rowUsed = value;
        }

        int[] ISudokuBoard.colUsed
        {
            get => colUsed;
            set => colUsed = value;
        }

        int[] ISudokuBoard.boxUsed
        {
            get => boxUsed;
            set => boxUsed = value;
        }

        HashSet<SquareCell> ISudokuBoard.EmptyCells
        {
            get => emptyCells;
            set => emptyCells = value;
        }

        int ISudokuBoard.fullmask
        {
            get => fullmask;
            set => fullmask = value;
        }


        public SudokuBoard(string boardString)
        {
            Constants.boardLen = (int)Math.Sqrt(boardString.Length);
            Constants.SetSymbol();
            boxLen = (int)Math.Sqrt(Constants.boardLen);

            fullmask = (1 << Constants.boardLen) - 1;

            // Initialize Count Arrays
            RowCounts = new int[Constants.boardLen, Constants.boardLen];
            ColCounts = new int[Constants.boardLen, Constants.boardLen];
            BoxCounts = new int[Constants.boardLen, Constants.boardLen];

            rowUsed = new int[Constants.boardLen];
            colUsed = new int[Constants.boardLen];
            boxUsed = new int[Constants.boardLen];

            counterEmptyNeighbors = new int[Constants.boardLen, Constants.boardLen];

            emptyCells = new HashSet<SquareCell>();
            board = new SquareCell[Constants.boardLen, Constants.boardLen];

            InitializeBoard(boardString);
            InitializePossibleValues();

            if (!IsValidBoard())
                throw new Exception("The provided board is not valid.");
        }

        public int BoxIndex(int row, int col) => (row / boxLen) * boxLen + (col / boxLen);

        private bool IsValidPlace(int row, int col, char value)
        {
            int bit = SudokuHelper.BitFromChar(value);
            return (rowUsed[row] & bit) == 0
                && (colUsed[col] & bit) == 0
                && (boxUsed[BoxIndex(row, col)] & bit) == 0;
        }

        public void InitializeNeighbors(SquareCell cell)
        {
            var neighbors = new HashSet<SquareCell>();

            for (int row = 0; row < Constants.boardLen; row++)
            {
                if (row != cell.Row)
                    neighbors.Add(board[row, cell.Col]);
            }
            for (int col = 0; col < Constants.boardLen; col++)
            {
                if (col != cell.Col)
                    neighbors.Add(board[cell.Row, col]);
            }
            int boxRowStart = (cell.Row / Constants.boxLen) * Constants.boxLen;
            int boxColStart = (cell.Col / Constants.boxLen) * Constants.boxLen;
            for (int r = boxRowStart; r < boxRowStart + Constants.boxLen; r++)
            {
                for (int c = boxColStart; c < boxColStart + Constants.boxLen; c++)
                {
                    if (r != cell.Row || c != cell.Col)
                        neighbors.Add(board[r, c]);
                }
            }
            cell.Neighbors = neighbors.ToArray();
        }

        private void InitializeBoard(string boardString)
        {
            for (int row = 0; row < Constants.boardLen; row++)
            {
                for (int col = 0; col < Constants.boardLen; col++)
                {
                    char value = boardString[row * Constants.boardLen + col];
                    board[row, col] = new SquareCell(row, col, value);
                    if (value == Constants.emptyCell)
                    {
                        emptyCells.Add(board[row, col]);
                        board[row, col].PossibleMask = fullmask;
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
            // Initialize neighbors for each cell
            for (int row = 0; row < Constants.boardLen; row++)
            {
                for (int col = 0; col < Constants.boardLen; col++)
                {
                    InitializeNeighbors(board[row, col]);
                }
            }
        }

        public void InitializePossibleValues()
        {
            foreach (var cell in emptyCells)
            {
                cell.PossibleMask = ~(rowUsed[cell.Row] | colUsed[cell.Col] | boxUsed[BoxIndex(cell.Row, cell.Col)]) & fullmask;
                counterEmptyNeighbors[cell.Row, cell.Col] = CountEmptyNeighbors(cell.Row, cell.Col);

                UpdateCounts(cell.Row, cell.Col, cell.PossibleMask, 1);
            }
        }

        private void UpdateCounts(int row, int col, int mask, int delta)
        {
            int b = BoxIndex(row, col);
            while (mask != 0)
            {
                int bit = mask & -mask;
                mask ^= bit;           
                int d = SudokuHelper.IndexFromBit(bit);

                RowCounts[row, d] += delta;
                ColCounts[col, d] += delta;
                BoxCounts[b, d] += delta;
            }
        }

        private void DecrementSingleCount(int row, int col, char value)
        {
            int index = Constants.CharToIndex[value];
            RowCounts[row, index]--;
            ColCounts[col, index]--;
            BoxCounts[BoxIndex(row, col), index]--;
        }

        public bool IsHiddenSingle(int row, int col, char value)
        {
            int d = Constants.CharToIndex[value];
            return RowCounts[row, d] == 1 ||
                   ColCounts[col, d] == 1 ||
                   BoxCounts[BoxIndex(row, col), d] == 1;
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
                    maxDegree = counterEmptyNeighbors[cell.Row, cell.Col];
                }
                else if (options == minOptions)
                {
                    int degree = counterEmptyNeighbors[cell.Row, cell.Col];
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
        public int CountEmptyNeighbors(int row, int col)
        {
            int count = 0;
            foreach (var neighbor in board[row, col].Neighbors)
            {
                if (neighbor.Value == Constants.emptyCell)
                    count++;
            }
            return count;
        }

        public bool PlaceNumber(int row, int col, char value, Stack<Move> moves)
        {
            if (!IsValidPlace(row, col, value))
                return false;

            SquareCell cell = board[row, col];
            int bit = SudokuHelper.BitFromChar(value);

            moves.Push(new Move(cell, cell.PossibleMask));

            UpdateCounts(row, col, cell.PossibleMask, -1);

            // Set value
            cell.Value = value;
            emptyCells.Remove(cell);
            cell.PossibleMask = 0;

            // Update board usage
            rowUsed[row] = SudokuHelper.AddBit(rowUsed[row], bit);
            colUsed[col] = SudokuHelper.AddBit(colUsed[col], bit);
            boxUsed[BoxIndex(row, col)] = SudokuHelper.AddBit(boxUsed[BoxIndex(row, col)], bit);

            bool flag = true;
            // Update neighbors
            foreach (SquareCell neighbor in cell.Neighbors)
            {
                if (neighbor.Value == Constants.emptyCell)
                    counterEmptyNeighbors[neighbor.Row, neighbor.Col]--;

                if (neighbor.Value == Constants.emptyCell && neighbor.Contains(value))
                {
                    moves.Push(new Move(neighbor, neighbor.PossibleMask));

                    DecrementSingleCount(neighbor.Row, neighbor.Col, value);

                    neighbor.RemovePossibleValue(value);
                    if (neighbor.PossibleMask == 0)
                        flag = false;
                }
            }
            return flag;
        }

        public void RemoveNumbers(Stack<Move> moves, int checkpoint)
        {
            while (moves.Count > checkpoint)
            {
                Move move = moves.Pop();
                SquareCell cell = move.Cell;
                bool wasFilled = cell.Value != Constants.emptyCell;

                int oldMask = cell.PossibleMask;

                if (wasFilled)
                {
                    int bit = SudokuHelper.BitFromChar(cell.Value);
                    rowUsed[cell.Row] = SudokuHelper.ClearBit(rowUsed[cell.Row], bit);
                    colUsed[cell.Col] = SudokuHelper.ClearBit(colUsed[cell.Col], bit);
                    boxUsed[BoxIndex(cell.Row, cell.Col)] = SudokuHelper.ClearBit(boxUsed[BoxIndex(cell.Row, cell.Col)], bit);

                    cell.Value = Constants.emptyCell;
                    emptyCells.Add(cell);
                    foreach (var neighbor in cell.Neighbors)
                    {
                        counterEmptyNeighbors[neighbor.Row, neighbor.Col]++;
                    }
                }

                cell.PossibleMask = move.PreviousMask;

                if (wasFilled)
                {
                    // If cell was filled, we put back all its options
                    UpdateCounts(cell.Row, cell.Col, cell.PossibleMask, 1);
                }
                else
                {
                    // If neighbor update, put back the specific bits that were stripped
                    int restoredBits = move.PreviousMask & ~oldMask;
                    if (restoredBits != 0)
                    {
                        UpdateCounts(cell.Row, cell.Col, restoredBits, 1);
                    }
                }
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

        public bool IsNakedSingle(int row, int col)
        {
            return board[row, col].Value == Constants.emptyCell && board[row, col].PossibleMask != 0 && (board[row, col].PossibleMask & (board[row, col].PossibleMask - 1)) == 0;
        }

        public List<char> GetLCVValues(SquareCell cell)
        {
            List<Tuple<char, int>> valueConstraints = new List<Tuple<char, int>>();
            int mask = cell.PossibleMask;
            while (mask != 0)
            {
                int bit = SudokuHelper.LowestBit(mask);
                mask = SudokuHelper.ClearLowestBit(mask);
                char value = SudokuHelper.MaskToChar(bit);
                int constraintCount = 0;
                foreach (SquareCell neighbor in cell.Neighbors)
                {
                    if (neighbor.Value == Constants.emptyCell && neighbor.Contains(value))
                    {
                        constraintCount++;
                    }
                }
                valueConstraints.Add(new Tuple<char, int>(value, constraintCount));
            }
            // Sort by constraint count (ascending)
            valueConstraints.Sort((a, b) => a.Item2.CompareTo(b.Item2));
            return valueConstraints.Select(vc => vc.Item1).ToList();
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

        void ISudokuBoard.InitializeBoard(string boardString)
        {
            InitializeBoard(boardString);
        }

        void ISudokuBoard.UpdateCounts(int r, int c, int mask, int delta)
        {
            UpdateCounts(r, c, mask, delta);
        }

        public void DecrementSingleCount(int r, int c, int d)
        {
            RowCounts[r, d]--;
            ColCounts[c, d]--;
            BoxCounts[BoxIndex(r, c), d]--;
        }

    }
}