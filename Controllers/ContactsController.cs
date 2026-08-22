using BestStoreAPI.Dtos;
using BestStoreAPI.Models;
using BestStoreAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BestStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase   // ekder et3aml wea al Requst and Respons API 
    {
        private readonly ApplicationDbContext _context;  // we call it Dependency Injection Field
       

        //private readonly List<string> listSubject = new List<string>()
        //{
        //    "Order Status" , "Refound Requst" , "Job Application" , "Other"

        //};
        public ContactsController(ApplicationDbContext context )
        {
            _context = context;
           
        }

       
        [HttpGet("subjects")]
        public IActionResult GetSubjects()
        {
            var u = _context.Subjects.ToList();
            return Ok(u);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetContacts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10) // offser bassed 
        {
            // تأكد أن القيم صحيحة (لتجنب الأخطاء إذا أرسل المستخدم قيم سالبة)
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            // جلب البيانات مع تطبيق البيجنيشن
            var contacts = _context.Contacts
                .Include(c => c.Subject)  //  eagry loding 
                .OrderBy(c => c.Id) // الترتيب إجباري قبل استخدام Skip
                .Skip((pageNumber - 1) * pageSize) // تخطي العناصر الخاصة بالصفحات السابقة
                .Take(pageSize) // أخذ عدد العناصر المطلوب لهذه الصفحة فقط
                .ToList();

            return Ok(contacts);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public IActionResult GetContact(int id)
        {
            var contact = _context.Contacts.Include(c => c.Subject).FirstOrDefault(c => c.Id == id);  
           // var cone = _context.Contacts.Where(e=>e.Id == id);  //مرن، يستخدم لأي شرط، لكن دايمًا ينفذ Query على قاعدة البيانات.
           // Find(id)
           if (contact == null)
            {
                return NotFound ();
            }
           return Ok(contact);  

        }
        [HttpPost]
        public IActionResult CreatContact( ContactDto contactDto )
        {
            var sub = _context.Subjects.Find(contactDto.SubjectId);
            if(sub == null)
            {
                ModelState.AddModelError("Subject", "pleas Select a Valid Subject");
                return BadRequest(ModelState);
            }
            Contact contact = new Contact()
            {
                FirstName = contactDto.FirstName,
                LastName = contactDto.LastName,
                Email = contactDto.Email,
                Phone = contactDto.Phone ?? "",
                Subject = sub,
                Message = contactDto.Message,
                CreatedAt= DateTime.Now

            };
            _context.Contacts.Add(contact);
            _context.SaveChanges();

            // send confirmation 
            string emailsubject = "Contact Confirmation";
            string username = contactDto.FirstName + " " + contactDto.LastName;
            string emailMessage = "Dear" + username + "\n" + "we thank you to confirm the email .team support can help you";
            //EmailSender emailSender = new EmailSender();
            //_emailsender.SendEmail(emailsubject,contact.Email , username,emailMessage).Wait();
            return Ok(contact);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateContact (int id ,  ContactDto contactDto )
        {
            var sub = _context.Subjects.Find(contactDto.SubjectId);
            if (sub == null)
            {
                ModelState.AddModelError("Subject", "pleas Select a Valid Subject");
                return BadRequest(ModelState);
            }
            var contact = _context.Contacts.Find(id);
            if (contact == null)
            {
                return NotFound ();
            }

            contact.FirstName = contactDto.FirstName;
            contact.LastName = contactDto.LastName;
            contact.Email = contactDto.Email;
            contact.Phone = contactDto.Phone ?? "";
            contact.Subject = sub;
            contact.Message = contactDto.Message;

            _context.SaveChanges();



            return Ok(contact);

        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteContact( int id )
        {
            var contact = _context.Contacts.Find(id);
            if (contact == null)
            {
                return NotFound();
            }

            _context.Contacts.Remove(contact);
            _context.SaveChanges();

            //var con = new Contact()  // the second method 
            //{
            //    Id = id

            //};
            //_context.Contacts.Remove(con);
            //_context.SaveChanges();

            return Ok();


        }




    }
}
