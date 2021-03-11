using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace Sudoku.Spec
{
    public class SodokuSolverWill
    {
        private int[,] _completed1 = new int[9, 9]
        {
            {5, 3, 4, 6, 7, 8, 9, 1, 2},
            {6, 7, 2, 1, 9, 5, 3, 4, 8},
            {1, 9, 8, 3, 4, 2, 5, 6, 7},
            {8, 5, 9, 7, 6, 1, 4, 2, 3},
            {4, 2, 6, 8, 5, 3, 7, 9, 1},
            {7, 1, 3, 9, 2, 4, 8, 5, 6},
            {9, 6, 1, 5, 3, 7, 2, 8, 4},
            {2, 8, 7, 4, 1, 9, 6, 3, 5},
            {3, 4, 5, 2, 8, 6, 1, 7, 9}
        };

        [Fact]
        public void JustReturnACompletedSudoku()
        {
            var ut = new Solver();
            Assert.Equal( _completed1, ut.Solve(_completed1));
        }

        [Fact]
        public void KnockOutValue_knocksOutASingleValue()
        {
            var completedWithout1 = new int[9, 9]
            {
                {5, 3, 4, 6, 7, 8, 9, 1, 2},
                {6, 7, 2, 1, 9, 5, 3, 4, 8},
                {1, 9, 8, 3, 4, 2, 5, 6, 7},
                {8, 5, 9, 7, 6, 1, 4, 2, 3},
                {4, 2, 6, 8, 5, 3, 7, 9, 1},
                {7, 1, 3, 9, 2, 4, 8, 5, 6},
                {9, 6, 1, 5, 3, 7, 2, 8, 4},
                {2, 8, 7, 4, 1, 9, 6, 3, 5},
                {3, 4, 5, 2, 8, 6, 1, 7, 0}
            };

            Assert.Equal(completedWithout1, KnockOutValue(_completed1,8,8));
        }

        private int[,] KnockOutValue(int[,] completed, int x, int y)
        {
            var copy = (int[,])completed.Clone();
            copy[y, x] = 0;
            return copy;
        }

        [Fact]
        public void Solve1MissingSpace()
        {
            var sudoku = KnockOutValue(_completed1, 8, 8);
            var ut = new Solver();
            Assert.Equal(_completed1, ut.Solve(sudoku));
        }

        [Fact]
        public void SolveAllMissing1Space()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    var sudoku = KnockOutValue(_completed1, i, j);
                    var ut = new Solver();
                    Assert.Equal(_completed1, ut.Solve(sudoku));
                }
            }
        }

        [Fact]
        public void Solve2MissingInRow()
        {
            var sudokuCopy = (int[,])_completed1.Clone();
            sudokuCopy[0, 8] = 0;
            sudokuCopy[8, 8] = 0;

            var solver = new Solver();
            Assert.Equal(_completed1, solver.Solve(sudokuCopy) );
        }

        [Fact]
        public void Solve2MissingInColumn()
        {
            var sudokuCopy = (int[,])_completed1.Clone();
            sudokuCopy[8, 0] = 0;
            sudokuCopy[8, 8] = 0;

            var solver = new Solver();
            Assert.Equal(_completed1, solver.Solve(sudokuCopy));
        }
    }

    public class Solver
    {
        // we know we have one blank space
        public int[,] Solve(int[,] toSolve)
        {
            var blanks = FindBlanks(toSolve);
            return Solve(toSolve, blanks).Result;

        }

        public SolveResult Solve(int[,] toSolve, List<Cell> blanks)
        {
            if (!blanks.Any()) return new SolveResult(SudokuState.Solved, toSolve);

            var blank = blanks.First();
            var validCandidates = FindValidCandidatesForSingleBlankSpace(toSolve, blank);

            if (!validCandidates.Any()) return new SolveResult(SudokuState.Invalid, null);
            foreach (var validCandidate in validCandidates)
            {
                var wip = (int[,])toSolve.Clone();
                wip[blank.Y, blank.X] = validCandidate;

                var nextBlanks = FindBlanks(wip);
                var result = Solve(wip, nextBlanks);
                if (result.Solved == SudokuState.Solved) return result;
            }

            return new SolveResult(SudokuState.Invalid, null);

        }

        private List<int> FindValidCandidatesForSingleBlankSpace(int[,] toSolve, Cell blankSpace)
        {
            var x = blankSpace.X;
            var valuesInColumn = GetNumbersInSudokuByColumn(toSolve, x);
            var missingNumbers = GetMissingNumbers(valuesInColumn.ToList());
            return missingNumbers;
        }

        private List<int> GetMissingNumbers(List<int> valuesInColumn)
        {
            var all = Enumerable.Range(1, 9);
            var missing = all.Where(i => !valuesInColumn.Contains(i));
            return missing.ToList();
        }

        private IEnumerable<int> GetNumbersInSudokuByColumn(int[,] toSolve, int x)
        {
            for (int i = 0; i < 9; i++)
            {
                var val = toSolve[i, x];
                if (val == 0)
                {
                    continue;
                }

                yield return val;
            }
        }

        private List<Cell> FindBlanks(int[,] toSolve)
        {
            var result = new List<Cell>();
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    if (toSolve[y, x] == 0)
                    {
                        result.Add(new Cell(x, y));
                    }
                }
            }

            return result;
        }
    }

    public enum SudokuState
    {
        Solved,
        Invalid
    }

    public class SolveResult
    {
        public SudokuState Solved { get; }
        public int[,] Result { get; }

        public SolveResult(SudokuState solved, int[,] result)
        {
            Solved = solved;
            Result = result;
        }
    }

    public class Cell
    {
        public int X { get; }
        public int Y { get; }

        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
