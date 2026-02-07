using OmegaSudoku.Exceptions;
using OmegaSudoku.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OmegaSudoku.Core
{
    class SudokuBoard : ISudokuBoard
    {
        public SquareCell[] board;

        private int boxLen;
        private readonly int boardLen;

        public int[] RowCounts;
        public int[] ColCounts;
        public int[] BoxCounts;

        private int[] rowUsed;
        private int[] colUsed;
        private int[] boxUsed;
        private int[,] counterEmptyNeighbors;

        private HashSet<SquareCell> emptyCells;

        private int fullmask;

        public bool HasEmptyCells => emptyCells.Count > 0;

        public HashSet<SquareCell> EmptyCells => emptyCells;
        public int[] RowUsed => rowUsed;
        public int[] ColUsed => colUsed;
        public int[] BoxUsed => boxUsed;

        public int numOfFilledCells { get; set; }
        SquareCell[] ISudokuBoard.board
        {
            get => board;
            set => board = value;
        }

        int ISudokuBoard.boxLen
        {
            get => boxLen;
            set => boxLen = value;
        }

        int[] ISudokuBoard.RowCounts
        {
            get => RowCounts;
            set => RowCounts = value;
        }

        int[] ISudokuBoard.ColCounts
        {
            get => ColCounts;
            set => ColCounts = value;
        }

        int[] ISudokuBoard.BoxCounts
        {
            get => BoxCounts;
            set => BoxCounts = value;
        }

        int[] ISudokuBoard.rowUsed
        {
            get => rowUsed;
            set => rowUsed = value;
        }

        int[] ISudokuBoard.colUsed
        {
            get => colUsed;
            set => colUsed = value;
        }

        int[] ISudokuBoard.boxUsed
        {
            get => boxUsed;
            set => boxUsed = value;
        }

        HashSet<SquareCell> ISudokuBoard.EmptyCells
        {
            get => emptyCells;
            set => emptyCells = value;
        }

        int ISudokuBoard.fullmask
        {
            get => fullmask;
            set => fullmask = value;
        }


        /// <summary>
        /// Initializes a new instance of the SudokuBoard class from a string representation of the puzzle.
        /// </summary>
        /// <param name="boardString">A string representing the initial state of the Sudoku board. The string length must be a perfect square, and
        /// its square root must also be a perfect square to form a valid Sudoku grid. Each character represents a cell
        /// value or an empty cell, as defined by the implementation.</param>
        /// <exception cref="InvalidPuzzleException">Thrown if the provided board string is null, empty, not a perfect square in length, does not correspond to a
        /// valid Sudoku grid size, or represents an invalid Sudoku board.</exception>
        public SudokuBoard(string boardString)
        {
            //check for invalid board before initialization
            if(boardString == null || boardString == "" || boardString.Length == 1)
                throw new InvalidPuzzleException("The provided board string is null or empty or single.");
            double sqrtLen = Math.Sqrt(boardString.Length);
            if(sqrtLen % 1 != 0)
                throw new InvalidBoardLengthException("The provided board string length is not a perfect square.");

            Constants.boardLen = (int)sqrtLen;
            this.boardLen = Constants.boardLen;
            Constants.SetSymbol();
            double sqrtBoxLen = Math.Sqrt(boardLen);
            if(sqrtBoxLen % 1 != 0)
                throw new InvalidBoardLengthException("The board length does not have an integer square root, invalid for Sudoku.");

            boxLen = (int)sqrtBoxLen;

            int clues = boardString.Count(c => c != Constants.emptyCell);


            fullmask = (1 << boardLen) - 1;

            // Initialize Count Arrays
            RowCounts = new int[boardLen*boardLen];
            ColCounts = new int[boardLen*boardLen];
            BoxCounts = new int[boardLen * boardLen];

            rowUsed = new int[boardLen];
            colUsed = new int[boardLen];
            boxUsed = new int[boardLen];

            counterEmptyNeighbors = new int[boardLen, boardLen];

            emptyCells = new HashSet<SquareCell>();
            board = new SquareCell[boardLen*boardLen];

            InitializeBoard(boardString);
            InitializePossibleValues();

            if (!IsValidBoard())
                throw new InvalidPuzzleException("The provided board is not valid.");
        }


        /// <summary>
        /// Initializes the Neighbors property of the specified cell with all other cells in the same row, column, and
        /// box.
        /// </summary>
        /// <remarks>This method identifies all cells that share a row, column, or box with the specified
        /// cell, excluding the cell itself, and assigns them to the cell's Neighbors property. This is typically used
        /// in Sudoku board implementations to facilitate constraint checking.</remarks>
        /// <param name="cell">The cell for which to determine and assign neighboring cells. Cannot be null.</param>
        public void InitializeNeighbors(SquareCell cell)
        {
            var neighbors = new HashSet<SquareCell>();

            for (int row = 0; row < boardLen; row++)
            {
                if (row != cell.Row)
                    neighbors.Add(board[row*boardLen + cell.Col]);
            }
            for (int col = 0; col < boardLen; col++)
            {
                if (col != cell.Col)
                    neighbors.Add(board[cell.Row*boardLen + col]);
            }
            int boxRowStart = (cell.Row / boxLen) * boxLen;
            int boxColStart = (cell.Col / boxLen) * boxLen;
            for (int r = boxRowStart; r < boxRowStart + boxLen; r++)
            {
                for (int c = boxColStart; c < boxColStart + boxLen; c++)
                {
                    if (r != cell.Row || c != cell.Col)
                        neighbors.Add(board[r * boardLen + c]);
                }
            }
            cell.Neighbors = neighbors.ToArray();
        }

        /// <summary>
        /// Initializes the Sudoku board state from a string representation of the board.
        /// </summary>
        /// <remarks>This method resets the board, sets up all cells and their possible values, and
        /// identifies empty cells based on the provided string. The length and content of the string must match the
        /// expected board size and allowed cell values.</remarks>
        /// <param name="boardString">A string containing the initial values for all cells in the board, ordered row by row. Each character
        /// represents a cell value and must be valid for the board size.</param>
        private void InitializeBoard(string boardString)
        {
            SquareCell curr = null;
            for (int row = 0; row < boardLen; row++)
            {
                for (int col = 0; col < boardLen; col++)
                {
                    char value = boardString[row * boardLen + col];
                    curr = board[row*boardLen + col] = new SquareCell(row, col, value);
                    SudokuHelper.ValidateChar(value, boardLen);
                    if (value == Constants.emptyCell)
                    {
                        emptyCells.Add(curr);
                        curr.PossibleMask = fullmask;
                    }
                    else
                    {
                        int bit = SudokuHelper.BitFromChar(value);
                        rowUsed[row] |= bit;
                        colUsed[col] |= bit;
                        boxUsed[curr.BoxIndex] |= bit;
                        curr.PossibleMask = bit;
                    }
                }
            }
            // Initialize neighbors for each cell
            for (int row = 0; row < boardLen; row++)
            {
                for (int col = 0; col < boardLen; col++)
                {
                    InitializeNeighbors(board[row * boardLen + col]);
                }
            }
        }
        /// <summary>
        /// Initializes the set of possible values for each empty cell based on the current state of the puzzle.
        /// </summary>
        /// <remarks>Call this method after modifying the puzzle grid to update the possible values for
        /// all empty cells. This method recalculates possible values by considering the values already used in each
        /// row, column, and box.</remarks>
        public void InitializePossibleValues()
        {
            foreach (var cell in emptyCells)
            {
                cell.PossibleMask = ~(rowUsed[cell.Row] | colUsed[cell.Col] | boxUsed[cell.BoxIndex]) & fullmask;
                counterEmptyNeighbors[cell.Row, cell.Col] = CountEmptyNeighbors(cell.Row, cell.Col);

                UpdateCounts(cell.Row, cell.Col, cell.PossibleMask, 1);
            }
        }
        /// <summary>
        /// Updates the candidate digit counts for the specified row, column, and box based on the provided mask and
        /// delta.
        /// </summary>
        /// <remarks>This method is typically used to increment or decrement the counts of possible digits
        /// for a cell in a Sudoku puzzle. The mask allows updating multiple digits at once. The method affects the
        /// counts for the specified row, column, and the corresponding 3x3 box.</remarks>
        /// <param name="row">The zero-based index of the row to update.</param>
        /// <param name="col">The zero-based index of the column to update.</param>
        /// <param name="mask">A bitmask indicating which candidate digits to update. Each set bit corresponds to a digit to be affected.</param>
        /// <param name="delta">The value to add to or subtract from the candidate counts. Typically 1 to increment or -1 to decrement.</param>

        private void UpdateCounts(int row, int col, int mask, int delta)
        {
            int b = board[row * boardLen + col].BoxIndex;
            while (mask != 0)
            {
                int bit = mask & -mask;
                mask ^= bit;           
                int d = SudokuHelper.BitToIndex(bit);

                RowCounts[row*boardLen+ d] += delta;
                ColCounts[col*boardLen+ d] += delta;
                BoxCounts[b * boardLen + d] += delta;
            }
        }
        /// <summary>
        /// Decrements the count of the specified character in the row, column, and box at the given positions.
        /// </summary>
        /// <param name="row">The zero-based index of the row in which to decrement the count.</param>
        /// <param name="col">The zero-based index of the column in which to decrement the count.</param>
        /// <param name="value">The character whose count is to be decremented at the specified row and column.</param>
        public void DecrementSingleCount(int row, int col, char value)
        {
            int index = Constants.CharToIndex[value];
            RowCounts[row*boardLen+ index]--;
            ColCounts[col * boardLen + index]--;
            BoxCounts[board[row * boardLen + col].BoxIndex*boardLen+ index]--;
        }

        /// <summary>
        /// Determines whether the specified value is a hidden single in the given cell of the Sudoku grid.
        /// </summary>
        /// <remarks>A hidden single occurs when a candidate value appears only once in a row, column, or
        /// box, making it the only possible value for that cell according to Sudoku rules.</remarks>
        /// <param name="row">The zero-based index of the row containing the cell to check.</param>
        /// <param name="col">The zero-based index of the column containing the cell to check.</param>
        /// <param name="value">The candidate value to test for a hidden single, represented as a character.</param>
        /// <returns>true if the specified value is a hidden single in the given cell; otherwise, false.</returns>
        public bool IsHiddenSingle(int row, int col, char value)
        {
            int d = Constants.CharToIndex[value];
            return RowCounts[row * boardLen + d] == 1 ||
                   ColCounts[col * boardLen + d] == 1 ||
                   BoxCounts[board[row * boardLen + col].BoxIndex*boardLen+ d] == 1;
        }

        /// <summary>
        /// Selects the most constrained empty cell based on the number of possible values and, in case of a tie, the
        /// number of empty neighbors.
        /// </summary>
        /// <remarks>This method prioritizes cells with the fewest possible values (naked singles are
        /// returned immediately). If multiple cells have the same number of possibilities, the cell with the most empty
        /// neighbors is chosen. This selection strategy can help optimize puzzle-solving algorithms by reducing
        /// branching.</remarks>
        /// <returns>A reference to the best candidate empty cell, or null if there are no empty cells remaining.</returns>
        public SquareCell GetBestCell()
        {
            if (emptyCells.Count == 0)
                return null;

            SquareCell bestCell = null;
            int minOptions = int.MaxValue;
            int maxDegree = -1;

            foreach (var cell in emptyCells)
            {
                int options = cell.PossibleCount;

                // Naked single: immediately return
                if (options == 1)
                    return cell;

                if (options < minOptions)
                {
                    minOptions = options;
                    bestCell = cell;
                    maxDegree = counterEmptyNeighbors[cell.Row, cell.Col];
                }
                else if (options == minOptions)
                {
                    int degree = counterEmptyNeighbors[cell.Row, cell.Col];
                    if (degree > maxDegree)
                    {
                        maxDegree = degree;
                        bestCell = cell;
                    }
                }
            }

            return bestCell;
        }

        /// <summary>
        /// Counts the number of empty neighboring cells adjacent to the specified cell on the board.
        /// </summary>
        /// <param name="row">The zero-based row index of the cell whose neighbors are to be evaluated.</param>
        /// <param name="col">The zero-based column index of the cell whose neighbors are to be evaluated.</param>
        /// <returns>The number of neighboring cells that are empty. Returns 0 if there are no empty neighbors.</returns>
        // Degree heuristic: counts empty neighbors
        public int CountEmptyNeighbors(int row, int col)
        {
            int count = 0;
            foreach (var neighbor in board[row * boardLen + col].Neighbors)
            {
                if (neighbor.Value == Constants.emptyCell)
                    count++;
            }
            return count;
        }
        /// <summary>
        /// Attempts to place the specified number in the given cell and updates the game state accordingly.
        /// </summary>
        /// <remarks>If the placement is invalid according to Sudoku rules, the method returns false and
        /// does not modify the board or the moves stack. On a successful placement, the method updates the board state
        /// and propagates the effects to neighboring cells. The moves stack can be used to revert the changes if
        /// needed.</remarks>
        /// <param name="row">The zero-based row index of the cell where the number is to be placed.</param>
        /// <param name="col">The zero-based column index of the cell where the number is to be placed.</param>
        /// <param name="value">The character representing the number to place in the cell. Must be a valid Sudoku value.</param>
        /// <param name="moves">A stack used to record the moves and previous possible values for undo or backtracking purposes. The method
        /// pushes the current state of affected cells onto this stack.</param>
        /// <returns>true if the number was successfully placed and the move does not violate Sudoku constraints; otherwise,
        /// false.</returns>
        public bool PlaceNumber(int row, int col, char value, Stack<Move> moves)
        {

            SquareCell cell = board[row * boardLen + col];
            int bit = SudokuHelper.BitFromChar(value);

            if((cell.PossibleMask & bit) == 0)
                return false;

            moves.Push(new Move(cell, cell.PossibleMask));

            //maintenance:
            UpdateCounts(row, col, cell.PossibleMask, -1);
            //update values
            cell.Value = value;
            emptyCells.Remove(cell);
            cell.PossibleMask = 0;

            bool flag = true;
            //propagate:
            foreach (SquareCell neighbor in cell.Neighbors)
            {
                if (neighbor.Value == Constants.emptyCell)
                    counterEmptyNeighbors[neighbor.Row, neighbor.Col]--;

                if (neighbor.Value == Constants.emptyCell && neighbor.Contains(value))
                {
                    moves.Push(new Move(neighbor, neighbor.PossibleMask));
                    
                    DecrementSingleCount(neighbor.Row, neighbor.Col, value);

                    if(!neighbor.RemovePossibleValue(value))
                        flag = false;
                }
            }
            return flag;
        }

        /// <summary>
        /// Reverts moves from the specified stack until the number of moves equals the given checkpoint, restoring the
        /// puzzle state to that point.
        /// </summary>
        /// <remarks>Use this method to backtrack the puzzle state to a previous point, such as during
        /// puzzle solving or undo operations. All changes made by moves above the checkpoint will be reversed,
        /// including cell values and related state.</remarks>
        /// <param name="moves">The stack of moves to be reverted. Moves are removed from this stack until its count matches the checkpoint
        /// value.</param>
        /// <param name="checkpoint">The target number of moves to retain in the stack. All moves above this count will be undone.</param>
        public void RemoveNumbers(Stack<Move> moves, int checkpoint)
        {
            while (moves.Count > checkpoint)
            {
                Move move = moves.Pop();
                SquareCell cell = move.Cell;
                bool wasFilled = cell.Value != Constants.emptyCell;

                int oldMask = cell.PossibleMask;
                if (wasFilled)
                {
                    //make it empty
                    cell.Value = Constants.emptyCell;
                    emptyCells.Add(cell);
                    //propagate neighbors
                    foreach (var neighbor in cell.Neighbors)
                    {
                        counterEmptyNeighbors[neighbor.Row, neighbor.Col]++;
                    }
                }

                cell.PossibleMask = move.PreviousMask;

                if (wasFilled)
                {
                    // If cell was filled, we put back all its options
                    UpdateCounts(cell.Row, cell.Col, cell.PossibleMask, 1);
                }
                else
                {
                    // If neighbor update, put back the specific bits that were stripped
                    int restoredBits = SudokuHelper.ClearBit(move.PreviousMask, oldMask);
                    if (restoredBits != 0)
                    {
                        UpdateCounts(cell.Row, cell.Col, restoredBits, 1);
                    }
                }
            }
        }

        
        /// <summary>
        /// Determines whether the specified cell is a naked single, meaning it has exactly one possible candidate value
        /// remaining.
        /// </summary>
        /// <remarks>A naked single occurs when a cell is empty and only one candidate value remains
        /// possible for that cell. This method does not modify the board.</remarks>
        /// <param name="row">The zero-based row index of the cell to check.</param>
        /// <param name="col">The zero-based column index of the cell to check.</param>
        /// <returns>true if the cell at the specified row and column is empty and has exactly one possible candidate; otherwise,
        /// false.</returns>
        public bool IsNakedSingle(int row, int col)
        {
            return board[row * boardLen + col].Value == Constants.emptyCell && board[row * boardLen + col].PossibleMask != 0 && (board[row * boardLen + col].PossibleMask & (board[row * boardLen + col].PossibleMask - 1)) == 0;
        }

        /// <summary>
        /// Determines whether the current Sudoku board state is valid according to Sudoku rules.
        /// </summary>
        /// <remarks>A valid board has no repeated non-empty values in any row, column, or 3x3 box. Empty
        /// cells must not be in a failed state as determined by the cell's validation logic.</remarks>
        /// <returns>true if the board contains no duplicate values in any row, column, or box, and all empty cells are in a
        /// valid state; otherwise, false.</returns>
        public bool IsValidBoard()
        {
            int[] checkRowUsed = new int[boardLen];
            int[] checkColUsed = new int[boardLen];
            int[] checkBoxUsed = new int[boardLen];

            for (int row = 0; row < boardLen; row++)
            {
                for (int col = 0; col < boardLen; col++)
                {
                    SquareCell cell = board[row * boardLen + col];

                    if (cell.Value == Constants.emptyCell)
                    {
                        if (cell.Failed())
                            return false;
                    }
                    else
                    {
                        //check for duplicates
                        int bit = SudokuHelper.BitFromChar(cell.Value);
                        int boxIdx = board[row * boardLen + col].BoxIndex;
                        bool isDuplicate = (checkRowUsed[row] & bit) != 0 ||
                                           (checkColUsed[col] & bit) != 0 ||
                                           (checkBoxUsed[boxIdx] & bit) != 0;
                        if (isDuplicate)
                            return false;
                        checkRowUsed[row] = SudokuHelper.AddBit(checkRowUsed[row],bit);
                        checkColUsed[col] = SudokuHelper.AddBit(checkColUsed[col],bit);
                        checkBoxUsed[boxIdx] = SudokuHelper.AddBit(checkBoxUsed[boxIdx], bit);
                    }
                }
            }

            return true;
        }
        
      
        /// <summary>
        /// Displays the current state of the board to the console in a formatted layout.
        /// </summary>
        /// <remarks>Use this method to visually inspect the board's contents during execution. The output
        /// includes row and column separators to enhance readability.</remarks>
        public void PrintBoard()
        {
            Console.WriteLine();
            for (int row = 0; row < boardLen; row++)
            {
                if (row % boxLen == 0 && row != 0)
                    Console.WriteLine(new string('-', (boxLen * boardLen) - 3));

                for (int col = 0; col < boardLen; col++)
                {
                    if (col % boxLen == 0 && col != 0)
                        Console.Write(" | ");

                    Console.Write(board[row * boardLen + col].Value + " ");
                }
                Console.WriteLine();
            }
        }
        /// <summary>
        /// Returns a string representation of the board by concatenating the values of all cells in row-major order.
        /// </summary>
        /// <returns>A string containing the values of the board's cells, ordered from top-left to bottom-right.</returns>
        public override string ToString()
        {
            string str = "";
            for (int row = 0; row < boardLen; row++)
                for (int col = 0; col < boardLen; col++)
                    str += board[row * boardLen + col].Value;
            return str;
        }
        //call these functions via interface to avoid accessibility issues
        void ISudokuBoard.InitializeBoard(string boardString)
        {
            InitializeBoard(boardString);
        }

        void ISudokuBoard.UpdateCounts(int r, int c, int mask, int delta)
        {
            UpdateCounts(r, c, mask, delta);
        }

    }
}