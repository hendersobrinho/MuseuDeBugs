using Microsoft.EntityFrameworkCore; // traz as peças do EF Core que estou usando aqui, tipo DbContext, DbSet<> e DbContextOptions<>
using MuseuDeBugs.Api.Entities; // para reconhecer a entidade Bug dentro desse arquivo

namespace MuseuDeBugs.Api.Data
{
    public class AppDbContext : DbContext // esse é o contexto da aplicação no EF Core; herdo de DbContext para ter os recursos de contexto/banco
    {
        // Aqui eu apresento a entidade Bug para o EF Core saber que ele irá trabalhar com ela
        public DbSet<Bug> Bugs { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        // aqui ele recebe do Program.cs as configurações do contexto, como provider e conexão, e repassa isso para a classe base DbContext
        {
        }
    }
}