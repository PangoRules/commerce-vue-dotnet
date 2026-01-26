using System.Diagnostics.CodeAnalysis;
using Commerce.Shared.Requests;
using FluentValidation;

namespace Commerce.Shared.Validators;

[ExcludeFromCodeCoverage]
public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(100).WithMessage("Product name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Product description must not exceed 500 characters.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be a non-negative value.");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity must be a non-negative integer.");
    }
}

[ExcludeFromCodeCoverage]
public class GetProductsQueryParamsValidator : AbstractValidator<GetProductsQueryParams>
{
    public GetProductsQueryParamsValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");

        RuleFor(x => x.SortBy)
            .IsInEnum().WithMessage("SortBy must be a valid enum value.");

        RuleFor(x => x.SortDescending)
            .NotNull().WithMessage("SortDescending must be specified.");
    }
}