using Microsoft.EntityFrameworkCore;

namespace Matheus.Modelos;

public class AppDataContext : DbContext
{
    public DbSet<Funcionario> Funcionarios{get; set;}
    public DbSet<Folha> Folhas{get; set;}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=jean_matheus.db");
    }
}