using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BestStoreAPI.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string OrderStatus { get; set; }
        [Precision(16,2)]
        public string ShippingFee { get; set; } = "";
        [MaxLength(100)]
        public string DeliveryAddress { get; set; } = "";
        [MaxLength(100)]
        public string PaymentMethod { get; set; } = "";

        public string PaymentStatus { get; set; } = "";

        //navigation properties
        public User User { get; set; } = null!;
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }

}
