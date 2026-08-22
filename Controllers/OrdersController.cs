using BestStoreAPI.Dtos;
using BestStoreAPI.Models;
using BestStoreAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace BestStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult GetOrders()
        {
            var orders = _context.Orders.ToList();
            return Ok(orders);
        }
        [Authorize]
        [HttpPost]
        public IActionResult creatOrder(OrderDto orderdto)
        {
            // check if the payment is vavlid or not 
            if (!OrderHelper.PaymentMethods.ContainsKey(orderdto.PaymentMethod))
            {
                ModelState.AddModelError("PaymentMethod", "Invalid payment method.");
                return BadRequest(ModelState);
            }
            string userIdString = User.FindFirst("id")?.Value;

            if (!int.TryParse(userIdString, out int userid))
            {
                // إذا التوكن ما بي ID أو مخربط
                return Unauthorized("User ID not found in the token.");
            }
            var user = _context.Users.Find(userid);
            if (user == null)
            {
                return NotFound("Order , unable to create the order");
            }
            var productDictionary = OrderHelper.GetProductDictionary(orderdto.ProdectIdentifier);

            //create the order
            Order order = new Order();
            order.UserId = userid;
            order.OrderDate = DateTime.Now;
            order.ShippingFee = OrderHelper.shippingFee.ToString();
            order.DeliveryAddress = orderdto.DeliveryAddress;
            order.PaymentMethod = orderdto.PaymentMethod;
            order.PaymentStatus = OrderHelper.PaymentStatuses[0];
            order.OrderStatus = OrderHelper.OrderStatuses[0];

            foreach (var pair in productDictionary)
            {
                int productId = pair.Key;
                var product = _context.Prodects.Find(productId);
                if (product == null)
                {
                    return NotFound($"Product with ID {productId} not found.");
                }
                var orderItem = new OrderItem();
                // امسح سطر الـ productId القديم، واستخدم الكائن مباشرة:
                orderItem.Prodect = product;  // الـ EF Core راح ياخذ الـ ID من هذا الكائن ويربطه صح
                orderItem.Quantity = pair.Value;
                orderItem.UnitPrice = product.price;

                order.OrderItems.Add(orderItem);
            }
            if (order.OrderItems.Count == 0)
            {
                return BadRequest("order , unable to creat the order.");
            }

            // Save the order to the database
            _context.Orders.Add(order);
            _context.SaveChanges();

            foreach (var item in order.OrderItems)
            {
                item.Order = null; // Remove the reference to the order to avoid circular reference

            }
            order.User.Password = ""; // Remove the password from the user object


            return Ok(order);





        }
        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
        {
            // 1. نستخرج الـ ID كنص
            string userIdString = User.FindFirst("id")?.Value;

            // 2. نحوله إلى رقم (int) حتى نقدر نستخدمه بالداتا بيس
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(); // إذا التوكن مابي ID صحيح
            }

            // هسه نستخدم الـ userId (الرقم) للبحث عن الرول
            string role = _context.Users.Find(userId)?.Role ?? "";

            Order? order = null;
            if (role == "admin")
            {
                order = _context.Orders.Include(o => o.User)
                    .Include(o => o.OrderItems).ThenInclude(oi => oi.Prodect)
                    .FirstOrDefault(o => o.Id == id);
            }
            else
            {
                order = _context.Orders.Include(o => o.User)
                   .Include(o => o.OrderItems).ThenInclude(oi => oi.Prodect)
                   // 3. هسه المقارنة صارت صحيحة 100% (رقم == رقم)
                   .FirstOrDefault(o => o.Id == id && o.UserId == userId);
            }

            if (order == null)
            {
                return NotFound();
            }

            foreach (var item in order.OrderItems)
            {
                item.Order = null; // Remove the reference to the order to avoid circular reference
            }

            if (order.User != null)
            {
                order.User.Password = ""; // Remove the password from the user object
            }

            return Ok(order);
        }
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public IActionResult UpdateOrder(int id, string? paymentStatus, string? orderStatus)
        {
            if(paymentStatus == null && orderStatus == null)
            {
                ModelState.AddModelError("Update", "Please provide at least one status to update.");
                return BadRequest(ModelState);
            }
            if (paymentStatus != null && !OrderHelper.PaymentStatuses.Contains(paymentStatus))
            { 
                ModelState.AddModelError("PaymentStatus", "Invalid payment status.");
                return BadRequest(ModelState);
            }

            if (orderStatus != null && !OrderHelper.OrderStatuses.Contains(orderStatus))
            {
                ModelState.AddModelError("OrderStatus", "Invalid order status.");
                return BadRequest(ModelState);
            }
            var order = _context.Orders.Find(id);
            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");

            }
            if (paymentStatus != null)
            {
                order.PaymentStatus = paymentStatus;
            }
            if (orderStatus != null)
            {
                order.OrderStatus = orderStatus;
            }
            _context.SaveChanges();
            return Ok(order);
        }
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }
            _context.Orders.Remove(order);
            _context.SaveChanges();
            return Ok($"Order with ID {id} has been deleted.");
        }
    }
} 