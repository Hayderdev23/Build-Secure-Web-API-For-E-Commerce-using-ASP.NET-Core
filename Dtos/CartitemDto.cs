using BestStoreAPI.Models;

namespace BestStoreAPI.Dtos
{
    public class CartitemDto
    {
        public Prodect Prodect { get; set; } = new Prodect();
        public int Quantity { get; set; }
    }
}
