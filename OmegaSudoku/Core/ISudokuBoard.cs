using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaSudoku.Core
{
    interface ISudokuBoard
    {
        SquareCell[] board {  get; set; }
        int boxLen {  get; set; }

        // Hidden Single Counts
        int[] RowCounts {  get; set; }
        int[] ColCounts {  get; set; }
        int[] BoxCounts {  get; set; }

        // Global Constraints
        int[] rowUsed {  get; set; }
        int[] colUsed {  get; set; }
        int[] boxUsed {  get; set; }

        // Core Collection
        HashSet<SquareCell> EmptyCells { get; set; }

        int fullmask {  get; set; }

        bool HasEmptyCells { get; }


        void InitializeBoard(string boardString);
        SquareCell GetBestCell();


        bool PlaceNumber(int row, int col, char value, Stack<Move> moves);

        void RemoveNumbers(Stack<Move> moves, int checkpoint);
        void UpdateCounts(int r, int c, int mask, int delta);

        void DecrementSingleCount(int r, int c, char d);

        bool IsHiddenSingle(int r, int c, char v);

        bool IsNakedSingle(int r, int c);

        void InitializeNeighbors(SquareCell cell);
        bool IsValidBoard();
        void PrintBoard();
    }
}
