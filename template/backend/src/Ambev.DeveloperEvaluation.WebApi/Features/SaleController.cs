using Ambev.DeveloperEvaluation.Application.Carts;
using Ambev.DeveloperEvaluation.Application.Carts.DTOs;
using Ambev.DeveloperEvaluation.Application.Sales;
using Ambev.DeveloperEvaluation.Application.Sales.DTOs;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    public readonly ISaleService _service;
    public readonly IMapper _mapper;

    public SalesController(ISaleService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
        
    }

    [HttpPost]
    public async Task<ActionResult> CreateSale(SaleDTO saleDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var sale = _mapper.Map<Sale>(saleDto);
            var createdSale = await _service.CreateSale(sale);
            return Ok(createdSale);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SaleDTO>>> GetAllSales()
    {
        var sales = await _service.GetAllSales();
        return Ok(_mapper.Map<IEnumerable<SaleDTO>>(sales));
    }

    [HttpGet("{saleNumber}")]
    public async Task<ActionResult<SaleDTO>> GetSaleById(int saleNumber)
    {
        var sale = await _service.GetSaleById(saleNumber);
        if (sale == null) return NotFound();
        var saleDto = _mapper.Map<SaleDTO>(sale);
        return Ok(saleDto);
    }

    [HttpPut("{saleNumber}")]
    public async Task<ActionResult> UpdateSale(int saleNumber, SaleDTO saleDto)
    {

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); 
        }

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

        foreach (var saleItem in saleDto.Items)
        {
            var saleIt = existingSale.Items
                .FirstOrDefault(p => p.Id == saleItem.Id);
            if (saleIt != null)
            {
                saleIt.UnitPrice = saleItem.UnitPrice;
                saleIt.Quantity = saleItem.Quantity;
                // update total ?
            }
            else
            {
                return NotFound("Item Not Found");
            }

        }

        await _service.UpdateSale(existingSale);
        return Ok(_mapper.Map<SaleDTO>(existingSale));
    }

    [HttpDelete("{saleNumber}")]
    public async Task<ActionResult> DeleteSale(int saleNumber)
    {
        var sale = await _service.GetSaleById(saleNumber);
        if (sale == null)
        {
            return NotFound();
        }

        await _service.DeleteSale(saleNumber);
        return Ok(_mapper.Map<SaleDTO>(sale));
    }
}
