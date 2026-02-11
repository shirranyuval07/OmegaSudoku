The project is a Generic Sudoku Solver. Run by pressing the green arrow. In the program file the code is run through the UI class. It can do all sorts of things such as:
1. solving a given sudoku.
2. generating a sudoku based on a given board size.
3. Put a board into a file.
4. reading from the file.
5. Show the check and runtime for 50k hard sudoku (9x9)
## built with
  *in visual studio 2026
  
  *C#
## Installation
  *click on the <> code
  
  *Either Copy link and the:
  
   open visual studio 2022/2026
   
   Clone a repository
   
   insert the copied link into Repository Location
   
   click clone
   
  *Download Zip
  
   Extract zip and open Visual Studio 2022/2026
   
   at the right youll have open and click on the Extracted file
   
   *run as mentioned at the start.
   
## Usage
  Solve sudoku boards, save into text files valid boards, solve saved boards.
## Project Structure
  tests
  
	OmegaSudoku.Tests.Unit
	
  OmegaSudoku
  
	Core
	
	Exceptions
	
	FilesData
	
	Logic
	
	UI
	
	Utils
	
	program.cs -> run the program from here
  
## Author
  Yuval Shir-Ran.
## Purpose
  Solving sudoku boards.

## How it works:
The solver revolves around a simple backtracking algorithem including solving naked+hidden singles,
getting the best cell to "Guess" and degree heuristic to keep a count of how many cells it affects.
backtracking is a brute force search to try and find the right solution to the board following certain constraints:
each cell in a row/col/box must be filled with 1-9 (or more if the board is bigger than 9x9) with no duplicates.
you pick an empty cell and try a value that can be placed there and continue this path,
if that path is wrong (duplicates are found or there are no more options to this cell)
backtrack to the last stage.
naked singles are cells with only one possible value according to their according row/col/box (all are filled
except this). hidden singles are cells with only one possible value because their surrounding row/col/box are containing that same value
thus this is considered hidden and you can fill that cell.
the solver find these cells and fills them before backtracking while progress is made because filling a cell 
may result in the finding of another single.
Best cell to guess is picking the cell with the least amount of possible values so it is more likely to be a correct guess.
going over less possibilities.
Degree heuristic is to find the tie breaker between two cells that have the same amount of possible values.
it tells the GetBestCell function to pick the cell that affects the highest number of empty cells.

If the board is solved it will show the result. if theres an error it will print the fitting error.
## Example
Inserting the board is done in a string format:

000000000000600000070000200050000000000000700000100000001000068008500010090000000

this will result with this result:

----------------------------
| 0 0 0  | 0 0 0  | 0 0 0 |
| 0 0 0  | 6 0 0  | 0 0 0 |
| 0 7 0  | 0 0 0  | 2 0 0 |
----------------------------

| 0 5 0  | 0 0 0  | 0 0 0 |
| 0 0 0  | 0 0 0  | 7 0 0 |
| 0 0 0  | 1 0 0  | 0 0 0 |
----------------------------

| 0 0 1  | 0 0 0  | 0 6 8 |
| 0 0 8  | 5 0 0  | 0 1 0 |
| 0 9 0  | 0 0 0  | 0 0 0 |
----------------------------
after Solving:

----------------------------
| 4 6 9  | 8 5 2  | 1 7 3 |
| 2 1 3  | 6 7 9  | 8 4 5 |
| 8 7 5  | 3 4 1  | 2 9 6 |
----------------------------

| 1 5 2  | 7 3 6  | 9 8 4 |
| 6 8 4  | 2 9 5  | 7 3 1 |
| 9 3 7  | 1 8 4  | 6 5 2 |
----------------------------

| 7 4 1  | 9 2 3  | 5 6 8 |
| 3 2 8  | 5 6 7  | 4 1 9 |
| 5 9 6  | 4 1 8  | 3 2 7 |
----------------------------
00:00.033018
give sudoku board

as you can see it prints the board in a normal format (meaning not in a string but a grid)
it shows the solved board and prints the runtime:

## Unsolvable board Example:

123000000456000000000090000000000000000000000000000000000000000000000000000000000

81 is the input length

0.9135802469135802

----------------------------
| 1 2 3  | 0 0 0  | 0 0 0 |
| 4 5 6  | 0 0 0  | 0 0 0 |
| 0 0 0  | 0 9 0  | 0 0 0 |
----------------------------

| 0 0 0  | 0 0 0  | 0 0 0 |
| 0 0 0  | 0 0 0  | 0 0 0 |
| 0 0 0  | 0 0 0  | 0 0 0 |
----------------------------

| 0 0 0  | 0 0 0  | 0 0 0 |
| 0 0 0  | 0 0 0  | 0 0 0 |
| 0 0 0  | 0 0 0  | 0 0 0 |
----------------------------

An error occurred: Sudoku Exception found: Board is unsolvable  Sudoku could not be solved
give sudoku board