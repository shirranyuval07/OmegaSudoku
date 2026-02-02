using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaSudoku
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
            ConstraintPropagations.FillAllSingles(forcedMoves, board);
            if (!board.HasEmptyCells)
                return true;
            SquareCell cell = board.GetBestCell();
            if (cell == null || cell.PossibleCount == 0)
            {
                return false;
            }
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
                ConstraintPropagations.FillAffectedSingles(cell.Row,cell.Col,forcedMoves, board);

                if (Solves(board,forcedMoves))
                    return true;

                board.RemoveNumbers(forcedMoves,checkpointGuess);
            }


            board.RemoveNumbers(forcedMoves,checkpointMove);
            return false;
        }
        
    }

}
