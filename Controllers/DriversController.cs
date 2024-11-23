using CachingWebApi.Data;
using CachingWebApi.Models;
using CachingWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CachingWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ILogger<CustomersController> _logger;
    private readonly ICacheService _cacheService;
    private readonly AppDbContext _appDbContext;

    public CustomersController(ILogger<CustomersController> logger
        , ICacheService cacheService
        , AppDbContext appDbContext)
    {
        _logger = logger;
        _cacheService = cacheService;
        _appDbContext = appDbContext;
    }

    [HttpGet("Customers")]
    public async Task<IActionResult> Get()
    {
        // check cache data
        var cacheData = _cacheService.GetData<IEnumerable<Customer>>("Customers");

        if (cacheData is not null && cacheData.Count() > 0)
        {
            return Ok(cacheData);
        }

        cacheData = await _appDbContext.Customers.ToListAsync();

        //Set expiry time
        var expiryTime = DateTimeOffset.Now.AddSeconds(30);

        _cacheService.SetData<IEnumerable<Customer>>("Customers", cacheData, expiryTime);

        return Ok(cacheData);
    }

    [HttpPost("AddCustomer")]
    public async Task<IActionResult> Post(Customer Customer)
    {
        var addedObj = await _appDbContext.Customers.AddAsync(Customer);

        var expriryTime = DateTimeOffset.Now.AddSeconds(30);

        _cacheService.SetData<Customer>($"Customer{Customer.Id}", addedObj.Entity, expriryTime);

        await _appDbContext.SaveChangesAsync();

        return Ok(addedObj.Entity);
    }

    [HttpDelete("DeleteCustomer")]
    public async Task<IActionResult> Delete(int id)
    {
        var exist = await _appDbContext.Customers.FirstOrDefaultAsync(x => x.Id == id);

        if (exist is not null)
        {
            _appDbContext.Remove(exist);
            _cacheService.RemoveData($"Customer{exist.Id}");
            await _appDbContext.SaveChangesAsync();
            return NoContent();
        }
        else
        {
            return NotFound();
        }
    }
}
