namespace Core.Entities;
// not an entity! not using EF Core
// shopping cart is controlled mostly by client
// using Redis as a way to store client side state
// still giving us ability to access this from api
// potential to get marketing info of what ppl put to cart and remove from it 
public class ShoppingCart
{
    public required string Id { get; set; }
    public List<CartItem> Items { get; set; } = [];
    public int? DeliveryMethodId { get; set; }
    public string? ClientSecret { get; set; } // get this back when we craete a payment intent 
    // needed when user wants to make payment => then use client secret to do that from client browser to stripe
    public string? PaymentIntentId { get; set; } // can be changed if user changes cart content or delivery method
    public AppCoupon? Coupon { get; set; }
}
