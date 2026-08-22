namespace BestStoreAPI.Dtos
{
    public class CartDto
    {
        public List<CartitemDto> CartItems { get; set; } = new List<CartitemDto>();
        public decimal TotalPrice { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal SubTotal { get; set; }
    }
}
