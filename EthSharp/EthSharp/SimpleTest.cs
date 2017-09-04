using EthSharp.ContractDevelopment;

public class SimpleTest : Contract
{
    public UInt256 StoredData { get; private set; }

    public void Increase()
    {
        StoredData = StoredData + 1;
    }
}