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
        public int numOfFilledCells { get; set; }

        public SudokuBoard(String board)
        {
            this.boardLen = (int)Math.Sqrt(board.Length); 
            this.board = new SquareCell[this.boardLen, this.boardLen];
            for (int row = 0; row < this.boardLen; row++)
                for (int col = 0; col < this.boardLen; col++)
                    this.board[row, col] = new SquareCell(row, col, board[row* this.boardLen + col] - '0');


        }

        public void printBoard()
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
