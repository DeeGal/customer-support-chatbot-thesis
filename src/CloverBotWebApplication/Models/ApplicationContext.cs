using Microsoft.EntityFrameworkCore;
using CloverBotWebApplication.Models;

namespace CloverBotWebApplication.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        // Conversation logs for storing each chat exchange
        public DbSet<ConversationLog> ConversationLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ConversationLog table mapping
            modelBuilder.Entity<ConversationLog>(entity =>
            {
                entity.ToTable("ConversationLogs");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.UserId)
                    .HasMaxLength(100);

                entity.Property(e => e.UserMessage)
                    .IsRequired();

                entity.Property(e => e.BotResponse)
                    .IsRequired();

                entity.Property(e => e.Timestamp)
                    .HasDefaultValueSql("GETDATE()");
            });           
        }
    }
}
