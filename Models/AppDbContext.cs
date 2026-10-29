using Microsoft.EntityFrameworkCore;
using MyDotNetApi.Models;

namespace MyDotNetApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Khai báo bảng Patients trong SQL Server
    public DbSet<Patient> Patients => Set<Patient>();
}