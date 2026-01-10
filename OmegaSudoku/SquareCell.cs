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

        private List<int> possibleValues;

        private int value { get; set; }
        public SquareCell(int row, int col, int value)
        {
            this.row = row;
            this.col = col;
            this.value = value;
            this.possibleValues = new List<int>();
        }
        public void SetPossibleValues(List<int> values)
        {
            this.possibleValues = values;
        }
        public List<int> PossibleValues
        {
            get { return possibleValues; }
            set { possibleValues = value; }
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
        public int Value
        {
            get { return value; }
            set { this.value = value; }
        }
        public override string ToString()
        {
            return "(" + row + "," + col + ")" +" And its value is: "+value;
        }
        public bool Failed()
        {
            return possibleValues.Count == 0;
        }
    }
}