using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;

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

        private int[,] allCells;
        public bool HasEmptyCells => emptyCells.Count > 0;

        public HashSet<SquareCell> EmptyCells
        {
            get { return emptyCells; }
        }

        public int[,] AllCells
        {
            get { return allCells; }
        }

        public int[] RowUsed
        {
            get { return rowUsed; }
        }
        public int[] ColUsed
        {
            get { return colUsed; }
        }
        public int[] BoxUsed
        {
            get { return boxUsed; }
        }
        public int numOfFilledCells { get; set; }

        public SudokuBoard(string boardString)
        {
            Constants.boardLen = (int)Math.Sqrt(boardString.Length);
            Constants.SetSymbol();
            boxLen = (int)Math.Sqrt(Constants.boardLen);
            rowUsed = new int[Constants.boardLen];
            colUsed = new int[Constants.boardLen];
            boxUsed = new int[Constants.boardLen];

            this.emptyCells = new HashSet<SquareCell>();
            this.allCells = new int[Constants.boardLen, Constants.boardLen];


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
                        allCells[row, col] = CreateMask();
                    }
                    if (value != Constants.emptyCell)
                    {
                        int bit = 1 << (value - '1');

                        rowUsed[row] |= bit;
                        colUsed[col] |= bit;
                        boxUsed[BoxIndex(row, col)] |= bit;

                        allCells[row, col] = bit;
                    }
                }
            }
            InitializePossibleValues();

            if (!IsValidBoard())
                throw new Exception("The provided board is not valid.");
        }


        public HashSet<SquareCell> GetEmptyCells() { return this.emptyCells; }
        public int BoxIndex(int row, int col)
        {
            return (row / boxLen) * boxLen + (col / boxLen);
        }

        private bool IsValidPlace(int row, int col, char value)
        {
            int bit = 1 << (value - '1');

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
                            {
                                mask |= SudokuHelper.BitFromChar(d);
                            }
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
            SquareCell sc = this.emptyCells.First();
            foreach (SquareCell cell in emptyCells)
            {
                if (cell.PossibleCount < sc.PossibleCount)
                    sc = cell;
            }

            return sc;
        }
        /*
        public int CountNumEmpty(int row, int col)
        {
            int count = 0;
            count += SudokuHelper.CountBits(GetPossibleValues(row, col));
            for (int r = 0; r < Constants.boardLen; r++)
            {
                if (r != row)
                    count += SudokuHelper.CountBits(GetPossibleValues(r, col));

            }
            for (int c = 0; c < Constants.boardLen; c++)
            {
                if (c != col)
                    count += SudokuHelper.CountBits(GetPossibleValues(row, c));
            }
            int boxRowStart = (row / boxLen) * boxLen;
            int boxColStart = (col / boxLen) * boxLen;
            for (int r = boxRowStart; r < boxRowStart + boxLen; r++)
            {
                for (int c = boxColStart; c < boxColStart + boxLen; c++)
                {
                    if (r != row && c != col)
                        count += SudokuHelper.CountBits(GetPossibleValues(r, c));
                }
            }
            return count;
        }*/

        //returns bit representation of possible values for cell.
        public int GetPossibleValues(int row, int col)
        {
            return allCells[row, col];
        }
        public int CreateMask()
        {
            int mask = 0;
            for (int i = 0; i < Constants.boardLen; i++)
            {
                mask |= (1 << i);
            }
            return mask;
        }

        public bool IsValidBoard()
        {
            for (int row = 0; row < Constants.boardLen; row++)
            {
                for (int col = 0; col < Constants.boardLen; col++)
                {
                    if (board[row, col].Value == Constants.emptyCell &&
                        board[row, col].Failed())
                        return false;
                }
            }
            return true;
        }



        public bool PlaceNumber(int row, int col, char value, Stack<Move> moves)
        {
            if (!IsValidPlace(row, col, value))
                return false;

            SquareCell cell = board[row, col];

            int bit = 1 << (value - '1');

            // Save current mask of this cell
            moves.Push(new Move(cell, cell.PossibleMask));

            // Set value
            cell.Value = value;
            emptyCells.Remove(cell);
            cell.PossibleMask = 0;

            // Update board usage
            rowUsed[row] |= bit;
            colUsed[col] |= bit;
            boxUsed[BoxIndex(row, col)] |= bit;

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
                    // Undo number placement
                    int bit = SudokuHelper.BitFromChar(cell.Value);
                    rowUsed[cell.Row] &= ~bit;
                    colUsed[cell.Col] &= ~bit;
                    boxUsed[BoxIndex(cell.Row, cell.Col)] &= ~bit;

                    cell.Value = Constants.emptyCell;
                    emptyCells.Add(cell);
                }

                // Restore previous possible mask
                cell.PossibleMask = move.PreviousMask;
            }
        }




        public void PrintBoard()
        {
            Console.WriteLine();
            for (int row = 0; row < Constants.boardLen; row++)
            {
                if (row % boxLen == 0 && row != 0)
                    Console.WriteLine(new string('-', (boxLen * Constants.boardLen) - 3)); // -3 bc of the Console.Write(" | "); (size is 3)

                for (int col = 0; col < Constants.boardLen; col++)
                {
                    if (col % boxLen == 0 && col != 0)
                        Console.Write(" | ");

                    Console.Write(board[row, col].Value + " ");
                }
                Console.WriteLine();
            }
        }


        /*“Check everywhere this value could go. 
         * If it fits anywhere else → not hidden. 
         * If it fits nowhere else → hidden single.”*/
        public bool IsHiddenSingle(int row, int col, char value)
        {
            bool hiddenInRow = true;
            bool hiddenInCol = true;
            bool hiddenInBox = true;
            // Check if the value is a hidden single in its row
            for (int c = 0; c < Constants.boardLen && hiddenInRow; c++)
            {
                if (c != col && this.board[row, c].Value == Constants.emptyCell && this.board[row, c].Contains(value))
                    hiddenInRow = false;
            }
            // Check if the value is a hidden single in its column
            for (int r = 0; r < Constants.boardLen && hiddenInCol; r++)
            {
                if (r != row && this.board[r, col].Value == Constants.emptyCell && this.board[r, col].Contains(value))
                    hiddenInCol = false;
            }
            // Check if the value is a hidden single in its box
            int boxRowStart = (row / boxLen) * boxLen;
            int boxColStart = (col / boxLen) * boxLen;
            for (int r = boxRowStart; r < boxRowStart + boxLen && hiddenInBox; r++)
            {
                for (int c = boxColStart; c < boxColStart + boxLen && hiddenInBox; c++)
                {
                    if ((r != row || c != col) && this.board[r, c].Value == Constants.emptyCell && this.board[r, c].Contains(value))
                        hiddenInBox = false;
                }
            }
            return hiddenInBox || hiddenInCol || hiddenInRow;
        }

        public bool IsNakedSingle(int row, int col)
        {
            return this.board[row, col].Value == Constants.emptyCell && SudokuHelper.CountBits(this.board[row, col].PossibleMask) == 1;
        }

        public HashSet<SquareCell> GetNakedPairs()
        {
            HashSet<SquareCell> pairs = new HashSet<SquareCell>();
            foreach (SquareCell cell in emptyCells)
            {
                if (SudokuHelper.CountBits(cell.PossibleMask)== 2)
                    pairs.Add(cell);
            }
            return pairs;
        }



        //for naked pairs from here:
        public void RemovePossibilitiesFromRow(int row, int colFirst, int colSecond, HashSet<char> values)
        {
            List<char> valsList = values.ToList();

            for (int i = 0; i < Constants.boardLen; i++)
            {
                foreach (char c in valsList)
                {
                    if (i != colFirst && i != colSecond)
                        board[row, i].RemovePossibleValue(c);
                }
            }
        }




        public void RemovePossibilitiesFromCol(int col, int rowFirst, int rowSecond, HashSet<char> values)
        {
            List<char> valsList = values.ToList();

            for (int i = 0; i < Constants.boardLen; i++)
            {
                foreach (char c in valsList)
                {
                    if (i != rowFirst && i != rowSecond)
                        board[i, col].RemovePossibleValue(c);
                }
            }
        }
        public void RemovePossibilitiesFromBox(int row, int col, int rowFirst, int rowSecond, int colFirst, int colSecond, HashSet<char> values)
        {
            int boxRowStart = (row / boxLen) * boxLen;
            int boxColStart = (col / boxLen) * boxLen;
            List<char> valsList = values.ToList();
            for (int r = boxRowStart; r < boxRowStart + boxLen; r++)
            {
                for (int c = boxColStart; c < boxColStart + boxLen; c++)
                {
                    foreach (char v in valsList)
                    {
                        if (!((r == rowFirst && c == colFirst) || (r == rowSecond && c == colSecond)))
                        {
                            board[r, c].RemovePossibleValue(v);
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            string str = "";
            for (int row = 0; row < Constants.boardLen; row++)
            {
                for (int col = 0; col < Constants.boardLen; col++)
                {
                    str += board[row, col].Value;
                }
            }
            return str;
        }
    }
}
