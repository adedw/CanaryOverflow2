using CanaryOverflow2.Domain.Common;
using CanaryOverflow2.Domain.Question.Models;
using CanaryOverflow2.Infrastructure.IntegrationTests.Fixtures;

using CommunityToolkit.Diagnostics;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

namespace CanaryOverflow2.Infrastructure.IntegrationTests;

[Trait("Category", "Integration")]
public class QuestionRepositoryTests : IClassFixture<EventSourcedRepositoryFixture<Guid, Question>>
{
    private readonly ServiceProvider _serviceProvider;

    public QuestionRepositoryTests(EventSourcedRepositoryFixture<Guid, Question> eventSourcedRepositoryFixture)
    {
        Guard.IsNotNull(eventSourcedRepositoryFixture.ServiceProvider);
        _serviceProvider = eventSourcedRepositoryFixture.ServiceProvider;
    }

    [Fact]
    public async Task Save_restore()
    {
        var questionId = Guid.Parse("622f9e4a-1453-4749-a216-a5d19698b8c2");
        const string title = "title test";
        const string body = "text test";
        var createdAt = new DateTime(2023, 03, 19, 21, 38, 16, 0, DateTimeKind.Utc);

        var questionRepository = _serviceProvider.GetRequiredService<IAggregateRepository<Guid, Question>>();
        var question = new Question(questionId, title, body, createdAt);
        await questionRepository.SaveAsync(question);

        var foundQuestion = await questionRepository.FindAsync(questionId);

        foundQuestion.Should().Be(question);
        foundQuestion.Version.Should().Be(question.Version);
    }
}