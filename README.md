# Sudoku Solver

The sudoku grid to be solved should be input as follows:
006020300100605009009803400008902100700000008003708200002301500800206001005090600

Empty grids are denoted with a 0. All other grids have a value between 1 and 9.

This sudoku solver iterated local search, more specifically the hill climbing algorithm. To avoid getting stuck in a local optimum, if the score plateau's for too
long a number of random swaps is done. The sudoku's current score is calculated by summing over the number of repeated numbers in each row and column.
The goal is to get the score to 0.

The completed sudoku with score for each row and columns is printed at the end of the run:

## Parameters

There are several parameters that can be changed:
  - The number of iterations which indicates a score plateau.
  - The number of random swaps
  - The stop criterium.
