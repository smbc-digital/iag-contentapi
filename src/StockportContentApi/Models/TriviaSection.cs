namespace StockportContentApi.Models;

public record TriviaSection(string Heading, IEnumerable<Trivia> Trivia);