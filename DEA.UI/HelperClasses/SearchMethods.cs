using DEA.Next.Entities;

namespace DEA.UI.HelperClasses
{
    public static class SearchMethods
    {
        public static List<CustomerDetails> SearchCustomers(List<CustomerDetails> customers, string searchText, string searchType)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                throw new ArgumentException("Search text cannot be empty or null.", nameof(searchText));
            }

            searchText = searchText.Trim();

            return searchType switch
            {
                "Id" => Guid.TryParse(searchText, out Guid customerId)
                        ? [.. customers.Where(c => c.Id == customerId)]
                        : throw new ArgumentException("Invalid Customer ID format.", nameof(searchText)),
                "ProjectId" => [.. customers
                .Where(c => c.ProjectId
                .Contains(searchText, StringComparison.OrdinalIgnoreCase))],
                "CustomerName" => [.. customers
                .Where(c => c.CustomerName
                .Contains(searchText, StringComparison.OrdinalIgnoreCase))],
                _ => throw new ArgumentException("Invalid search type.", nameof(searchType)),
            };
        }
    }
}
