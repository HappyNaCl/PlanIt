namespace PlanIt.Contracts.Attraction.Request;

public record UpdateAttractionRequest(
    string Name,
    string Description,
    int Capacity);