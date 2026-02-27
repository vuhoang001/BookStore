using Mediator;

namespace BookStore.Catalog.Features.Authors.Create;

public sealed record CreateAuthorCommand(string Name) : ICommand<Guid>;

public sealed class CreateAuthorHandler(IAuthorRepository authorRepository) : ICommandHandler<CreateAuthorCommand, Guid>
{
    public async ValueTask<Guid> Handle(CreateAuthorCommand command, CancellationToken cancellationToken)
    {
        var result = await authorRepository.AddAsync(new Author(command.Name), cancellationToken);

        await authorRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        return result.Id;
    }
}
