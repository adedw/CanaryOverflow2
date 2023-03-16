using System;

using CanaryOverflow2.Domain.Common;

namespace CanaryOverflow2.Domain.Question.Models;

public record QuestionCreated(Guid Id, string Title, string Body, DateTime CreatedAt) : IDomainEvent;

public record TitleUpdated(string Title) : IDomainEvent;

public record BodyUpdated(string Body) : IDomainEvent;

public record QuestionApproved : IDomainEvent;