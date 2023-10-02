namespace MusicStore.Dto.Response;

public class ConcertSingleDtoResponse
{
    public long Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Place { get; set; } = default!;
    public string DateEvent { get; set; } = default!;
    public string TimeEvent { get; set; } = default!;
    public string? ImageUrl { get; set; } = default!;
    public int TicketQuantity { get; set; } 
    public decimal UnitPrice { get; set; }
    public string Status { get; set; } = default!;
    public GenreDtoResponse Gen { get; set; } = default!;
}