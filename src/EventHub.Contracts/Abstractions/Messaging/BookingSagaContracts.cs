namespace BuildingBlocks.Abstractions.Messaging;

/// <summary>
/// Published when a booking reserve flow starts and saga orchestration should begin.
/// </summary>
public sealed record ReserveSeatStartedV1(
    Guid MessageId,
    string CorrelationId,
    Guid BookingId,
    Guid EventId,
    Guid CustomerId,
    string SeatNumber,
    DateTime OccurredOnUtc);

/// <summary>
/// Requests a payment charge for a reserved booking.
/// </summary>
public sealed record ChargePaymentRequestedV1(
    Guid MessageId,
    string CorrelationId,
    Guid BookingId,
    Guid CustomerId,
    decimal Amount,
    string Currency,
    string Provider,
    string IdempotencyKey,
    IReadOnlyDictionary<string, string> Metadata,
    DateTime OccurredOnUtc);

/// <summary>
/// Reports the result of a payment attempt.
/// </summary>
public sealed record PaymentResultReceivedV1(
    Guid MessageId,
    string CorrelationId,
    Guid BookingId,
    Guid PaymentIntentId,
    string Status,
    string? ProviderReference,
    string? Error,
    DateTime OccurredOnUtc);

/// <summary>
/// Requests booking confirmation after successful payment.
/// </summary>
public sealed record ConfirmBookingRequestedV1(
    Guid MessageId,
    string CorrelationId,
    Guid BookingId,
    Guid CustomerId,
    decimal PaidAmount,
    string Currency,
    DateTime OccurredOnUtc);

/// <summary>
/// Requests booking cancellation and seat release as compensation.
/// </summary>
public sealed record CancelBookingRequestedV1(
    Guid MessageId,
    string CorrelationId,
    Guid BookingId,
    string Reason,
    DateTime OccurredOnUtc);

/// <summary>
/// Published when booking has reached confirmed state.
/// </summary>
public sealed record BookingConfirmedV1(
    Guid MessageId,
    string CorrelationId,
    Guid BookingId,
    Guid EventId,
    Guid CustomerId,
    DateTime OccurredOnUtc);

/// <summary>
/// Requests outbound customer notifications for a confirmed booking.
/// </summary>
public sealed record NotifyBookingRequestedV1(
    Guid MessageId,
    string CorrelationId,
    Guid BookingId,
    Guid CustomerId,
    string Email,
    string? Phone,
    string Template,
    IReadOnlyDictionary<string, string> Model,
    DateTime OccurredOnUtc);

/// <summary>
/// Published by Notification.Service once a booking notification (email + SMS) has been
/// successfully delivered. Signals the saga that the booking flow is fully complete.
/// </summary>
public sealed record NotificationSucceededV1(
    Guid MessageId,
    string CorrelationId,
    Guid BookingId,
    DateTime OccurredOnUtc);

/// <summary>
/// Published by Notification.Service after exhausting its retry policy. Triggers the saga's refund
/// compensation: the customer was charged but never told their booking succeeded, so the safe
/// outcome is to refund and let them retry. The <see cref="Reason"/> carries the last exception
/// message for observability; it is not part of the refund decision itself.
/// </summary>
public sealed record NotificationFailedV1(
    Guid MessageId,
    string CorrelationId,
    Guid BookingId,
    string Reason,
    DateTime OccurredOnUtc);

/// <summary>
/// Saga → Payment.Service. Requests reversal of the charge captured during the booking flow.
/// </summary>
/// <remarks>
/// Idempotent at the Payment.Service end: a second message with the same <see cref="MessageId"/>
/// is collapsed by the inbox; a second message with a *different* MessageId but the same
/// <see cref="PaymentIntentId"/> finds an already-refunded intent and republishes the existing
/// <see cref="RefundCompletedV1"/> rather than re-charging the provider.
/// </remarks>
public sealed record RefundRequestedV1(
    Guid MessageId,
    string CorrelationId,
    Guid BookingId,
    Guid PaymentIntentId,
    string Reason,
    DateTime OccurredOnUtc);

/// <summary>
/// Payment.Service → saga. Confirms the refund landed. Carries enough fields for the saga
/// to attribute the outcome to a specific booking and for analytics to count refunds per provider.
/// </summary>
public sealed record RefundCompletedV1(
    Guid MessageId,
    string CorrelationId,
    Guid BookingId,
    Guid PaymentIntentId,
    string Status,
    string? ProviderRefundReference,
    string? Error,
    DateTime OccurredOnUtc);

/// <summary>
/// Booking → saga (F5). Published by <c>BookingSagaTimeoutWorker</c> when a saga has been sitting
/// in <c>Refunding</c> longer than <c>BookingSagaPaymentOptions.RefundTimeoutSeconds</c>. The
/// state machine reacts by transitioning to a terminal <c>RefundFailed</c> state and finalizing.
/// </summary>
/// <remarks>
/// The contract carries enough context that ops can grep logs by booking id and reconstruct
/// what happened without joining against the saga state table. <see cref="ElapsedSeconds"/> is
/// the wall-clock delta between the original refund request and this expiry — useful for tuning
/// the timeout threshold over time.
/// </remarks>
public sealed record RefundTimeoutExpiredV1(
    Guid MessageId,
    string CorrelationId,
    Guid BookingId,
    Guid PaymentIntentId,
    int ElapsedSeconds,
    DateTime OccurredOnUtc);