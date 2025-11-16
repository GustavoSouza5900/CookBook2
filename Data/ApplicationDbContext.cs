using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CookBook.Models;

namespace CookBook.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Receita> Receita { get; set; }
    public DbSet<Ingrediente> Ingrediente { get; set; }
    public DbSet<Comentario> Comentario { get; set; }
    public DbSet<ReceitaIngrediente> ReceitaIngrediente { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // MUITO IMPORTANTE: Garante que as configurações padrão do Identity sejam aplicadas primeiro
        base.OnModelCreating(builder);

        // Configuração para a Chave Primária Composta da Tabela de Junção N:N
        builder.Entity<ReceitaIngrediente>()
            .HasKey(ri => new { ri.ReceitaId, ri.IngredienteId });
        
        builder.Entity<ReceitaIngrediente>()
            .HasOne(ri => ri.Receita)
            .WithMany(r => r.ReceitaIngredientes)
            .HasForeignKey(ri => ri.ReceitaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configuração da Relação 1:N entre Comentário e o Usuário (IdentityUser)
        builder.Entity<Comentario>()
            .HasOne(c => c.User) // Um Comentário tem Um Usuário
            .WithMany() // Um Usuário tem Muitos Comentários (sem uma coleção direta no IdentityUser)
            .HasForeignKey(c => c.UserId) // A chave estrangeira é o UserId
            .OnDelete(DeleteBehavior.Restrict); // Evita deletar o usuário se ele tiver comentários

        builder.Entity<Comentario>()
            .HasOne(c => c.Receita)
            .WithMany(r => r.Comentarios) // Relação configurada no Model Receita
            .HasForeignKey(c => c.ReceitaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configuração da Relação 1:N entre Receita e o Usuário (IdentityUser)
        builder.Entity<Receita>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict); 

        builder.Entity<Ingrediente>().HasData(
                new Ingrediente { Id = 1, Nome = "Arroz" },
                new Ingrediente { Id = 2, Nome = "Feijão" },
                new Ingrediente { Id = 3, Nome = "Frango" },
                new Ingrediente { Id = 4, Nome = "Carne Moída" },
                new Ingrediente { Id = 5, Nome = "Cebola" },
                new Ingrediente { Id = 6, Nome = "Alho" },
                new Ingrediente { Id = 7, Nome = "Azeite de Oliva" }
            );
    }
}
