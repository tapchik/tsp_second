using System;

namespace tsp_second
{

    class Program
    {
        static void Main(string[] args)
        {
            BranchAndBound brchAndBnd;
            if (args.Length > 0) // taking first argument as a filepath
                brchAndBnd = new BranchAndBound(args[0]);
            else 
                brchAndBnd = new BranchAndBound(4);
            
            brchAndBnd.Solve();
            for (int i = 0; i < brchAndBnd.pOpt.Length; i++)
            {
                Console.WriteLine($"Вершина {i}: {brchAndBnd.pOpt[i]}");
            }
            Console.WriteLine($"Расстояние: {brchAndBnd.sMin}");
        }
    }
}
