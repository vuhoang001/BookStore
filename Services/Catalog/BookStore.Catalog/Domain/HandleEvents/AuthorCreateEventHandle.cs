using BookStore.Catalog.Domain.Events;
using Mediator;

namespace BookStore.Catalog.Domain.HandleEvents;

public class AuthorCreateEventHandle(IAuthorRepository authorRepository) : INotificationHandler<AuthorCreateEvent>
{
    public ValueTask Handle(AuthorCreateEvent notification, CancellationToken cancellationToken)
    {
        var name = notification.Name;


        Console.WriteLine("---------------------------------------------------------------------------------------");
        Console.WriteLine("AuthorCreateEventHandle: Author created with name: {0}", name);
        Console.WriteLine("---------------------------------------------------------------------------------------");
        return new ValueTask();
    }
}
