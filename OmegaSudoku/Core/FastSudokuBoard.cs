using OmegaSudoku.Exceptions;
using OmegaSudoku.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OmegaSudoku.Core
{
    class FastSudokuBoard : ISudokuBoard
    {
        public SquareCell[] board;
        private int boxLen;
        private int boardLen;

        // Hidden Single Counts
        public int[] RowCounts;
        public int[] ColCounts;
        public int[] BoxCounts;

        // Global Constraints
        private int[] rowUsed;
        private int[] colUsed;
        private int[] boxUsed;

        public List<SquareCell> EmptyCells;

        private int fullmask;
        public int EmptyCount => EmptyCells.Count;

        List<SquareCell> ISudokuBoard.EmptyCells
        {
            get => EmptyCells;
            set => EmptyCells = value;
        }
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


        int ISudokuBoard.fullmask
        {
            get => fullmask;
            set => fullmask = value;
        }

        public bool HasEmptyCells => EmptyCells.Count > 0;

        // Constructor for initializing the board from a string
        /// <summary>
        /// Initializes a new instance of the FastSudokuBoard class using the specified board string representation.
        /// </summary>
        /// <remarks>The board string should contain only valid symbols for the Sudoku variant being used.
        /// The constructor validates the board size and content to ensure it can be used as a starting point for
        /// solving or further manipulation.</remarks>
        /// <param name="boardString">A string representing the initial state of the Sudoku board. The string length must be a perfect square, and
        /// the board must represent a valid Sudoku puzzle or an empty board.</param>
        /// <exception cref="InvalidPuzzleException">Thrown if the board string is null, empty, its length is not a perfect square, the board size is not valid
        /// for Sudoku, the board has too few clues (1-3), or the board does not represent a valid Sudoku puzzle.</exception>
        public FastSudokuBoard(string boardString)
        {
            if (boardString == null || boardString == "")
                throw new InvalidPuzzleException("The provided board string is null or empty.");
            double sqrtLen = Math.Sqrt(boardString.Length);
            if (sqrtLen % 1 != 0)
                throw new InvalidBoardLengthException("The provided board string length is not a perfect square.");

            Constants.boardLen = (int)sqrtLen;
            Constants.SetSymbol();
            double sqrtBoxLen = Math.Sqrt(Constants.boardLen);
            if (sqrtBoxLen % 1 != 0)
                throw new InvalidBoardLengthException("The board length does not have an integer square root, invalid for Sudoku.");

            boxLen = (int)sqrtBoxLen;
            fullmask = (1 << Constants.boardLen) - 1;

            int len = Constants.boardLen;
            this.boardLen = len;
            rowUsed = new int[len];
            colUsed = new int[len];
            boxUsed = new int[len];

            RowCounts = new int[len* len];
            ColCounts = new int[len* len];
            BoxCounts = new int[len * len];

            EmptyCells = new List<SquareCell>();
            board = new SquareCell[len * len];

            InitializeBoard(boardString);
            int clues = Constants.boardLen * Constants.boardLen - EmptyCells.Count;
            if (clues > 0 && clues < 4)// 1-3 clues takes too much time (about 1.5 minutes for 25x25)
                throw new InvalidPuzzleException("Board has too few clues (1-3). " +
                                                  "It must be empty (0) or have a valid puzzle start.");
            if (!IsValidBoard())
                throw new InvalidPuzzleException("The provided board string represents an invalid Sudoku board.");
        }

        /// <summary>
        /// Calculates the zero-based index of the box that contains the specified cell in a grid partitioned into
        /// square boxes.
        /// </summary>
        /// <remarks>The grid is assumed to be divided into square boxes of size determined by the value
        /// of boxLen. </remarks>
        /// <param name="row">The zero-based row index of the cell.</param>
        /// <param name="col">The zero-based column index of the cell.</param>
        /// <returns>The zero-based index of the box containing the cell at the specified row and column.</returns>

        /// <summary>
        /// Initializes the Sudoku board state from the specified string representation.
        /// </summary>
        /// <remarks>This method resets the board, populates all cells, and updates tracking structures
        /// for used values and empty cells. Any invalid character in the input string will result in an exception.
        /// After initialization, the board and all related state are ready for solving or further processing.</remarks>
        /// <param name="boardString">A string containing the board layout, where each character represents the value of a cell. The string must
        /// have a length equal to the total number of cells and use valid characters for the board size.</param>
        private void InitializeBoard(string boardString)
        {
            SquareCell curr = null;
            for (int i = 0; i < Constants.boardLen * Constants.boardLen; i++)
            {
                int r = i / Constants.boardLen;
                int c = i % Constants.boardLen;
                char val = boardString[i];
                curr = board[r*boardLen + c] = new SquareCell(r, c, val);

                SudokuHelper.ValidateChar(val, Constants.boardLen);

                if (val == Constants.emptyCell)
                {
                    EmptyCells.Add(curr);
                }
                else
                {
                    int bit = SudokuHelper.BitFromChar(val);
                    //add bits.
                    rowUsed[r] |= bit;
                    colUsed[c] |= bit;
                    boxUsed[curr.BoxIndex] |= bit;
                    curr.PossibleMask = 0;
                }
            }

            for (int r = 0; r < Constants.boardLen; r++)
                for (int c = 0; c < Constants.boardLen; c++)
                    InitializeNeighbors(board[r * boardLen + c]);

            InitializePossibleValues();
        }
        public void InitializePossibleValues()
        {
            foreach (var cell in EmptyCells)
            {
                cell.PossibleMask = ~(rowUsed[cell.Row] | colUsed[cell.Col] | boxUsed[cell.BoxIndex]) & fullmask;
                UpdateCounts(cell.Row, cell.Col, cell.PossibleMask, 1);
            }
        }
        /// <summary>
        /// Initializes the Neighbors property of the specified cell with all other cells in the same row, column, and
        /// box.
        /// </summary>
        /// <remarks>This method should be called before using the Neighbors property of a cell to ensure
        /// it is correctly populated. The Neighbors property will include all cells that share a row, column, or box
        /// with the specified cell, excluding the cell itself.</remarks>
        /// <param name="cell">The cell for which to initialize the Neighbors property. Cannot be null.</param>
        private void InitializeNeighbors(SquareCell cell)
        {
            List<SquareCell> l = new List<SquareCell>();
            int row = cell.Row, col = cell.Col;
            for (int k = 0; k < Constants.boardLen; k++)
            {
                if (k != col) l.Add(board[row*boardLen+ k]);
                if (k != row) l.Add(board[k * boardLen + col]);
            }
            int br = (row / boxLen) * boxLen, bc = (col / boxLen) * boxLen;
            for (int i = 0; i < boxLen; i++) for (int j = 0; j < boxLen; j++)
                {
                    int nr = br + i, nc = bc + j;
                    if (nr != row && nc != col) l.Add(board[nr * boardLen + nc]);
                }
            cell.Neighbors = l.ToArray();
        }
        /// <summary>
        /// Finds and returns the empty cell with the fewest possible values remaining, prioritizing cells with only one
        /// possible value.
        /// </summary>
        /// <remarks>This method is typically used to select the next cell to fill in constraint-based
        /// puzzles such as Sudoku. If a cell with only one possible value is found, it is returned immediately. If any
        /// empty cell has no possible values, indicating an unsolvable state, the method returns null.</remarks>
        /// <returns>The empty cell with the minimal number of possible values, or null if there are no empty cells or if an
        /// impossible cell is encountered.</returns>
        public SquareCell GetBestCell()
        {
            if (EmptyCells.Count == 0) return null;

            SquareCell bestCell = null;
            int minOptions = int.MaxValue;

            foreach (var cell in EmptyCells)
            {
                // Count is auto-updated by the setter in SquareCell
                int opts = cell.PossibleCount;
                if (opts == 0) return null; // Impossible
                if (opts == 1) return cell; // Instant Naked Single

                if (opts < minOptions)
                {
                    minOptions = opts;
                    bestCell = cell;
                }
            }
            return bestCell;
        }

        /// <summary>
        /// Attempts to place the specified number in the given cell and updates the game state accordingly.
        /// </summary>
        /// <remarks>This method updates the board state, possible values for affected cells, and tracking
        /// structures for Sudoku constraints. If the placement causes any neighboring cell to have no possible values,
        /// the method returns false to indicate an invalid move. The moves stack can be used to revert changes if
        /// needed.</remarks>
        /// <param name="row">The zero-based row index of the cell where the number is to be placed.</param>
        /// <param name="col">The zero-based column index of the cell where the number is to be placed.</param>
        /// <param name="value">The character representing the number to place in the cell. Must be a valid Sudoku digit.</param>
        /// <param name="moves">A stack used to record the moves and state changes for potential undo operations. The method pushes affected
        /// cells and their previous possible values onto this stack.</param>
        /// <returns>true if the number was successfully placed and the board remains valid; otherwise, false if the placement
        /// results in an invalid state.</returns>
        public bool PlaceNumber(int row, int col, char value, Stack<Move> moves)
        {
            SquareCell cell = board[row * boardLen + col];
            int bit = SudokuHelper.BitFromChar(value);

            moves.Push(new Move(cell, cell.PossibleMask));

            // Maintenance
            UpdateCounts(row, col, cell.PossibleMask, -1);

            //set value and update appropriate attributes
            cell.Value = value;
            cell.PossibleMask = 0;
            EmptyCells.Remove(cell);

            // Propagate
            foreach (SquareCell neighbor in cell.Neighbors)
            {
                if (neighbor.Value == Constants.emptyCell && (neighbor.PossibleMask & bit) != 0)
                {
                    moves.Push(new Move(neighbor, neighbor.PossibleMask));

                    DecrementSingleCount(neighbor.Row, neighbor.Col, value);

                    //clear bit
                    if (!neighbor.RemovePossibleValue(value))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Reverts moves from the specified stack until the number of moves equals the given checkpoint, restoring the
        /// previous state of each affected cell.
        /// </summary>
        /// <remarks>Use this method to backtrack to a previous puzzle state by undoing moves beyond a
        /// certain point. The method updates cell values, possible value masks, and related tracking structures to
        /// reflect the reverted state.</remarks>
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
                cell.PossibleMask = move.PreviousMask; 

                if (wasFilled)
                {
                    //set cell to empty.
                    cell.Value = Constants.emptyCell;
                    EmptyCells.Add(cell);

                    //add 1 to count 
                    UpdateCounts(cell.Row, cell.Col, cell.PossibleMask, 1);
                }
                else
                {
                    // For unfilled cells, restore any bits that were removed.
                    int restoredBits = SudokuHelper.ClearBit(move.PreviousMask ,oldMask);
                    if (restoredBits != 0)
                        UpdateCounts(cell.Row, cell.Col, restoredBits, 1);
                }
            }
        }

        /// <summary>
        /// Updates the candidate digit counts for the specified row, column, and box based on the provided mask and
        /// delta.
        /// </summary>
        /// <remarks>This method is used to adjust candidate digit counts when placing or
        /// removing a digit in a Sudoku puzzle. The counts for all digits represented in the mask are updated in the
        /// specified row, column, and corresponding box.</remarks>
        /// <param name="r">The zero-based index of the row to update.</param>
        /// <param name="c">The zero-based index of the column to update.</param>
        /// <param name="mask">A bitmask indicating which digits to update. Each set bit represents a digit whose count will be modified.</param>
        /// <param name="delta">The value to add to or subtract from the candidate counts. Typically 1 to increment or -1 to decrement.</param>
        private void UpdateCounts(int r, int c, int mask, int delta)
        {
            int b = board[r*boardLen+ c].BoxIndex;
            while (mask != 0)
            {
                int bit = SudokuHelper.LowestBit(mask);
                //clear lowest bit
                mask ^= bit;
                int d = SudokuHelper.BitToIndex(bit);
                RowCounts[r*boardLen+ d] += delta;
                ColCounts[c * boardLen + d] += delta;
                BoxCounts[b * boardLen + d] += delta;
            }
        }

        /// <summary>
        /// Decrements the count of the specified digit in the given row, column, and box.
        /// </summary>
        /// <param name="r">The zero-based index of the row in which to decrement the digit count.</param>
        /// <param name="c">The zero-based index of the column in which to decrement the digit count.</param>
        /// <param name="d">The zero-based index of the digit whose count is to be decremented.</param>
        private void DecrementSingleCount(int r, int c, char d)
        {
            int index = Constants.CharToIndex[d];

            RowCounts[r * boardLen + index]--;
            ColCounts[c * boardLen + index]--;
            BoxCounts[board[r*boardLen+c].BoxIndex*boardLen+ index]--;
        }

        /// <summary>
        /// Determines whether the specified value is a hidden single in the given cell of the puzzle.
        /// </summary>
        /// <remarks>A value is considered a hidden single in a cell if it appears only once as a
        /// candidate in its row, column, or box. This method can be used to identify unique placements for a value
        /// according to Sudoku solving techniques.</remarks>
        /// <param name="r">The zero-based row index of the cell to check.</param>
        /// <param name="c">The zero-based column index of the cell to check.</param>
        /// <param name="v">The value to check for as a hidden single. Must be a valid puzzle value.</param>
        /// <returns>true if the specified value is a hidden single in the given cell; otherwise, false.</returns>
        public bool IsHiddenSingle(int r, int c, char v)
        {
            int d = Constants.CharToIndex[v];
            return RowCounts[r * boardLen + d] == 1 || ColCounts[c * boardLen + d] == 1 || BoxCounts[board[r * boardLen + c].BoxIndex *boardLen+ d] == 1;
        }

        /// <summary>
        /// Determines whether the specified cell contains a naked single, meaning it has exactly one possible candidate
        /// value.
        /// </summary>
        /// <param name="r">The zero-based row index of the cell to check.</param>
        /// <param name="c">The zero-based column index of the cell to check.</param>
        /// <returns>true if the cell at the specified row and column has exactly one possible candidate value; otherwise, false.</returns>
        public bool IsNakedSingle(int r, int c) => board[r * boardLen + c].PossibleCount == 1;

    
        /// <summary>
        /// Determines whether the current board state is valid according to Sudoku rules.
        /// </summary>
        /// <remarks>A valid board has no repeated values in any row, column, or 3x3 box, and all empty
        /// cells must not be in a failed state. This method does not check whether the board is completely solved, only
        /// that it is currently valid.</remarks>
        /// <returns>true if the board contains no duplicate values in any row, column, or box, and all empty cells are in a
        /// valid state; otherwise, false.</returns>
        public bool IsValidBoard()
        {
            int[] checkRowUsed = new int[Constants.boardLen];
            int[] checkColUsed = new int[Constants.boardLen];
            int[] checkBoxUsed = new int[Constants.boardLen];

            for (int row = 0; row < Constants.boardLen; row++)
            {
                for (int col = 0; col < Constants.boardLen; col++)
                {
                    SquareCell cell = board[row * boardLen + col];

                    if (cell.Value == Constants.emptyCell)
                    {
                        if (cell.Failed())
                            return false;
                    }
                    else
                    {
                        int bit = SudokuHelper.BitFromChar(cell.Value);
                        int boxIdx = board[row*boardLen+ col].BoxIndex;
                        bool isDuplicate = (checkRowUsed[row] & bit) != 0 ||
                                           (checkColUsed[col] & bit) != 0 ||
                                           (checkBoxUsed[boxIdx] & bit) != 0;
                        if (isDuplicate)
                            return false;
                        checkRowUsed[row] = SudokuHelper.AddBit(checkRowUsed[row], bit);
                        checkColUsed[col] = SudokuHelper.AddBit(checkColUsed[col], bit);
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
        /// includes row and column separators for improved readability.</remarks>
        public void PrintBoard()
        {
            Console.WriteLine();
            for (int row = 0; row < boardLen; row++)
            {
                if (row % boxLen == 0)
                {
                    int width = (boardLen * 2) + ((boardLen / boxLen - 1) * 3) + 4;
                    Console.WriteLine(new string('-', width));

                    Console.Write("| ");
                    for (int col = 0; col < boardLen; col++)
                    {
                        if (col % boxLen == 0 && col != 0)
                            Console.Write(" | ");
                        var val = board[row * boardLen + col].Value;
                        Console.ForegroundColor = (val == Constants.emptyCell) ? ConsoleColor.DarkGray : ConsoleColor.Cyan;
                        Console.Write(val + " ");
                        Console.ResetColor();
                    }

                    Console.Write("|");
                    Console.WriteLine();
                }

                int bottomWidth = (boardLen * 2) + ((boardLen / boxLen - 1) * 3) + 4;
                Console.WriteLine(new string('-', bottomWidth));
            }
        }
        //only resets the board state if the size is the same (used for generating new puzzles from solved boards), otherwise reinitializes everything
        public void ResetBoard(string boardString)
        {
            //check for invalid board before initialization
            if (boardString == null || boardString == "" || boardString.Length == 1)
                throw new InvalidPuzzleException("The provided board string is null or empty or single.");
            double sqrtLen = Math.Sqrt(boardString.Length);
            if (sqrtLen % 1 != 0)
                throw new InvalidBoardLengthException("The provided board string length is not a perfect square.");

            Constants.boardLen = (int)sqrtLen;
            this.boardLen = Constants.boardLen;
            Constants.SetSymbol();
            double sqrtBoxLen = Math.Sqrt(boardLen);
            if (sqrtBoxLen % 1 != 0)
                throw new InvalidBoardLengthException("The board length does not have an integer square root, invalid for Sudoku.");

            boxLen = (int)sqrtBoxLen;

            int clues = boardString.Count(c => c != Constants.emptyCell);


            // Initialize Count Arrays
            Array.Clear(RowCounts);
            Array.Clear(ColCounts);
            Array.Clear(BoxCounts);

            Array.Clear(rowUsed);
            Array.Clear(colUsed);
            Array.Clear(boxUsed);

            EmptyCells.Clear();
            //reusing memory for cells, just resetting values and possible masks instead of creating new instances
            int totalCells = boardLen * boardLen;
            for (int i = 0; i < totalCells; i++)
            {
                SquareCell cell = board[i]; // Reuse memory
                char value = boardString[i];

                // Reset Cell State
                cell.Value = value;
                cell.Degree = 0; // Reset heuristic
                // Note: Neighbors are already linked, no need to touch them!

                if (value == Constants.emptyCell)
                {
                    EmptyCells.Add(cell);
                    cell.PossibleMask = fullmask;
                }
                else
                {
                    // Setup constraints for pre-filled cells
                    int bit = SudokuHelper.BitFromChar(value);
                    rowUsed[cell.Row] |= bit;
                    colUsed[cell.Col] |= bit;
                    boxUsed[cell.BoxIndex] |= bit;
                    cell.PossibleMask = bit; // Solved
                }
            }
            InitializePossibleValues();

            if (!IsValidBoard())
                throw new InvalidPuzzleException("The provided board is not valid.");
        }
        //call these functions via interface
        void ISudokuBoard.ResetBoard(string boardString)
        {
            ResetBoard(boardString);
        }
        void ISudokuBoard.InitializeBoard(string boardString)
        {
            InitializeBoard(boardString);
        }

        void ISudokuBoard.UpdateCounts(int r, int c, int mask, int delta)
        {
            UpdateCounts(r, c, mask, delta);
        }

        void ISudokuBoard.DecrementSingleCount(int r, int c, char d)
        {
            DecrementSingleCount(r, c, d);
        }

        void ISudokuBoard.InitializeNeighbors(SquareCell cell)
        {
            InitializeNeighbors(cell);
        }
    }
}