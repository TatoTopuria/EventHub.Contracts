namespace BuildingBlocks.Abstractions.Messaging;

/// <summary>
/// Booking → Catalog fan-out integration event published when the canonical state of an Event
/// aggregate changes (reschedule, organizer change). Catalog uses it as the trigger for cache
/// invalidation; future consumers (Analytics, Realtime) can subscribe to the same routing key.
/// </summary>
/// <remarks>
/// Lives in BuildingBlocks rather than inside Catalog or Booking so both ends reference the same
/// shape — see audit gap G5 for the prior "consumer had a private nested record nobody published"
/// problem this contract closes. Wire format is plain JSON with the property names below, matching
/// <see cref="System.Text.Json.JsonSerializerDefaults.Web"/> (camelCase).
/// </remarks>
public sealed record EventUpdatedIntegrationEventV1(
    Guid MessageId,
    string CorrelationId,
    Guid EventId,
    DateTime OccurredOnUtc);
