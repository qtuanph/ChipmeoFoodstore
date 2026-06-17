namespace FoodstoreApi.Usecase.DTOs.Customer;

public class CustomerDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string? Phone { get; set; }
    public string Email { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public int Points { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class CustomerRegisterDto
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? Phone { get; set; }
}

public class CustomerLoginDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class CustomerLoginResultDto
{
    public CustomerDto Customer { get; set; } = null!;
    public string Token { get; set; } = null!;
}

public class CustomerUpdateDto
{
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public string? AvatarUrl { get; set; }
    public bool? IsActive { get; set; }
    public int? Points { get; set; }
}
public class CreateCustomerDto
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateCustomerAdminDto
{
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public string? AvatarUrl { get; set; }
    public bool? IsActive { get; set; }
    public int? Points { get; set; }
}




