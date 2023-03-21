using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

using CanaryOverflow2.Domain.Common;

using CommunityToolkit.Diagnostics;

using JetBrains.Annotations;

namespace CanaryOverflow2.Domain.Question.Models;

[DebuggerDisplay("{Id}")]
[JsonConverter(typeof(QuestionJsonConverter))]
public class Question : AggregateRoot<Guid, Question>
{
    #region JsonConverter

    private class QuestionJsonConverter : JsonConverter<Question>
    {
        public override Question Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType is not JsonTokenType.StartObject)
                throw new JsonException();

            var question = new Question();

            while (reader.Read())
            {
                if (reader.TokenType is JsonTokenType.EndObject)
                    return question;

                if (reader.TokenType is not JsonTokenType.PropertyName)
                    throw new JsonException();

                var propName = reader.GetString();
                reader.Read();

                switch (propName)
                {
                    case "id":
                        question.Id = reader.GetGuid();
                        break;

                    case "title":
                        question._title = reader.GetString();
                        break;

                    case "body":
                        question._body = reader.GetString();
                        break;
                    
                    case "createdAt":
                        question._createdAt = reader.GetDateTime();
                        break;
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, Question value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString("id", value.Id);
            writer.WriteString("title", value._title);
            writer.WriteString("body", value._body);
            writer.WriteString("createdAt", value._createdAt);

            writer.WriteEndObject();
        }
    }

    #endregion
    
    private readonly IQuestionStateMachine _stateMachine;
    private string? _title;
    private string? _body;
    private DateTime _createdAt;

    [UsedImplicitly]
    private Question() : this(QuestionState.Unapproved)
    {
    }

    private Question(QuestionState questionState)
    {
        _stateMachine = new QuestionStateMachine(questionState);
    }

    public Question(Guid id, string? title, string? body, DateTime createdAt) : this(QuestionState.Unapproved)
    {
        Guard.IsNotEqualTo(id, Guid.Empty);
        Guard.IsNotNullOrWhiteSpace(title);
        Guard.IsNotNullOrWhiteSpace(body);

        Append(new QuestionCreated(id, title, body, createdAt));
    }

    public void UpdateTitle(string? title)
    {
        Guard.IsNotNullOrEmpty(title);

        Append(new TitleUpdated(title));
    }

    public void UpdateBody(string? body)
    {
        Guard.IsNotNullOrWhiteSpace(body);

        Append(new BodyUpdated(body));
    }

    public void Approve()
    {
        Append(new QuestionApproved());
    }

    protected override void When(IDomainEvent @event)
    {
        switch (@event)
        {
            case QuestionCreated questionCreated:
                Apply(questionCreated);
                break;

            case TitleUpdated titleUpdated:
                Apply(titleUpdated);
                break;

            case BodyUpdated bodyUpdated:
                Apply(bodyUpdated);
                break;

            case QuestionApproved questionApproved:
                Apply(questionApproved);
                break;

            default:
                ThrowHelper.ThrowNotSupportedException($"Question does not support '{@event.GetType().Name}' event.");
                break;
        }
    }

    #region Event appliers

    private void Apply(QuestionCreated questionCreated)
    {
        Id = questionCreated.Id;
        _title = questionCreated.Title;
        _body = questionCreated.Body;
        _createdAt = questionCreated.CreatedAt;
    }

    private void Apply(TitleUpdated titleUpdated)
    {
        _title = titleUpdated.Title;
    }

    private void Apply(BodyUpdated bodyUpdated)
    {
        _body = bodyUpdated.Body;
    }

    private void Apply(QuestionApproved _)
    {
        _stateMachine.SetApproved();
    }

    #endregion
}