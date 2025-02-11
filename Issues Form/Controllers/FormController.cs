﻿using Issues_Form.Models;
using Issues_Form.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Console;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.SqlServer.Server;
using System.Net.Mail;
using System.Net;

namespace Issues_Form.Controllers
{
    [Authorize]
    public class FormController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;
        private readonly string defaultSender = "robin28@student.ub.ac.id";
        private readonly string defaultRecipient = "robin28@student.ub.ac.id";

        public FormController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("AccessDenied", "Form");
            }
            var form = context.Form.OrderByDescending(p => p.Id).ToList();
            return View(form);
        }

        [Authorize(Roles = "User")]
        public IActionResult Create()
        {
            ViewBag.Categories = context.Category_Param.Select(c => new SelectListItem
            {
                Value = c.Category_Issues,
                Text = c.Category_Issues
            }).ToList();

            ViewBag.Companies = context.Company_Param.Select(c => new SelectListItem
            {
                Value = c.Company_Name,
                Text = c.Company_Name
            }).ToList();

            ViewBag.Buildings = context.Building_Param.Select(b => new SelectListItem
            {
                Value = b.Building,
                Text = b.Building
            }).ToList();

            return View();
        }


        [HttpPost]
        [Authorize(Roles = "User")]
        public IActionResult Create(FormDto formDto, bool ccRequest)
        {
            if (ccRequest && string.IsNullOrWhiteSpace(formDto.CCEmail))
            {
                ModelState.AddModelError("CCEmail", "CC Email is required when CC this request is checked.");
            }

            if (ModelState.IsValid)
            {
                // Save or process the data
                string newFileName = "AttachIssues_" + formDto.Name + "_" + DateTime.Now.ToString("HHmmss_dd-MM-yyyy");
                string finalAttachPath; //for email attachment only

                if (formDto.Attach != null)
                {
                    if (formDto.Attach.Length > (10 * 1024 * 1024)) // max 10 MB file size
                    {
                        ModelState.AddModelError("Attach", "File size cannot exceed 10MB.");
                        return View(formDto);
                    }

                    newFileName += Path.GetExtension(formDto.Attach!.FileName);

                    string attachDirectory = environment.WebRootPath + "/form/";
                    Directory.CreateDirectory(attachDirectory); // Create directory if not exists

                    string attachFullPath = Path.Combine(attachDirectory, newFileName);
                    finalAttachPath = attachFullPath;

                    using (var stream = System.IO.File.Create(attachFullPath))
                    {
                        formDto.Attach.CopyTo(stream);
                    }
                }
                else
                {
                    newFileName = "-";
                    finalAttachPath = "-";
                }

                // Save the new product in the database
                Form form = new Form()
                {
                    Name = formDto.Name,
                    Email = formDto.Email,
                    CCEmail = formDto.CCEmail,
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
                        $"Report ID: {form.Id}<br>" +
                        $"Name: {formDto.Name}<br>" +
                        $"Email: {formDto.Email}<br>" +
                        $"CC Email: {formDto.CCEmail}<br>" +
                        $"Phone Number: {formDto.PhoneNumber}<br>" +
                        $"Subject: {formDto.Subject}" +
                        $"<br>Category: {formDto.Category}" +
                        $"<br>Building: {formDto.Building}" +
                        $"<br>Company: {formDto.Company}" +
                        $"<br>Description: {formDto.Description}" +
                        $"<br><br>We apologize for any inconvenience caused and appreciate your report. Our team has initiated an investigation process to identify the root cause of this issue and is actively working to rectify it. Should further assistance be required, our team members will reach out to you promptly to provide additional support in resolving this matter." +
                        $"<br><br>Thank you for your patience and understanding.<br><br>";

                // Call SendMail method
                if (form.CCEmail == null)
                {
                    SendMail(new Mail
                    {
                        From = defaultSender,
                        To = $"{form.Email},{defaultRecipient}",
                        Subject = subject,
                        Body = body,
                        AttachmentPath = "-"
                    });
                }
                else
                {
                    SendMail(new Mail
                    {
                        From = defaultSender,
                        To = $"{form.Email},{defaultRecipient},{form.CCEmail}",
                        Subject = subject,
                        Body = body,
                        AttachmentPath = "-"
                    });
                }
                return RedirectToAction("Confirmation", "Form");
            }

            // If we got this far, something failed, redisplay form
            ViewBag.Categories = context.Category_Param.Select(c => new SelectListItem
            {
                Value = c.Category_Issues,
                Text = c.Category_Issues
            }).ToList();

            ViewBag.Companies = context.Company_Param.Select(c => new SelectListItem
            {
                Value = c.Company_Name,
                Text = c.Company_Name
            }).ToList();

            ViewBag.Buildings = context.Building_Param.Select(b => new SelectListItem
            {
                Value = b.Building,
                Text = b.Building
            }).ToList();

            return View(formDto);
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

            string attachmentPath = model.AttachmentPath;
            if (attachmentPath != "-")
            {
                // Add attachment to the email
                System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(attachmentPath);
                mailMessage.Attachments.Add(attachment);
            }

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
                CCEmail = form.CCEmail,
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
            ViewData["Status"] = form.Status;
            ViewData["AdminComment"] = form.AdminComment;

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

            //replace or add the status and AdminComment database
            form.Status = formDto.Status;
            form.AdminComment = formDto.AdminComment;
            context.SaveChanges();

            // pre-call SendMail method
            string subject = "Issues Form Submission: " + form.Subject;
            string body = $"Dear {form.Name}," +
                        $"<br><br>Thank you for submitting the Issues Form. Below are the details:<br><br>" +
                        $"Report ID: {form.Id}<br>" +
                        $"Name: {form.Name}<br>" +
                        $"Email: {form.Email}<br>" +
                        $"CC Email: {form.CCEmail}<br>" +
                        $"Phone Number: {form.PhoneNumber}<br>" +
                        $"Subject: {form.Subject}<br>" +
                        $"Category: {form.Category}<br>" +
                        $"Building: {form.Building}<br>" +
                        $"Company: {form.Company}<br>" +
                        $"Description: {form.Description}" +
                        $"<br><br>Thank you for your patience. After investigating this issue, we have identified some areas requiring clarification." +
                        $"<br>To ensure we're on the same page, we've outlined a few comments and questions below:" +
                        $"<br><br>Issues Status: {form.Status}"+
                        $"<br>From Admin: {form.AdminComment}<br><br>";

            // Call SendMail method
            if (form.CCEmail == null){
                SendMail(new Mail
                {
                    From = defaultSender,
                    To = $"{form.Email},{defaultRecipient}",
                    Subject = subject,
                    Body = body,
                    AttachmentPath = "-"
                });
            }
            else
            {
                SendMail(new Mail
                {
                    From = defaultSender,
                    To = $"{form.Email},{defaultRecipient},{form.CCEmail}",
                    Subject = subject,
                    Body = body,
                    AttachmentPath = "-"
                });
            }
            return RedirectToAction("Index", "Form");
        }

        public IActionResult Delete(int id)
        {
            var form = context.Form.Find(id);

            if (form == null)
            {
                return RedirectToAction("Index", "Form");
            }

            string finalPath = environment.WebRootPath + "/form/" + form.Attachment;
            if (System.IO.File.Exists(finalPath))
            {
                System.IO.File.Delete(finalPath); // Delete associated file if it exists
            }

            context.Form.Remove(form);
            context.SaveChanges(); // Remove form entry from the database

            return RedirectToAction("Index", "Form");
        }
    }
}