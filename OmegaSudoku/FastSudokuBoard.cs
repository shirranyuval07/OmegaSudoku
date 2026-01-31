using System;
using System.Collections.Generic;
using System.Linq;

namespace OmegaSudoku
{
    class FastSudokuBoard : ISudokuBoard
    {
        public SquareCell[,] board;
        private int boxLen;

        // Hidden Single Counts
        public int[,] RowCounts;
        public int[,] ColCounts;
        public int[,] BoxCounts;

        // Global Constraints
        private int[] rowUsed;
        private int[] colUsed;
        private int[] boxUsed;

        public HashSet<SquareCell> EmptyCells;

        private int fullmask;
        public int EmptyCount => EmptyCells.Count;


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
            get => EmptyCells;
            set => EmptyCells = value;
        }

        int ISudokuBoard.fullmask
        {
            get => fullmask;
            set => fullmask = value;
        }

        public bool HasEmptyCells => EmptyCells.Count > 0;


        public FastSudokuBoard(string boardString)
        {
            if (boardString == null || boardString == "")
                throw new InvalidPuzzleException("The provided board string is null or empty.");
            double sqrtLen = Math.Sqrt(boardString.Length);
            if (sqrtLen % 1 != 0)
                throw new InvalidPuzzleException("The provided board string length is not a perfect square.");

            Constants.boardLen = (int)sqrtLen;
            Constants.SetSymbol();
            double sqrtBoxLen = Math.Sqrt(Constants.boardLen);
            if (sqrtBoxLen % 1 != 0)
                throw new InvalidPuzzleException("The board length does not have an integer square root, invalid for Sudoku.");

            boxLen = (int)sqrtBoxLen;
            fullmask = (1 << Constants.boardLen) - 1;

            int len = Constants.boardLen;
            rowUsed = new int[len];
            colUsed = new int[len];
            boxUsed = new int[len];

            RowCounts = new int[len, len];
            ColCounts = new int[len, len];
            BoxCounts = new int[len, len];

            EmptyCells = new HashSet<SquareCell>();
            board = new SquareCell[len, len];

            InitializeBoard(boardString);
            int clues = Constants.boardLen * Constants.boardLen - EmptyCells.Count;
            if (clues > 0 && clues < 4)// 1-3 clues takes too much time (about 1.5 minutes for 25x25)
                throw new InvalidPuzzleException("Board has too few clues (1-3). " +
                                                  "It must be empty (0) or have a valid puzzle start.");
            if (!IsValidBoard())
                throw new InvalidPuzzleException("The provided board string represents an invalid Sudoku board.");
        }

        public int BoxIndex(int row, int col) => (row / boxLen) * boxLen + (col / boxLen);

        private void InitializeBoard(string boardString)
        {
            for (int i = 0; i < Constants.boardLen * Constants.boardLen; i++)
            {
                int r = i / Constants.boardLen;
                int c = i % Constants.boardLen;
                char val = boardString[i];
                board[r, c] = new SquareCell(r, c, val);

                SudokuHelper.ValidateChar(val, Constants.boardLen);

                if (val == Constants.emptyCell)
                {
                    EmptyCells.Add(board[r, c]);
                }
                else
                {
                    int bit = SudokuHelper.BitFromChar(val);
                    rowUsed[r] |= bit;
                    colUsed[c] |= bit;
                    boxUsed[BoxIndex(r, c)] |= bit;
                    board[r, c].PossibleMask = 0;
                }
            }

            for (int r = 0; r < Constants.boardLen; r++)
                for (int c = 0; c < Constants.boardLen; c++)
                    InitializeNeighbors(board[r, c]);

            foreach (var cell in EmptyCells)
            {
                int used = rowUsed[cell.Row] | colUsed[cell.Col] | boxUsed[BoxIndex(cell.Row, cell.Col)];
                cell.PossibleMask = fullmask & ~used;
                UpdateCounts(cell.Row, cell.Col, cell.PossibleMask, 1);
            }
        }

        public SquareCell GetBestCell()
        {
            if (EmptyCells.Count == 0) return null;

            SquareCell bestCell = null;
            int minOptions = int.MaxValue;

            foreach (var cell in EmptyCells)
            {
                // Count is auto-updated by the setter in SquareCell
                int opts = cell.PossibleCount;
                if (opts == 0) return null; // Impossible
                if (opts == 1) return cell; // Instant Naked Single

                if (opts < minOptions)
                {
                    minOptions = opts;
                    bestCell = cell;
                }
            }
            return bestCell;
        }

        public bool PlaceNumber(int row, int col, char value, Stack<Move> moves)
        {
            SquareCell cell = board[row, col];
            int bit = SudokuHelper.BitFromChar(value);

            moves.Push(new Move(cell, cell.PossibleMask));

            // Maintenance
            UpdateCounts(row, col, cell.PossibleMask, -1);

            cell.Value = value;
            cell.PossibleMask = 0;
            EmptyCells.Remove(cell);

            rowUsed[row] |= bit;
            colUsed[col] |= bit;
            boxUsed[BoxIndex(row, col)] |= bit;

            // Propagate
            foreach (SquareCell neighbor in cell.Neighbors)
            {
                if (neighbor.Value == Constants.emptyCell && (neighbor.PossibleMask & bit) != 0)
                {
                    moves.Push(new Move(neighbor, neighbor.PossibleMask));

                    DecrementSingleCount(neighbor.Row, neighbor.Col, Constants.CharToIndex[value]);

                    neighbor.PossibleMask &= ~bit;

                    if (neighbor.PossibleMask == 0) return false;
                }
            }
            return true;
        }

