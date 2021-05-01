using CourseWork.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CourseWork.Controllers
{
    public class HomeController : Controller
    {
        private readonly WorkContext db;
        public HomeController(WorkContext context)
        {
            db = context;
        }

        public IActionResult Index(string Mess, string Type)
        {
            if (Mess != null)
            {
                ViewBag.Messeng = Mess;
                ViewBag.Type = Type;
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Messeng(Request messengs)
        {
            if (messengs != null)
            {
                if (messengs.Name != null & messengs.SyrName != null & messengs.Messeng != null & messengs.Email != null)
                {
                    try
                    {
                        db.Requests.Add(messengs);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index", "Home", new { Mess = "Заявка успешно отправлена.", Type = "alert-success" });
                    }
                    catch
                    {
                        return RedirectToAction("Index", "Home", new { Mess = "При записи в базу данных произошла ошибка.", Type = "alert-danger" });
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Home", new { Mess = "Поля формы не могут быть пустыми!", Type = "alert-danger" });
                }
            }
            return RedirectToAction("Index", "Home", new { Mess = "При отправке заявки произошла ошибка.", Type = "alert-danger" });
        }
    }
}
