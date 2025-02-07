using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.UpdateCart;

public class UpdateCartRequestValidator : AbstractValidator<UpdateCartRequest>
{
    public UpdateCartRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Date)
            .NotEmpty();
    }
}
