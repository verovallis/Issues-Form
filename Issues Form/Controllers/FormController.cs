using Issues_Form.Models;
using Issues_Form.Services;
using Microsoft.AspNetCore.Mvc;

namespace Issues_Form.Controllers
{
    public class FormController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;

        public FormController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        public IActionResult Index()
        {
            var form = context.Form.OrderByDescending(p => p.Id).ToList();
            return View(form);
        }
        public IActionResult Create()
        {
            var form = context.Form.OrderByDescending(p => p.Id).ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(FormDto formDto)
        {
            /*
            if (formDto.Attach == null)
            {
                ModelState.AddModelError("ImageFile", "The image file is required");
            }
            if (!ModelState.IsValid)
            {
                return View(formDto);
            }
            */

            //save image file
            string newFileName = DateTime.Now.ToString("fffssmmHHddMMyyyy");
            if (formDto.Attach != null)
            {
                newFileName += Path.GetExtension(formDto.Attach!.FileName);

                string imageFullPath = environment.WebRootPath + "/form/" + newFileName;
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    formDto.Attach.CopyTo(stream);
                }
            }
            else
            {
                newFileName = "-";
            }

            //save the new product in the database
            Form form = new Form()
            {
                Name = formDto.Name,
                Email = formDto.Email,
                PhoneNumber = formDto.PhoneNumber,
                Subject = formDto.Subject,
                Category = formDto.Category,
                Building = formDto.Building,
                Company = formDto.Company,
                Description = formDto.Description,
                Attachment = newFileName,
                CreatedAt = DateTime.Now,
            };

            context.Form.Add(form);
            context.SaveChanges();

            return RedirectToAction("Index","Form");
        }

    }
}