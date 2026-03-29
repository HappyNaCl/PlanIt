using MediatR;
using PlanIt.Application.Me.Result;

namespace PlanIt.Application.Me.Queries;

public record MeQuery(
    Guid UserId) : IRequest<MeResult>;