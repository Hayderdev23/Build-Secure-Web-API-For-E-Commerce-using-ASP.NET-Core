using BestStoreAPI.Dtos;
using BestStoreAPI.Models;
using BestStoreAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace BestStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;    //هذا عبارة عن "واجهة" (Interface) جاهزة من مايكروسوفت. وظيفتها إعطاء التطبيق معلومات عن البيئة التي يعمل بها حالياً، بالإضافة إلى مسارات المجلدات داخل السيرفر.
        private readonly List<string> ListCategory = new List<string>()
        {
            "iphone" , "computer" , "Acssessories" ,"printers", "Cameras", "Other"
        };
        public ProductsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context; 
            _env = env;

        }
        [HttpGet("categories")]
        public IActionResult GetCategories()
        {
            return Ok (ListCategory);
        }


        //[HttpGet]
        //public IActionResult Getproduct()
        //{
        //    var pro = _context.Prodects.ToList();
        //    if (pro == null)
        //    {
        //        return NotFound();

        //    }
        //    return Ok(pro);

        //}
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public IActionResult CreateProduct([FromForm] ProductsDTO productsDTO)
        {
            if(!ListCategory.Contains(productsDTO.Category))
            {
                ModelState.AddModelError("category", "pleas select a valide caegory");
                return BadRequest(ModelState);
            }




            if (productsDTO.ImageFileName == null)
            {
                ModelState.AddModelError("ImageFile", "The Image File is Required");
                return BadRequest(ModelState);
            }

            string imageFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(productsDTO.ImageFileName.FileName);

            string imageFolder = Path.Combine(_env.WebRootPath, "Images", "Products");

            if (!Directory.Exists(imageFolder))
            {
                Directory.CreateDirectory(imageFolder);
            }

            string fullPath = Path.Combine(imageFolder, imageFileName);

            using (var stream = System.IO.File.Create(fullPath))
            {
                productsDTO.ImageFileName.CopyTo(stream);
            }

            Prodect product = new Prodect()
            {
                Name = productsDTO.Name,
                Brand = productsDTO.Brand,
                Category = productsDTO.Category,
                price = productsDTO.price,
                Description = productsDTO.Description ?? "",
                ImageFileName = imageFileName,
                CreatAt = DateTime.Now,
            };

            _context.Prodects.Add(product);
            _context.SaveChanges();

            return Ok(product);
        }
        [HttpGet("{id}")]
        public IActionResult GetProdect(int id)
        {
            var pro = _context.Prodects.FirstOrDefault(c => c.Id == id);
            // var cone = _context.Contacts.Where(e=>e.Id == id);  //مرن، يستخدم لأي شرط، لكن دايمًا ينفذ Query على قاعدة البيانات.
            // Find(id)
            if (pro == null)
            {
                return NotFound();
            }
            return Ok(pro);

        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult UpdateProduct([FromForm]int id, ProductsDTO productsDTO)
        {
            if (!ListCategory.Contains(productsDTO.Category))
            {
                ModelState.AddModelError("category", "pleas select a valide caegory");
                return BadRequest(ModelState);
            }
            var pro = _context.Prodects.Find(id);
            if (pro == null)
            {
                return NotFound();
            }
            string imageFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            imageFileName += Path.GetExtension(productsDTO.ImageFileName.FileName);
            string imageFolder = _env.WebRootPath + "/images/Product/";
            using (var stream =System.IO.File.Create(imageFolder+imageFileName))
            {
                productsDTO.ImageFileName.CopyTo(stream);
            }
            //////////////
            //deleat the old image 
            System.IO.File.Delete(imageFolder + productsDTO.ImageFileName);
            
            Prodect prodect = new Prodect();
            prodect.Name=productsDTO.Name;
            prodect.Brand=productsDTO.Brand;
            prodect.price = productsDTO.price;
            prodect.Description = productsDTO.Description??="";
            prodect.ImageFileName = imageFileName;
            _context.SaveChanges();


            return Ok(prodect);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct([FromForm]int id)
        {
            var pro = _context.Prodects.Find(id);
            if(pro == null) { return NotFound(); }

            string imagesFolder = _env.WebRootPath + "/Images/Products/";
            System.IO.File.Delete(imagesFolder+pro.ImageFileName);

            _context.Prodects.Remove(pro);
            _context.SaveChanges();




            return Ok();

        }
        [HttpGet]
        public IActionResult GetProducts(string? search , string? category , int? minPrice , int? maxPrice , string? sort , string? order )
        {
            IQueryable<Prodect> query = _context.Prodects;
            // search functionality 
            if (search != null)
            {
                query = query.Where(p=>p.Name.Contains(search) || p.Description.Contains(search));
            }
            if (category != null)
            {
                query = query.Where(p=>p.Category==category);
            }
            if(minPrice != null)
            {
                query = query.Where(p=>p.price >= minPrice);
            }
            if(maxPrice != null)
            {
                query=query.Where(p=>p.price <= maxPrice);
            }
            //sort functonality
            if (sort == null) sort = "id";
            if (order == null || order != "asc") order = "desc"; 
            if (sort .ToLower() == "name")
            {
                if (order == "asc")
                {
                    query = query.OrderBy(p=>p.Name);
                }
                else
                {
                    query = query.OrderByDescending(p=>p.Name);
                }
            }

            else if (sort.ToLower() == "brand")
            {
                if (order == "asc")
                {
                    query = query.OrderBy(p => p.Brand);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Brand);
                }
            }
            else if (sort.ToLower() == "category")
            {
                if (order == "asc")
                {
                    query = query.OrderBy(p => p.Category);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Category);
                }
            }
            else if (sort.ToLower() == "price")
            {
                if (order == "asc")
                {
                    query = query.OrderBy(p => p.price);
                }
                else
                {
                    query = query.OrderByDescending(p => p.price);
                }
            }
            else if (sort.ToLower() == "date")
            {
                if (order == "asc")
                {
                    query = query.OrderBy(p => p.CreatAt);
                }
                else
                {
                    query = query.OrderByDescending(p => p.CreatAt);
                }
            }
            else
            {
                if (order == "asc")
                {
                    query =query.OrderBy(p => p.Id);
                }
                else
                {
                    query = query . OrderByDescending(p => p.Id);
                }
                    
            }

            // pagination functonlty 




            var products = query.ToList();
            return Ok(products);
        }

        
    }
}
