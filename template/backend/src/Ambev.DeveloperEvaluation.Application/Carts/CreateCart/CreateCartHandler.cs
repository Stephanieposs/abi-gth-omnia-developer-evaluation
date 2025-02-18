using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Carts.CreateCart;

public class CreateCartHandler : IRequestHandler<CreateCartCommand, CreateCartResult>
{
    private readonly ICartRepository _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateCartHandler> _logger;

    public CreateCartHandler(ICartRepository repo, IMapper mapper, ILogger<CreateCartHandler> logger)
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CreateCartResult> Handle(CreateCartCommand command, CancellationToken cancellationToken)
    {

        var validator = new CreateCartValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogError("Validation Result is not valid when creating cart in handler", command);
            return null;
            //throw new ValidationException(validationResult.Errors);
        }

        var cart = _mapper.Map<Cart>(command);


        var createdCart = await _repo.AddCartAsync(cart);
        _logger.LogInformation("Created Cart ", createdCart);

        return _mapper.Map<CreateCartResult>(createdCart);
    }
}
