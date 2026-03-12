using Mediator;

namespace Application.Common.Mediator;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
