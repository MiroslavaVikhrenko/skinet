using Core.Entities;

namespace Core.Interfaces;

public interface IPaymentService
{
    Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId);
    // backend doesn't take any credit cardinfo from user, backend communicate with stripe
    // payment on client's side - communication between client's browser and stripe services
    // but payment only accepted if there is already payment intent in place
}
