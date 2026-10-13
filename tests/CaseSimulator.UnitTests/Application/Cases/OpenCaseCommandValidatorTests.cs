using CaseSimulator.Application.Cases.Commands.OpenCase;
using FluentAssertions;
using Xunit;

namespace CaseSimulator.UnitTests.Application.Cases;

public class OpenCaseCommandValidatorTests
{
    [Fact]
    public void WhenCaseIdIsEmpty_ShouldBeInvalid()
    {
        var validator = new OpenCaseCommandValidator();

        var result = validator.Validate(new OpenCaseCommand(Guid.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(x => x.PropertyName == nameof(OpenCaseCommand.CaseId));
    }

    [Fact]
    public void WhenCaseIdIsNotEmpty_ShouldBeValid()
    {
        var validator = new OpenCaseCommandValidator();

        var result = validator.Validate(new OpenCaseCommand(Guid.NewGuid()));

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}
