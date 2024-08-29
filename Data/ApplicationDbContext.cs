using AppShowDoMilhao.Models;
using AppShowDoMilhao.Models.UsuarioModel;
using Microsoft.EntityFrameworkCore;

namespace AppShowDoMilhao.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Pergunta> Perguntas { get; set; }  // Corrigido para DbSet<Pergunta>

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>()
                .ToTable("Usuarios");

            modelBuilder.Entity<Pergunta>()
                .ToTable("Perguntas");
        }
    }
}
