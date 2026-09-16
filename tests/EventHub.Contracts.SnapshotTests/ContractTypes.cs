using System.Reflection;
using BuildingBlocks.Abstractions.Messaging;

namespace EventHub.Contracts.SnapshotTests;

/// <summary>
/// Shared discovery of the integration-contract record types. A "contract" is a public,
/// non-abstract class in the <c>BuildingBlocks.Abstractions.Messaging</c> namespace whose name
/// ends in a version suffix (<c>V1</c>, <c>V2</c>, ...). This deliberately excludes the messaging
/// abstractions (IInboxMessageStore, IOutboxReader, OutboxMessage, IdempotentConsumerExecutor),
/// which are not on the wire.
/// </summary>
internal static class ContractTypes
{
    private const string ContractNamespace = "BuildingBlocks.Abstractions.Messaging";

    public static IReadOnlyList<Type> All { get; } =
        typeof(BookingConfirmedV1).Assembly
            .GetTypes()
            .Where(t => t.Namespace == ContractNamespace
                        && t is { IsClass: true, IsAbstract: false }
                        && IsVersionedContract(t.Name))
            .OrderBy(t => t.Name, StringComparer.Ordinal)
            .ToArray();

    private static bool IsVersionedContract(string name)
    {
        // ...V1 / ...V2 etc. Last char digit, preceded by 'V'.
        var i = name.Length - 1;
        if (i < 1 || !char.IsDigit(name[i])) return false;
        while (i > 0 && char.IsDigit(name[i])) i--;
        return name[i] == 'V';
    }

    /// <summary>Primary (longest) constructor — the positional record's wire order.</summary>
    public static ConstructorInfo PrimaryConstructor(Type t) =>
        t.GetConstructors().OrderByDescending(c => c.GetParameters().Length).First();

    public static string Pretty(Type t)
    {
        if (t.IsGenericType)
        {
            var name = t.Name[..t.Name.IndexOf('`')];
            var args = string.Join(", ", t.GetGenericArguments().Select(Pretty));
            return $"{name}<{args}>";
        }
        return t.Name;
    }
}
