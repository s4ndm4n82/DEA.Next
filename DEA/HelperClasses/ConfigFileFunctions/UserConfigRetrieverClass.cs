using DEA.Next.Entities;
using DEA.Next.Interfaces;

namespace DEA.Next.HelperClasses.ConfigFileFunctions;

internal class UserConfigRetriever
{
    private static IUserConfigRepository? _repository;
    public static void Initialize(IUserConfigRepository repository)
    {
        _repository = repository;
    }

    public static async Task<IEnumerable<CustomerDetails>> RetrieveAllUserConfig()
    {
        if (_repository is null) throw new InvalidOperationException("Service provider not registered ...");
        
        return await _repository.GetAllCustomerDetails();
    }
    
    public static async Task<CustomerDetails> RetrieveUserConfigById(Guid cid)
    {
        if (_repository is null) throw new InvalidOperationException("Service provider not registered ...");
        
        return await _repository.GetClientDetailsById(cid);
    }

    public static async Task<FtpDetails> RetrieveFtpConfigById(Guid cid)
    {
        if (_repository is null) throw new InvalidOperationException("Service provider not registered ...");
        
        return await _repository.GetFtpDetailsById(cid);
    }
    
    public static async Task<EmailDetails> RetrieveEmailConfigById(Guid cid)
    {
        if (_repository is null) throw new InvalidOperationException("Service provider not registered ...");
        
        return await _repository.GetEmailDetailsById(cid);
    }
    
    public static async Task<IEnumerable<DocumentDetails>> RetrieveDocumentConfigById(Guid cid)
    {
        if (_repository is null) throw new InvalidOperationException("Service provider not registered ...");
        
        return await _repository.GetDocumentDetailsById(cid);
    }
}