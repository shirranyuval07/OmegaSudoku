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
            InitializePossibleValues();
            SolveBoard();
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
        private bool IsValidPlace(int row, int col, int value)
        {
            if (!this.rowUsed[row,value] && !this.colUsed[col,value] && !this.boxUsed[(row / boxLen) * boxLen + (col / boxLen),value])
                return true;
            return false;
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
            //SolveHiddenSingles();

        }

        private bool CanPlace(int row, int col, int value)
        {
            return !rowUsed[row, value]
                && !colUsed[col, value]
                && !boxUsed[(row / boxLen) * boxLen + (col / boxLen), value];
        }


        private void FillNakedSingles()
        {
            // Iterate through all cells and fill in naked singles
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
