using OmegaSudoku.Core;
using OmegaSudoku.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Buffers;

namespace OmegaSudoku.Logic
{
    static class ConstraintPropagations
    {
        private static Queue<SquareCell> queue = new Queue<SquareCell>();
        private static HashSet<SquareCell> visited = new HashSet<SquareCell>();
        /// <summary>
        /// Attempts to fill all cells in the Sudoku board that can be determined by naked or hidden singles, applying
        /// moves until no further progress can be made.
        /// </summary>
        /// <remarks>This method repeatedly searches for naked singles (cells with only one possible
        /// value) and hidden singles (values that can only go in one cell within a unit) and fills them. The process
        /// continues until no further singles can be found. The board is modified in place, and all moves are recorded
        /// in the provided stack. This method does not guarantee that the puzzle will be completely solved, only tx`hat
        /// all singles are filled.</remarks>
        /// <param name="squareCells">A stack used to record each move made when placing a number in a cell. Moves are pushed onto this stack as
        /// they are applied.</param>
        /// <param name="board">The Sudoku board on which to perform the single-candidate filling operations. Must provide access to empty
        /// cells and support placing numbers.</param>
        /// <returns>true if at least one cell was filled during the operation; otherwise, false.</returns>
        public static bool FillAllSingles(Stack<Move> squareCells, ISudokuBoard board)
        {
            bool progress = true;//tracks whether any cells were filled during the process, allowing for multiple iterations until no more singles are found
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

        /// <summary>
        /// Attempts to fill all affected single-candidate cells adjacent to the specified cell on the Sudoku board,
        /// propagating as new singles are created.
        /// </summary>
        /// <remarks>This method examines all empty neighbor cells of the specified cell and fills any
        /// that are singles (cells with only one possible candidate), continuing recursively as new singles are
        /// created. It is typically used after placing a value to propagate deterministic fills. The stack provided in
        /// squareCells will be updated with each move made by this method.</remarks>
        /// <param name="row">The zero-based row index of the starting cell.</param>
        /// <param name="col">The zero-based column index of the starting cell.</param>
        /// <param name="squareCells">A stack used to record moves for undo or tracking purposes. Each placed value is pushed onto this stack.</param>
        /// <param name="board">The Sudoku board on which to perform the operation. Must not be null.</param>
        /// <returns>true if at least one cell was filled during the operation; otherwise, false.</returns>
        public static bool FillAffectedSingles(int row, int col, Stack<Move> squareCells, ISudokuBoard board)
        {

            bool progress = false;
            queue.Clear();
            visited.Clear();

            // Seed with neighbors of the starting cell
            foreach (SquareCell neighbor in board.board[row*Constants.boardLen+ col].Neighbors)
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