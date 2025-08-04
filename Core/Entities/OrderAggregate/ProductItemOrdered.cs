namespace Core.Entities.OrderAggregate;

public class ProductItemOrdered // owned entity, remain the same in history even if later product changed after the item was ordered
{
    public int ProductId { get; set; }
    public required string ProductName { get; set; }
    public required string PictureUrl { get; set; }
}
