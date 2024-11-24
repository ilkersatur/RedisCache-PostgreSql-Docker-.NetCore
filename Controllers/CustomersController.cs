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
        var expiryTime = DateTimeOffset.Now.AddSeconds(120);

        _cacheService.SetData<IEnumerable<Customer>>("Customers", cacheData, expiryTime);

        return Ok(cacheData);
    }

    [HttpPost("AddCustomer")]
    public async Task<IActionResult> Post(Customer Customer)
    {
        // Yeni müşteri ekleme
        var addedObj = await _appDbContext.Customers.AddAsync(Customer);
        await _appDbContext.SaveChangesAsync();

        // Tek müşteri için cache ekleme
        var expiryTime = DateTimeOffset.Now.AddSeconds(120);
        _cacheService.SetData<Customer>($"Customer{Customer.Id}", addedObj.Entity, expiryTime);

        // Tüm müşteri cache'ini güncelle
        var cacheData = await _appDbContext.Customers.ToListAsync();
        _cacheService.SetData<IEnumerable<Customer>>("Customers", cacheData, expiryTime);

        return Ok(addedObj.Entity);
    }

    [HttpDelete("DeleteCustomer")]
    public async Task<IActionResult> Delete(int id)
    {
        // Silinecek müşteri var mı kontrolü
        var exist = await _appDbContext.Customers.FirstOrDefaultAsync(x => x.Id == id);

        if (exist is not null)
        {
            // Müşteri sil
            _appDbContext.Remove(exist);
            await _appDbContext.SaveChangesAsync();

            // Tek müşteri cache'ini temizle
            _cacheService.RemoveData($"Customer{exist.Id}");

            // Tüm müşteri cache'ini güncelle
            var cacheData = await _appDbContext.Customers.ToListAsync();
            var expiryTime = DateTimeOffset.Now.AddSeconds(120);
            _cacheService.SetData<IEnumerable<Customer>>("Customers", cacheData, expiryTime);

            return NoContent();
        }
        else
        {
            return NotFound();
        }
    }

    [HttpPost("AddCustomerWithoutRefreshingCache")]
    public async Task<IActionResult> PostWithoutRefreshingCache(Customer Customer)
    {
        var addedObj = await _appDbContext.Customers.AddAsync(Customer);

        var expriryTime = DateTimeOffset.Now.AddSeconds(120);

        _cacheService.SetData<Customer>($"Customers{Customer.Id}", addedObj.Entity, expriryTime);

        await _appDbContext.SaveChangesAsync();

        return Ok(addedObj.Entity);
    }

    [HttpDelete("DeleteCustomerWithoutRefreshingCache")]
    public async Task<IActionResult> DeleteWithoutRefreshingCache(int id)
    {
        var exist = await _appDbContext.Customers.FirstOrDefaultAsync(x => x.Id == id);

        if (exist is not null)
        {
            _appDbContext.Remove(exist);
            _cacheService.RemoveData($"Customers{exist.Id}");
            await _appDbContext.SaveChangesAsync();
            return NoContent();
        }
        else
        {
            return NotFound();
        }
    }

    [HttpPut("RefreshCustomerListCache")]
    public async Task<IActionResult> RefreshCustomerListCache()
    {
        var customerList = await _appDbContext.Customers.ToListAsync();

        var expiryDate = DateTimeOffset.Now.AddSeconds(120);

        _cacheService.SetData<IEnumerable<Customer>>($"Customers", customerList, expiryDate);

        return Ok(customerList);
    }

    [HttpDelete("ClearCache")]
    public IActionResult ClearCache()
    {
        // Tüm cache'i temizle
        _cacheService.ClearAll();

        return Ok("Cache temizlendi.");
    }
}
