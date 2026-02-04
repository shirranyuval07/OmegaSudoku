using OmegaSudoku.Core;
using OmegaSudoku.Exceptions;
using OmegaSudoku.Logic;
using System;
using System.Collections.Generic;
using System.Text;

namespace OmegaSudoku.Tests.Unit
{
    public class SolverTests_UnsolvableBoards
    {
        [Fact]
        public void TestUnsolvableBoard()
        {
            string puzzle = "123000000456000000000090000000000000000000000000000000000000000000000000000000000";
            string puzzle2 = "000020003000000000000100000100000030006000000007000500039000000020010000050000001";
            string puzzle3 = "000000051260000000008600000000071020140050000000000300000300400500900000700000000";
            string puzzle4 = "400800502208400973000002840796500000000673019532980000070200195600105000000390400";
            string puzzle5 = "000010500604000000000005000000300062510400000700000000052000100000008030000600000";
            try
            {
                for (int i = 0; i < 5; i++)
                {
                    string currentPuzzle = i switch
                    {
                        0 => puzzle,
                        1 => puzzle2,
                        2 => puzzle3,
                        3 => puzzle4,
                        4 => puzzle5,
                        _ => throw new SudokuException("Invalid puzzle index.")
                    };
                    // Arrange
                    SudokuBoard board = new SudokuBoard(currentPuzzle);
                    // Act
                    bool solved = Solver.Solve(board);
                    if (!solved)
                        throw new UnsolvableSudokuException("The Sudoku puzzle is unsolvable.");
                }
            }
            catch (SudokuException ex)
            {
                // Assert
                Assert.Equal("Sudoku Exception found: Board is unsolvable  The Sudoku puzzle is unsolvable.", ex.Message);
                return;
            }
            throw new Exception("TestUnsolvableBoard failed: Expected SudokuException was not thrown.");
        }

    }
}