        public void RemoveNumbers(Stack<Move> moves, int checkpoint)
        {
            while (moves.Count > checkpoint)
            {
                Move move = moves.Pop();
                SquareCell cell = move.Cell;
                bool wasFilled = cell.Value != Constants.emptyCell;

                int oldMask = cell.PossibleMask;
                cell.PossibleMask = move.PreviousMask; 

                if (wasFilled)
                {
                    int bit = SudokuHelper.BitFromChar(cell.Value);
                    rowUsed[cell.Row] &= ~bit;
                    colUsed[cell.Col] &= ~bit;
                    boxUsed[BoxIndex(cell.Row, cell.Col)] &= ~bit;

                    cell.Value = Constants.emptyCell;
                    EmptyCells.Add(cell);

                    UpdateCounts(cell.Row, cell.Col, cell.PossibleMask, 1);
                }
                else
                {
                    int restoredBits = move.PreviousMask & ~oldMask;
                    if (restoredBits != 0)
                        UpdateCounts(cell.Row, cell.Col, restoredBits, 1);
                }
            }
        }

        private void UpdateCounts(int r, int c, int mask, int delta)
        {
            int b = BoxIndex(r, c);
            while (mask != 0)
            {
                int bit = mask & -mask;
                mask ^= bit;
                int d = SudokuHelper.IndexFromBit(bit);
                RowCounts[r, d] += delta;
                ColCounts[c, d] += delta;
                BoxCounts[b, d] += delta;
            }
        }

        private void DecrementSingleCount(int r, int c, int d)
        {
            RowCounts[r, d]--;
            ColCounts[c, d]--;
            BoxCounts[BoxIndex(r, c), d]--;
        }

        public bool IsHiddenSingle(int r, int c, char v)
        {
            int d = Constants.CharToIndex[v];
            return RowCounts[r, d] == 1 || ColCounts[c, d] == 1 || BoxCounts[BoxIndex(r, c), d] == 1;
        }

        public bool IsNakedSingle(int r, int c) => board[r, c].PossibleCount == 1;

        private void InitializeNeighbors(SquareCell cell)
        {
            var l = new List<SquareCell>();
            int r = cell.Row, c = cell.Col;
            for (int k = 0; k < Constants.boardLen; k++)
            {
                if (k != c) l.Add(board[r, k]);
                if (k != r) l.Add(board[k, c]);
            }
            int br = (r / boxLen) * boxLen, bc = (c / boxLen) * boxLen;
            for (int i = 0; i < boxLen; i++) for (int j = 0; j < boxLen; j++)
                {
                    int nr = br + i, nc = bc + j;
                    if (nr != r && nc != c) l.Add(board[nr, nc]);
                }
            cell.Neighbors = l.ToArray();
        }

        public bool IsValidBoard()
        {
            int[] checkRowUsed = new int[Constants.boardLen];
            int[] checkColUsed = new int[Constants.boardLen];
            int[] checkBoxUsed = new int[Constants.boardLen];

            for (int row = 0; row < Constants.boardLen; row++)
            {
                for (int col = 0; col < Constants.boardLen; col++)
                {
                    SquareCell cell = board[row, col];

                    if (cell.Value == Constants.emptyCell)
                    {
                        if (cell.Failed())
                            return false;
                    }
                    else
                    {
                        int bit = SudokuHelper.BitFromChar(cell.Value);
                        int boxIdx = BoxIndex(row, col);
                        bool isDuplicate = (checkRowUsed[row] & bit) != 0 ||
                                           (checkColUsed[col] & bit) != 0 ||
                                           (checkBoxUsed[boxIdx] & bit) != 0;
                        if (isDuplicate)
                            return false;
                        checkRowUsed[row] |= bit;
                        checkColUsed[col] |= bit;
                        checkBoxUsed[boxIdx] |= bit;
                    }
                }
            }

            return true;
        }

        public void PrintBoard()
        {
            Console.WriteLine();
            for (int r = 0; r < Constants.boardLen; r++)
            {
                if (r % boxLen == 0 && r != 0) Console.WriteLine(new string('-', (boxLen * Constants.boardLen * 2)));
                for (int c = 0; c < Constants.boardLen; c++)
                {
                    if (c % boxLen == 0 && c != 0) Console.Write("| ");
                    Console.Write(board[r, c].Value + " ");
                }
                Console.WriteLine();
            }
        }

        void ISudokuBoard.InitializeBoard(string boardString)
        {
            InitializeBoard(boardString);
        }

        void ISudokuBoard.UpdateCounts(int r, int c, int mask, int delta)
        {
            UpdateCounts(r, c, mask, delta);
        }

        void ISudokuBoard.DecrementSingleCount(int r, int c, int d)
        {
            DecrementSingleCount(r, c, d);
        }

        void ISudokuBoard.InitializeNeighbors(SquareCell cell)
        {
            InitializeNeighbors(cell);
        }
    }
}