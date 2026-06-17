namespace FoodstoreApi.Usecase.DTOs.Report;

public class ComboRecommendationDto
{
    public string Item1Name { get; set; } = string.Empty;
    public string Item2Name { get; set; } = string.Empty;
    public int Frequency { get; set; }
    public decimal TotalOriginalPrice { get; set; }
    public decimal SuggestedPrice { get; set; }
    public string Reason { get; set; } = string.Empty;
}




