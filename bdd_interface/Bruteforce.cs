using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bdd_interface
{
    class Bruteforce
    {
        public string[] solves { get; set; }
        public void initSolvesFullBrutforce(int tableSize)
        {
            List<string> solves = new List<string>();
            bool[][] vars = new bool[tableSize][];
            for (int i = 0; i < tableSize; i++)
            {
                vars[i] = new bool[tableSize];
            }
            do
            {
                nextStateFullBrutforce(ref vars, tableSize);
                if (checkIsTrueSolve(ref vars, tableSize))
                {
                    solves.Add(toMyString(ref vars, tableSize));
                }
            } while (!isOnlyNulls(ref vars));
            this.solves = solves.ToArray();
        }
        public void initSolvesOptimizeBrutforce(int tableSize)
        {

            List<string> solves = new List<string>();
            bool[][] vars = getNewTable(tableSize);
            int[] projectionOfTable = getProjectionOfTable(tableSize);
            while (nextPermutation(projectionOfTable))
            {
                setVarsByProjection(ref vars, projectionOfTable, tableSize);
                if (checkIsTrueSolve(ref vars, tableSize))
                {
                    solves.Add(toMyString(ref vars, tableSize));
                }
            }
            this.solves = solves.ToArray();

        }

        private bool[][] getNewTable(int tableSize)
        {
            bool[][] vars = new bool[tableSize][];
            for (int i = 0; i < tableSize; i++)
            {
                vars[i] = new bool[tableSize];
            }
            return vars;
        }

        private void setVarsByProjection(ref bool[][] vars, int[] projectionOfTable, int tableSize)
        {
            vars = getNewTable(tableSize);
            for (int i = 0; i < tableSize; i++)
            {
                vars[i][projectionOfTable[i]] = true;
            }
        }


        private int[] getProjectionOfTable(int tableSize)
        {
            int[] projection = new int[tableSize];
            for (int i = 0; i < tableSize; i++)
            {
                projection[i] = i;
            }
            return projection;
        }

        public bool nextPermutation(int[] perm)
        {
            int n = perm.Length;
            int k = -1;
            for (int i = 1; i < n; i++)
                if (perm[i - 1] < perm[i])
                    k = i - 1;
            if (k == -1)
            {
                for (int i = 0; i < n; i++)
                    perm[i] = i;
                return false;
            }
            int l = k + 1;
            for (int i = l; i < n; i++)
                if (perm[k] < perm[i])
                    l = i;
            int t = perm[k];
            perm[k] = perm[l];
            perm[l] = t;
            Array.Reverse(perm, k + 1, perm.Length - (k + 1));
            return true;
        }
        string toMyString(ref bool[][] vars, int tableSize)
        {
            StringBuilder sb = new StringBuilder(tableSize * tableSize);
            for (int i = 0; i < tableSize; i++)
            {
                for (int j = 0; j < tableSize; j++)
                {
                    if (vars[i][j] == true)
                        sb.Append('1');
                    else
                        sb.Append('0');
                }
            }
            return sb.ToString();
        }
        private void nextStateFullBrutforce(ref bool[][] vars, int tableSize)
        {
            int iter = 0;
            while (iter < tableSize * tableSize)
            {
                if (vars[iter / tableSize][iter % tableSize] == false)
                {
                    vars[iter / tableSize][iter % tableSize] = true;
                    return;
                }
                else
                {
                    vars[iter / tableSize][iter % tableSize] = false;
                    iter++;
                }
            }
        }

        private bool isOnlyNulls(ref bool[][] vars)
        {
            foreach (var i in vars)
            {
                foreach (var j in i)
                {
                    if (j == true)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        bool checkIsTrueSolve(ref bool[][] vars, int tableSize)
        {
            bool result = true;
            for (int i = 0; i < tableSize; i++)
            {
                result &= getGorizontal(i, tableSize, ref vars);
                result &= getVertical(i, tableSize, ref vars);
                result &= getDiagonalNW1(i, tableSize, ref vars);
                result &= getDiagonalNW2(i, tableSize, ref vars);
                result &= getDiagonalNE1(i, tableSize, ref vars);
                result &= getDiagonalNE2(i, tableSize, ref vars);
            }
            return result;
        }

        private bool getGorizontalConj(int numberOfRow, int numberWithNot, int tableSize, ref bool[][] vars)
        {
            bool result = true;
            for (int i = 0; i < tableSize; i++)
            {
                if (i != numberWithNot)
                {
                    result &= !vars[numberOfRow][i];
                }
                else
                {
                    result &= vars[numberOfRow][i];
                }
            }
            return result;
        }
        private bool getGorizontal(int numberOfRow, int tableSize, ref bool[][] vars)
        {
            bool result = false;
            for (int numberWithNot = 0; numberWithNot < tableSize; numberWithNot++)
            {
                result |= getGorizontalConj(numberOfRow, numberWithNot, tableSize, ref vars);
            }
            return result;
        }
        private bool getVerticalConj(int numberOfColumn, int numberWithNot, int tableSize, ref bool[][] vars)
        {
            bool result = true;
            for (int i = 0; i < tableSize; i++)
            {
                if (i != numberWithNot)
                {
                    result &= !vars[i][numberOfColumn];
                }
                else
                {
                    result &= vars[i][numberOfColumn];
                }
            }
            return result;
        }
        private bool getVertical(int numberOfColumn, int tableSize, ref bool[][] vars)
        {
            bool result = false;
            for (int numberWithNot = 0; numberWithNot < tableSize; numberWithNot++)
            {
                result |= getVerticalConj(numberOfColumn, numberWithNot, tableSize, ref vars);
            }
            return result;
        }

        private bool getDiagonalNWConj1(int numberOfDiagonal, int numberWithNot, int tableSize, ref bool[][] vars)
        {
            int len = tableSize - numberOfDiagonal;
            bool root = true;
            for (int j = 0; j < len; j++)
            {
                if (j != numberWithNot)
                    root &= !(vars[j][j + numberOfDiagonal]);
                else
                    root &= (vars[j][j + numberOfDiagonal]);

            }
            return root;
        }
        private bool getDiagonalNWConj2(int numberOfDiagonal, int numberWithNot, int tableSize, ref bool[][] vars)
        {
            int len = tableSize - numberOfDiagonal;
            bool root = true;
            for (int j = 0; j < len; j++)
            {
                if (j != numberWithNot)
                    root &= !(vars[j + numberOfDiagonal][j]);
                else
                    root &= (vars[j + numberOfDiagonal][j]);

            }
            return root;
        }
        private bool getDiagonalNW1(int numberOfDiagonal, int tableSize, ref bool[][] vars)
        {
            bool root = false;
            for (int k = -1; k < tableSize; k++)
            {
                bool diagonalNWConj = getDiagonalNWConj1(numberOfDiagonal, k, tableSize, ref vars);
                root |= diagonalNWConj;
            }
            return root;
        }
        private bool getDiagonalNW2(int numberOfDiagonal, int tableSize, ref bool[][] vars)
        {
            bool root = false;
            for (int k = -1; k < tableSize; k++)
            {
                bool diagonalNWConj = getDiagonalNWConj2(numberOfDiagonal, k, tableSize, ref vars);
                root |= diagonalNWConj;
            }
            return root;
        }

        private bool getDiagonalNEConj1(int numberOfDiagonal, int numberWithNot, int tableSize, ref bool[][] vars)
        {
            int len = tableSize - numberOfDiagonal;
            bool root = true;
            for (int j = 0; j < len; j++)
            {
                if (j != numberWithNot)
                    root &= !(vars[j][tableSize - 1 - j - numberOfDiagonal]);
                else
                    root &= (vars[j][tableSize - 1 - j - numberOfDiagonal]);

            }
            return root;
        }
        private bool getDiagonalNEConj2(int numberOfDiagonal, int numberWithNot, int tableSize, ref bool[][] vars)
        {
            int len = tableSize - numberOfDiagonal;
            bool root = true;
            for (int j = 0; j < len; j++)
            {
                if (j != numberWithNot)
                    root &= !(vars[j + numberOfDiagonal][tableSize - 1 - j]);
                else
                    root &= (vars[j + numberOfDiagonal][tableSize - 1 - j]);

            }
            return root;
        }
        private bool getDiagonalNE1(int numberOfDiagonal, int tableSize, ref bool[][] vars)
        {
            bool root = false;
            for (int k = -1; k < tableSize; k++)
            {
                bool diagonalNEConj = getDiagonalNEConj1(numberOfDiagonal, k, tableSize, ref vars);
                root |= diagonalNEConj;
            }
            return root;
        }
        private bool getDiagonalNE2(int numberOfDiagonal, int tableSize, ref bool[][] vars)
        {
            bool root = false;
            for (int k = -1; k < tableSize; k++)
            {
                bool diagonalNEConj = getDiagonalNEConj2(numberOfDiagonal, k, tableSize, ref vars);
                root |= diagonalNEConj;
            }
            return root;
        }
    }
}
