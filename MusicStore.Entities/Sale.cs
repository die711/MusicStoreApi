namespace MusicStore.Entities;

public class Sale : EntityBase
{
    public Customer Customer { get; set; } = default!;
    public long CustomerId { get; set; }
    public Concert Concert { get; set; } = default!;
    public long ConcertId { get; set; }
    public DateTime SaleDate { get; set; }
    public string OperationNumber { get; set; } = default!;
    public decimal Total { get; set; }
    public short Quantity { get; set; }
}