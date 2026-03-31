using MediatR;
using PlanIt.Application.Common.Interfaces.Authentication;
using PlanIt.Application.Common.Interfaces.Persistence;
using PlanIt.Application.Me.Results;

namespace PlanIt.Application.Me.Queries;

public class MeQueryHandler(
        IUserRepository userRepository
    ) : IRequestHandler<MeQuery, MeResult>
{
    public async Task<MeResult> Handle(MeQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetById(request.UserId);

        return new MeResult(
            user.Id,
            user.Username,
            user.Email,
            user.Role);
    }
}