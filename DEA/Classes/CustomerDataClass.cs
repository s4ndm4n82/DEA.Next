using DEA.Next.Data;
using DEA.Next.Entities;
using Microsoft.EntityFrameworkCore;

namespace DEA.Next.Classes;

public class CustomerDataClass
{
    private readonly DataContext _context;

    public CustomerDataClass(DataContext context)
    {
        _context = context;
    }
    
    public async Task<List<CustomerDetails>> GetAllCustomers()
    {
        return await _context.CustomerDetails.ToListAsync();
    }
}