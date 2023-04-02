using System.Net.Mime;

using CanaryOverflow2.WebAPI.Models;
using CanaryOverflow2.WebAPI.Services;

using Microsoft.AspNetCore.Mvc;

namespace CanaryOverflow2.WebAPI.Controllers;

[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[Route("api/[controller]")]
public class QuestionsController : ControllerBase
{
    private readonly IQuestionService _questionService;

    public QuestionsController(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    /// <summary>
    /// Create new question.
    /// </summary>
    /// <param name="createQuestionDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>A newly created question.</returns>
    [HttpPost]
    [Consumes(typeof(CreateQuestionDto), MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(QuestionCreatedDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Post([FromBody] CreateQuestionDto createQuestionDto,
        CancellationToken cancellationToken)
    {
        var questionCreatedDto = await _questionService.CreateAsync(createQuestionDto, cancellationToken);

        return Created($"/api/questions/{questionCreatedDto.Id}", questionCreatedDto);
    }

    [HttpPatch("{id:guid}/title")]
    [Consumes(typeof(UpdateQuestionTitleDto), MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(QuestionUpdatedDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateTitle([FromRoute] Guid id,
        [FromBody] UpdateQuestionTitleDto updateQuestionTitleDto, CancellationToken cancellationToken)
    {
        var questionUpdatedDto = await _questionService.UpdateTitleAsync(id, updateQuestionTitleDto, cancellationToken);

        return Ok(questionUpdatedDto);
    }
    
    [HttpPatch("{id:guid}/body")]
    [Consumes(typeof(UpdateQuestionBodyDto), MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(QuestionUpdatedDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateBody([FromRoute] Guid id,
        [FromBody] UpdateQuestionBodyDto updateQuestionTitleDto, CancellationToken cancellationToken)
    {
        var questionUpdatedDto = await _questionService.UpdateBodyAsync(id, updateQuestionTitleDto, cancellationToken);

        return Ok(questionUpdatedDto);
    }

    [HttpPatch("{id:guid}/approve")]
    [ProducesResponseType(typeof(QuestionUpdatedDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Approve([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var questionUpdatedDto = await _questionService.ApproveAsync(id, cancellationToken);

        return Ok(questionUpdatedDto);
    }
}