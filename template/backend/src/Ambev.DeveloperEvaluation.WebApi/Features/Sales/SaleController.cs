using Ambev.DeveloperEvaluation.Application.Carts;
using Ambev.DeveloperEvaluation.Application.Carts.DTOs;
using Ambev.DeveloperEvaluation.Application.Sales;
using Ambev.DeveloperEvaluation.Application.Sales.DTOs;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Services;
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
    public async Task<ActionResult> CreateSale(SaleDTO saleDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        try
        {
            var sale = _mapper.Map<Sale>(saleDto);
            var createdSale = await _service.CreateSale(sale);
            return Ok(createdSale);
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
        return Ok(sales);
    }

    [HttpGet("{saleNumber}")]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<ActionResult<SaleDTO>> GetSaleById(int saleNumber)
    {
        var sale = await _service.GetSaleById(saleNumber);
        if (sale == null) return NotFound();
        var saleDto = _mapper.Map<SaleDTO>(sale);
        return Ok(sale);
    }

    [HttpPut("{saleNumber}")]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<ActionResult> UpdateSale(int saleNumber, SaleDTO saleDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        try
        {
            var existingSale = await _service.GetSaleById(saleNumber);
            if (existingSale == null)
            {
                return NotFound();
            }

            existingSale.BranchName = saleDto.BranchName;
            existingSale.BranchId = saleDto.BranchId;
            existingSale.CustomerId = saleDto.CustomerId;
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
                    return NotFound("Item Not Found");
                }
            }

            await _service.UpdateSale(existingSale);
            return Ok(existingSale);
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
                return NotFound();
            }

            await _service.DeleteSale(saleNumber);
            return Ok(sale);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting sales.");
            return StatusCode(500, "Internal Server Error");
        }

    }
}
