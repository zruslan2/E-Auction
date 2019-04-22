using E_Auction.Core.DataModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Auction.Infrastructure
{
    public class AplicationDbContext : DbContext
    {
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<AuctionFileMeta> AuctionFiles { get; set; }
        public DbSet<AuctionCategory> AuctionCategories { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrganizationType> OrganizationTypes { get; set; }
        public DbSet<OrganizationRating> OrganizationRatings { get; set; }
        public DbSet<OrganizationFile> OrganizationFiles { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeePosition> EmployeePositions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserPasswordHistory> UserPasswordHistorys { get; set; }
        public DbSet<UserAutorizationHistory> UserAutorizationHistorys { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Organization>()
                .HasMany(p => p.Auctions)
                .WithRequired(p => p.Organization)
                .HasForeignKey(p => p.OrganizationId);
            modelBuilder
                .Entity<Organization>()
                .HasMany(p => p.Bids)
                .WithRequired(p => p.Organization)
                .HasForeignKey(p => p.OrganizationId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<OrganizationType>()
                .HasMany(p => p.Organizations)
                .WithRequired(p => p.OrganizationType)
                .HasForeignKey(p => p.OrganizationTypeId);
            modelBuilder
                .Entity<Organization>()
                .HasMany(p => p.OrganizationRatings)
                .WithRequired(p => p.Organization)
                .HasForeignKey(p => p.OrganizationId);
            modelBuilder
                .Entity<Organization>()
                .HasMany(p => p.OrganizationFiles)
                .WithRequired(p => p.Organization)
                .HasForeignKey(p => p.OrganizationId);
            modelBuilder
                .Entity<Organization>()
                .HasMany(p => p.Transactions)
                .WithRequired(p => p.Organization)
                .HasForeignKey(p => p.OrganizationId);

            modelBuilder
                .Entity<Auction>()
                .HasRequired(p => p.Category)
                .WithMany(p => p.Auctions)
                .HasForeignKey(p => p.CategoryId);
            modelBuilder
                .Entity<Auction>()
                .HasMany(p => p.Files)
                .WithRequired(p => p.Auction)
                .HasForeignKey(p => p.AuctionId);
            modelBuilder
                .Entity<Auction>()
                .HasMany(p => p.Bids)
                .WithRequired(p => p.Auction)
                .HasForeignKey(p => p.AuctionId);

            modelBuilder.Entity<EmployeePosition>()
                .HasMany(p => p.Employees)
                .WithRequired(p => p.EmployeePosition)
                .HasForeignKey(p => p.EmployeePositionId);

            modelBuilder
                .Entity<User>()
                .HasMany(p => p.UserPasswordHistories)
                .WithRequired(p => p.User)
                .HasForeignKey(p => p.UserId);
            modelBuilder
                .Entity<User>()
                .HasMany(p => p.UserAutorizationHistory)
                .WithRequired(p => p.User)
                .HasForeignKey(p => p.UserId);          

            base.OnModelCreating(modelBuilder);
        }

        public AplicationDbContext() : base("E-AuctionConnectionString")
        {

        }
    }
}
