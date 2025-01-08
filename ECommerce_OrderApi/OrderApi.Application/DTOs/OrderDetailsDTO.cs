
using System.ComponentModel.DataAnnotations;


namespace OrderApi.Application.DTOs
{
    public record OrderDetailsDTO
    (
        [Required] int OrderId,
        [Required] int ProductId,
        [Required] int Client,
        [Required] string Name,
        [Required, EmailAddress] string Email,
        [Required ] string Address,
        [Required] string TelephoneNumber,
        [Required] string ProductName,
        [Required] int PurchasedQuantity,
        [Required, DataType(DataType.Currency)] decimal UnitPrice,
        [Required, DataType(DataType.Currency)] decimal TotalPrice,
        [Required] DateTime OrderedDate
    );
}
