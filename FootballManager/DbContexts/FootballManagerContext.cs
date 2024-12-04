using Microsoft.EntityFrameworkCore;
using FootballManager.Entities;
using Bogus;
using Bogus.DataSets;
using FootballManager.Models;
using Microsoft.VisualBasic;

namespace FootballManager.DbContexts
{
    public class FootballManagerContext : DbContext
    {
        public DbSet<Coach> Coaches { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<League> Leagues { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Standing> Standings { get; set; }
        public DbSet<Team> Teams { get; set; }

        public FootballManagerContext(DbContextOptions options) : base(options)
        { }       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Standing>()
                .HasKey(s => new { s.TeamId, s.LeagueYear });

            modelBuilder.Entity<Player>()
                .HasOne(p => p.Team) 
                .WithMany(t => t.Players) 
                .HasForeignKey(p => p.TeamId) 
                .IsRequired(false); 

            modelBuilder.Entity<Game>(entity =>
            {
                // HomeTeam relationship
                entity
                    .HasOne(g => g.HomeTeam)
                    .WithMany()
                    .HasForeignKey(g => g.HomeTeamId)
                    .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes

                // AwayTeam relationship
                entity
                    .HasOne(g => g.AwayTeam)
                    .WithMany()
                    .HasForeignKey(g => g.AwayTeamId)
                    .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes
            });
        }
        
    }
}
