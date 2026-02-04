using OmegaSudoku.Core;
using OmegaSudoku.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaSudoku.Logic
{
    static class Solver
    {

        public static bool Solve(ISudokuBoard board)
        {
            Stack<Move> moves = new Stack<Move>();
            return Solves(board, moves);
        }

        public static bool Solves(ISudokuBoard board,  Stack<Move> forcedMoves)
        {
            int checkpointMove = forcedMoves.Count;
            //fill as many singles as possible to save time on iteration.
            ConstraintPropagations.FillAllSingles(forcedMoves, board);
            if (!board.HasEmptyCells)
                return true;
            //get best cell to apply.
            SquareCell cell = board.GetBestCell();
            if (cell == null || cell.PossibleCount == 0)
            {
                return false;
            }
            //go over possible values
            int mask = cell.PossibleMask;
            while (mask != 0)
            {

                int bit = SudokuHelper.LowestBit(mask);
                mask = SudokuHelper.ClearLowestBit(mask);

                char value = SudokuHelper.MaskToChar(bit);

                int checkpointGuess = forcedMoves.Count;
                if (!board.PlaceNumber(cell.Row, cell.Col, value, forcedMoves))
                {
                    board.RemoveNumbers(forcedMoves, checkpointGuess);
                    continue;   
                }
                //if number was placed its row/col/box values can be changed so we try to fill singles for according row/col/box.
                ConstraintPropagations.FillAffectedSingles(cell.Row,cell.Col,forcedMoves, board);

                if (Solves(board,forcedMoves))
                    return true;

                //backtrack for guess
                board.RemoveNumbers(forcedMoves,checkpointGuess);
            }

            //backtrack for forced guess
            board.RemoveNumbers(forcedMoves,checkpointMove);
            return false;
        }
        
    }

}
