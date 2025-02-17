using DEA.Next.Entities;

namespace DEA.UI.HelperClasses
{
    public static class SearchMethods
    {
        public static List<CustomerDetails> SearchCustomers(List<CustomerDetails> customers, string searchText, string searchType)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                MessageBox.Show("Please enter a search text.", "Search Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return customers;
            }

            searchText = searchText.Trim();

            return searchType switch
            {
                "Id" => Guid.TryParse(searchText, out Guid customerId)
                        ? customers.Where(c => c.Id == customerId).ToList()
                        : throw new ArgumentException("Invalid Customer ID format.", nameof(searchText)),
                "ProjectId" => customers.Where(c => c.ProjectId.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList(),
                "CustomerName" => customers.Where(c => c.CustomerName.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList(),
                _ => throw new ArgumentException("Invalid search type.", nameof(searchType)),
            };
        }
    }
}
