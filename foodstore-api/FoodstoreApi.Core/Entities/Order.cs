namespace FoodstoreApi.Core.Entities;

public partial class Order
{
    public int Id { get; set; }

    public string OrderCode { get; set; } = null!;

    public int? SourceId { get; set; }

    public int? EmployeeId { get; set; }

    public int? CustomerId { get; set; }

    public int? DiscountId { get; set; }

    public decimal? SubtotalAmount { get; set; }

    public decimal? DiscountAmount { get; set; }

    public decimal? VatAmount { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? QrPaymentUrl { get; set; }



    public DateTime? PaidAt { get; set; }

    public string? Status { get; set; }

    public string? Note { get; set; }

    public DateTime? PrintedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Discount? Discount { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = new List<OrderStatusHistory>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Source? Source { get; set; }

    public virtual Employee? UpdatedByNavigation { get; set; }
}




