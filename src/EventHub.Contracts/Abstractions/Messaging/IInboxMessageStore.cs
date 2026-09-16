namespace BuildingBlocks.Abstractions.Messaging;

/// <summary>
/// Coordinates idempotent processing lifecycle for consumed integration messages.
/// </summary>
public interface IInboxMessageStore
{
    /// <summary>
    /// Attempts to acquire processing ownership for a message.
    /// </summary>
    Task<bool> TryBeginProcessingAsync(string messageId, CancellationToken cancellationToken);

    /// <summary>
    /// Marks a message as successfully processed.
    /// </summary>
    Task MarkProcessedAsync(string messageId, DateTime processedAtUtc, CancellationToken cancellationToken);

    /// <summary>
    /// Releases in-flight ownership for retry when processing fails.
    /// </summary>
    Task ReleaseAsync(string messageId, CancellationToken cancellationToken);
}