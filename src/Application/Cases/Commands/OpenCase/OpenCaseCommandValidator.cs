using FluentValidation;

namespace CaseSimulator.Application.Cases.Commands.OpenCase;

public class OpenCaseCommandValidator : AbstractValidator<OpenCaseCommand>
{
    public OpenCaseCommandValidator()
    {
        RuleFor(x => x.CaseId).NotEqual(Guid.Empty).WithMessage("CaseId cannot be empty");
    }
}