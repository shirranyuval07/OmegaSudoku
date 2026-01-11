using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaSudoku
{
    static class Solver
    {
        public static void Solve(SudokuBoard board)
        {
            if(board == null) return;
            if(board.emptyCellsCount == 0) return;
            int len = board.GetBoardLen();
            for (int row = 0; row < len; row++)
            {
                for (int col = 0; col < len; col++)
                {
                    board.FillNakedSingles();
                    board.SolveHiddenSingles();
                    

                }
            }

        }
    }
}
