using Microsoft.Extensions.DependencyInjection;
using Rumpelstiltskin.Shared.Commands;

namespace Rumpelstiltskin.Shared.Handlers
{
    public static class CommandHandlerExensions
    {
        /// <summary>Adds <see cref="ICommandHandler{TCommand}"/> for <typeparamref name="THandler"/>.</summary>
        /// <typeparam name="TCommand">
        /// The type of the command.
        /// </typeparam>
        /// <typeparam name="THandler">
        /// The command handler that handles <typeparamref name="TCommand"/>'s.
        /// </typeparam>
        public static IServiceCollection AddCommandHandler<TCommand, THandler>(this IServiceCollection services)
            where TCommand : class, ICommand
            where THandler : class, ICommandHandler<TCommand>
        {
            Guard.NotNull(services, nameof(services));
            return services.AddSingleton<ICommandHandler<TCommand>, THandler>();
        }

        /// <summary>Adds <see cref="IRequestHandler{TRequest, TResponse}"/> for <typeparamref name="THandler"/>.</summary>
        /// <typeparam name="TRequest">
        /// The type of the request.
        /// </typeparam>
        /// <typeparam name="TResponse">
        /// The type of the response.
        /// </typeparam>
        /// <typeparam name="THandler">
        /// The command handler that handles <typeparamref name="TRequest"/>'s.
        /// </typeparam>
        public static IServiceCollection AddRequestHandler<TRequest, TResponse, THandler>(this IServiceCollection services)
            where THandler : class, IRequestHandler<TRequest, TResponse>
        {
            Guard.NotNull(services, nameof(services));
            return services.AddSingleton<IRequestHandler<TRequest, TResponse>, THandler>();
        }
    }
}
