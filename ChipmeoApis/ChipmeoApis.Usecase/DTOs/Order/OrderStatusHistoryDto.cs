namespace ChipmeoApis.Usecase.DTOs.Order;

public class OrderStatusHistoryDto
{
    public int Id { get; set; }
    public string? FromStatus { get; set; }
    public string ToStatus { get; set; } = string.Empty;
    public int? ChangedBy { get; set; }
    public string? ChangedByName { get; set; }
    public DateTime? ChangedAt { get; set; }
    public string? Note { get; set; }
}




