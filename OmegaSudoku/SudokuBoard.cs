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

        private HashSet<char>[] rowUsed;
        private HashSet<char>[] colUsed;
        private HashSet<char>[] boxUsed;

        private HashSet<SquareCell> emptyCells;
        public bool HasEmptyCells => emptyCells.Count > 0;

        public HashSet<SquareCell> EmptyCells
        {
            get { return emptyCells; }
        }

        public HashSet<char>[] RowUsed
        {
            get { return rowUsed; }
        }
        public HashSet<char>[] ColUsed
        {
            get { return colUsed; }
        }
        public HashSet<char>[] BoxUsed
        {
            get { return boxUsed; }
        }
        public int numOfFilledCells { get; set; }

        public SudokuBoard(string boardString)
        {
            Constants.boardLen = (int)Math.Sqrt(boardString.Length);
            Constants.SetSymbol();
            boxLen = (int)Math.Sqrt(Constants.boardLen);
            rowUsed = new HashSet<char>[Constants.boardLen];
            colUsed = new HashSet<char>[Constants.boardLen];
            boxUsed = new HashSet<char>[Constants.boardLen];
            for (int i = 0; i < Constants.boardLen; i++)
            {
                rowUsed[i] = new HashSet<char>();
                colUsed[i] = new HashSet<char>();
                boxUsed[i] = new HashSet<char>();
            }

            this.emptyCells = new HashSet<SquareCell>();


            board = new SquareCell[Constants.boardLen, Constants.boardLen];

            for (int row = 0; row < Constants.boardLen; row++)
            {

                for (int col = 0; col < Constants.boardLen; col++)
                {
                    char value = boardString[row * Constants.boardLen + col];
                    board[row, col] = new SquareCell(row, col, value);
                    if (value == '0')
                        emptyCells.Add(board[row, col]);
                }


            }

            AddPreExistingNumbers();
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
            return !rowUsed[row].Contains(value)
                && !colUsed[col].Contains(value)
                && !boxUsed[BoxIndex(row, col)].Contains(value);

        }

        private void AddPreExistingNumbers()
        {
            for (int row = 0; row < Constants.boardLen; row++)
            {
                for (int col = 0; col < Constants.boardLen; col++)
                {
                    char value = board[row, col].Value;
                    if (value != '0')
                    {
                        rowUsed[row].Add(value);
                        colUsed[col].Add(value);
                        boxUsed[BoxIndex(row, col)].Add(value);

                    }
                }
            }
        }

        public void InitializePossibleValues()
        {

            for (int row = 0; row < Constants.boardLen; row++)
            {
                for (int col = 0; col < Constants.boardLen; col++)
                {
                    if (board[row, col].Value == '0')
                    {
                        var possibleValues = new HashSet<char>();
                        foreach (char d in Constants.symbols)
                        {
                            if (IsValidPlace(row, col, d))
                            {
                                possibleValues.Add(d);
                            }
                        }
                        board[row, col].SetPossibleValues(possibleValues);
                    }
                }
            }
        }

        public SquareCell GetFirstEmptyCellWithFewestPossibilities()
        {
            SquareCell sc = this.emptyCells.First();
            foreach(SquareCell cell in emptyCells)
            {
                if(cell.PossibleValues.Count() < sc.PossibleValues.Count())
                    sc = cell;
            }
            return sc;
        }

        private bool IsValidBoard()
        {
            for (int row = 0; row < Constants.boardLen; row++)
            {
                for (int col = 0; col < Constants.boardLen; col++)
                {
                    if (board[row, col].Value == '0' &&
                        board[row, col].PossibleValues.Count == 0)
                        return false;
                }
            }
            return true;
        }
      
        public bool PlaceNumber(int row, int col, char value,Stack<SquareCell> squareCells)
        {
            if (!IsValidPlace(row, col, value))
                return false;

            squareCells.Push(board[row, col]);
            rowUsed[row].Add(value);
            colUsed[col].Add(value);
            boxUsed[BoxIndex(row, col)].Add(value);

            board[row, col].Value = value;
            emptyCells.Remove(board[row, col]);

            InitializePossibleValues();


            return true;
        }

        public void RemoveNumbers(Stack<SquareCell> squareCells)
        {
            while(squareCells.Count > 0)
            {
                SquareCell curr =  squareCells.Pop();
                int row = curr.Row;
                int col = curr.Col;
                char value = board[row, col].Value;
                rowUsed[row].Remove(value);
                colUsed[col].Remove(value);
                boxUsed[BoxIndex(row, col)].Remove(value);

                board[row, col].Value = '0';
                emptyCells.Add(board[row, col]);
            }
            

            InitializePossibleValues();
        }



        public void PrintBoard()
        {
            Console.WriteLine();
            for (int row = 0; row < Constants.boardLen; row++)
            {
                if (row % boxLen == 0 && row != 0)
                    Console.WriteLine(new string('-',(boxLen * Constants.boardLen) -3)); // -3 bc of the Console.Write(" | "); (size is 3)

                for (int col = 0; col < Constants.boardLen; col++)
                {
                    if (col % boxLen == 0 && col != 0)
                        Console.Write(" | ");

                    Console.Write(board[row, col].Value + " ");
                }
                Console.WriteLine();
            }
        }

        //public void RemoveNakedPairs()
        //{
        //    HashSet<SquareCell> pairs = GetNakedPairs();
        //    List<SquareCell> pairsList = pairs.ToList();
        //    foreach (SquareCell first in pairsList)
        //    {
        //        foreach (SquareCell second in pairsList)
        //        {
        //            if (first != second)
        //            {
        //                //if are equal pairs
        //                if (first.PossibleValues.SetEquals(second.PossibleValues))
        //                {
        //                    bool inSameRow = first.Row == second.Row;
        //                    bool inSameCol = first.Col == second.Col;
        //                    bool inSameBox = BoxIndex(first.Row, first.Col) == BoxIndex(second.Row, second.Col);
        //                    if (inSameRow)
        //                        RemovePossibilitiesFromRow(first.Row, first.Col, second.Col, first.PossibleValues);
        //                    if (inSameCol)
        //                        RemovePossibilitiesFromCol(first.Col, first.Row, second.Row, first.PossibleValues);
        //                    if (inSameBox)
        //                        RemovePossibilitiesFromBox(first.Row, first.Col, first.Row, second.Row, first.Col, second.Col, first.PossibleValues);
        //                }
        //            }
        //        }
        //    }

        //}


        //public void FillAllSingles(Stack<SquareCell> squareCells)
        //{
            
        //    bool progress = true;
        //    while (progress)
        //    {
        //        progress = false;
        //        foreach (SquareCell cell in emptyCells.ToList())
        //        {
        //            if(IsNakedSingle(cell.Row,cell.Col))
        //            {
        //                char value = cell.PossibleValues.First();
        //                PlaceNumber(cell.Row, cell.Col, value,squareCells);
        //                progress = true;
        //            }
        //            else
        //            {
        //                foreach(char val in cell.PossibleValues)
        //                {
        //                    if(IsHiddenSingle(cell.Row,cell.Col,val))
        //                    {
        //                        PlaceNumber(cell.Row, cell.Col, val, squareCells);
        //                        progress = true;
        //                        break;
        //                    }
        //                }
        //            }
        //            if (progress)
        //                break;
        //        }
        //    }
        //}

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
                if (c != col && this.board[row, c].Value == '0' && this.board[row, c].PossibleValues.Contains(value))
                    hiddenInRow = false;
            }
            // Check if the value is a hidden single in its column
            for (int r = 0; r < Constants.boardLen && hiddenInCol; r++)
            {
                if (r != row && this.board[r, col].Value == '0' && this.board[r, col].PossibleValues.Contains(value))
                    hiddenInCol = false;
            }
            // Check if the value is a hidden single in its box
            int boxRowStart = (row / boxLen) * boxLen;
            int boxColStart = (col / boxLen) * boxLen;
            for (int r = boxRowStart; r < boxRowStart + boxLen && hiddenInBox; r++)
            {
                for (int c = boxColStart; c < boxColStart + boxLen && hiddenInBox; c++)
                {
                    if ((r != row || c != col) && this.board[r, c].Value == '0' && this.board[r, c].PossibleValues.Contains(value))
                        hiddenInBox = false;
                }
            }
            return hiddenInBox || hiddenInCol || hiddenInRow;
        }

        public bool IsNakedSingle(int row, int col)
        {
            return this.board[row, col].Value == '0' && this.board[row, col].PossibleValues.Count == 1;
        }
        
        public HashSet<SquareCell> GetNakedPairs()
        {
            HashSet<SquareCell> pairs = new HashSet<SquareCell>();
            foreach(SquareCell cell in emptyCells)
            {
                if (cell.PossibleValues.Count == 2)
                    pairs.Add(cell);
            }
            return pairs;
        }




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
                        if ((r != rowFirst && r != rowSecond) || (c != colFirst && c != colSecond))
                            board[r, c].RemovePossibleValue(v);
                    }
                }
            }
        }
    }
}
