﻿using EcommerceDDD.Quotes.Domain;

namespace EcommerceDDD.Quotes.Tests.Domain;

public class QuoteCreationTests
{
    [Fact]
    public void CreateQuote_WithCustomerId_ReturnsOpenQuote()
    {
        // Given
        CustomerId _customerId = CustomerId.Of(Guid.NewGuid());

        // When
        var quote = Quote.Create(_customerId);

        // Then
        Assert.NotNull(quote);
        quote.CustomerId.Should().Be(quote.CustomerId);
        quote.CreatedAt.Should().NotBe(null);
        quote.Status.Should().Be(QuoteStatus.Open);
    }
}
