using Microsoft.EntityFrameworkCore;
using RulesEngineDemo.Domain.Entities;

namespace RulesEngineDemo.Infrastructure.Data;

public class LeaveDbContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }
    public DbSet<LeaveRequest> LeaveRequests { get; set; }

    public LeaveDbContext(DbContextOptions<LeaveDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
