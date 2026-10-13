using CaseSimulator.Application.Cases.Queries.GetCaseById;
using FluentAssertions;
using Xunit;

namespace CaseSimulator.UnitTests.Application.Cases;

public class GetCaseByIdQueryValidatorTests
{
    private readonly GetCaseCommandValidator _validator = new();

    [Fact]
    public void WhenCaseIdIsEmpty_ShouldBeInvalid()
    {
        var result = _validator.Validate(new GetCaseByIdQuery(Guid.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(x => x.PropertyName == nameof(GetCaseByIdQuery.CaseId));
    }

    [Fact]
    public void WhenCaseIdIsNotEmpty_ShouldBeValid()
    {
        var result = _validator.Validate(new GetCaseByIdQuery(Guid.NewGuid()));

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}
