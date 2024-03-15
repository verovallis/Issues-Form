using Microsoft.AspNetCore.Mvc;

namespace Issues_Form.Controllers
{
    public class ContactsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
