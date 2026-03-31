namespace PlanIt.Contracts.Attraction.Request;

public record CreateAttractionRequest(
    string Name,
    string Description,
    int Capacity);
