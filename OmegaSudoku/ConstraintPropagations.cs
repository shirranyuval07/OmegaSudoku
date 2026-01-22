using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaSudoku
{
    static class ConstraintPropagations
    {
        public static bool RemoveNakedPairs(SudokuBoard board)
        {
            bool changed = false;

            return changed;
        }
        
        public static bool FillAllSingles(Stack<Move> squareCells, SudokuBoard board)
        {
            bool progress = true;
            while (progress)
            {
                progress = false;
                foreach (SquareCell cell in board.EmptyCells.ToList())
                {

                    if (board.IsNakedSingle(cell.Row, cell.Col))
                    {
                        char value = cell.PossibleValues.First();
                        board.PlaceNumber(cell.Row, cell.Col, value, squareCells);
                        progress = true;
                        break;
                    }
                    else
                    {
                        foreach (char val in cell.PossibleValues)
                        {
                            if (board.IsHiddenSingle(cell.Row, cell.Col, val))
                            {
                                board.PlaceNumber(cell.Row, cell.Col, val, squareCells);
                                progress = true;
                                break;
                            }
                        }
                    }
                    if (progress)
                        break;
                }
            }
            return progress;
        }
       
    }
}
