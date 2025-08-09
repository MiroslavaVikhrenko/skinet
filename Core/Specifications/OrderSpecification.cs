using Core.Entities.OrderAggregate;

namespace Core.Specifications;

public class OrderSpecification : BaseSpecification<Order>
{
    public OrderSpecification(string email) : base(x => x.BuyerEmail == email)
    {
        AddInclude(x => x.OrderItems);
        AddInclude(x => x.DeliveryMethod);
        AddOrderByDescending(x => x.OrderDate); // order by latest date
    }

    // id = order id
    public OrderSpecification(string email, int id) : base(x => x.BuyerEmail == email && x.Id == id)
    {
        AddInclude("OrderItems");
        AddInclude("DeliveryMethod");
    }

    // second param for distinction from 1st constructor to avoid creating a different class
    public OrderSpecification(string paymentIntentId, bool isPaymentIntent) : base(x => x.PaymentIntentId == paymentIntentId)
    {
        AddInclude("OrderItems");
        AddInclude("DeliveryMethod");
    }

    // ADMIN 
    public OrderSpecification(OrderSpecParams specParams) : base(x =>
        string.IsNullOrEmpty(specParams.Status) || x.Status == ParseStatus(specParams.Status)
    )
    {
        AddInclude("OrderItems");
        AddInclude("DeliveryMethod");
        ApplyPaging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
        AddOrderByDescending(x => x.OrderDate);
    }
    // get order by id for admin
    public OrderSpecification(int id) : base(x => x.Id == id)
    {
        AddInclude("OrderItems");
        AddInclude("DeliveryMethod");
    }

    // helper method as Status is enum
    private static OrderStatus? ParseStatus(string status)
    {
        // 2nd param is ignoreCase (true) 
        if (Enum.TryParse<OrderStatus>(status, true, out var result)) return result;
        return null; // no filtering will take place
    }
}
