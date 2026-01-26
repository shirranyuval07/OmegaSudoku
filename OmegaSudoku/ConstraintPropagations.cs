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
            //doesnt work well rn.
            public static bool RemoveNakedPairs(SudokuBoard board,Stack<Move> squareCells)
            {
                bool changed = false;
                Dictionary<int, List<SquareCell>> valuePairs = board.GetNakedPairs();
                foreach(var pair in valuePairs)
                {
                    int mask = pair.Key;
                    List<SquareCell> cells = pair.Value;

                    SquareCell cell1 = cells[0];
                    SquareCell cell2 = cells[1];

                    var neighbors1 = new HashSet<SquareCell>(cell1.GetNeighbors().Select(loc => board.board[loc.Item1, loc.Item2]));
                    var neighbors2 = new HashSet<SquareCell>(cell2.GetNeighbors().Select(loc => board.board[loc.Item1, loc.Item2]));

                    neighbors1.IntersectWith(neighbors2);
                    neighbors1.Remove(cell1);
                    neighbors1.Remove(cell2);

                    foreach (SquareCell peer in neighbors1)
                    {
                        if (peer.Value != Constants.emptyCell)
                            continue;
                        int beforeMask = peer.PossibleMask;
                        int newMask = SudokuHelper.ClearBit(beforeMask, mask);
                        if(newMask != beforeMask)
                        {
                            squareCells.Push(new Move(peer, beforeMask));
                            peer.PossibleMask= newMask;
                            changed = true;
                        }
                    }

                }
            
                return changed;
            }
        
            public static bool FillAllSingles(Stack<Move> squareCells, SudokuBoard board)
            {
                bool progress = true;
                while (progress)
                {
                    progress = false;
                    foreach (SquareCell cell in board.EmptyCells)
                    {
                        int bit = SudokuHelper.LowestBit(cell.PossibleMask);
                        char value = SudokuHelper.MaskToChar(bit);
                        if(board.IsNakedSingle(cell.Row, cell.Col))
                        {
                            
                            board.PlaceNumber(cell.Row, cell.Col, value, squareCells);
                            progress = true;
                            break;
                        }
                        else
                        {
                            int mask = cell.PossibleMask;
                            while(mask != 0)
                            {
                                bit = SudokuHelper.LowestBit(mask);
                                mask = SudokuHelper.ClearLowestBit(mask);
                                value = SudokuHelper.MaskToChar(bit);
                                if (board.IsHiddenSingle(cell.Row, cell.Col, value))
                                {
                                    board.PlaceNumber(cell.Row, cell.Col, value, squareCells);
                                    progress = true;
                                    break;
                                }
                            }

                    }
                    if (progress)
                            break;
                    }
                }
                return progress;
            }



        public static bool FillAffectedSingles(int row, int col, Stack<Move> squareCells, SudokuBoard board)
        {
            bool progress = false;

            Queue<SquareCell> queue = new Queue<SquareCell>();
            HashSet<SquareCell> visited = new HashSet<SquareCell>();

            // Seed with neighbors of the starting cell
            foreach (var (r, c) in board.board[row, col].GetNeighbors())
            {
                SquareCell neighbor = board.board[r, c];
                if(neighbor.Value == Constants.emptyCell && visited.Add(neighbor))
                    queue.Enqueue(neighbor);
            }

            while (queue.Count > 0)
            {
                SquareCell cell = queue.Dequeue();
                bool filled = false;
                int bit = SudokuHelper.LowestBit(cell.PossibleMask);
                char value = SudokuHelper.MaskToChar(bit);

                if(board.IsNakedSingle(cell.Row, cell.Col))
                {
                    board.PlaceNumber(cell.Row, cell.Col, value, squareCells);
                    filled = true;
                }
                else
                {
                    int mask = cell.PossibleMask;
                    while (mask != 0)
                    {
                        bit =SudokuHelper.LowestBit(mask);
                        mask = SudokuHelper.ClearLowestBit(mask);
                        value = SudokuHelper.MaskToChar(bit);
                        if(board.IsHiddenSingle(cell.Row, cell.Col, value))
                        {
                            board.PlaceNumber(cell.Row, cell.Col, value, squareCells);
                            filled = true;
                            break;
                        }
                    }
                }

                if(filled)
                {
                    progress = true;
                    // Only enqueue neighbors of newly filled cells
                    foreach (var (nr, nc) in cell.GetNeighbors())
                    {
                        SquareCell neighbor = board.board[nr, nc];
                        if(neighbor.Value == Constants.emptyCell && visited.Add(neighbor))
                            queue.Enqueue(neighbor);
                    }
                }
            }

            return progress;
        }


    }
}
