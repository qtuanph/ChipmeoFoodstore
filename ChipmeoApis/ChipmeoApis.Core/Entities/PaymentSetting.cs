using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChipmeoApis.Core.Entities;

[Table("payment_settings")]
public class PaymentSetting
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("bank_id")]
    [MaxLength(50)]
    public string BankId { get; set; } = null!;

    [Column("bank_account")]
    [MaxLength(50)]
    public string BankAccount { get; set; } = null!;

    [Column("bank_name")]
    [MaxLength(100)]
    public string BankName { get; set; } = null!;

    [Column("bank_account_name")]
    [MaxLength(200)]
    public string? BankAccountName { get; set; }

    [Column("template")]
    [MaxLength(20)]
    public string Template { get; set; } = "compact2";

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("is_default")]
    public bool IsDefault { get; set; } = false;
}




