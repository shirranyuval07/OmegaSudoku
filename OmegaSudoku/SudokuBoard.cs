using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaSudoku
{
    class SudokuBoard
    {
        public SquareCell[,] board;
        public int boardLen { get; set; }

        public int boxLen { get; set; }
        public int numOfFilledCells { get; set; }

        private bool[,] rowUsed;
        private bool[,] colUsed;
        private bool[,] boxUsed;

        public SudokuBoard(String board)
        {
            this.boardLen = (int)Math.Sqrt(board.Length);
            this.boxLen = (int)Math.Sqrt(this.boardLen);
            rowUsed = new bool[boardLen, boardLen + 1];
            colUsed = new bool[boardLen, boardLen + 1];
            boxUsed = new bool[boardLen, boardLen + 1];

            this.board = new SquareCell[this.boardLen, this.boardLen];
            for (int row = 0; row < this.boardLen; row++)
                for (int col = 0; col < this.boardLen; col++)
                    this.board[row, col] = new SquareCell(row, col, board[row* this.boardLen + col] - '0');
            AddPreExistingNumbers();
            PrintBoard();
            Console.WriteLine("Board After Solving: ");
            InitializePossibleValues();
            SolveBoard();
            if(!IsValidBoard())
            {
                throw new Exception("The provided board is not valid.");
            }
        }

        private bool CheckRow(int row, int value)
        {
            return this.rowUsed[row, value];
        }
        private bool CheckCol(int col, int value)
        {
            return this.colUsed[col, value];
        }
        private bool CheckBox(int row, int col, int value)
        {
            return this.boxUsed[(row / boxLen) * boxLen + (col / boxLen), value];
        }



        private void InitializePossibleValues()
        {
            for (int row = 0; row < this.boardLen; row++)
            {
                for (int col = 0; col < this.boardLen; col++)
                {
                    if (this.board[row,col].Value == 0)
                    {
                        List<int> possibleValues = new List<int>();
                        for (int d = 1; d <= this.boardLen; d++)
                        {
                            if (IsValidPlace(row, col, d))
                            {
                                possibleValues.Add(d);
                            }
                        }
                        this.board[row, col].SetPossibleValues(possibleValues);
                    }
                }
            }
        }
        private bool IsValidBoard()
        {
            // Check if the board is valid
            for (int row = 0; row < this.boardLen; row++)
            {
                for (int col = 0; col < this.boardLen; col++)
                {
                    int value = this.board[row, col].Value;
                    if (value == 0)
                    {
                        if (!IsValidPlace(row, col, value))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        private bool IsValidPlace(int row, int col, int value)
        {
            return !rowUsed[row, value]
                && !colUsed[col, value]
                && !boxUsed[(row / boxLen) * boxLen + (col / boxLen), value];
        }

        private void PlaceNumber(int row, int col, int value)
        {
            if(IsValidPlace(row, col, value))
            {
                this.rowUsed[row,value] = true;
                this.colUsed[col, value] = true;
                this.boxUsed[(row / boxLen) * boxLen + (col / boxLen), value] = true;
                this.board[row, col] = new SquareCell(row,col,value);
            }
        }
        private void RemoveNumber(int row, int col, int value)
        {
            this.rowUsed[row, value] = false;
            this.colUsed[col, value] = false;
            this.boxUsed[(row / boxLen) * boxLen + (col / boxLen), value] = false;
            this.board[row, col] = new SquareCell(row, col, 0);
        }

        private void AddPreExistingNumbers()
        {
            for (int row = 0; row < this.boardLen; row++)
            {
                for (int col = 0; col < this.boardLen; col++)
                {
                    if (this.board[row,col].Value != 0)
                    {
                        int d = this.board[row,col].Value;
                        this.rowUsed[row,d] = true;
                        this.colUsed[col,d] = true;
                        this.boxUsed[(row / boxLen) * boxLen + (col / boxLen),d] = true;
                    }
                }
            }
        }

        private void SolveBoard()
        {
            // Implementation of a backtracking algorithm to solve the Sudoku board
            FillNakedSingles();
            SolveHiddenSingles();

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
            while(progressMade)
            {
                progressMade = false;
                for (int row = 0; row < this.boardLen; row++)
                {
                    for (int col = 0; col < this.boardLen; col++)
                    {
                        if (this.board[row, col].Value == 0)
                        {
                            List<int> possibleValues = this.board[row, col].PossibleValues;
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
            while(progressMade)
            {
                progressMade = false;
                for (int row = 0; row < this.boardLen; row++)
                {
                    for (int col = 0; col < this.boardLen; col++)
                    {
                        if (this.board[row, col].Value == 0 && this.board[row, col].PossibleValues.Count == 1)
                        {
                            int value = this.board[row, col].PossibleValues[0];
                            PlaceNumber(row, col, value);
                            progressMade = true;
                        }
                    }
                }
            }
        }

        public void PrintBoard()
        {
            Console.WriteLine();
            for (int row = 0; row < this.boardLen; row++)
            {
                for(int col = 0; col < this.boardLen; col++)
                    Console.Write(this.board[row, col].Value + "|");
                Console.WriteLine("");
                Console.WriteLine("------------------");
            }
        }



    }
}
