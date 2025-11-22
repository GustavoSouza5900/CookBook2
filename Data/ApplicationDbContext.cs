using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore; // ESSENCIAL para DbContext
using CookBook.Models;

namespace CookBook.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Receita> Receitas { get; set; } // <--- Corrigindo o erro!
    public DbSet<Receita> Receita { get; set; }
    public DbSet<Ingrediente> Ingrediente { get; set; }
    public DbSet<Comentario> Comentario { get; set; }
    public DbSet<ReceitaIngrediente> ReceitaIngrediente { get; set; }
    public DbSet<ReceitaCurtida> ReceitaCurtida { get; set; }
    public DbSet<ReceitaSalva> ReceitaSalva { get; set; }
    public DbSet<Badge> Badge { get; set; }
    public DbSet<UserBadge> UserBadge { get; set; }

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

        builder.Entity<ReceitaCurtida>()
            .HasKey(rc => new { rc.ReceitaId, rc.UserId });
        
        builder.Entity<ReceitaCurtida>()
            .HasOne(rc => rc.Receita)
            .WithMany(r => r.ReceitaCurtidas)
            .HasForeignKey(rc => rc.ReceitaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ReceitaCurtida>()
            .HasOne(rc => rc.User)
            .WithMany() // User não precisa de uma coleção de Curtidas
            .HasForeignKey(rc => rc.UserId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.Entity<ReceitaSalva>()
                .HasKey(rs => new { rs.ReceitaId, rs.UserId });

        builder.Entity<ReceitaSalva>()
            .HasOne(rs => rs.Receita)
            .WithMany(r => r.ReceitaSalvas)
            .HasForeignKey(rs => rs.ReceitaId)
            .OnDelete(DeleteBehavior.Cascade); 
        
        builder.Entity<ReceitaSalva>()
            .HasOne(rs => rs.User)
            .WithMany()
            .HasForeignKey(rs => rs.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<UserBadge>()
            .HasKey(ub => new { ub.UserId, ub.BadgeId });
        
        builder.Entity<UserBadge>()
            .HasOne(ub => ub.Badge)
            .WithMany() 
            .HasForeignKey(ub => ub.BadgeId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Entity<UserBadge>()
            .HasOne(ub => ub.User)
            .WithMany() // ApplicationUser não precisa de uma lista de UserBadges
            .HasForeignKey(ub => ub.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Badge>().HasData(
            new Badge { 
                Id = 1,
                Name = "Chef Novato", 
                Description = "Publique sua primeira receita.", 
                IconClass = "fas fa-star", 
                TriggerEvent = "RECIPE_COUNT_1" 
            },
            new Badge { 
                Id = 2,
                Name = "Mestre Culinário", 
                Description = "Publique 10 receitas.", 
                IconClass = "fas fa-trophy", 
                TriggerEvent = "RECIPE_COUNT_10" 
            },
            new Badge { 
                Id = 3,
                Name = "Primeira de Muitas", 
                Description = "Receba 1 curtidas no total.", 
                IconClass = "fas fa-heart", 
                TriggerEvent = "TOTAL_LIKES_1" 
            },
            new Badge { 
                Id = 4,
                Name = "Popular", 
                Description = "Receba 50 curtidas no total.", 
                IconClass = "fas fa-fire", 
                TriggerEvent = "TOTAL_LIKES_50" 
            },
            new Badge { 
                Id = 5,
                Name = "Opnião Própria", 
                Description = "Publique seu primeiro comentário", 
                IconClass = "fas fa-comment", 
                TriggerEvent = "COMMENT_COUNT_1" 
            },
            new Badge { 
                Id = 6,
                Name = "Comentarista Ativo", 
                Description = "Publique 5 comentários.", 
                IconClass = "fas fa-comments", 
                TriggerEvent = "COMMENT_COUNT_5" 
            }
        );
    }
}
