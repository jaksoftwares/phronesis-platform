using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phronesis.Domain.Commerce;

namespace Phronesis.Infrastructure.Persistence.Configurations;

public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.ToTable("SubscriptionPlans");
        builder.HasKey(sp => sp.Id);

        builder.Property(sp => sp.Name).IsRequired().HasMaxLength(100);
        builder.Property(sp => sp.Description).HasMaxLength(500);
        builder.Property(sp => sp.PlanCode).IsRequired().HasMaxLength(50);
        
        builder.HasIndex(sp => sp.PlanCode).IsUnique();

        builder.Property(sp => sp.Price).HasColumnType("decimal(18,2)");
        builder.Property(sp => sp.Currency).IsRequired().HasMaxLength(3);
    }
}

public class UserSubscriptionConfiguration : IEntityTypeConfiguration<UserSubscription>
{
    public void Configure(EntityTypeBuilder<UserSubscription> builder)
    {
        builder.ToTable("UserSubscriptions");
        builder.HasKey(us => us.Id);

        builder.HasIndex(us => us.UserId);

        builder.HasOne(us => us.User)
            .WithMany()
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(us => us.Plan)
            .WithMany()
            .HasForeignKey(us => us.SubscriptionPlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.ToTable("PaymentTransactions");
        builder.HasKey(pt => pt.Id);

        builder.HasIndex(pt => pt.UserId);
        builder.HasIndex(pt => pt.ProviderTransactionId).IsUnique().HasFilter("[ProviderTransactionId] IS NOT NULL");
        builder.HasIndex(pt => pt.ReferenceId);

        builder.Property(pt => pt.Amount).HasColumnType("decimal(18,2)");
        builder.Property(pt => pt.Currency).IsRequired().HasMaxLength(3);
        builder.Property(pt => pt.ProviderTransactionId).HasMaxLength(100);
        builder.Property(pt => pt.ReferenceId).IsRequired().HasMaxLength(100);

        builder.HasOne(pt => pt.User)
            .WithMany()
            .HasForeignKey(pt => pt.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(o => o.Id);

        builder.HasIndex(o => o.OrderNumber).IsUnique();
        builder.Property(o => o.OrderNumber).IsRequired().HasMaxLength(50);
        builder.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
        builder.Property(o => o.Currency).IsRequired().HasMaxLength(3);

        builder.HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.PaymentTransaction)
            .WithMany()
            .HasForeignKey(o => o.PaymentTransactionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");
        builder.HasKey(i => i.Id);

        builder.HasIndex(i => i.InvoiceNumber).IsUnique();
        builder.Property(i => i.InvoiceNumber).IsRequired().HasMaxLength(50);
        builder.Property(i => i.PdfUri).HasMaxLength(500);

        builder.HasOne(i => i.Order)
            .WithOne(o => o.Invoice)
            .HasForeignKey<Invoice>(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class RefundRequestConfiguration : IEntityTypeConfiguration<RefundRequest>
{
    public void Configure(EntityTypeBuilder<RefundRequest> builder)
    {
        builder.ToTable("RefundRequests");
        builder.HasKey(rr => rr.Id);

        builder.Property(rr => rr.Amount).HasColumnType("decimal(18,2)");
        builder.Property(rr => rr.Reason).IsRequired().HasMaxLength(500);
        builder.Property(rr => rr.AdminNotes).HasMaxLength(1000);

        builder.HasOne(rr => rr.User)
            .WithMany()
            .HasForeignKey(rr => rr.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(rr => rr.PaymentTransaction)
            .WithMany()
            .HasForeignKey(rr => rr.PaymentTransactionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
