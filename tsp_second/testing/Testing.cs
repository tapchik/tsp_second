using System;

class Testing
{

    public bool TestRandomGenerate(int n)
    {
        BranchAndBound brchAndBnd = new BranchAndBound(n);
        brchAndBnd.PrintMatrix();
        brchAndBnd.Solve();
        brchAndBnd.PrintResults();
        return true;
    }

    public bool Test2()
    {
        BranchAndBound brchAndBnd = new BranchAndBound("input/test1.txt");
        brchAndBnd.PrintMatrix();
        brchAndBnd.Solve();
        brchAndBnd.PrintResults();
        return true;
    }
}