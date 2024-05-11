using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.Problems;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database;

public class StakeholdersContext : DbContext
{

    public DbSet<User> Users { get; set; }
    public DbSet<Person> People { get; set; }
    public DbSet<Follower> Followers { get; set; }
   
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<Problem> Problem { get; set; }
    public DbSet<ProblemResolvingNotification> ProblemResolvingNotifications { get; set; }
    public DbSet<Message> Messages { get; set; }

    public StakeholdersContext(DbContextOptions<StakeholdersContext> options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("stakeholders");

        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

        modelBuilder.Entity<Problem>().Property(item => item.Answer).HasColumnType("jsonb").IsRequired(false);

        modelBuilder.Entity<Problem>().Property(item => item.Comments).HasColumnType("jsonb");

        ConfigureStakeholder(modelBuilder);
    }

    private static void ConfigureStakeholder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>()
            .HasOne(r => r.User)
            .WithOne()
            .HasForeignKey<Person>(s => s.UserId);
        

        modelBuilder.Entity<Rating>()
           .HasOne(s => s.User)
           .WithOne()
           .HasForeignKey<Rating>(s => s.UserId);


    }
}