using BestStoreAPI.Dtos;
using BestStoreAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BestStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }






        //[HttpGet]
        //public IActionResult GetCart(string productIdentifiers)
        //{
        //    var productDictionary = OrderHelper.GetProductDictionary(productIdentifiers);
        //    return Ok(productDictionary);
        //}


        [HttpGet ("PaymentMethos")]
        public IActionResult GetPaymentMethods()
        {
            var paymentMethods = OrderHelper.PaymentMethods;
            return Ok(paymentMethods);
        }

        [HttpGet]
        public IActionResult GetCart(string productIdentifiers)
        {
            CartDto cartDto = new CartDto();
            cartDto.CartItems = new List<CartitemDto>();
            cartDto.SubTotal = 0;
            cartDto.ShippingFee = OrderHelper.shippingFee;
            cartDto.TotalPrice = 0;

            var productDictionary = OrderHelper.GetProductDictionary(productIdentifiers);
            foreach (var pair in productDictionary)
            {
                int productId = pair.Key;
                var product = _context.Prodects.FirstOrDefault(p => p.Id == productId);
                if (product == null)
                {
                    continue;
                }
                var cartItemDto = new CartitemDto();
                cartItemDto.Prodect = product;
                cartItemDto.Quantity = pair.Value;
                cartDto.CartItems.Add(cartItemDto);
                cartDto.SubTotal += product.price * pair.Value;
                cartDto.TotalPrice = cartDto.SubTotal + cartDto.ShippingFee;
                 

            }
            return Ok(cartDto);



        }
    }
}
