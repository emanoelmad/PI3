using AppShowDoMilhao.Models;
using AppShowDoMilhao.Models.PartidaModel;
using AppShowDoMilhao.Models.PerguntaModel;
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
        public DbSet<Pergunta> Perguntas { get; set; }
        public DbSet<Partida> Partidas { get; set; }
        public DbSet<PerguntaAprovacao> PerguntaAprovacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>()
                .ToTable("Usuarios");

            modelBuilder.Entity<Pergunta>()
            .ToTable("Perguntas");

            modelBuilder.Entity<Partida>()
                .ToTable("Partidas");

            modelBuilder.Entity<PerguntaAprovacao>()
                .ToTable("PerguntaAprovacoes");
        }
    }
}
