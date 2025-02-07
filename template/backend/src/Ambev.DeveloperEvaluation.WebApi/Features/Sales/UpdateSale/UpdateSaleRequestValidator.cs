using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

public class UpdateSaleRequestValidator : AbstractValidator<UpdateSaleRequest>
{
    public UpdateSaleRequestValidator()
    {
        RuleFor(x => x.CartId)
            .NotEmpty();

        RuleFor(x => x.Date)
            .NotEmpty();
    }
}
