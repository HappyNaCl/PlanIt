using MediatR;
using PlanIt.Application.Me.Results;

namespace PlanIt.Application.Me.Queries;

public record MeQuery(
    Guid UserId) : IRequest<MeResult>;