using DEA.Next.Entities;
using Microsoft.EntityFrameworkCore;

namespace DEA.Next.Data;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<CustomerDetails> CustomerDetails { get; set; }
}