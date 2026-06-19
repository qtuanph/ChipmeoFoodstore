namespace FoodstoreApi.Usecase.DTOs.Blog;

public class CmsDashboardStatsDto
{
    public int TotalPosts { get; set; }
    public int PublishedPosts { get; set; }
    public int DraftPosts { get; set; }
    public int ScheduledPosts { get; set; }
    public int TotalCategories { get; set; }
    public int TotalTags { get; set; }
    public int TotalViews { get; set; }
    public int RecentPostsCount { get; set; }
    public int FeaturedPostsCount { get; set; }
}
