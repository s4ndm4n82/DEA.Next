using DEA.Next.HelperClasses.ConfigFileFunctions;

namespace DEA.Next.Extensions;

public static class CreateUrl
{
    public static async Task<(string MainDomain, string Quer)> SplitUrl(this Guid clientId)
    {
        var customerDetails = await UserConfigRetriever.RetrieveUserConfigById(clientId);
        var url = customerDetails.Domain;
        
        var index = url.LastIndexOf('/');
        if (index == -1) return (url, string.Empty);
        
        var mainDomain = url[..index];
        var query = url[(index + 1)..];
        
        return (mainDomain, query);
    }
}