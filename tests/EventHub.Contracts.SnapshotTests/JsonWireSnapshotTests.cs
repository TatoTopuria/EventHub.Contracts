using System.Text.Json;
using BuildingBlocks.Abstractions.Messaging;

namespace EventHub.Contracts.SnapshotTests;

/// <summary>
/// Invariant §2.4: <see cref="EventUpdatedIntegrationEventV1"/> travels over raw RabbitMQ as plain
/// JSON with Web (camelCase) naming — Catalog's consumer deserializes exactly these property names.
/// This freezes the serialized key set so a casing/naming change is caught before it breaks
/// cache invalidation across the Booking → Catalog fan-out.
/// </summary>
public class JsonWireSnapshotTests
{
    [Fact]
    public void EventUpdated_integration_event_json_keys_are_frozen()
    {
        var sample = new EventUpdatedIntegrationEventV1(
            MessageId: Guid.Empty,
            CorrelationId: "corr",
            EventId: Guid.Empty,
            OccurredOnUtc: DateTime.UnixEpoch);

        var json = JsonSerializer.Serialize(sample, new JsonSerializerOptions(JsonSerializerDefaults.Web));

        using var doc = JsonDocument.Parse(json);
        var keys = doc.RootElement.EnumerateObject()
            .Select(p => p.Name)
            .OrderBy(k => k, StringComparer.Ordinal);

        Snapshot.Match(string.Join("\n", keys), "eventupdated-json-keys");
    }
}
