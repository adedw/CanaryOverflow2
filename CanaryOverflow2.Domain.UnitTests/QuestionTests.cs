using CanaryOverflow2.Domain.Question;
using CanaryOverflow2.Domain.Question.Models;

using FluentAssertions.Execution;

namespace CanaryOverflow2.Domain.UnitTests;

using Question = Question.Models.Question;

public class QuestionTests
{
    [Fact]
    public void Check_event_on_question_create()
    {
        var questionId = Guid.NewGuid();
        const string title = "title test";
        const string body = "text test";
        var createdAt = DateTime.Now;

        var question = new Question(questionId, title, body, createdAt);

        var events = question.GetUncommittedEvents();
        using (new AssertionScope())
        {
            events.Should().ContainSingle();

            var @event = events.Single();
            @event.Should().BeOfType<QuestionCreated>()
                .Which
                .Should().Be(new QuestionCreated(questionId, title, body, createdAt));
        }
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000", "some title","some text")]
    [InlineData("233292c1-e9b6-4d6c-8034-47de06257515", "","some text")]
    [InlineData("233292c1-e9b6-4d6c-8034-47de06257515", "some title","")]
    public void Create_question_with_invalid_input(Guid id, string? title, string? text)
    {
        var act = () => new Question(id, title, text, DateTime.Now);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Update_with_invalid_title()
    {
        var questionId = Guid.NewGuid();
        const string title = "title test";
        const string body = "text test";
        var createdAt = DateTime.Now;

        var question = new Question(questionId, title, body, createdAt);

        var act = () => question.UpdateTitle(null);

        using (new AssertionScope())
        {
            act.Should().Throw<ArgumentNullException>();
            question.GetUncommittedEvents().Should().ContainSingle();
        }
    }

    [Fact]
    public void Update_title()
    {
        var questionId = Guid.NewGuid();
        const string title = "title test";
        const string body = "text test";
        var createdAt = DateTime.Now;
        const string updatedTitle = "new title";

        var question = new Question(questionId, title, body, createdAt);

        question.UpdateTitle(updatedTitle);

        var events = question.GetUncommittedEvents();
        using (new AssertionScope())
        {
            events.Should().HaveCount(2);

            var @event = events.Last();
            @event.Should().BeOfType<TitleUpdated>()
                .Which
                .Should().Be(new TitleUpdated(updatedTitle));
        }
    }
    
    [Fact]
    public void Update_with_invalid_body()
    {
        var questionId = Guid.NewGuid();
        const string title = "title test";
        const string body = "text test";
        var createdAt = DateTime.Now;
        
        var question = new Question(questionId, title, body, createdAt);

        var act = () => question.UpdateBody(null);

        act.Should().Throw<ArgumentNullException>();
    }
    
    [Fact]
    public void Update_to_some_text()
    {
        var questionId = Guid.NewGuid();
        const string title = "title test";
        const string body = "text test";
        var createdAt = DateTime.Now;
        const string updatedBody = "new body";
        
        var question = new Question(questionId, title, body, createdAt);
        
        question.UpdateBody(updatedBody);
        
        var events = question.GetUncommittedEvents();
        using (new AssertionScope())
        {
            events.Should().HaveCount(2);

            var @event = events.Last();
            @event.Should().BeOfType<BodyUpdated>()
                .Which
                .Should().Be(new BodyUpdated(updatedBody));
        }
    }

    [Fact]
    public void Change_state_from_approved_to_answered()
    {
        var questionId = Guid.NewGuid();
        const string title = "title test";
        const string body = "text test";
        var createdAt = DateTime.Now;
        
        var question = new Question(questionId, title, body, createdAt);
        
        question.Approve();

        var events = question.GetUncommittedEvents();
        using (new AssertionScope())
        {
            events.Should().HaveCount(2);

            var @event = events.Last();
            @event.Should().BeOfType<QuestionApproved>();
        }
    }

}