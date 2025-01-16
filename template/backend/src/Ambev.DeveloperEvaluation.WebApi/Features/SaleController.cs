using Ambev.DeveloperEvaluation.Application.Carts;
using Ambev.DeveloperEvaluation.Application.Carts.DTOs;
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

    [HttpPost]     //("from-cart/{cartId}")]
    public async Task<ActionResult> CreateSale(SaleDTO saleDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); 
        }

        var sale = _mapper.Map<Sale>(saleDto);

        foreach (var items in saleDto.Items)
        {

            var saleItem = new SaleItem
            {
                ProductName = items.ProductName,  
                ProductId = items.ProductId,
                Quantity = items.Quantity,
                UnitPrice = items.UnitPrice,
            };
            //cart.CartProductsList.Add(cartProduct);
        }

        var createdSale = await _service.CreateSale(sale);
        return Ok(createdSale);
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

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateSale(int id, SaleDTO saleDto)
    {

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); 
        }

        var existingSale = await _service.GetSaleById(id);
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
                
                saleIt.ProductName = saleItem.ProductName;
                saleIt.UnitPrice = saleItem.UnitPrice;
                saleIt.Quantity = saleItem.Quantity;
            }
            // implementar codigo se não existir o Item
        }

        await _service.UpdateSale(existingSale);
        return Ok(_mapper.Map<SaleDTO>(existingSale));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSale(int id)
    {
        var sale = await _service.GetSaleById(id);
        if (sale == null)
        {
            return NotFound();
        }

        await _service.DeleteSale(id);
        return Ok(sale);
    }
}
