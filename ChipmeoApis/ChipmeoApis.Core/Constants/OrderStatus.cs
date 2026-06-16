namespace ChipmeoApis.Core.Constants;

public static class OrderStatus
{
    public const string Pending = "pending";
    public const string Paid = "paid";
    public const string Preparing = "preparing";
    public const string Served = "served";
    public const string Completed = "completed";
    public const string Cancelled = "cancelled";

    public static readonly string[] Active =
        [Paid, Preparing, Served];

    public static readonly HashSet<string> ActiveSet = [.. Active];

    public static readonly Dictionary<string, List<string>> KitchenTransitions = new()
    {
        [Paid] = [Preparing, Cancelled],
        [Preparing] = [Served, Paid]
    };
}
