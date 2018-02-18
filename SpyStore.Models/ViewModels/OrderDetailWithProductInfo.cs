using SpyStore.Models.ViewModels.Base;
using System.ComponentModel.DataAnnotations;

namespace SpyStore.Models.ViewModels
{
    public class OrderDetailWithProductInfo : ProductAndCategoryBase
    {
        public int OrderId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [DataType(DataType.Currency), Display(Name = "Total")]
        public decimal? LineItemTotal { get; set; }
    }
}
