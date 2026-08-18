using MediatR;
using Shared.Domain;

namespace Shared.Application.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}