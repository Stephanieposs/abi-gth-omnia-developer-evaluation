
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.CreateCart;

public class CreateCartRequestValidator : AbstractValidator<CreateCartRequest>
{
    public CreateCartRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Date)
            .NotEmpty();
    }
}
