using DEA.Next.Data;
using Microsoft.EntityFrameworkCore;

namespace DEA.UI.HelperClasses
{
    internal class CheckDbConnection(DataContext context)
    {
        private readonly DataContext _context = context;

        public async Task<bool> CheckDataBaseExistsAsync(string databaseName)
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT 1 FROM pg_database WHERE datname = @databaseName";
                var parameter = command.CreateParameter();
                parameter.ParameterName = "databaseName";
                parameter.Value = databaseName;
                command.Parameters.Add(parameter);

                var result = await command.ExecuteScalarAsync();
                return result != null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to connect to the database. Error: {ex.Message}",
                    "Database Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
