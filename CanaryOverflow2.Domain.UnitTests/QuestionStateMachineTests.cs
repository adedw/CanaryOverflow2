using CanaryOverflow2.Domain.Question.Models;

namespace CanaryOverflow2.Domain.UnitTests;

[Trait("Category", "Unit")]
public class QuestionStateMachineTests
{
    [Fact]
    public void Unapproved_to_approved()
    {
        var machine = new QuestionStateMachine(QuestionState.Unapproved);

        machine.SetApproved();

        machine.State.Should().Be(QuestionState.Approved);
    }

    [Theory]
    [InlineData(QuestionState.Approved, QuestionState.Approved)]
    [InlineData(QuestionState.Answered, QuestionState.Answered)]
    public void Try_to_approve_from_others(QuestionState initialState, QuestionState targetState)
    {
        var machine = new QuestionStateMachine(initialState);

        var act = () => machine.SetApproved();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("No valid leaving transitions are permitted from state*");
        machine.State.Should().Be(targetState);
    }

    [Fact]
    public void Approved_to_answered()
    {
        var machine = new QuestionStateMachine(QuestionState.Approved);

        machine.SetAnswered();

        machine.State.Should().Be(QuestionState.Answered);
    }

    [Theory]
    [InlineData(QuestionState.Unapproved, QuestionState.Unapproved)]
    [InlineData(QuestionState.Answered, QuestionState.Answered)]
    public void Try_to_answer_from_others(QuestionState initialState, QuestionState targetState)
    {
        var machine = new QuestionStateMachine(initialState);

        var act = () => machine.SetAnswered();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("No valid leaving transitions are permitted from state*");
        machine.State.Should().Be(targetState);
    }

    [Fact]
    public void Answered_to_unanswered()
    {
        var machine = new QuestionStateMachine(QuestionState.Answered);

        machine.SetUnanswered();

        machine.State.Should().Be(QuestionState.Approved);
    }

    [Theory]
    [InlineData(QuestionState.Unapproved, QuestionState.Unapproved)]
    [InlineData(QuestionState.Approved, QuestionState.Approved)]
    public void Try_to_cancel_answer_from_others(QuestionState initialState, QuestionState targetState)
    {
        var machine = new QuestionStateMachine(initialState);

        var act = () => machine.SetUnanswered();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("No valid leaving transitions are permitted from state*");
        machine.State.Should().Be(targetState);
    }
}