namespace FoodstoreApi.Usecase.DTOs.Order;

public class OrderStatusHistoryDto
{
    public Guid Id { get; set; }
    public string? FromStatus { get; set; }
    public string ToStatus { get; set; } = string.Empty;
    public Guid? ChangedBy { get; set; }
    public string? ChangedByName { get; set; }
    public DateTime? ChangedAt { get; set; }
    public string? Note { get; set; }
}
