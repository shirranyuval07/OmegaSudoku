using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaSudoku
{
    class SquareCell
    {
        private int row { get; set; }
        private int col { get; set; }
        public SquareCell(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
        public int Row
        {
            get { return row; }
            set { row = value; }
        }
        public int Col
        {
            get { return col; }
            set { col = value; }
        }
        public override string ToString()
        {
            return "(" + row + "," + col + ")";
        }
    }
}