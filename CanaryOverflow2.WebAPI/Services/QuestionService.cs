using CanaryOverflow2.Domain.Common;
using CanaryOverflow2.Domain.Question.Models;
using CanaryOverflow2.WebAPI.Models;

namespace CanaryOverflow2.WebAPI.Services;

public class QuestionService : IQuestionService
{
    private readonly IAggregateRepository<Guid, Question> _questionRepository;

    public QuestionService(IAggregateRepository<Guid, Question> questionRepository)
    {
        _questionRepository = questionRepository;
    }

    public async Task<QuestionCreatedDto> CreateAsync(CreateQuestionDto createQuestionDto,
        CancellationToken cancellationToken = default)
    {
        var id = Guid.NewGuid();
        var createdAt = DateTime.Now;
        (string title, string body) = createQuestionDto;
        var question = new Question(id, title, body, createdAt);

        await _questionRepository.SaveAsync(question, cancellationToken);

        return QuestionCreatedDto.From(question);
    }

    public async Task<QuestionUpdatedDto> UpdateTitleAsync(Guid id, UpdateQuestionTitleDto updateQuestionTitleDto,
        CancellationToken cancellationToken = default)
    {
        var question = await _questionRepository.FindAsync(id, cancellationToken);

        question.UpdateTitle(updateQuestionTitleDto.Title);
        
        await _questionRepository.SaveAsync(question, cancellationToken);

        return QuestionUpdatedDto.From(question);
    }

    public async Task<QuestionUpdatedDto> UpdateBodyAsync(Guid id, UpdateQuestionBodyDto updateQuestionBodyDto,
        CancellationToken cancellationToken = default)
    {
        var question = await _questionRepository.FindAsync(id, cancellationToken);

        question.UpdateBody(updateQuestionBodyDto.Body);
        
        await _questionRepository.SaveAsync(question, cancellationToken);

        return QuestionUpdatedDto.From(question);
    }

    public async Task<QuestionUpdatedDto> ApproveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var question = await _questionRepository.FindAsync(id, cancellationToken);

        question.Approve();

        await _questionRepository.SaveAsync(question, cancellationToken);

        return QuestionUpdatedDto.From(question);
    }
}