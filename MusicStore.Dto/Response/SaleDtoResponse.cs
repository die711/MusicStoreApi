using Microsoft.VisualBasic.CompilerServices;

namespace MusicStore.Dto.Response;

public class SaleDtoResponse
{
    public long SaleId { get; set; }
    public string DateEvent { get; set; } = default!;
    public String TimeEvent { get; set; } = default!;
    public string Genre { get; set; } = default!;
    public string ImageUrl { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string OperationNumber { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public int Quantity { get; set; }
    public string SaleDate { get; set; } = default!;
    public decimal Total { get; set; }
    
}