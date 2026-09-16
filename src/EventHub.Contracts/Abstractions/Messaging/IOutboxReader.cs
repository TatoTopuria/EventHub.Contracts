namespace BuildingBlocks.Abstractions.Messaging;

/// <summary>
/// Provides batched retrieval and completion operations for outbox publishers.
/// </summary>
public interface IOutboxReader
{
    /// <summary>
    /// Gets pending outbox messages ordered by occurrence time.
    /// </summary>
    Task<IReadOnlyCollection<OutboxMessage>> GetPendingAsync(int batchSize, CancellationToken cancellationToken);

    /// <summary>
    /// Marks a message as published.
    /// </summary>
    Task MarkProcessedAsync(Guid messageId, DateTime processedAtUtc, CancellationToken cancellationToken);
}