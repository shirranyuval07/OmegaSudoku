using System;
using System.Collections.Generic;
using System.Linq;

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


            board = new SquareCell[boardLen, boardLen];

            for (int row = 0; row < boardLen; row++)
            {
                for (int col = 0; col < boardLen; col++)
                {
                    int value = boardString[row * boardLen + col] - '0';
                    board[row, col] =
                        new SquareCell(row, col, value);
                    if (value == 0)
                        emptyCells.Add(board[row, col]);
                }
            }

            AddPreExistingNumbers();
            InitializePossibleValues();

            if (!IsValidBoard())
                throw new Exception("The provided board is not valid.");
        }

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


            board[row, col] = new SquareCell(row, col, value);


            rowCandidateCount[row][value]--;
            colCandidateCount[col][value]--;
            boxCandidateCount[BoxIndex(row, col)][value]--;


            return true;
        }


        public void PrintBoard()
        {
            Console.WriteLine();
            for (int row = 0; row < boardLen; row++)
            {
                for (int col = 0; col < boardLen; col++)
                    Console.Write(board[row, col].Value + "|");
                Console.WriteLine();
            }
        }



        /*“Check everywhere this value could go. 
         * If it fits anywhere else → not hidden. 
         * If it fits nowhere else → hidden single.”*/
        private bool IsHiddenSingle(int row, int col, int value)
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

        private void SolveHiddenSingles()
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

        private void FillNakedSingles()
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
