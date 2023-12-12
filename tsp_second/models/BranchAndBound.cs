using System;
using System.Collections.Generic;

class BranchAndBound
{
    public int sMin;
    int n0;
    public int[] pOpt;
    Node firstNode;

    public BranchAndBound(int n0)
    {
        this.n0 = n0;
        this.sMin = int.MaxValue;
        this.pOpt = new int[this.n0];
        this.firstNode = new Node(this.n0);
    }

    public BranchAndBound(string filePath)
    {
        this.firstNode = new Node(filePath);
        this.n0 = firstNode.n;
        this.sMin = int.MaxValue;
        this.pOpt = new int[this.n0];
    }

    public void PrintMatrix()
    {
        this.firstNode.PrintMatrix();
        Console.WriteLine();
    }

    public void PrintResults()
    {
        for (int i = 0; i < this.pOpt.Length; i++)
                Console.WriteLine($"Вершина {i}: {this.pOpt[i]}");
            Console.WriteLine($"Кратчайшее расстроение: {this.sMin}");
        Console.WriteLine();
    }

    public void Solve()
    {
        while (firstNode != null)
        {
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
                    firstNode = firstNode.parent;
                }
                else
                {
                    Tuple <int,int> forbiddenBranch = ChooseForbiddenBranch(firstNode.startP, firstNode.endP, firstNode.p, this.n0, bestBranchPositions);
                    firstNode.matrix[forbiddenBranch.Item1, forbiddenBranch.Item2] = int.MaxValue;
                    firstNode.s += (firstNode.secondMinRows[bestBranch.Item1] > firstNode.secondMinColumns[bestBranch.Item2]) ? firstNode.secondMinRows[bestBranch.Item1] : firstNode.secondMinColumns[bestBranch.Item2];
                    Node newNode = new Node(firstNode.n - 1, firstNode);
                    newNode.matrix = DownsizeMatrix(bestBranch.Item1, bestBranch.Item2, firstNode.matrix);
                    newNode.startP = DeleteNumberFromArray(bestBranch.Item1, firstNode.startP, firstNode.n);
                    newNode.endP = DeleteNumberFromArray(bestBranch.Item2, firstNode.endP, firstNode.n);
                    newNode.p = PNew(bestBranchPositions.Item1, bestBranchPositions.Item2, firstNode.p);
                    newNode.s = firstNode.s;
                    newNode.s0 = firstNode.s0;
                    firstNode = newNode;
                }
            }
            else
            {
                firstNode = firstNode.parent;
            }
        }
    }

    static private List<Tuple<int,int>> FindBiggestPath(int[] p, int n0, Tuple<int, int> bestBranchPositions, int n)
    {
        List<Tuple<int, int>> path = new List<Tuple<int, int>> { };
        List<Tuple<int, int>> bestPath = new List<Tuple<int, int>> { bestBranchPositions };
        p[bestBranchPositions.Item1] = bestBranchPositions.Item2;
        for (int i = 0; i < n0; i++)
        {
            int n1 = n0 - (n - 1);
            int i1 = i;
            while (n1 != 0)
            {
                if (p[i1] != -1)
                {
                    if (path.Count == 0)
                    {
                        path.Add(new Tuple<int, int>(i1, p[i1]));
                        i1 = p[i1];
                    }
                    else
                    {
                        path.Add(new Tuple<int, int>(i1, p[i1]));
                    }
                }
                else
                {
                    n1 = 1;
                }
                n1 -= 1;
            }
            if (path.Count > bestPath.Count)
            {
                bestPath = new List<Tuple<int, int>>(path);
            }
            path = new List<Tuple<int, int>> { };
        }
        return bestPath;
    }

    static private Tuple<int, int> ChooseForbiddenBranch(int[] startP, int[] endP, int[] p, int n0, Tuple<int, int> bestBranchPositions)
    {
        int[] pCopy = new int[n0];
        p.CopyTo(pCopy, 0);
        List<Tuple<int, int>> bestPath = FindBiggestPath(pCopy, n0, bestBranchPositions, startP.Length);
        int[] startPCopy = new int[startP.Length];
        int[] endPCopy = new int[endP.Length];
        startP.CopyTo(startPCopy, 0);
        endP.CopyTo(endPCopy, 0);
        int left = bestPath[0].Item1;
        int right = bestPath[bestPath.Count - 1].Item2;
        for (int i = 0; i < startPCopy.Length; i++)
        {
            if (startPCopy[i] == bestPath[bestPath.Count - 1].Item2)
            {
                left = i;
            }
        }
        for (int i = 0; i < endP.Length; i++)
        {
            if (endPCopy[i] == bestPath[0].Item1)
            {
                right = i;
            }
        }
        return new Tuple<int, int>(left, right);
    }

    static public int[] PNew(int row, int column, int[] array)
    {
        int len = array.Length;
        int[] new_array = new int[len];
        array.CopyTo(new_array, 0);
        new_array[row] = column;
        return new_array;
    }

    static public int[,] DownsizeMatrix(int rowToRemove, int columnToRemove, int[,] originalMatrix)
    {
        int len = originalMatrix.GetLength(0) - 1;
        int[,] result = new int[len, len];
        for (int i = 0; i < len; i++)
        {
            for (int j = 0; j < len; j++)
            {
                int cursor_i = (i < rowToRemove) ? i : i+1;
                int cursor_j = (j < columnToRemove) ? j : j+1;
                result[i,j] = originalMatrix[cursor_i, cursor_j];
            }
        }
        return result;
    }

    public static int[] DeleteNumberFromArray(int numberToRemove, int[] originalArray, int originalN)
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
                    if (w < matrix[i, secondMinRows[i]])
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
            int min = this.MinValueInRow(ref matrix, i);
            min = (min == int.MaxValue) ? 0 : min;
            s += min;
            s0 += min;
            //Reducing row
            for (int j = 0; j < n; j++)
            {
                if (matrix[i, j] != int.MaxValue)
                    matrix[i, j] -=  min;
            }
        }
    }
    
    private void ReductionColumns(ref int[,] matrix, int n, ref int s, ref int s0)
    {
        //Reducing columns of matrix M
        for (int j = 0; j < n; j++)
        {
            int min = this.MinValueInColumn(ref matrix, j);
            min = (min == int.MaxValue) ? 0 : min;
            s += min;
            s0 += min;
            //Reducing column
            for (int i = 0; i < n; i++)
            {
                if (matrix[i, j] != int.MaxValue)
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

    public int[] SecondMinRows(int[] zeroRows, int[,] matrix, int n)
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

    private int MinValueInRow(ref int[,] matrix, int row)
    {
        int min_value = int.MaxValue;
        //Finding minimum by current row
        int n = matrix.GetLength(0);
        for (int j = 0; j < n; j++)
        {
            if (min_value > matrix[row,j])
                min_value = matrix[row,j];
        }
        min_value = min_value == int.MaxValue ? 0 : min_value;
        return min_value;
    }

    private int MinValueInColumn(ref int[,] matrix, int column)
    {
        int min_value = int.MaxValue;
        //Finding minimum by current column
        int n = matrix.GetLength(0);
        for (int i = 0; i < n; i++)
        {
            if (min_value > matrix[i,column])
                min_value = matrix[i,column];
        }
        min_value = min_value == int.MaxValue ? 0 : min_value;
        return min_value;
    }

}