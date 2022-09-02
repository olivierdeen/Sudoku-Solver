# Sudoku Solver

The sudoku grid to be solved should be input as follows:
0 0 6 0 2 0 3 0 0 1 0 0 6 0 5 0 0 9 0 0 9 8 0 3 4 0 0 0 0 8 9 0 2 1 0 0 7 0 0 0 0 0 0 0 8 0 0 3 7 0 8 2 0 0 0 0 2 3 0 1 5 0 0 8 0 0 2 0 6 0 0 1 0 0 5 0 9 0 6 0 0

For clarity spaces have been added between numbers, but the actual input should be without whitespaces.
Empty grids are denoted with a 0. All other grids have a value between 1 and 9.

This sudoku solver iterated local search, more specifically the hill climbing algorithm. To avoid getting stuck in a local optimum, if the score plateau's for too
long a number of random swaps is done. The sudoku's current score is calculated by summing over the number of repeated numbers in each row and column.
The goal is to get the score to 0.

## Parameters

There are several parameters that can be changed:
  - The number of iterations which indicates a score plateau.
  - The number of random swaps
  - The stop criterium.
