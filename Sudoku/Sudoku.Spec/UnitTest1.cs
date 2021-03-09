using System;
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
    }

    public class Solver
    {
        public int[,] Solve(int[,] toSolve)
        {
            return (int[,])toSolve.Clone();
        }
    }
}
