using System;
using System.IO;

class Node
{
    public Node next;
    public int n; // is used to generate matrix n by n
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
        this.next = null;
        this.n = n0;
        this.s = 0;
        this.s0 = 0;
        this.matrix = FillMatrix(this.n);
        this.matrix = PriceUpMainDiagonal(this.matrix);
        this.startP = GenerateAscendingArray(this.n);
        this.endP = GenerateAscendingArray(this.n);
        this.p = GeneratePathArray(this.n);
    }

    public Node(string filePath)
    {
        this.next = null;
        this.matrix = ReadMatrixFromFile(filePath);
        this.matrix = PriceUpMainDiagonal(this.matrix);
        this.n = this.matrix.GetLength(0);
        this.s = 0;
        this.s0 = 0;
        this.startP = GenerateAscendingArray(this.n);
        this.endP = GenerateAscendingArray(this.n);
        this.p = GeneratePathArray(this.n);
    }

    public Node(int n, Node next)
    {
        this.next = next;
        this.n = n;
    }

    static private int [,] ReadMatrixFromFile(String filePath) {
        string[] lines = File.ReadAllLines(filePath);
        // creating matrix with correct dimentions
        int columns = lines.Length;
        int rows = lines[0].Split('\t').Length;
        int[,] matrix = new int[rows,columns];
        // filling matrix with values from the file
        for (int i = 0; i < rows; i++)
        {
            string[] values = lines[i].Split('\t');
            for (int j = 0; j < columns; j++)
            {
                matrix[i, j] = int.Parse(values[j]);
            }
        }
        return matrix;
    }

    static private int[,] FillMatrix(int length)
    {
        Random rand = new Random();
        int[,] matrix = new int[length,length];
        int len = matrix.GetLength(0);
        for (int i = 0; i < len; i++)
            for (int j = 0; j < len; j++)
                matrix[i, j] = rand.Next(1, 51);
        return matrix;
    }

    static private int[,] PriceUpMainDiagonal(int[,] matrix)
    {
        // items on main diagonal have high value
        int high_price = int.MaxValue;
        int len = matrix.GetLength(0);
        for (int i = 0; i < len; i++)
            matrix[i, i] = high_price;
        return matrix;
    }

    static private int[] GenerateAscendingArray(int n)
    {
        int[] array = new int[n];
        for (int i = 0; i < n; i++)
        {
            array[i] = i;
        }
        return array;
    }

    static private int[] GeneratePathArray(int n)
    {
        int[] pathArray = new int[n];
        for (int i = 0; i < n; i++)
        {
            pathArray[i] = -1;
        }
        return pathArray;
    }

    public void PrintMatrix()
    {
        Console.WriteLine("Матрица: ");
        int len = this.matrix.GetLength(0);
        for (int i = 0; i < len; i++)
        {
            for (int j = 0; j < len; j++)
            {
                int value = matrix[i,j];
                int infinity = int.MaxValue - 3000;
                string symbol = (value > infinity) ? "∞" : value.ToString();
                Console.Write(symbol + "\t");
            }
            Console.WriteLine();
        }
    }
}