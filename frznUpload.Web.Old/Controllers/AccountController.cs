using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using frznUpload.Web.Data;
using Microsoft.AspNetCore.Mvc;


namespace frznUpload.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager manager;

        public AccountController(UserManager manager)
        {
            this.manager = manager;
        }

        public IActionResult Index()
        {
            return View("Login");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string name, string password)
        {
            if (!ModelState.IsValid)
                return View();

            try
            {
                await manager.SignIn(HttpContext, name, password);
                return View((false, "Logged in"));
            }
            catch(InvalidOperationException)
            {
                return View((true, "Username not found"));
            }
            catch (UnauthorizedAccessException)
            {
                return View((true, "Wrong password"));
            }
        }

        public async Task<IActionResult> Logout()
        {
            //await HttpContext.LogoutAsync();

            return View("LoggedOut");
        }
    }
}