using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace Sudoku
{

    class Number
    {
        public bool Gefixeerd;  // Geeft aan of een nummer gefixeerd is, d.w.z. of die al aan het begin werd meegegeven
        public int Nummer;      // Het nummer in de puzzel
        public Number(int nummer, bool gefixeerd)
        {
            this.Nummer = nummer;
            this.Gefixeerd = gefixeerd;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Number[,] grid = new Number[9, 9];
            string[,] blocks = new string[3, 3];

            // Houdt de scores bij per rij en kolom
            int[] rij_scores = new int[9];
            int[] col_scores = new int[9];

            string line = Console.ReadLine().Replace(" ", "");

            // String voor elk 3x3 block met alle getallen 1 t/m 9
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    blocks[i, j] = "123456789";
                }
            }

            // per vakje de betreffende waarde initialiseren
            for (int i = 0; i < 9; i++)
            {
                var row_ = line.Substring(0, 9); // selecteer eerste 9 getallen

                for (int j = 0; j < 9; j++)
                {
                    // Initialiseer een nieuwe Number met getal en gefixeerd = true
                    Number n = new Number(row_[j] - '0', true); // converteer van char naar int

                    grid[i, j] = n;

                    // Als een block van 3x3 dat getal bevat dan,
                    if (blocks[i / 3, j / 3].Contains(row_[j]))
                    {
                        // Verwijder dat getal uit de string. Zo houden we alleen de missende getallen van 1 t/m 9 over
                        blocks[i / 3, j / 3] = blocks[i / 3, j / 3].Replace(row_[j], ' ').Replace(" ", "");
                    }
                }

                line = line.Remove(0, 9); // Volgende regel dus eerste 9 getallen verwijderen
            }

            // Initaliseer getallen 1-9 op de lege plekken
            ProblemStates(grid, blocks, rij_scores, col_scores);

            Solve(200, grid, rij_scores, col_scores);
        }

        // Printen om te checken hoe het grid eruit ziet
        public static void Printen(Number[,] puzzle, int row1, int col1, int row2, int col2, int[] best_rij_scores, int[] best_col_scores)
        {
            for (int i = 1; i < 10; i++)
            {
                string row = "";
                for (int j = 1; j < 10; j++)
                {
                    if ((i - 1 == row1 && j - 1 == col1) || (i - 1 == row2 && j - 1 == col2))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(puzzle[i - 1, j - 1].Nummer);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write(puzzle[i - 1, j - 1].Nummer);
                    }
                    if (j % 3 == 0)
                    { Console.Write("|"); }                     // Voeg | toe

                    if (j == 9)
                    {
                        Console.Write(" " + best_rij_scores[i - 1]);
                    }
                }
                Console.WriteLine(row);

                if (i % 3 == 0)
                { Console.WriteLine("------------"); }  // Voeg --- toe
            }
            Console.WriteLine("");
            Console.WriteLine(best_col_scores[0] + "" + best_col_scores[1] + "" + best_col_scores[2] + "" + best_col_scores[3] + "" + best_col_scores[4] + "" + best_col_scores[5] + "" + best_col_scores[6] + "" + best_col_scores[7] + "" + best_col_scores[8]);
            Console.WriteLine("");
            Console.WriteLine("Total score is: " + (best_rij_scores.Sum() + best_col_scores.Sum()));
            Console.WriteLine("");
        }

        // Maakt een diepe kopie van het grid, dus niet alleen met pointers naar de oorspronkelijke array
        public static void Kopie(Number[,] grid, Number[,] kopie_grid)
        {
            // kopieer ieder getal individueel naar de nieuwe grid
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    var kopie = new Number(grid[i, j].Nummer, grid[i, j].Gefixeerd);
                    kopie_grid[i, j] = kopie;
                }
            }
        }

        public static bool Solve(int local_iteraties, Number[,] grid, int[] rij_scores, int[] col_scores)
        {
            Random rnd = new Random();
            Random Swaprnd = new Random(); // zodat de random swap random functie unbiased is vergeleken de eerder gebruikte random

            int iteraties = 0;

            // maximaal aantal iteraties dat als plateau classificeert initaliseren
            int max_iteraties = local_iteraties;

            // Reken score uit door sum van rij + col te nemen
            int score = rij_scores.Sum() + col_scores.Sum();

            while (score > 0)
            {
                // onthoud beste resultaat  zodat deze na alle swaps als best vergeleken kan worden met de huidige beste
                Number[,] best_grid = new Number[9, 9];
                Kopie(grid, best_grid);

                int best_score = score;
                int[] best_rij_scores = rij_scores.ToArray();
                int[] best_col_scores = col_scores.ToArray();

                // random 3x3 blok selecteren
                int xblok = rnd.Next(0, 3) * 3;
                int yblok = rnd.Next(0, 3) * 3;

                // met 2 loops van 0-9 kunnen we alle mogelijke swaps in het 3x3 blok bekijken
                for (int i = 0; i < 9; i++)
                {
                    if (!best_grid[xblok + (i / 3), yblok + (i % 3)].Gefixeerd)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            // als gefixeerde tegel of we vergelijken een tegel met zichzelf, skip
                            if (!best_grid[xblok + (j / 3), yblok + (j % 3)].Gefixeerd && (((xblok + (i / 3)) != (xblok + (j / 3))) || ((yblok + (i % 3)) != (yblok + (j % 3)))))
                            {
                                iteraties += 1;

                                Number[,] new_grid = new Number[9, 9];
                                Kopie(best_grid, new_grid);
                                int[] new_rij_scores = best_rij_scores.ToArray();
                                int[] new_col_scores = best_col_scores.ToArray();
                                int new_score;

                                // Wissel de gekozen getallen
                                swap(new_grid, xblok + (i / 3), yblok + (i % 3), xblok + (j / 3), yblok + (j % 3));

                                // Bereken nieuwe score door alleen de aangepaste rijen opnieuw te berekenen
                                Evaluate(new_grid, xblok + (i / 3), yblok + (i % 3), xblok + (j / 3), yblok + (j % 3), new_rij_scores, new_col_scores);
                                new_score = new_rij_scores.Sum() + new_col_scores.Sum();

                                // Als score van swap beter is dan vorige swaps, sla die op
                                if (new_score <= best_score)
                                {
                                    if (best_score < score)
                                    {
                                        local_iteraties = max_iteraties;
                                    }
                                    else
                                    {
                                        // Als score niet beter is,
                                        local_iteraties -= 1;
                                    }

                                    best_score = new_score;
                                    Kopie(new_grid, best_grid);
                                    best_rij_scores = new_rij_scores.ToArray();
                                    best_col_scores = new_col_scores.ToArray();
                                }
                                else
                                {
                                    // Als score niet beter is,
                                    local_iteraties -= 1;
                                }
                            }
                        }
                    }
                }

                // Als best score van alle mogelijke swaps in 1 blok beter is dan de huidige (best) score, vervang hem
                if (best_score <= score)
                {
                    Kopie(best_grid, grid);
                    rij_scores = best_rij_scores.ToArray();
                    col_scores = best_col_scores.ToArray();
                    score = best_score;
                }

                // als maximaal aantal iteraties verlopen zijn zonder verbetering = plateau = 2 random swaps
                if (local_iteraties < 0)
                {
                    // 2 random swaps om uit plateau te komen
                    int S = 2;

                    RandomSwap(grid, rij_scores, col_scores, S, Swaprnd);
                    score = rij_scores.Sum() + col_scores.Sum();

                    local_iteraties = max_iteraties;
                }
                else if (iteraties > 10000000) // maximaal 10 miljoen swaps proberen. Stopcriterium voor plateaus
                {
                    // Printen(grid, 0, 0, 0, 0, rij_scores, col_scores);   // Print de beste grid als deze niet opgelost kan worden
                    return false;
                }
            }

            // Print de oplossing met grid[0,0] in het rood, het rode getal heeft hier heeft geen betekenis
            Printen(grid, 0, 0, 0, 0, rij_scores, col_scores);
            return true;
        }

        // Wissel de getallen in de meegegeven posities in het grid
        public static void swap(Number[,] grid, int row1, int col1, int row2, int col2)
        {
            int swap1 = grid[row1, col1].Nummer;
            int swap2 = grid[row2, col2].Nummer;

            grid[row1, col1].Nummer = swap2;
            grid[row2, col2].Nummer = swap1;
        }


        // Wissel willkeurig getallen
        public static void RandomSwap(Number[,] grid, int[] rij_scores, int[] col_scores, int S, Random rnd)
        {
            // Loop door S swaps
            for (int i = 0; i < S; i++)
            {
                // iedere swap een random blok met random getallen gewisseld
                int xblokR = rnd.Next(0, 3) * 3;
                int yblokR = rnd.Next(0, 3) * 3;

                int row1 = rnd.Next(0, 3) + xblokR;
                int row2 = rnd.Next(0, 3) + xblokR;
                int col1 = rnd.Next(0, 3) + yblokR;
                int col2 = rnd.Next(0, 3) + yblokR;

                // als geen valid swap dan nog eens itereren
                if (grid[row1, col1].Gefixeerd || grid[row2, col2].Gefixeerd || (row1 == row2 && col1 == col2))
                {
                    i -= 1;
                }
                else
                { // swap en bereken nieuwe score
                    swap(grid, row1, col1, row2, col2);
                    Evaluate(grid, row1, col1, row2, col2, rij_scores, col_scores);
                }
            }
        }

        // Creeërt een probleemtoestand waarbij de Sudoku volledig ingevuld is met in ieder vak (3x3) alle cijfers 1 t/m 9
        public static void ProblemStates(Number[,] grid, string[,] blocks, int[] rij_scores, int[] col_scores)
        {
            // vul ontbrekende cijfers voor ieder 3x3 blok in
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid[i, j].Nummer == 0)
                    {
                        grid[i, j].Nummer = blocks[i / 3, j / 3].ToCharArray()[0] - '0';
                        grid[i, j].Gefixeerd = false;
                        blocks[i / 3, j / 3] = blocks[i / 3, j / 3].Remove(0, 1);
                    }
                }
            }

            // Scores voor iedere row/col berekenen
            for (int i = 0; i < 9; i++)
            {
                HashSet<int> rijdubbels = new HashSet<int>();
                HashSet<int> kolomdubbels = new HashSet<int>();
                for (int j = 0; j < 9; j++)
                {
                    if (rijdubbels.Contains(grid[i, j].Nummer))
                        rij_scores[i]++;
                    else
                        rijdubbels.Add(grid[i, j].Nummer);

                    if (kolomdubbels.Contains(grid[j, i].Nummer))
                        col_scores[i]++;
                    else
                        kolomdubbels.Add(grid[j, i].Nummer);
                }
            }
        }

        // Zoals in opdrachtbeschrijving staat herberekenen we de score door alleen de geswapte row/col opnieuw te berekenen
        public static void Evaluate(Number[,] grid, int row1, int col1, int row2, int col2, int[] rij_scores, int[] col_scores)
        {
            rij_scores[row1] = 0;
            col_scores[col1] = 0;


            // Bereken scores van rij1 en col1 opnieuw
            HashSet<int> rijdubbels = new HashSet<int>();
            HashSet<int> kolomdubbels = new HashSet<int>();
            for (int i = 0; i < 9; i++)
            {
                // Als het getal in de hashset staat
                if (rijdubbels.Contains(grid[row1, i].Nummer))
                {   // Score +1
                    rij_scores[row1]++;
                }
                else
                {   // Voeg het anders aan de hashset toe
                    rijdubbels.Add(grid[row1, i].Nummer);
                }


                if (kolomdubbels.Contains(grid[i, col1].Nummer))
                {
                    col_scores[col1]++;
                }
                else
                {
                    kolomdubbels.Add(grid[i, col1].Nummer);
                }
            }

            rij_scores[row2] = 0;
            col_scores[col2] = 0;

            // Bereken scores van rij2 en col2 opnieuw
            HashSet<int> rijdubbels2 = new HashSet<int>();
            HashSet<int> kolomdubbels2 = new HashSet<int>();
            for (int i = 0; i < 9; i++)
            {
                if (rijdubbels2.Contains(grid[row2, i].Nummer))
                {
                    rij_scores[row2]++;
                }
                else
                {
                    rijdubbels2.Add(grid[row2, i].Nummer);
                }

                if (kolomdubbels2.Contains(grid[i, col2].Nummer))
                {
                    col_scores[col2]++;
                }
                else
                {
                    kolomdubbels2.Add(grid[i, col2].Nummer);
                }
            }
        }
    }
}
