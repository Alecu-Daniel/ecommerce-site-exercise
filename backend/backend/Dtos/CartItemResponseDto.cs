using Microsoft.AspNetCore.Mvc;

namespace backend.Dtos
{
    public class CartItemResponseDto : Controller
    {
        public int ProductId { get; set; }
        public string Title { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
        
}
