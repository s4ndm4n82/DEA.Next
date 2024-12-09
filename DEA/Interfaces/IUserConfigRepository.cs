using DEA.Next.Entities;

namespace DEA.Next.Interfaces;

public interface IUserConfigRepository
{
    Task<IEnumerable<CustomerDetails>> GetAllCustomerDetails();
    Task<CustomerDetails> GetClientDetailsById(Guid id);
    Task<FtpDetails> GetFtpDetailsById(Guid id);
    Task<EmailDetails> GetEmailDetailsById(Guid id);
    Task<IEnumerable<DocumentDetails>> GetDocumentDetailsById(Guid id);
}