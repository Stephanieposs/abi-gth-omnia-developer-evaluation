using Ambev.DeveloperEvaluation.Application.Carts;
using Ambev.DeveloperEvaluation.Application.Carts.CreateCart;
using Ambev.DeveloperEvaluation.Application.Carts.DeleteCart;
using Ambev.DeveloperEvaluation.Application.Carts.GetAllCarts;
using Ambev.DeveloperEvaluation.Application.Carts.GetCart;
using Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;
using Ambev.DeveloperEvaluation.Application.Sales;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetAllSales;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetAllSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSaleById;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    public readonly IMapper _mapper;
    public readonly ILogger<SalesController> _logger;
    private readonly IMediator _mediator;

    public SalesController(IMapper mapper, ILogger<SalesController> logger, IMediator mediator)
    {
        _mapper = mapper;
        _logger = logger;
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<ActionResult> CreateSale(CreateSaleRequest saleDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ModelState is not valid when creating {saleDto}", saleDto);
            return BadRequest();
        }

        try
        {
            
            var sale = _mapper.Map<Sale>(saleDto);
            //var createdSale = await _service.CreateSale(sale);
            var command = _mapper.Map<CreateSaleCommand>(sale);
            var createdSale = await _mediator.Send(command);

            var responseMap = _mapper.Map<CreateSaleResponse>(createdSale);

            return Ok(responseMap);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating sales.");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<ActionResult<List<GetAllSalesResponse>>> GetAllSales()
    {
        //var sales = await _service.GetAllSales();
        var query = new GetAllSalesQuery();
        var result = await _mediator.Send(query);

        var response = _mapper.Map<List<GetAllSalesResponse>>(result);

        return Ok(response);
    }

    [HttpGet("{saleNumber}")]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<ActionResult<GetSaleByIdResponse>> GetSaleById(int saleNumber)
    {
        //var sale = await _service.GetSaleById(saleNumber);
        var query = new GetSaleQuery(saleNumber);
        var sale = await _mediator.Send(query);

        if (sale == null) return NotFound();

        var responseMap = _mapper.Map<GetSaleByIdResponse>(sale);

        return Ok(responseMap);
    }

    [HttpPut("{saleNumber}")]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<ActionResult> UpdateSale(int saleNumber, UpdateSaleRequest saleDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ModelState is not valid when creating {saleDto}", saleDto);
            return BadRequest();
        }

        try
        {
            
            var command = _mapper.Map<UpdateSaleCommand>(saleDto);
            command.SaleNumber = saleNumber;
            var createdSale = await _mediator.Send(command);

            var response = _mapper.Map<UpdateSaleResponse>(createdSale);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating sales.");
            return StatusCode(500, "Internal Server Error");
        }


    }

    [HttpDelete("{saleNumber}")]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<ActionResult> DeleteSale(int saleNumber)
    {
        try
        {
            var query = new GetSaleQuery(saleNumber);
            var sale = await _mediator.Send(query);

            if (sale == null)
            {
                _logger.LogWarning("Sale {saleNumber} wasn't found", saleNumber);
                return NotFound();
            }

            var command = new DeleteSaleCommand(saleNumber);
            await _mediator.Send(command);

            return Ok($"Deleted {saleNumber}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting sales.");
            return StatusCode(500, "Internal Server Error");
        }

    }
}
