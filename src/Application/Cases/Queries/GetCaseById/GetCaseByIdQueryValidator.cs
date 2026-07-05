using FluentValidation;

namespace CaseSimulator.Application.Cases.Queries.GetCaseById;

public class GetCaseCommandValidator : AbstractValidator<GetCaseByIdQuery>
{
    public GetCaseCommandValidator()
    {
        RuleFor(x => x.CaseId).NotEqual(Guid.Empty).WithMessage("CaseId cannot be empty");
    }
}