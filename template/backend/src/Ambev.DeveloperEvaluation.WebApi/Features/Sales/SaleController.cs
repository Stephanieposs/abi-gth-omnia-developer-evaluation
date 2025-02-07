using Ambev.DeveloperEvaluation.Application.Carts;
using Ambev.DeveloperEvaluation.Application.Carts.DTOs;
using Ambev.DeveloperEvaluation.Application.Sales;
using Ambev.DeveloperEvaluation.Application.Sales.DTOs;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetAllSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSaleById;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    public readonly ISaleService _service;
    public readonly IMapper _mapper;
    public readonly ILogger<SalesController> _logger;

    public SalesController(ISaleService service, IMapper mapper, ILogger<SalesController> logger)
    {
        _service = service;
        _mapper = mapper;
        _logger = logger;
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
            var createdSale = await _service.CreateSale(sale);

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
    public async Task<ActionResult<IEnumerable<SaleDTO>>> GetAllSales()
    {
        var sales = await _service.GetAllSales();

        var response = _mapper.Map<List<GetAllSalesResponse>>(sales);

        return Ok(response);
    }

    [HttpGet("{saleNumber}")]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<ActionResult<SaleDTO>> GetSaleById(int saleNumber)
    {
        var sale = await _service.GetSaleById(saleNumber);
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
            var existingSale = await _service.GetSaleById(saleNumber);
            if (existingSale == null)
            {
                _logger.LogWarning("Sale {saleNumber} wasn't found", saleNumber);
                return NotFound();
            }

            existingSale.BranchName = saleDto.BranchName;
            existingSale.BranchId = saleDto.BranchId;
            //existingSale.CustomerId = saleDto.CustomerId;
            existingSale.CustomerName = saleDto.CustomerName;
            existingSale.Date = DateTime.UtcNow;

            var sale = _mapper.Map<Sale>(saleDto);

            foreach (var saleItem in sale.Items)
            {
                var saleIt = existingSale.Items
                    .FirstOrDefault(p => p.Id == saleItem.Id);
                if (saleIt != null)
                {
                    saleIt.UnitPrice = saleItem.UnitPrice;
                    saleIt.Quantity = saleItem.Quantity;
                }
                else
                {
                    _logger.LogWarning("SaleItem {saleItem} wasn't found", saleItem);
                    return NotFound("Item Not Found");
                }
            }

            var response = _mapper.Map<UpdateSaleResponse>(existingSale);

            await _service.UpdateSale(existingSale);
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
            var sale = await _service.GetSaleById(saleNumber);
            if (sale == null)
            {
                _logger.LogWarning("Sale {saleNumber} wasn't found", saleNumber);
                return NotFound();
            }

            await _service.DeleteSale(saleNumber);
            return Ok($"Deleted {saleNumber}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting sales.");
            return StatusCode(500, "Internal Server Error");
        }

    }
}
