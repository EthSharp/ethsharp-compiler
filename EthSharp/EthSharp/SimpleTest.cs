using EthSharp.ContractDevelopment;

public class SimpleTest : Contract
{
    public int Test2()
    {
        return Test() + 1;
    }

    private int Test()
    {
        return 1;
    }
}