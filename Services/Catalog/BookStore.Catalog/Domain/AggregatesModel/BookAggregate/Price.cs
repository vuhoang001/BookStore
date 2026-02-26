using BookStore.Catalog.Exceptions;
using BuildingBlocks.SharedKernel.SeedWork;

namespace BookStore.Catalog.Domain.AggregatesModel.BookAggregate;

public sealed class Price() : ValueObject
{
    public Price(decimal originalPrice, decimal? discountPrice)
        : this()
    {
        OriginalPrice = ValidateOriginalPrice(originalPrice);
        DiscountPrice = ValidateDiscountPrice(discountPrice, OriginalPrice);
    }

    private decimal OriginalPrice { get; }
    private decimal? DiscountPrice { get; }

    /// <summary>
    /// Validates that the original price is non-negative.
    /// </summary>
    /// <param name="originalPrice">The original price to validate</param>
    /// <returns>The validated original price</returns>
    /// <exception cref="CatalogDomainException">Thrown when original price is negative</exception>
    private static decimal ValidateOriginalPrice(decimal originalPrice)
    {
        return originalPrice < 0
            ? throw new CatalogDomainException(
                "Original price must be greater than or equal to 0."
            )
            : originalPrice;
    }

    /// <summary>
    /// Validates that the discount price is between 0 and the original price.
    /// </summary>
    /// <param name="discountPrice">The discount price to validate</param>
    /// <param name="originalPrice">The original price to validate against</param>
    /// <returns>The validated discount price</returns>
    /// <exception cref="CatalogDomainException">Thrown when discount price is invalid</exception>
    private static decimal? ValidateDiscountPrice(decimal? discountPrice, decimal originalPrice)
    {
        return discountPrice < 0 || discountPrice > originalPrice
            ? throw new CatalogDomainException(
                "Discount price must be greater than or equal to 0 and less than or equal to original price."
            )
            : discountPrice;
    }


    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return OriginalPrice;
        yield return DiscountPrice ?? 0;
    }
}
