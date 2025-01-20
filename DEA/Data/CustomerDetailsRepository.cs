using DEA.Next.Entities;
using DEA.Next.Interfaces;
using Microsoft.EntityFrameworkCore;
using WriteLog;

namespace DEA.Next.Data;

public class CustomerDetailsRepository(DataContext context) : IUserConfigRepository
{
    public async Task<IEnumerable<CustomerDetails>> GetAllCustomerDetails()
    {
        try
        {
            return await context.CustomerDetails.ToListAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<CustomerDetails> GetClientDetailsById(Guid id)
    {
        var customer = await context.CustomerDetails.FindAsync(id);

        if (customer != null) return customer;
        
        WriteLogClass.WriteToLog(0, $"Customer with id {id} not found ....", 3);
        throw new NullReferenceException($"Customer with id {id} not found ....");
    }

    public async Task<CustomerDetails> GetFtpDetailsById(Guid id)
    {
        var customerFtp = await context.CustomerDetails
            .Include(f => f.FtpDetails)
            .FirstOrDefaultAsync(g => g.Id.Equals(id));
        
        if (customerFtp != null) return customerFtp;
        
        WriteLogClass.WriteToLog(0, $"Customer with id {id} not found ....", 3);
        throw new NullReferenceException($"Customer with id {id} not found ....");
    }

    public async Task<CustomerDetails> GetEmailDetailsById(Guid id)
    {
        var customerEmail = await context.CustomerDetails
            .Include(e => e.EmailDetails)
            .FirstOrDefaultAsync(g => g.Id.Equals(id));
        
        if (customerEmail != null) return customerEmail;
        
        WriteLogClass.WriteToLog(0, $"Customer with id {id} not found ....", 3);
        throw new NullReferenceException($"Customer with id {id} not found ....");
    }

    public async Task<IEnumerable<DocumentDetails>> GetDocumentDetailsById(Guid id)
    {
        return await context.CustomerDetails
            .SelectMany(g => g.DocumentDetails)
            .Where(h => h.CustomerDetailsId.Equals(id))
            .ToListAsync();
    }
}