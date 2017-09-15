using EthSharp.ContractDevelopment;

public class SimpleTest : Contract
{
    public UInt256 Test2()
    {
        return Test() + 1;
    }

    private UInt256 Test()
    {
        return 1;
    }
}