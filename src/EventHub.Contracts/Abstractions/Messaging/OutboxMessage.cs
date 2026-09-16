namespace BuildingBlocks.Abstractions.Messaging;

/// <summary>
/// Represents a persisted integration message waiting for transport publication.
/// </summary>
public sealed record OutboxMessage(
    Guid Id,
    string Type,
    string RoutingKey,
    string CorrelationId,
    string Payload,
    DateTime OccurredOnUtc,
    DateTime? ProcessedOnUtc = null);