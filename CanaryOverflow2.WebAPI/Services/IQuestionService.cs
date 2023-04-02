using CanaryOverflow2.WebAPI.Models;

namespace CanaryOverflow2.WebAPI.Services;

public interface IQuestionService
{
    Task<QuestionCreatedDto> CreateAsync(CreateQuestionDto createQuestionDto,
        CancellationToken cancellationToken = default);

    Task<QuestionUpdatedDto> UpdateTitleAsync(Guid id, UpdateQuestionTitleDto updateQuestionTitleDto,
        CancellationToken cancellationToken = default);

    Task<QuestionUpdatedDto> UpdateBodyAsync(Guid id, UpdateQuestionBodyDto updateQuestionBodyDto,
        CancellationToken cancellationToken = default);

    Task<QuestionUpdatedDto> ApproveAsync(Guid id, CancellationToken cancellationToken = default);
}