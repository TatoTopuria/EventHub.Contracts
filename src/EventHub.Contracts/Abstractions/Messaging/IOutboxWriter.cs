namespace BuildingBlocks.Abstractions.Messaging;

/// <summary>
/// Persists integration messages into service outbox as part of local transaction.
/// </summary>
public interface IOutboxWriter
{
    /// <summary>
    /// Appends an outbox message to be published later.
    /// </summary>
    Task AddAsync(OutboxMessage message, CancellationToken cancellationToken);
}