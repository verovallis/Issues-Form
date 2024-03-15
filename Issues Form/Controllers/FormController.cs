﻿using Issues_Form.Models;
using Issues_Form.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Console;
using Microsoft.SqlServer.Server;
using Issues_Form.Models;
using System.Net.Mail;
using System.Net;

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
            */
            if (!ModelState.IsValid)
            {
                return View(formDto);
            }

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

            string defaultSender = "robin28@student.ub.ac.id";
            string defaultRecipient = "robin28@student.ub.ac.id";
            string subject = "Issues Form Submission: " + formDto.Subject;
            string body = $"Dear {formDto.Name}," +
                        $"<br><br>Thank you for submitting the Issues Form. Below are the details:<br><br>" +
                        $"Name: {formDto.Name}<br>" +
                        $"Email: {formDto.Email}<br>" +
                        $"Phone Number: {formDto.PhoneNumber}<br>" +
                        $"Subject: {formDto.Subject}" +
                        $"<br>Category: {formDto.Category}" +
                        $"<br>Building: {formDto.Building}" +
                        $"<br>Company: {formDto.Company}" +
                        $"<br>Description: {formDto.Description}" +
                        $"<br><br>We apologize for any inconvenience caused and appreciate your report. Our team has initiated an investigation process to identify the root cause of this issue and is actively working to rectify it. Should further assistance be required, our team members will reach out to you promptly to provide additional support in resolving this matter." +
                        $"<br><br>Thank you for your patience and understanding.<br><br>";

            // Call SendMail method
            SendMail(new Mail
            {
                From = defaultSender,
                To = $"{formDto.Email},{defaultRecipient}",
                Subject = subject,
                Body = body
            });

            return RedirectToAction("Confirmation","Form");
        }

        [HttpPost]
        public ActionResult SendMail(Issues_Form.Models.Mail model)
        {
            MailMessage mailMessage = new MailMessage(model.From, model.To);
            
            mailMessage.Subject = model.Subject;
            // Create a multi-part email with both HTML and plain text bodies
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(model.Body, null, "text/html");
            mailMessage.AlternateViews.Add(htmlView);

            mailMessage.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;


            NetworkCredential cred = new NetworkCredential("robin28@student.ub.ac.id", "vrvx owbf atue hvro");
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = cred;
            smtp.Send(mailMessage);

            return RedirectToAction("Confirmation", "Form");
        }

        public IActionResult Confirmation()
        {
            return View();
        }

        public IActionResult Edit(int id)
        {
            var form = context.Form.Find(id);

            if(form == null)
            {
                return RedirectToAction("Index", "Form");
            }


            //create formDto from form
            var formDto = new FormDto()
            {
                Name = form.Name,
                Email = form.Email,
                PhoneNumber = form.PhoneNumber,
                Subject = form.Subject,
                Category = form.Category,
                Building = form.Building,
                Company = form.Company,
                Description = form.Description,
            };

            //pull from db_form
            ViewData["FormId"] = form.Id;
            ViewData["Attachment"] = form.Attachment;
            ViewData["CreatedAt"] = form.CreatedAt;

            return View(formDto);
        }

        [HttpPost]
        public IActionResult Edit(int id, FormDto formDto)
        {
            var form = context.Form.Find(id);
            if (form == null)
            {
                return RedirectToAction("Index", "Form");
            }

            if (!ModelState.IsValid)
            {
                ViewData["FormId"] = form.Id;
                ViewData["Attachment"] = form.Attachment;
                ViewData["CreatedAt"] = form.CreatedAt.ToString("dd/MM/yyyy");

                return View(formDto);
            }


            // update the image file if we have a new image file
            string newFileName = form.Attachment;
            if (formDto.Attach != null)
            {
                newFileName = DateTime.Now.ToString("fffssmmHHddMMyyyy");
                newFileName += Path.GetExtension(formDto.Attach.FileName);

                string imageFullPath = environment.WebRootPath + "/form/" + newFileName;
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    formDto.Attach.CopyTo(stream);
                }

                //delete old image
                string oldImageFullPath = environment.WebRootPath + "/form/" + form.Attachment;
                System.IO.File.Delete(oldImageFullPath);
            }

            //save the new product in the database
            form.Name = formDto.Name;
            form.Email = formDto.Email;
            form.PhoneNumber = formDto.PhoneNumber;
            form.Subject = formDto.Subject;
            form.Category = formDto.Category;
            form.Building = formDto.Building;
            form.Company = formDto.Company;
            form.Description = formDto.Description;
            form.Attachment = newFileName;

            context.SaveChanges();

            return RedirectToAction("Index", "Form");
        }

        public IActionResult Delete(int id)
        {
            var form = context.Form.Find(id);

            if (form == null)
            {
                return RedirectToAction("Index", "Form");
            }

            string imageFullPath = environment.WebRootPath + "/form/" + form.Attachment;
            System.IO.File.Delete(imageFullPath);

            context.Form.Remove(form);
            context.SaveChanges(true);
            return RedirectToAction("Index", "Form");
        }
    }
}