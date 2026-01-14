using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaSudoku
{
    static class ConstraintPropagations
    {
        public static void RemoveNakedPairs(SudokuBoard board)
        {
            HashSet<SquareCell> pairs = board.GetNakedPairs();
            List<SquareCell> pairsList = pairs.ToList();
            foreach (SquareCell first in pairsList)
            {
                foreach (SquareCell second in pairsList)
                {
                    if (first != second)
                    {
                        //if are equal pairs
                        if (first.PossibleValues.SetEquals(second.PossibleValues))
                        {
                            bool inSameRow = first.Row == second.Row;
                            bool inSameCol = first.Col == second.Col;
                            bool inSameBox = board.BoxIndex(first.Row, first.Col) == board.BoxIndex(second.Row, second.Col);
                            if (inSameRow)
                                board.RemovePossibilitiesFromRow(first.Row, first.Col, second.Col, first.PossibleValues);
                            if (inSameCol)
                                board.RemovePossibilitiesFromCol(first.Col, first.Row, second.Row, first.PossibleValues);
                            if (inSameBox)
                                board.RemovePossibilitiesFromBox(first.Row, first.Col, first.Row, second.Row, first.Col, second.Col, first.PossibleValues);
                        }
                    }
                }
            }
        }
        public static void FillAllSingles(Stack<SquareCell> squareCells, SudokuBoard board)
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
        }

    }
}
