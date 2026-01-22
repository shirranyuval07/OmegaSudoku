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
            return Solves(board, new Stack<Move>());
        }
        public static bool Solves(SudokuBoard board, Stack<Move> implementedCells)
        {
            if (board == null) return false;
            //ConstraintPropagations.RemoveNakedPairs(board);
            ConstraintPropagations.FillAllSingles(implementedCells, board);
            if (!board.HasEmptyCells)
                return true;
            SquareCell emptyCell = board.GetBestCell();
            if (!emptyCell.PossibleValues.Any())
                return false;

            foreach (char value in emptyCell.PossibleValues.ToList())
            {
                implementedCells = new Stack<Move>();

                board.PlaceNumber(emptyCell.Row, emptyCell.Col, value, implementedCells);

                //ConstraintPropagations.RemoveNakedPairs(board);
                ConstraintPropagations.FillAllSingles(implementedCells, board);

                if (Solves(board, implementedCells))
                    return true;
                board.RemoveNumbers(implementedCells);
            }
            return false;
        }

    }
}
