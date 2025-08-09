namespace Core.Specifications;

public class OrderSpecParams : PagingParams
{
    // filter orders by status (1 status at a time)
    public string? Status { get; set; }
}
