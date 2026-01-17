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
            Stack<SquareCell> implementedCells = new Stack<SquareCell>();
            if (board == null) return false;
            ConstraintPropagations.FillAllSingles(implementedCells,board);
            ConstraintPropagations.RemoveNakedPairs(board);
            if (!board.HasEmptyCells) return true;
            
            SquareCell emptyCell = board.GetFirstEmptyCellWithFewestPossibilities();
            if (!emptyCell.PossibleValues.Any())
                return false;

            foreach(char value in emptyCell.PossibleValues)
            {
                implementedCells = new Stack<SquareCell>();
                board.PlaceNumber(emptyCell.Row,emptyCell.Col,value,implementedCells);
                ConstraintPropagations.FillAllSingles(implementedCells, board);
                ConstraintPropagations.RemoveNakedPairs(board);
                if (Solve(board))
                    return true;
                board.RemoveNumbers(implementedCells);
            }
            return false;
        }
    }
}
