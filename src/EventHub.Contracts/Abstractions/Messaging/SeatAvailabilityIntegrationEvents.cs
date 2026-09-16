namespace BuildingBlocks.Abstractions.Messaging;

// Booking → Realtime seat-availability integration events. Published by Booking's outbox and
// consumed by Realtime.Service over RAW RabbitMQ as Web (camelCase) JSON, dispatched by routing
// key (booking.seat.reserved / booking.seat.released). Unlike the MassTransit saga contracts, the
// wire identity here is the routing key + JSON property names — NOT the .NET type URN — so these
// live in the shared contracts package without any wire impact, letting Realtime drop its
// project reference to Booking.Service.Application.

public sealed record SeatReservedIntegrationEvent(
    Guid MessageId,
    string CorrelationId,
    Guid EventId,
    string SeatNumber,
    DateTime OccurredOnUtc);

public sealed record SeatReleasedIntegrationEvent(
    Guid MessageId,
    string CorrelationId,
    Guid EventId,
    string SeatNumber,
    DateTime OccurredOnUtc);
