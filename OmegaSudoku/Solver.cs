using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaSudoku
{
    static class Solver
    {
        public static bool Solve(SudokuBoard board)
        {
            if(board == null) return false;
            board.FillAllSingles();
            if (!board.HasEmptyCells) return true;
            
            SquareCell emptyCell = board.GetFirstEmptyCellWithFewestPossibilities();
            if (!emptyCell.PossibleValues.Any())
                return false;

            foreach(char value in emptyCell.PossibleValues)
            {
                board.PlaceNumber(emptyCell.Row,emptyCell.Col,value);
                //board.FillNakedSingles();
                if (Solve(board))
                    return true;
                board.RemoveNumber(emptyCell.Row,emptyCell.Col);
            }
            return false;


            

        }
    }
}
