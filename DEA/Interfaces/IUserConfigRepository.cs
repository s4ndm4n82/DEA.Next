using DEA.Next.Entities;

namespace DEA.Next.Interfaces;

public interface IUserConfigRepository
{
    Task<IEnumerable<CustomerDetails>> GetAllCustomerDetails();
    Task<CustomerDetails> GetClientDetailsById(Guid id);
    Task<CustomerDetails> GetFtpDetailsById(Guid id);
    Task<CustomerDetails> GetEmailDetailsById(Guid id);
    Task<IEnumerable<DocumentDetails>> GetDocumentDetailsById(Guid id);
}