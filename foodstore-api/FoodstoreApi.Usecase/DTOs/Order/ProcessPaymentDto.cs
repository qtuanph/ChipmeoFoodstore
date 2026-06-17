namespace FoodstoreApi.Usecase.DTOs.Order;

public class ProcessPaymentDto
{
    public string PaymentMethod { get; set; } = null!; // cash, momo, zalopay, qr, banking
    public decimal? CashReceived { get; set; } // Required for cash only
}




