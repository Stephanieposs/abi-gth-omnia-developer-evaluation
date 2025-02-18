using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Carts.CreateCart;

public class CreateCartValidator : AbstractValidator<CreateCartCommand>
{
    public CreateCartValidator()
    {
        RuleFor(cart => cart.CartProductsList).NotEmpty();
        RuleFor(cart => cart.UserId).GreaterThan(-1);
    }
}
