using System.Collections.Generic;
using EthSharp.ContractDevelopment;

public class OwnedContract : Contract
{
    public Address Owner { get; private set; }

    public OwnedContract()
    {
        Owner = EvmContext.Message.Sender;
    }
}

public class Fundraiser : OwnedContract
{
    public UInt256 AllTimeEtherCounter { get; private set; }
    public UInt256 DepositDeadline { get; private set; }
    public Dictionary<Address, UInt256> Contributors { get; private set; }

    public Fundraiser(UInt256 deadline)
    {
        DepositDeadline = deadline;
    }

    [Payable]
    public bool Deposit()
    {
        AllTimeEtherCounter += EvmContext.Message.Value;
        Contributors[EvmContext.Message.Sender] += EvmContext.Message.Value;
        return true;
    }

    public bool Withdraw()
    {
        if (EvmContext.Message.Sender != Owner)
            throw new EvmException();

        EvmContext.Message.Sender.Transfer(this.Balance);
        return true;
    }
}