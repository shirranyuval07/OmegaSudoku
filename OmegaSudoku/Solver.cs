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
            if(Constants.boardLen <= 25) //for now cuz solves_big isnt good enough
                return Solves_Small(board, new Stack<Move>());
            else
                return Solves_Big(board,new Stack<Move>());

        }
        public static bool Solves_Small(SudokuBoard board,  Stack<Move> forcedMoves)
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

                if (Solves_Small(board,forcedMoves))
                    return true;

                board.RemoveNumbers(forcedMoves,checkpointGuess);
            }


            board.RemoveNumbers(forcedMoves,checkpointMove);
            return false;
        }
        
        public static bool Solves_Big(SudokuBoard board, Stack<Move> forcedMoves)
        {
            int checkpointMove = forcedMoves.Count;
            if (!board.HasEmptyCells)
                return true;
            SquareCell cell = board.GetBestCell();
            if (cell == null || cell.PossibleCount == 0)
            {
                return false;
            }
            foreach (var value in board.GetLCVValues(cell))
            {
                int checkpointGuess = forcedMoves.Count;
                if (!board.PlaceNumber(cell.Row, cell.Col, value, forcedMoves))
                    continue;

                if (Solves_Small(board, forcedMoves))
                    return true;

                board.RemoveNumbers(forcedMoves, checkpointGuess);
            }


            board.RemoveNumbers(forcedMoves, checkpointMove);
            return false;
        }
        
    }

}
