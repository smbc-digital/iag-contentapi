using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public record TriviaSection(string Heading, IEnumerable<Trivia> Trivia);
}
