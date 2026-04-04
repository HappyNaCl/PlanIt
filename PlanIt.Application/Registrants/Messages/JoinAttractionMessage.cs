namespace PlanIt.Application.Registrants.Messages;

public record JoinAttractionMessage(
    Guid AttractionId,
    Guid ScheduleId,
    Guid UserId,
    string IdempotencyKey)
{
    public static string Queue => "join-attraction";
}
