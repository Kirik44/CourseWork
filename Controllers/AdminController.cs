using CourseWork.Models;
using CourseWork.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CourseWork.Controllers
{
    public class AdminController : Controller
    {
        private readonly WorkContext db;
        public AdminController(WorkContext context)
        {
            db = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.Name != null)
            {
                return RedirectToAction("AdminPanel", "Admin");
            }
            ViewBag.Login = User.Identity.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                Moder user = await db.Moders.FirstOrDefaultAsync(u => u.Login == model.Login && u.Password == model.Password);
                if (user != null)
                {
                    await Authenticate(model.Login);
                    ViewBag.Login = User.Identity.Name;
                    return RedirectToAction("AdminPanel", "Admin");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            ViewBag.Login = User.Identity.Name;
            return View(model);
        }

        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            ClaimsIdentity id = new(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult AdminPanel(string Mess, string Type)
        {
            var Messengs = db.Requests.OrderByDescending(u => u.Id);

            if (Mess != null)
            {
                ViewBag.Messeng = Mess;
                ViewBag.Type = Type;
            }

            ViewBag.Login = User.Identity.Name;

            return View(Messengs);
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Editing(int? id)
        {
            ViewBag.Login = User.Identity.Name;
            try
            {
                if (id != null)
                {
                    var messeng = await db.Requests.FirstOrDefaultAsync(p => p.Id == id);
                    if (messeng != null)
                    {
                        ViewBag.Request = messeng;
                        ViewBag.RequestId = id;
                        return View();
                    }
                    return RedirectToAction("AdminPanel", "Admin", new { Mess = "Не удалось найти запись.", Type = "alert-danger" });
                }
                return RedirectToAction("AdminPanel", "Admin", new { Mess = "Пришло странное поле id.", Type = "alert-danger" });
            }
            catch
            {
                return RedirectToAction("AdminPanel", "Admin", new { Mess = "Произошла неизвесная ошибка.", Type = "alert-danger" });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Editing(Request messengs)
        {
            try
            {
                db.Requests.Update(messengs);
                await db.SaveChangesAsync();
                return RedirectToAction("AdminPanel", "Admin", new { Mess = "Данные успешно изменены.", Type = "alert-success" });
            }
            catch
            {
                return RedirectToAction("AdminPanel", "Admin", new { Mess = "Произошла неизвесная ошибка при изменении данных.", Type = "alert-danger" });
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult Moder(string Mess, string Type)
        {
            if (User.Identity.Name == "Admin")
            {
                if (Mess != null)
                {
                    ViewBag.Messeng = Mess;
                    ViewBag.Type = Type;
                }
                ViewBag.Login = User.Identity.Name;
                return View(db.Moders);
            }
            else
            {
                return RedirectToAction("AdminPanel", "Admin", new { Mess = "Данный пользователь не может зайти на эту страницу", Type = "alert-danger" });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddModer(Moder moder)
        {
            try
            {
                if (moder != null && User.Identity.Name == "Admin")
                {
                    db.Moders.Add(moder);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Moder", new { Mess = "Аккаунт модератора успешно добавлен.", Type = "alert-success" });
                }
                else
                {
                    return RedirectToAction("AdminPanel", "Admin", new { Mess = "Данный пользователь не может зайти на эту страницу", Type = "alert-danger" });
                }
            }
            catch
            {
                return RedirectToAction("Moder", new { Mess = "Произошла ошибка при добавлении модератора", Type = "alert-danger" });
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DeleteUser(int? id)
        {
            try
            {
                if (id != null && User.Identity.Name == "Admin")
                {
                    Moder moder = await db.Moders.FirstOrDefaultAsync(p => p.Id == id);
                    if (moder != null)
                    {
                        db.Moders.Remove(moder);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Moder", new { Mess = "Аккаунт модератора успешно удалён.", Type = "alert-success" });
                    }
                    else
                    {
                        return RedirectToAction("Moder", new { Mess = "Произошла ошибка при удалении модератора", Type = "alert-danger" });
                    }
                }
                else
                {
                    return RedirectToAction("Moder", new { Mess = "Произошла ошибка при удалении модератора", Type = "alert-danger" });
                }
            }
            catch
            {
                return RedirectToAction("Moder", new { Mess = "Произошла неизвесная ошибка при удалении модератора", Type = "alert-danger" });
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id != null)
                {
                    Request mess = await db.Requests.FirstOrDefaultAsync(p => p.Id == id);
                    if (mess != null)
                    {
                        db.Requests.Remove(mess);
                        await db.SaveChangesAsync();
                        return RedirectToAction("AdminPanel", new { Mess = "Аккаунт модератора успешно удалён.", Type = "alert-success" });
                    }
                    else
                    {
                        return RedirectToAction("AdminPanel", new { Mess = "Произошла ошибка при удалении сообщения, не удалось найти данную запись по Id в базе данных", Type = "alert-danger" });
                    }
                }
                else
                {
                    return RedirectToAction("AdminPanel", new { Mess = "Произошла ошибка при удалении сообщения, пришло пустое Id", Type = "alert-danger" });
                }
            }
            catch
            {
                return RedirectToAction("AdminPanel", new { Mess = "Произошла неизвесная ошибка при удалении сообщения", Type = "alert-danger" });
            }
        }
    }
}
