using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;

namespace OmegaSudoku
{
    class SudokuBoard
    {
        public SquareCell[,] board;

        private int boardLen;
        private int boxLen;

        private HashSet<int>[] rowUsed;
        private HashSet<int>[] colUsed;
        private HashSet<int>[] boxUsed;

        private HashSet<SquareCell> emptyCells;
        public bool HasEmptyCells => emptyCells.Count > 0;


        public HashSet<int>[] RowUsed
        {
            get { return rowUsed; }
        }
        public HashSet<int>[] ColUsed
        {
            get { return colUsed; }
        }
        public HashSet<int>[] BoxUsed
        {
            get { return boxUsed; }
        }
        Dictionary<int, int>[] rowCandidateCount;
        Dictionary<int, int>[] colCandidateCount;
        Dictionary<int, int>[] boxCandidateCount;
        public int numOfFilledCells { get; set; }

        public SudokuBoard(string boardString)
        {
            boardLen = (int)Math.Sqrt(boardString.Length);
            boxLen = (int)Math.Sqrt(boardLen);

            rowUsed = new HashSet<int>[boardLen];
            colUsed = new HashSet<int>[boardLen];
            boxUsed = new HashSet<int>[boardLen];
            for (int i = 0; i < boardLen; i++)
            {
                rowUsed[i] = new HashSet<int>();
                colUsed[i] = new HashSet<int>();
                boxUsed[i] = new HashSet<int>();
            }

            this.emptyCells = new HashSet<SquareCell>();

            this.rowCandidateCount = new Dictionary<int, int>[boardLen];
            this.colCandidateCount = new Dictionary<int, int>[boardLen];
            this.boxCandidateCount = new Dictionary<int, int>[boardLen];

            board = new SquareCell[boardLen, boardLen];

            for (int row = 0; row < boardLen; row++)
            {
                this.rowCandidateCount[row] = new Dictionary<int, int>();
                this.colCandidateCount[row] = new Dictionary<int, int>();
                this.boxCandidateCount[row] = new Dictionary<int, int>();
                for (int d = 1; d <= boardLen; d++)
                {
                    rowCandidateCount[row][d] = 0;
                    colCandidateCount[row][d] = 0;
                    boxCandidateCount[row][d] = 0;
                }

                for (int col = 0; col < boardLen; col++)
                {
                    int value = boardString[row * boardLen + col] - '0';
                    board[row, col] = new SquareCell(row, col, value);
                    if (value == 0)
                        emptyCells.Add(board[row, col]);
                }


            }

            AddPreExistingNumbers();
            InitializePossibleValues();

            if (!IsValidBoard())
                throw new Exception("The provided board is not valid.");
        }

        public int GetBoardLen() { return this.boardLen; }
        public HashSet<SquareCell> GetEmptyCells() { return this.emptyCells; }
        private int BoxIndex(int row, int col)
        {
            return (row / boxLen) * boxLen + (col / boxLen);
        }

        private bool IsValidPlace(int row, int col, int value)
        {
            return !rowUsed[row].Contains(value)
                && !colUsed[col].Contains(value)
                && !boxUsed[BoxIndex(row, col)].Contains(value);

        }

