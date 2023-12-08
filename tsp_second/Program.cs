using System;
using System.Collections.Generic;

namespace tsp_second
{
    class Node
    {
        Node parent;
        public int n;
        public int s0;
        public int s;
        public int[,] matrix;
        public int[] startP;
        public int[] endP;
        public int[] zeroRows;
        public int[] zeroColumns;
        public int[] secondMinRows;
        public int[] secondMinColumns;
        public int[] p;

        public Node(int n0)
        {
            this.parent = null;
            this.n = n0;
            this.s = 0;
            this.s0 = 0;
            this.matrix = NodeMatrixFill(this.n);
            this.startP = StartFill(this.n);
            this.endP = EndFill(this.n);
            this.p = new int[this.n];
            PrintMatrix(this.n);
        }

        public Node(int n, Node parent)
        {
            this.parent = parent;
            this.n = n;
        }

        //TODO: Make here read from file
        private int [,] NodeMatrixFill(int n)
        {
            Random rand = new Random();
            int[,] matrixStart = new int[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i==j)
                    {
                        matrixStart[i, j] = int.MaxValue;
                    }
                    else
                    {
                        matrixStart[i, j] = rand.Next(1, 51);
                    }
                }
            }
            return matrixStart;
        }

        private int [] StartFill(int n)
        {
            int[] startP = new int[n];
            for (int i = 0; i < n; i++)
            {
                startP[i] = i;
            }
            return startP;
        }

        private int[] EndFill(int n)
        {
            int[] endP = new int[n];
            for (int i = 0; i < n; i++)
            {
                endP[i] = i;
            }
            return endP;
        }

        private void PrintMatrix(int n)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write(this.matrix[i, j] + "\t");
                }
                Console.WriteLine();
            }
        }
    }

    class BranchAndBound
    {
        public int sMin;
        int n0;
        public int[] pOpt;
        List<Node> nodes;

        public BranchAndBound(int n0)
        {
            this.n0 = n0;
            this.sMin = int.MaxValue;
            this.pOpt = new int[this.n0];
            this.nodes = new List<Node> { new Node(this.n0) };
            BranchAndBoundSolver();
        }

        void BranchAndBoundSolver()
        {
            while (nodes.Count!=0)
            {
                Node firstNode = nodes[0];
                if (firstNode.s < this.sMin)
                {
                    ReductionRows(ref firstNode.matrix, firstNode.n, ref firstNode.s, ref firstNode.s0);
                    ReductionColumns(ref firstNode.matrix, firstNode.n, ref firstNode.s, ref firstNode.s0);
                    firstNode.zeroRows = ZeroRows(firstNode.matrix, firstNode.n);
                    firstNode.secondMinRows = SecondMinRows(firstNode.zeroRows, firstNode.matrix, firstNode.n);
                    firstNode.zeroColumns = ZeroColumns(firstNode.matrix, firstNode.n);
                    firstNode.secondMinColumns = SecondMinColumns(firstNode.zeroColumns, firstNode.matrix, firstNode.n);
                }
                if (firstNode.s < this.sMin)
                {
                    Tuple<int, int> bestBranch = ChooseBestBranch(firstNode.matrix, firstNode.secondMinRows, firstNode.secondMinColumns, firstNode.n);
                    Tuple<int,int> bestBranchPositions = new Tuple<int, int>(firstNode.startP[bestBranch.Item1], firstNode.endP[bestBranch.Item2]);
                    Console.WriteLine(bestBranchPositions.Item1 + " " + bestBranchPositions.Item2);
                    if (firstNode.n == 2)
                    {
                        Tuple<int, int> secondBranch = ChooseSecondBranch(firstNode.matrix, firstNode.n, bestBranch);
                        secondBranch = new Tuple<int, int>(firstNode.startP[secondBranch.Item1], firstNode.endP[secondBranch.Item2]);
                        FindPath(this.n0, firstNode.p, bestBranchPositions, secondBranch, ref this.pOpt);
                        this.sMin = firstNode.s0;
                        nodes.RemoveAt(0);
                    }
                    else
                    {
                        //Доделать поиск цикла из вершин
                        //Tuple <int,int> forbiddenBranch = ForbiddenBranch(firstNode.startP, firstNode.p, this.n0, bestBranchPositions);
                        firstNode.matrix[bestBranch.Item2, bestBranch.Item1] = int.MaxValue;
                        firstNode.s += (firstNode.secondMinRows[bestBranch.Item1] > firstNode.secondMinColumns[bestBranch.Item2]) ? firstNode.secondMinRows[bestBranch.Item1] : firstNode.secondMinColumns[bestBranch.Item2];
                        Node newNode = new Node(firstNode.n - 1, firstNode);
                        newNode.matrix = MatrixNew(bestBranch.Item1, bestBranch.Item2, firstNode.matrix, firstNode.n);
                        newNode.startP = DeleteNumberFromArray(bestBranch.Item1, firstNode.startP, firstNode.n);
                        newNode.endP = DeleteNumberFromArray(bestBranch.Item2, firstNode.endP, firstNode.n);
                        newNode.p = PNew(bestBranchPositions.Item1, bestBranchPositions.Item2, firstNode.p, this.n0);
                        newNode.s = firstNode.s;
                        newNode.s0 = firstNode.s0;
                        nodes[0] = newNode;
                    }

                }
                else
                {
                    nodes.RemoveAt(0);
                }
            }
        }

        //private Tuple<int, int> ForbiddenBranch(int[] startP, int[] p, int n0, Tuple<int, int> bestBranchPositions, int n)
        //{
        //    List<Tuple<int, int>> allNodes = new List<Tuple<int, int>> { };
        //    p[bestBranchPositions.Item1] = bestBranchPositions.Item2;
        //    for (int i = 0; i < n0-n; i++)
        //    {
        //        int n1 = n0 - n;
        //        while (n1 != 0)
        //        {
        //            if ()
        //            n1 -= 1;
        //        }
        //    }

        //}

        public int [] PNew(int row, int column, int [] pOriginal, int n)
        {
            int[] pNew = new int[n];
            for (int i = 0; i < n; i++)
            {
                pNew[i] = pOriginal[i];
            }
            pNew[row] = column;
            return pNew;
        }

        public int[,] MatrixNew(int rowToRemove, int columnToRemove, int[,] originalArray, int originalN)
        {
            int[,] result = new int[originalN-1, originalN-1];

            for (int i = 0, j = 0; i < originalN; i++)
            {
                if (i == rowToRemove)
                    continue;

                for (int k = 0, u = 0; k < originalN; k++)
                {
                    if (k == columnToRemove)
                        continue;

                    result[j, u] = originalArray[i, k];
                    u++;
                }
                j++;
            }

            return result;
        }

        public int[] DeleteNumberFromArray(int numberToRemove, int [] originalArray, int originalN)
        {
            int[] newArray = new int[originalN - 1];
            int j = 0;
            for (int i = 0; i < originalN; i++)
            {
                if (i != numberToRemove)
                {
                    newArray[j] = originalArray[i];
                    j += 1;
                }
            }
            return newArray;
        }


        private Tuple<int,int> ChooseBestBranch(int[,] matrix, int [] secondMinRows, int[] secondMinColumns, int n)
        {
            int w = -1;
            Tuple<int, int> branch = null;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (matrix[i, j] == 0)
                    {
                        if (w < matrix[i,secondMinRows[i]])
                        {
                            w = matrix[i, secondMinRows[i]];
                            branch = new Tuple<int, int>(i, j);
                        }
                        if (w < matrix[secondMinColumns[j], j])
                        {
                            w = matrix[secondMinColumns[j], j];
                            branch = new Tuple<int, int>(i, j);
                        }
                    }
                }
            }
            return branch;
        }

        private Tuple<int, int> ChooseSecondBranch(int [,] matrix, int n, Tuple<int, int> branch)
        {
            Tuple<int, int> secondBranch = null;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (matrix[i, j] == 0)
                    {
                        if (i != branch.Item1 && j != branch.Item2 || i != branch.Item1 && j == branch.Item2 || i == branch.Item1 && j != branch.Item2)
                        {
                            secondBranch = new Tuple<int, int>(i, j);
                        }
                    }
                }
            }
            return secondBranch;
        }

        private void FindPath(int n0, int[] p, Tuple<int,int> bestBranch, Tuple<int, int> secondBranch, ref int [] pOpt)
        {
            for (int i = 0; i < n0; i++)
            {
                pOpt[i] = p[i];
            }
            pOpt[bestBranch.Item1] = bestBranch.Item2;
            pOpt[secondBranch.Item1] = secondBranch.Item2;
        }

        private void ReductionRows(ref int[,] matrix, int n, ref int s, ref int s0)
        {
            //Reducing rows of matrix M
            for (int i = 0; i < n; i++)
            {
                int min = int.MaxValue;
                //Finding minimum by current row
                for (int j = 0; j < n; j++)
                {
                    if (min > matrix[i,j])
                    {
                        min = matrix[i,j];
                    }
                }
                min = min == int.MaxValue ? 0 : min;
                s += min;
                s0 += min;
                //Reducing row
                for (int j = 0; j < n; j++)
                {
                    if (matrix[i, j] != int.MaxValue)
                        matrix[i, j] = matrix[i, j] - min;
                }
            }
        }
        
        private void ReductionColumns(ref int[,] matrix, int n, ref int s, ref int s0)
        {
            //Reducing columns of matrix M
            for (int j = 0; j < n; j++)
            {
                int min = int.MaxValue;
                //Finding minimum by current column
                for (int i = 0; i < n; i++)
                {
                    if (min > matrix[i, j])
                    {
                        min = matrix[i, j];
                    }
                }
                min = min == int.MaxValue ? 0 : min;
                s += min;
                s0 += min;
                //Reducing column
                for (int i = 0; i < n; i++)
                {
                    if (matrix[i, j]!=int.MaxValue)
                        matrix[i, j] = matrix[i, j] - min;
                }
            }
        }

        public int[] ZeroRows(int[,] matrix, int n)
        {
            int[] zeroRows = new int[n];
            for (int i = 0; i < n; i++)
            {
                //Finding zero by current row
                for (int j = 0; j < n; j++)
                {
                    if (matrix[i,j]==0)
                    {
                        zeroRows[i] = j;
                        j = n;
                    }
                }
            }
            return zeroRows;
        }

        public int[] ZeroColumns(int[,] matrix, int n)
        {
            int[] zeroColumns = new int[n];
            for (int j = 0; j < n; j++)
            {
                //Finding zero by current column
                for (int i = 0; i < n; i++)
                {
                    if (matrix[i, j] == 0)
                    {
                        zeroColumns[j] = i;
                        i = n;
                    }
                }
            }
            return zeroColumns;
        }

        public int[] SecondMinRows(int [] zeroRows, int[,] matrix, int n)
        {
            int[] secondMinRows = new int[n];
            for (int i = 0; i < n; i++)
            {
                int min = int.MaxValue;
                int minPosition = zeroRows[i];
                for (int j = 0; j < n; j++)
                {
                    if (j != zeroRows[i] && matrix[i, j] < min)
                    {
                        min = matrix[i, j];
                        minPosition = j;
                    }
                }
                secondMinRows[i] = minPosition;
            }
            return secondMinRows;
        }

        public int[] SecondMinColumns(int[] zeroColumns, int[,] matrix, int n)
        {
            int[] secondMinColumns = new int[n];
            for (int j = 0; j < n; j++)
            {
                int min = int.MaxValue;
                int minPosition = zeroColumns[j];
                for (int i = 0; i < n; i++)
                {
                    if (i != zeroColumns[j] && matrix[i, j] < min)
                    {
                        min = matrix[i, j];
                        minPosition = i;
                    }
                }
                secondMinColumns[j] = minPosition;
            }
            return secondMinColumns;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            BranchAndBound brchAndBnd = new BranchAndBound(4);
            for (int i = 0; i < brchAndBnd.pOpt.Length; i++)
            {
                Console.WriteLine($" Вершина {i}: {brchAndBnd.pOpt[i]}");
            }
            Console.WriteLine($"Расстояние: {brchAndBnd.sMin}");
        }
    }
}
