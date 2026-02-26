namespace BookStore.Catalog.Exceptions;

public sealed class CatalogDomainException(string message) : Exception(message);