        private void AddPreExistingNumbers()
        {
            for (int row = 0; row < boardLen; row++)
            {
                for (int col = 0; col < boardLen; col++)
                {
                    int value = board[row, col].Value;
                    if (value != 0)
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
            ResetCandidateCounts();

            for (int row = 0; row < boardLen; row++)
            {
                for (int col = 0; col < boardLen; col++)
                {
                    if (board[row, col].Value == 0)
                    {
                        var possibleValues = new HashSet<int>();
                        for (int d = 1; d <= boardLen; d++)
                        {
                            if (IsValidPlace(row, col, d))
                            {
                                possibleValues.Add(d);
                                rowCandidateCount[row][d]++;
                                colCandidateCount[col][d]++;
                                boxCandidateCount[BoxIndex(row, col)][d]++;
                            }
                        }
                        board[row, col].SetPossibleValues(possibleValues);
                    }
                }
            }
        }
        private void ResetCandidateCounts()
        {
            for (int i = 0; i < boardLen; i++)
            {
                for (int d = 1; d <= boardLen; d++)
                {
                    rowCandidateCount[i][d] = 0;
                    colCandidateCount[i][d] = 0;
                    boxCandidateCount[i][d] = 0;
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
            for (int row = 0; row < boardLen; row++)
            {
                for (int col = 0; col < boardLen; col++)
                {
                    if (board[row, col].Value == 0 &&
                        board[row, col].PossibleValues.Count == 0)
                        return false;
                }
            }
            return true;
        }

        public bool PlaceNumber(int row, int col, int value)
        {
            if (!IsValidPlace(row, col, value))
                return false;


            rowUsed[row].Add(value);
            colUsed[col].Add(value);
            boxUsed[BoxIndex(row, col)].Add(value);

            board[row, col].Value = value;
            emptyCells.Remove(board[row, col]);

            InitializePossibleValues();



            return true;
        }

        public void RemoveNumber(int row,int col)
        {
            int value = board[row, col].Value;
            rowUsed[row].Remove(value);
            colUsed[col].Remove(value);
            boxUsed[BoxIndex(row, col)].Remove(value);

            board[row, col].Value = 0;
            emptyCells.Add(board[row, col]);

            InitializePossibleValues();
        }



        public void PrintBoard()
        {
            Console.WriteLine();
            for (int row = 0; row < boardLen; row++)
            {
                if (row % boxLen == 0 && row != 0)
                    Console.WriteLine(new string('-',(boxLen * boardLen) -3)); // -3 bc of the Console.Write(" | "); (size is 3)

                for (int col = 0; col < boardLen; col++)
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
        public bool IsHiddenSingle(int row, int col, int value)
        {
            // Check if the value is a hidden single in its row
            for (int c = 0; c < this.boardLen; c++)
            {
                if (c != col && this.board[row, c].Value == 0 && this.board[row, c].PossibleValues.Contains(value))
                    return false;
            }
            // Check if the value is a hidden single in its column
            for (int r = 0; r < this.boardLen; r++)
            {
                if (r != row && this.board[r, col].Value == 0 && this.board[r, col].PossibleValues.Contains(value))
                    return false;
            }
            // Check if the value is a hidden single in its box
            int boxRowStart = (row / boxLen) * boxLen;
            int boxColStart = (col / boxLen) * boxLen;
            for (int r = boxRowStart; r < boxRowStart + boxLen; r++)
            {
                for (int c = boxColStart; c < boxColStart + boxLen; c++)
                {
                    if ((r != row || c != col) && this.board[r, c].Value == 0 && this.board[r, c].PossibleValues.Contains(value))
                        return false;
                }
            }
            return true;
        }

        public void SolveHiddenSingles()
        {
            // Iterate through all cells and fill in hidden singles
            bool progressMade = true;
            while (progressMade)
            {
                progressMade = false;
                for (int row = 0; row < this.boardLen; row++)
                {
                    for (int col = 0; col < this.boardLen; col++)
                    {
                        if (this.board[row, col].Value == 0)
                        {
                            HashSet<int> possibleValues = this.board[row, col].PossibleValues;
                            foreach (int value in possibleValues)
                            {
                                if (IsHiddenSingle(row, col, value))
                                {
                                    PlaceNumber(row, col, value);
                                    progressMade = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void FillNakedSingles()
        {
            // Iterate through all cells and fill in naked singles
            bool progressMade = true;
            while (progressMade)
            {
                progressMade = false;
                for (int row = 0; row < this.boardLen; row++)
                {
                    for (int col = 0; col < this.boardLen; col++)
                    {
                        if (this.board[row, col].Value == 0 && this.board[row, col].PossibleValues.Count == 1)
                        {
                            int value = this.board[row, col].PossibleValues.First();
                            PlaceNumber(row, col, value);
                            progressMade = true;
                        }
                    }
                }
            }
        }

        

    }
}
