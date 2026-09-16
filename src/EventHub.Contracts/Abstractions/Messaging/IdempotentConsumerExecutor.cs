namespace BuildingBlocks.Abstractions.Messaging;

/// <summary>
/// Executes message handlers with inbox-based duplicate suppression.
/// </summary>
public sealed class IdempotentConsumerExecutor(TimeProvider timeProvider)
{
    /// <summary>
    /// Runs a message handler once per unique message id.
    /// </summary>
    public async Task<IdempotentConsumerExecutionResult> ExecuteAsync(
        string messageId,
        IInboxMessageStore inbox,
        Func<CancellationToken, Task> handler,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(messageId))
        {
            throw new ArgumentException("Message id must not be empty.", nameof(messageId));
        }

        ArgumentNullException.ThrowIfNull(inbox);
        ArgumentNullException.ThrowIfNull(handler);

        if (!await inbox.TryBeginProcessingAsync(messageId, cancellationToken))
        {
            return IdempotentConsumerExecutionResult.Duplicate;
        }

        try
        {
            await handler(cancellationToken);
            await inbox.MarkProcessedAsync(messageId, timeProvider.GetUtcNow().UtcDateTime, cancellationToken);
            return IdempotentConsumerExecutionResult.Processed;
        }
        catch
        {
            await inbox.ReleaseAsync(messageId, cancellationToken);
            throw;
        }
    }
}

public enum IdempotentConsumerExecutionResult
{
    Processed = 0,
    Duplicate = 1
}