using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phronesis.Domain.Support;

namespace Phronesis.Infrastructure.Persistence.Configurations;

public class SupportTicketConfiguration : IEntityTypeConfiguration<SupportTicket>
{
    public void Configure(EntityTypeBuilder<SupportTicket> builder)
    {
        builder.ToTable("SupportTickets");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Category).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Subject).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Description).IsRequired().HasMaxLength(4000);
        builder.Property(t => t.Status).IsRequired().HasMaxLength(50);
        builder.Property(t => t.Priority).IsRequired().HasMaxLength(50);

        builder.HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.AssignedAgent)
            .WithMany()
            .HasForeignKey(t => t.AssignedAgentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class TicketMessageConfiguration : IEntityTypeConfiguration<TicketMessage>
{
    public void Configure(EntityTypeBuilder<TicketMessage> builder)
    {
        builder.ToTable("TicketMessages");
        builder.HasKey(tm => tm.Id);

        builder.Property(tm => tm.Content).IsRequired().HasMaxLength(4000);

        builder.HasOne(tm => tm.SupportTicket)
            .WithMany(t => t.Messages)
            .HasForeignKey(tm => tm.SupportTicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tm => tm.Sender)
            .WithMany()
            .HasForeignKey(tm => tm.SenderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
