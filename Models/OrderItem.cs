namespace BestStoreAPI.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int productId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }


        //navigation properties
        public Order Order { get; set; }
        public Prodect Prodect { get; set; } = null!;


    }
}
