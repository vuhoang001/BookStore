using FluentValidation;

namespace BookStore.Basket.Feature.Order.Create;

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.OrderLines)
            .NotNull()
            .NotEmpty();

        RuleForEach(x => x.OrderLines).ChildRules(line =>
        {
            line.RuleFor(x => x.BookId)
                .NotEmpty();

            line.RuleFor(x => x.BookName)
                .NotEmpty()
                .MaximumLength(200);

            line.RuleFor(x => x.Quantity)
                .GreaterThan(0);

            line.RuleFor(x => x.UnitPrice)
                .GreaterThan(0);
        });
    }
}
