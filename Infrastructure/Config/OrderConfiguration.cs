using Core.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.OwnsOne(x => x.ShippingAddress, o => o.WithOwner()); // order owns shipping address
        builder.OwnsOne(x => x.PaymentSummary, o => o.WithOwner()); // order owns payment summary
        builder.Property(x => x.Status).HasConversion(
            o => o.ToString(),
            o => (OrderStatus)Enum.Parse(typeof(OrderStatus), o)
        ); // enum in string
        // configure decimals
        builder.Property(x => x.Subtotal).HasColumnType("decimal(18,2)");
        // configure relationships => 1 order can have many items
        builder.HasMany(x => x.OrderItems).WithOne().OnDelete(DeleteBehavior.Cascade);
        // datetime property configuration to prevent browser issues
        builder.Property(x => x.OrderDate).HasConversion(
            d => d.ToUniversalTime(),
            d => DateTime.SpecifyKind(d, DateTimeKind.Utc) // ensure no weirdness when return date from sql db
        ); // conversion from utc to local time is left for browser


    }
}
