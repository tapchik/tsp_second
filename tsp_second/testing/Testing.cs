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
}