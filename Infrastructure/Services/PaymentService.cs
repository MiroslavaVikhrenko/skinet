using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Infrastructure.Services;

public class PaymentService(IConfiguration config, ICartService cartService,
    IGenericRepository<Core.Entities.Product> productRepo,
    IGenericRepository<DeliveryMethod> dmRepo) : IPaymentService
{
    public async Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId)
    {
        StripeConfiguration.ApiKey = config["StripeSettings:SecretKey"];

        // Check and validate the cart
        var cart = await cartService.GetCartAsync(cartId);

        if (cart == null) return null;

        var shippingPrice = 0m; //decimal

        // DeliveryMethodId prop is optional => have access to HasValue

        if (cart.DeliveryMethodId.HasValue)
        {
            var deliveryMethod = await dmRepo.GetByIdAsync((int)cart.DeliveryMethodId);

            if (deliveryMethod == null) return null; // cant continue if dont have details of selected delivery method

            shippingPrice = deliveryMethod.Price;
        }

        // Validate and update the items in the cart (if needed)
        foreach (var item in cart.Items)
        {
            // for each item in cart => get from db
            var productItem = await productRepo.GetByIdAsync(item.ProductId);

            if (productItem == null) return null;

            // validate price in cart towards db
            if (item.Price != productItem.Price)
            {
                item.Price = productItem.Price; // ensure user is paying correct price
            }
        }

        var service = new PaymentIntentService(); // from Stripe
        PaymentIntent? intent = null; //initially

        // Check if we already have payment intent ID in the shopping cart
        if (string.IsNullOrEmpty(cart.PaymentIntentId))
        {
            // if there is none => create a new payment intent => create options to provide to Stripe
            var options = new PaymentIntentCreateOptions
            {
                // convert to long type as that's what Stripe using for amount
                Amount = (long)cart.Items.Sum(x => x.Quantity * (x.Price * 100)) + (long)shippingPrice * 100,
                Currency = "usd",
                PaymentMethodTypes = ["card"]
            };
            // update intent
            intent = await service.CreateAsync(options);
            // update cart as need to store payment intent ID and the client secret that we get back after have created payment intent
            cart.PaymentIntentId = intent.Id;
            cart.ClientSecret = intent.ClientSecret;
        }
        else
        {
            // if already have payment intent id then need to update amount in existing payment intent
            var options = new PaymentIntentUpdateOptions
            {
                Amount = (long)cart.Items.Sum(x => x.Quantity * (x.Price * 100)) + (long)shippingPrice * 100
            };
            intent = await service.UpdateAsync(cart.PaymentIntentId, options);
        }

        await cartService.SetCartAsync(cart);

        return cart;
    }
}
