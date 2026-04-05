using MediatR;
using PlanIt.Application.Registrants.Results;

namespace PlanIt.Application.Registrants.Queries.GetMyAttractions;

public record GetMyAttractionsQuery(Guid UserId) : IRequest<ICollection<MyAttractionResult>>;
