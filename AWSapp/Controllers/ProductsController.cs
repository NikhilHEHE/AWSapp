using AWSapp.Data;
using AWSapp.DTOs;
using AWSapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AWSapp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        AppDbContext context,
        ILogger<ProductsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        _logger.LogInformation("Getting all products");

        var products = await _context.Products
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .ToListAsync();

        return Ok(products);
    }

    // GET: api/products/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        _logger.LogInformation(
            "Getting product with ID {ProductId}",
            id);

        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound(new
            {
                message = $"Product with ID {id} was not found."
            });
        }

        return Ok(product);
    }

    // POST: api/products
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(
        ProductDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return BadRequest(new
            {
                message = "Product name is required."
            });
        }

        if (dto.Price < 0)
        {
            return BadRequest(new
            {
                message = "Product price cannot be negative."
            });
        }

        if (dto.StockQuantity < 0)
        {
            return BadRequest(new
            {
                message = "Stock quantity cannot be negative."
            });
        }

        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Created product with ID {ProductId}",
            product.Id);

        return CreatedAtAction(
            nameof(GetProduct),
            new { id = product.Id },
            product);
    }

    // PUT: api/products/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProduct(
        int id,
        ProductDto dto)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound(new
            {
                message = $"Product with ID {id} was not found."
            });
        }

        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return BadRequest(new
            {
                message = "Product name is required."
            });
        }

        if (dto.Price < 0)
        {
            return BadRequest(new
            {
                message = "Product price cannot be negative."
            });
        }

        if (dto.StockQuantity < 0)
        {
            return BadRequest(new
            {
                message = "Stock quantity cannot be negative."
            });
        }

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.StockQuantity = dto.StockQuantity;

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Updated product with ID {ProductId}",
            id);

        return Ok(product);
    }

    // DELETE: api/products/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound(new
            {
                message = $"Product with ID {id} was not found."
            });
        }

        _context.Products.Remove(product);

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Deleted product with ID {ProductId}",
            id);

        return NoContent();
    }
}