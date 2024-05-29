using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OperatorMO_ASPNET.DAL.Models;

namespace OperatorMO_ASPNET.DAL
{
    public partial class OperatorContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public OperatorContext(DbContextOptions<OperatorContext> options, IConfiguration configuration)
            : base(options)
        {
            Configuration = configuration;
        }

        public OperatorContext()
        {
        }

        public virtual DbSet<Tariff> Tariff { get; set; }
        public virtual DbSet<Contract> Contract { get; set; }
        public virtual DbSet<Client> Client { get; set; }
        public virtual DbSet<CategoryTransaction> CategoryTransaction { get; set; }
        public virtual DbSet<Chat> Chat { get; set; }
        public virtual DbSet<CostDetails> CostDetails { get; set; }
        public virtual DbSet<RoleUser> RoleUser { get; set; }
        public virtual DbSet<Services> Services { get; set; }
        public virtual DbSet<ServicesConnected> ServicesConnected { get; set; }
        public virtual DbSet<Transactions> Transactions { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Depositing> Depositing { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        => optionsBuilder.UseSqlServer("Server=LAPTOP-QBUR001L\\SQLEXPRESS;Database=OperatorMO;Trusted_Connection=True;Encrypt=False");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tariff>(entity =>
            {
                entity.Property(e => e.TariffId).IsRequired();
            });
            modelBuilder.Entity<CategoryTransaction>(entity =>
            {
                entity.Property(e => e.CategoryTransactionId).IsRequired();
            });
            modelBuilder.Entity<Chat>(entity =>
            {
                entity.Property(e => e.MessageId).IsRequired();
            });
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).IsRequired();
            });
            modelBuilder.Entity<CostDetails>(entity =>
            {
                entity.Property(e => e.CostDetailId).IsRequired();
            });
            modelBuilder.Entity<CostDetails>(entity =>
            {
                entity.HasOne(d => d.Contract)
                    .WithMany(p => p.CostDetails)
                    .HasForeignKey(d => d.ContractId_FK);
            });

            modelBuilder.Entity<Chat>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.Chat)
                    .HasForeignKey(d => d.UserId_FK);
            });
            modelBuilder.Entity<RoleUser>(entity =>
            {
                entity.Property(e => e.RoleId).IsRequired();
            });
            modelBuilder.Entity<Services>(entity =>
            {
                entity.Property(e => e.ServiceId).IsRequired();
            });
            modelBuilder.Entity<ServicesConnected>(entity =>
            {
                entity.Property(e => e.CSId).IsRequired();
            });
            modelBuilder.Entity<Depositing>(entity =>
            {
                entity.Property(e => e.Id).IsRequired();
            });
            modelBuilder.Entity<Depositing>(entity =>
            {
                entity.HasOne(d => d.Contract)
                    .WithMany(p => p.Depositing)
                    .HasForeignKey(d => d.ContractId_FK);
            });
            modelBuilder.Entity<ServicesConnected>(entity =>
            {
                entity.HasOne(d => d.Services)
                    .WithMany(p => p.ServicesConnected)
                    .HasForeignKey(d => d.ServiceId_FK);
            });
            modelBuilder.Entity<ServicesConnected>(entity =>
            {
                entity.HasOne(d => d.Contract)
                    .WithMany(p => p.ServicesConnected)
                    .HasForeignKey(d => d.ContractId_FK);
            });
            modelBuilder.Entity<CategoryTransaction>(entity =>
            {
                entity.Property(e => e.CategoryTransactionId).IsRequired();
            });
            modelBuilder.Entity<Transactions>(entity =>
            {
                entity.Property(e => e.TransactionId).IsRequired();
            });
            modelBuilder.Entity<Transactions>(entity =>
            {
                entity.HasOne(d => d.Tariff)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.TariffId_FK);
            });



            modelBuilder.Entity<CategoryEvents>(entity =>
            {
                entity.Property(e => e.CategoryEventId).IsRequired();
            });

            modelBuilder.Entity<EventsContract>(entity =>
            {
                entity.Property(e => e.Id).IsRequired();
            });
            modelBuilder.Entity<EventsContract>(entity =>
            {
                entity.HasOne(d => d.Contract)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.ContractId_FK);
            });

            modelBuilder.Entity<EventsContract>(entity =>
            {
                entity.HasOne(d => d.CategoryEvents)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.CategoryEventId_FK);
            });





            modelBuilder.Entity<Transactions>(entity =>
            {
                entity.HasOne(d => d.CategoryTransaction)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.CategoryTransaction_FK);
            });
            modelBuilder.Entity<User>()
          .HasOne(u => u.RoleUser)
          .WithMany(r => r.User)
          .HasForeignKey(u => u.RoleId_FK)
          .HasPrincipalKey(r => r.RoleId);


            modelBuilder.Entity<Contract>(entity =>
            {
                entity.Property(e => e.ContractId).IsRequired();
            });
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).IsRequired();
            });
            modelBuilder.Entity<Contract>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.Contract)
                    .HasForeignKey(d => d.UserId_FK);
            });

            modelBuilder.Entity<Contract>(entity =>
            {
                entity.HasOne(d => d.Tariff)
                    .WithMany(p => p.Contract)
                    .HasForeignKey(d => d.TariffId);
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.Property(e => e.ClientId).IsRequired();
            });

            modelBuilder.Entity<Contract>(entity =>
            {
                entity.HasOne(d => d.Client)
                    .WithMany(p => p.Contract)
                    .HasForeignKey(d => d.ClientId_FK);
            });
            modelBuilder.Entity<CostDetails>(entity =>
            {
                entity.HasOne(d => d.CategoryTransaction)
                    .WithMany(p => p.CostDetails)
                    .HasForeignKey(d => d.CategoryTransaction_FK);
            });
            modelBuilder.Entity<WriteOffs>(entity =>
            {
                entity.Property(e => e.Id).IsRequired();
            });
            modelBuilder.Entity<WriteOffs>(entity =>
            {
                entity.HasOne(d => d.Contract)
                    .WithMany(p => p.WriteOffs)
                    .HasForeignKey(d => d.ContractId_FK);
            });
        }
    }
}
