using System.Text;
using MassTransit;

namespace EventHub.Contracts.SnapshotTests;

/// <summary>
/// Invariant §2.1: MassTransit derives each RabbitMQ exchange / message identity from the .NET
/// type's namespace + name (<c>urn:message:BuildingBlocks.Abstractions.Messaging:BookingConfirmedV1</c>).
/// If extraction into EventHub.Contracts renames the namespace or a type, the saga silently goes
/// dark — publishers write to an exchange nobody binds. This freezes every URN.
/// </summary>
public class MessageUrnSnapshotTests
{
    [Fact]
    public void Saga_and_integration_contract_urns_are_frozen()
    {
        var sb = new StringBuilder();
        foreach (var t in ContractTypes.All)
            sb.AppendLine($"{t.Name} => {MessageUrn.ForTypeString(t)}");

        Snapshot.Match(sb.ToString(), "message-urns");
    }

    [Fact]
    public void Contract_set_is_non_empty()
    {
        // Guards against a refactor that moves the contracts out of the scanned namespace,
        // which would make every other snapshot here vacuously pass.
        Assert.True(ContractTypes.All.Count >= 13, $"expected >=13 versioned contracts, found {ContractTypes.All.Count}");
    }
}
