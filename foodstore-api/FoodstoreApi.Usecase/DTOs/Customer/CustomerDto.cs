namespace FoodstoreApi.Usecase.DTOs.Customer;

public class CustomerDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string CustomerCode { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string? AvatarUrl { get; set; }
    public int LoyaltyPoints { get; set; }
    public string? MembershipLevel { get; set; }
    public DateTime? Birthday { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
}

public class CustomerRegisterDto
{
    public string Name { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? Phone { get; set; }
}

public class CustomerLoginDto
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class CustomerLoginResultDto
{
    public CustomerDto Customer { get; set; } = null!;
    public string Token { get; set; } = null!;
}

public class CustomerUpdateDto
{
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? AvatarUrl { get; set; }
    public bool? IsActive { get; set; }
    public int? Points { get; set; }
}

public class CreateCustomerDto
{
    public string Name { get; set; } = null!;
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateCustomerAdminDto
{
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? AvatarUrl { get; set; }
    public bool? IsActive { get; set; }
    public int? Points { get; set; }
}

public class AddPointsDto
{
    public int Points { get; set; }
    public string Reason { get; set; } = null!;
}

public class CustomerOrderHistoryDto
{
    public Guid Id { get; set; }
    public string? OrderCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal? TotalAmount { get; set; }
    public string? Status { get; set; }
}

public class CustomerBirthdayDto
{
    public Guid Id { get; set; }
    public string CustomerCode { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Phone { get; set; }
    public DateTime? Birthday { get; set; }
    public string? MembershipLevel { get; set; }
}

public class UpcomingBirthdaysDto
{
    public List<CustomerBirthdayDto> ThisWeek { get; set; } = new();
    public List<CustomerBirthdayDto> ThisMonth { get; set; } = new();
    public int TotalThisWeek { get; set; }
    public int TotalThisMonth { get; set; }
}
