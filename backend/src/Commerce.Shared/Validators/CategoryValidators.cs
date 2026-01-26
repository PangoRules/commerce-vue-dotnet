using System.Diagnostics.CodeAnalysis;
using Commerce.Shared.Requests;
using FluentValidation;

namespace Commerce.Shared.Validators;

[ExcludeFromCodeCoverage]
public class GetCategoriesQueryParamsValidator : AbstractValidator<GetCategoriesQueryParams>
{
    public GetCategoriesQueryParamsValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
    }
}