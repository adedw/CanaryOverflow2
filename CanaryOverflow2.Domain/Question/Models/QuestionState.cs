namespace CanaryOverflow2.Domain.Question.Models;

public enum QuestionState
{
    /// <summary>
    /// Initial state. Does not allowed to show.
    /// </summary>
    Unapproved,

    /// <summary>
    /// Approved by moderator. Allowed to show. Currently no answer.
    /// </summary>
    Approved,

    /// <summary>
    /// Author set question as answered.
    /// </summary>
    Answered
}
