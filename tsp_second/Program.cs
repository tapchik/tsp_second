﻿using System;
using System.Reflection.Metadata.Ecma335;

namespace tsp_second
{

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0) // if no arguments were provided
            {
                Console.WriteLine("Provide a path to a file to read the matrix as an argument to run the program! \n");
                return;
            }

            if (args.Length == 1 && args[0] == "testing")
            {
                Testing testing = new Testing();
                testing.TestRandomGenerate(4);
                testing.Test2();
                Console.WriteLine("Looks like every test ran well!\n");
                return;
            }

            BranchAndBound brchAndBnd = new BranchAndBound(args[0]);
            brchAndBnd.PrintMatrix();
            brchAndBnd.Solve();
            brchAndBnd.PrintResults();
        }
    }
}
