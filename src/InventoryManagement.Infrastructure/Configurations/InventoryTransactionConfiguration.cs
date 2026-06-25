using InventoryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Configurations
{
    /// <summary>
    /// Fluent API configuration for the <see cref="InventoryTransaction"/> entity.
    /// </summary>
    public class InventoryTransactionConfiguration : IEntityTypeConfiguration<InventoryTransaction>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
        {
            builder.ToTable("InventoryTransactions");
            builder.HasKey(t => t.TransactionId);

            builder.Property(t => t.QuantityChanged)
                .IsRequired();

            builder.Property(t => t.TransactionType)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(t => t.TransactionDate)
                .IsRequired();

            builder.Property(t => t.Notes)
                .HasMaxLength(500);

            builder.HasOne(t => t.Product)
                .WithMany(p => p.InventoryTransactions)
                .HasForeignKey(t => t.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}