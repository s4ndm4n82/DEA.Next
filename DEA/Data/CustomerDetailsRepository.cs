using DEA.Next.Entities;
using DEA.Next.HelperClasses.OtherFunctions;
using DEA.Next.Interfaces;
using Microsoft.EntityFrameworkCore;
using WriteLog;

namespace DEA.Next.Data;

public class CustomerDetailsRepository : IUserConfigRepository
{
    private readonly DataContext _context;

    public CustomerDetailsRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CustomerDetails>> GetAllCustomerDetails()
    {
        return await _context.CustomerDetails.ToListAsync();
    }

    public async Task<CustomerDetails> GetCustomerDetailsById(Guid id)
    {
        var customer = await _context.CustomerDetails.FindAsync(id);

        if (customer != null) return customer;
        
        WriteLogClass.WriteToLog(0, $"Customer with id {id} not found ....", 3);
        throw new NullReferenceException($"Customer with id {id} not found ....");
    }

    public async Task<FtpDetails> GetFtpDetailsById(Guid id)
    {
        var customerFtp = await _context.CustomerDetails
            .SelectMany(f => f.FtpDetails)
            .FirstOrDefaultAsync(g => g.CustomerDetailsId.Equals(id));
        
        if (customerFtp != null) return customerFtp;
        
        WriteLogClass.WriteToLog(0, $"Customer with id {id} not found ....", 3);
        throw new NullReferenceException($"Customer with id {id} not found ....");
    }

    public async Task<EmailDetails> GetEmailDetailsById(Guid id)
    {
        
    }

    public async Task<CustomerDetails> GetClientByDeliveryMethod(string deliveryMethod)
    {
        throw new NotImplementedException();
    }
}