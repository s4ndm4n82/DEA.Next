using DEA.Next.Entities;

namespace DEA.Next.Interfaces;

public interface IUserConfigRepository
{
    Task<IEnumerable<CustomerDetails>> GetAllCustomerDetails();
    Task<CustomerDetails> GetCustomerDetailsById(Guid id);
    Task<FtpDetails> GetFtpDetailsById(Guid id);
    Task<EmailDetails> GetEmailDetailsById(Guid id);
    Task<CustomerDetails> GetClientByDeliveryMethod(string deliveryMethod);
}