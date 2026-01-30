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
        public static bool FillAllSingles(Stack<Move> squareCells, ISudokuBoard board)
        {
            bool progress = true;
            while (progress)
            {
                progress = false;

                bool foundNaked;
                do
                {
                    foundNaked = false;
                    foreach (SquareCell cell in board.EmptyCells)
                    {
                        if (cell.PossibleCount == 1)
                        {
                            int bit = SudokuHelper.LowestBit(cell.PossibleMask);
                            char value = SudokuHelper.MaskToChar(bit);

                            board.PlaceNumber(cell.Row, cell.Col, value, squareCells);

                            foundNaked = true;
                            progress = true;
                            break; 
                        }
                    }
                } while (foundNaked);

                foreach (SquareCell cell in board.EmptyCells)
                {
                    int mask = cell.PossibleMask;
                    bool placedHidden = false;

                    while (mask != 0)
                    {
                        int bit = SudokuHelper.LowestBit(mask);
                        mask = SudokuHelper.ClearLowestBit(mask);
                        char value = SudokuHelper.MaskToChar(bit);

                        if (board.IsHiddenSingle(cell.Row, cell.Col, value))
                        {
                            board.PlaceNumber(cell.Row, cell.Col, value, squareCells);

                            progress = true;
                            placedHidden = true;
                            break;
                        }
                    }

                    if (placedHidden)
                        break;
                }
            }
            return progress;
        }

        public static bool FillAffectedSingles(int row, int col, Stack<Move> squareCells, ISudokuBoard board)
        {

            bool progress = false;
            Queue<SquareCell> queue = new Queue<SquareCell>();
            HashSet<SquareCell> visited = new HashSet<SquareCell>();

            // Seed with neighbors of the starting cell
            foreach (SquareCell neighbor in board.board[row, col].Neighbors)
            {
                if (neighbor.Value == Constants.emptyCell && visited.Add(neighbor))
                    queue.Enqueue(neighbor);
            }

            while (queue.Count > 0)
            {
                SquareCell cell = queue.Dequeue();

                if (cell.Value != Constants.emptyCell) continue;

                bool filled = false;

                if (cell.PossibleCount == 1)
                {
                    int bit = SudokuHelper.LowestBit(cell.PossibleMask);
                    char value = SudokuHelper.MaskToChar(bit);
                    board.PlaceNumber(cell.Row, cell.Col, value, squareCells);
                    filled = true;
                }
                else
                {
                    int mask = cell.PossibleMask;
                    while (mask != 0)
                    {
                        int bit = SudokuHelper.LowestBit(mask);
                        mask = SudokuHelper.ClearLowestBit(mask);
                        char value = SudokuHelper.MaskToChar(bit);

                        if (board.IsHiddenSingle(cell.Row, cell.Col, value))
                        {
                            board.PlaceNumber(cell.Row, cell.Col, value, squareCells);
                            filled = true;
                            break;
                        }
                    }
                }

                if (filled)
                {
                    progress = true;
                    foreach (SquareCell neighbor in cell.Neighbors)
                    {
                        if (neighbor.Value == Constants.emptyCell && visited.Add(neighbor))
                            queue.Enqueue(neighbor);
                    }
                }
            }

            return progress;
        }
    }
}