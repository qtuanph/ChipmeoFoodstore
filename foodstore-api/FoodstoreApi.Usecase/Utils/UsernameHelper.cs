namespace FoodstoreApi.Usecase.Utils;

public static class UsernameHelper
{
    public static string Normalize(string username)
    {
        return username.Replace("-", " ").ToLowerInvariant();
    }
}
