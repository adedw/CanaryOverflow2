using CanaryOverflow2.Domain.Question.Models;

namespace CanaryOverflow2.WebAPI.Models;

public record CreateQuestionDto(string Title, string Body);

public record QuestionCreatedDto(Guid Id, string? Title, string? Body, DateTime CreatedAt, QuestionState State)
{
    public static QuestionCreatedDto From(Question question)
    {
        return new QuestionCreatedDto(question.Id, question.Title, question.Body, question.CreatedAt, question.State);
    }
}

public record QuestionUpdatedDto(Guid Id, string? Title, string? Body, DateTime CreatedAt, QuestionState State)
{
    public static QuestionUpdatedDto From(Question question)
    {
        return new QuestionUpdatedDto(question.Id, question.Title, question.Body, question.CreatedAt, question.State);
    }
}

public record UpdateQuestionTitleDto(string Title);

public record UpdateQuestionBodyDto(string Body);